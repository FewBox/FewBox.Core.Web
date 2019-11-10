using System.Collections.Generic;

namespace FewBox.Core.Web.Dto
{
    public class NotificationRequestDto
    {
        public IList<string> ToAddresses { get; set; }
        public string Name { get; set; }
        public string Param { get; set; }
    }
}