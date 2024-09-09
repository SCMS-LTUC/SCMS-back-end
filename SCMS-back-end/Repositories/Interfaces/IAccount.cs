using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Security.Claims;
using SCMS_back_end.Models.Dto.Request;
using SCMS_back_end.Models.Dto.Response;
using SCMS_back_end.Models;



namespace SCMS_back_end.Repositories.Interfaces
{
    public interface IAccount
    {
        public Task<DtoUserResponse> Register(DtoUserRegisterRequest registerDto, ModelStateDictionary modelState);
        public Task<DtoUserResponse> Register(DtoAdminRegisterRequest registerDto, ModelStateDictionary modelState);
        public Task<DtoUserResponse> Login(DtoUserLoginRequest loginDto);
        //public Task Logout();
        public Task<string> GenerateToken(User user);

        public Task<DtoUserResponse> RefreshToken(TokenDto tokenDto);
        
    }
}
