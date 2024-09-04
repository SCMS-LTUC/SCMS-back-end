using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
//using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using SCMS_back_end.Repositories.Interfaces;
using SCMS_back_end.Models;
using SCMS_back_end.Data;

namespace SCMS_back_end.Repositories.Services
{
    public class IdentityAccountService : IAccount
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private  RoleManager<IdentityRole> _roleManager;
        private StudyCenterDbContext _context; 

        private readonly IConfiguration _configuration;

        public IdentityAccountService(UserManager<User> userManager, SignInManager<User> signInManager,
            IConfiguration configuration, RoleManager<IdentityRole> roleManager, StudyCenterDbContext context)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _context = context;
        }

        public async Task<DtoUserResponse> Register(DtoUserRegisterRequest registerDto, ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                return null;
            }

            if (!IsValidRole(registerDto.Role))
            {
                modelState.AddModelError("Role", "Invalid role. Only 'Teacher' or 'Student' roles are allowed.");
                return null;
            }

            var user = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                    await _userManager.AddToRoleAsync(user, registerDto.Role);

                await AddRoleSpecificInfoAsync(registerDto, user);
                await _context.SaveChangesAsync();

                return await CreateDtoUserResponseAsync(user);
            }
            AddErrorsToModelState(result, modelState);
            return null;
        }
        public async Task<DtoUserResponse> Register(DtoAdminRegisterRequest registerDto, ModelStateDictionary modelState)
        {
            if (!modelState.IsValid)
            {
                return null;
            }

            var user = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");

                return await CreateDtoUserResponseAsync(user);
            }

            AddErrorsToModelState(result, modelState);
            return null;
        }

        public async Task<DtoUserResponse> Login(DtoUserLoginRequest loginDto)
        {
            var user = await _userManager.FindByNameAsync(loginDto.Username);           
            if (user != null)
            {
                bool passValidation = await _userManager.CheckPasswordAsync(user, loginDto.Password);
                if (passValidation)
                {
                    return new DtoUserResponse
                    {
                        Id = user.Id,
                        Username = user.Id,
                        Roles = await _userManager.GetRolesAsync(user),
                        Token = await GenerateToken(user)
                    };
                }
            }
            return null;
        }

        public async Task<string> GenerateToken(User user)
        {
            var userPrincliple = await _signInManager.CreateUserPrincipalAsync(user);
            if (userPrincliple == null)
            {
                return null;
            }

            var tokenExpiryInMinutes = _configuration.GetValue<int>("JWT:ExpiryInMinutes");
            var tokenExpiry = TimeSpan.FromMinutes(tokenExpiryInMinutes);

            var signInKey = JwtTokenService.GetSecurityKey(_configuration);

            var token = new JwtSecurityToken
                (
                expires: DateTime.UtcNow.Add(tokenExpiry),
                signingCredentials: new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256),
                claims: userPrincliple.Claims
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        
        //Helper methods
        private bool IsValidRole(string role)
        {
            return role == "Teacher" || role == "Student";
        }

        private async Task AddRoleSpecificInfoAsync(DtoUserRegisterRequest registerDto, User user)
        {
            switch (registerDto.Role)
            {
                case "Teacher":
                    var teacher = new Teacher
                    {
                        UserId = user.Id,
                        FullName = registerDto.FullName,
                        CourseLoad = registerDto.CourseLoad,
                        PhoneNumber = registerDto.PhoneNumber,
                        DepartmentId = registerDto.DepartmentId,
                    };
                    user.Teacher = teacher;
                    break;

                case "Student":
                    var student = new Student
                    {
                        UserId = user.Id,
                        FullName = registerDto.FullName,
                        Level = registerDto.Level,
                        PhoneNumber = registerDto.PhoneNumber,
                    };
                    user.Student = student;
                    break;
            }

            _context.Users.Update(user);
        }

        private void AddErrorsToModelState(IdentityResult result, ModelStateDictionary modelState)
        {
            foreach (var error in result.Errors)
            {
                var errorCode = error.Code switch
                {
                    string code when code.Contains("Password") => "Password",
                    string code when code.Contains("Email") => "Email",
                    string code when code.Contains("UserName") => "Username",
                    _ => ""
                };

                modelState.AddModelError(errorCode, error.Description);
            }
        }

        private async Task<DtoUserResponse> CreateDtoUserResponseAsync(User user)
        {
            return new DtoUserResponse
            {
                Id = user.Id,
                Username = user.Id,
                Roles = await _userManager.GetRolesAsync(user),
                Token = await GenerateToken(user) 
            };
        }

        //for test 
        public async Task<DtoUserResponse> userProfile(ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userManager.GetUserAsync(claimsPrincipal);

            return new DtoUserResponse()
            {
                Id = user.Id,
                Username = user.UserName,
                Token = await GenerateToken(user)
            };
        }

    }
}
