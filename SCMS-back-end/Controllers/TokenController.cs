using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Repositories.Interfaces;


namespace SCMS_back_end.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IAccount _userService;
        public TokenController(IAccount context)
        {
            _userService = context;
        }

        [HttpPost]
        private async Task<ActionResult<DtoUserResponse>> Refresh(TokenDto tokenDto)
        { 
            var result= await _userService.RefreshToken(tokenDto);
            return Ok(result);
        }

    }
}
