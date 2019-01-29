using FewBox.Core.Persistence.Orm;
using Microsoft.Extensions.Configuration;

namespace FewBox.Core.Web.Orm
{
    public class AppSettingOrmConfiguration : IOrmConfiguration
    {
        private string ConnectionString { get; set; }

        public AppSettingOrmConfiguration(IConfiguration configuration)
        {
            this.ConnectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public string GetConnectionString()
        {
            return this.ConnectionString;
        }
    }
}
