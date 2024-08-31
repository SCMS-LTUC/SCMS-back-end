﻿using Microsoft.AspNetCore.Identity;
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
            var user = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                //check if role exist before adding it 
                if (await _roleManager.RoleExistsAsync(registerDto.Role) && registerDto.Role != "Admin")
                    await _userManager.AddToRoleAsync(user, registerDto.Role);

                // save student/teacher info in their table
                switch(registerDto.Role)
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

                //update database 
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return new DtoUserResponse
                {
                    Id = user.Id,
                    Username = user.Id,
                    Roles = await _userManager.GetRolesAsync(user),
                    Token = await GenerateToken(user, TimeSpan.FromMinutes(7))
                };
            }
            foreach (var error in result.Errors)
            {
                var errorCode = error.Code.Contains("Password") ? nameof(registerDto) :
                                error.Code.Contains("Email") ? nameof(registerDto) :
                                error.Code.Contains("Username") ? nameof(registerDto) : "";

                modelState.AddModelError(errorCode, error.Description);
            }

            return null;

        }
        public async Task<DtoUserResponse> Register(DtoAdminRegisterRequest registerDto, ModelStateDictionary modelState)
        {
            var user = new User
            {
                UserName = registerDto.Username,
                Email = registerDto.Email
            };
            var result = await _userManager.CreateAsync(user, registerDto.Password);
            if (result.Succeeded)
            {
                    await _userManager.AddToRoleAsync(user, "Admin");

                return new DtoUserResponse
                {
                    Id = user.Id,
                    Username = user.Id,
                    Roles = await _userManager.GetRolesAsync(user),
                    Token = await GenerateToken(user, TimeSpan.FromMinutes(7))
                };
            }
            foreach (var error in result.Errors)
            {
                var errorCode = error.Code.Contains("Password") ? nameof(registerDto) :
                                error.Code.Contains("Email") ? nameof(registerDto) :
                                error.Code.Contains("Username") ? nameof(registerDto) : "";

                modelState.AddModelError(errorCode, error.Description);
            }

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
                        Token = await GenerateToken(user, TimeSpan.FromMinutes(7))
                    };
                }
            }
            return null;
        }

        public async Task<string> GenerateToken(User user, TimeSpan expiryDate)
        {
            var userPrincliple = await _signInManager.CreateUserPrincipalAsync(user);
            if (userPrincliple == null)
            {
                return null;
            }

            var signInKey = JwtTokenService.GetSecurityKey(_configuration);

            var token = new JwtSecurityToken
                (
                expires: DateTime.UtcNow + expiryDate,
                signingCredentials: new SigningCredentials(signInKey, SecurityAlgorithms.HmacSha256),
                claims: userPrincliple.Claims
                );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        //for test 
        public async Task<DtoUserResponse> userProfile(ClaimsPrincipal claimsPrincipal)
        {
            var user = await _userManager.GetUserAsync(claimsPrincipal);

            return new DtoUserResponse()
            {
                Id = user.Id,
                Username = user.UserName,
                Token = await GenerateToken(user, System.TimeSpan.FromMinutes(7)) // just for development purposes
            };
        }

       
    }
}
