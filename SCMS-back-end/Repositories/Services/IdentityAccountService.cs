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
using System.Security.Cryptography;

namespace SCMS_back_end.Repositories.Services
{
    public class IdentityAccountService : IAccount
    {
        private UserManager<User> _userManager;
        private SignInManager<User> _signInManager;
        private RoleManager<IdentityRole> _roleManager;
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

                // add student/teacher info
                await AddRoleSpecificInfoAsync(registerDto, user);
                await _context.SaveChangesAsync();

                return await CreateDtoUserResponseAsync(user, true);
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

                return await CreateDtoUserResponseAsync(user, true);
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
                    return await CreateDtoUserResponseAsync(user, true);
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
        public async Task Logout(ClaimsPrincipal userPrincipal)
        {
            var user = await _userManager.GetUserAsync(userPrincipal);
            if (user != null)
            {
                user.RefreshTokenExpireTime = DateTime.UtcNow;
                await _userManager.UpdateAsync(user);
            }
        }
        public async Task<DtoUserResponse> RefreshToken(TokenDto tokenDto)
        {
            var principle = GetPrincipalFromExpiredToken(tokenDto.AccessToken);
            var user = await _userManager.FindByNameAsync(principle.Identity.Name);
            if (user == null || user.RefreshToken != tokenDto.RefreshToken ||
                user.RefreshTokenExpireTime <= DateTime.Now)
            {
                throw new UnauthorizedAccessException("Invalid or expired refresh token.");
            }
            return await CreateDtoUserResponseAsync(user, false);
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
        private async Task<DtoUserResponse> CreateDtoUserResponseAsync(User user, bool populateExp)
        {
            var refreshToken = GenerateRefreshToken();
            user.RefreshToken = refreshToken;
            if (populateExp)
                user.RefreshTokenExpireTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            return new DtoUserResponse
            {
                Id = user.Id,
                Username = user.UserName,
                Roles = await _userManager.GetRolesAsync(user),
                AccessToken = await GenerateToken(user),
                RefreshToken = refreshToken
            };

        }
        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenValidationParameters = JwtTokenService.ValidateToken(_configuration);
            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principle = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);
            var jwtSecurityToken = securityToken as JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256,
                StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid Token");
            }
            return principle;
        }



       
    }
}
