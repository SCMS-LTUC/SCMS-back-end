using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Models;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;

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

            return Ok(user);
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



    }
}
