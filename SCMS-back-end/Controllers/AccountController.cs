using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using NuGet.Common;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;
using System.Security.Policy;

namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccount _userService;
        public AccountController(IAccount context)
        {
            _userService = context;
        }

        [HttpPost("Register")] //Register
        public async Task<ActionResult<DtoUserResponse>> Register(DtoUserRegisterRequest registerDto)
        {         
             var user = await _userService.Register(registerDto, this.ModelState);
            if (!ModelState.IsValid) return BadRequest(ModelState);
            if (user == null) return Unauthorized();

            return Ok($"{user.Username} registered successfully.");
        }

        [HttpPost("Login")] //Login
        public async Task<ActionResult<DtoUserResponse>> Login(DtoUserLoginRequest loginDto)
        {
            var user = await _userService.Login(loginDto);
            if (user == null) return Unauthorized("Invalid username or password.");

            if (user.Roles != null && user.Roles.Contains("Admin"))
                return Unauthorized("Admin users are not allowed to log in here.");

            return Ok(user);
        }

        [Authorize(Roles = "Student, Teacher")]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.Logout(User);
            return Ok(new { message = "Successfully logged out" });
        }

        [AllowAnonymous]
        [HttpPost("Refresh")]
        public async Task<ActionResult<DtoUserResponse>> Refresh(TokenDto tokenDto)
        {
            try
            {
                var result = await _userService.RefreshToken(tokenDto);
                return Ok(result);
            }
            catch (SecurityTokenException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (UnauthorizedAccessException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordReqDTO forgotPasswordDto)
        {
            var result = await _userService.ForgotPasswordAsync(forgotPasswordDto);
            if (!result)
            {
                return BadRequest("Failed to send reset email.");
            }

            return Ok("Password reset link sent.");
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordReqDTO resetPasswordDto)
        {
            var result = await _userService.ResetPasswordAsync(resetPasswordDto);
            if (!result)
            {
                return BadRequest("Error resetting password.");
            }

            return Ok("Password reset successfully.");
        }

        [HttpGet("reset-password")]
        public async Task<IActionResult> ResetPassword(string email, string token)
        {
            //return Ok(new
            //{
            //    Email = email,
            //    Token = token
            //});
            //return Ok("done successfully");
            var res = new ResetPasswordResDTO
            {
                Email = email,
                Token = token
            };

            return Ok(new
            {
                res,
            });
        }
    }
}
