namespace SCMS_back_end.Models.Dto.Response
{
    public class DtoUserResponse
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
        public IList<string> Roles { get; set; }
    }
}
