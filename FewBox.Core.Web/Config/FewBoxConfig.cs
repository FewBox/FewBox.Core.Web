namespace FewBox.Core.Web.Config
{
    public class FewBoxConfig
    {
        public HealthyConfig Healthy { get; set; }
        public JWTConfig JWT { get; set; }
        public NotificationConfig Notification { get; set; }
        public OrmConfig Orm { get; set; }
        public RedisDistributedCacheConfig RedisDistributedCache { get; set; }
        public SecurityEndpointConfig SecurityEndpoint { get; set; }
        public GoogleConfig Google { get; set; }
        public FacebookConfig Facebook { get; set; }
        public MicrosoftAccountConfig MicrosoftAccount { get; set; }
        public TwitterConfig Twitter { get; set; }
    }
}
