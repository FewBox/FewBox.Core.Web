namespace FewBox.Core.Web.Dto
{
    public class SignInRequestDto
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }
        public string ExpiredTimeSpan { get; set; }
    }
}