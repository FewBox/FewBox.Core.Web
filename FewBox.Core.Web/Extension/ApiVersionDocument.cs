using Microsoft.AspNetCore.Mvc;

namespace FewBox.Core.Web.Extension
{
    public class ApiVersionDocument
    {
        public ApiVersion ApiVersion { get; set; }
        public bool IsDefault { get; set; }
    }
}