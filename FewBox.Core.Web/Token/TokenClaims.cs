namespace FewBox.Core.Web.Token
{
    public static class TokenClaims
    {
        public const string Tenant = "http://schemas.fewbox.com/jwt/2019/04/identity/claims/tenant";
        public const string Id = "http://schemas.fewbox.com/jwt/2019/04/identity/claims/id";
        public const string Issuer = "http://schemas.fewbox.com/jwt/2019/04/identity/claims/issuer";
    }
}