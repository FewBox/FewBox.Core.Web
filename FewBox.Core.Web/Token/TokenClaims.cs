namespace FewBox.Core.Web.Token
{
    public static class TokenClaims
    {
        public const string Tenant = "http://schemas.fewbox.com/jwt/identity/claims/tenant";
        public const string Id = "http://schemas.fewbox.com/jwt/identity/claims/id";
        public const string Issuer = "http://schemas.fewbox.com/jwt/identity/claims/issuer";
        public const string Audience = "http://schemas.fewbox.com/jwt/identity/claims/audience";
        public const string Api = "http://schemas.fewbox.com/jwt/identity/claims/api";
        public const string Module = "http://schemas.fewbox.com/jwt/identity/claims/module";
    }
}