using System;

namespace FewBox.Core.Web.Config
{
    public class OrmConfig
    {
        internal OrmConnectionType InternalConnectionType
        {
            get
            {
                if (String.IsNullOrEmpty(this.ConnectionType))
                {
                    return OrmConnectionType.Unknown;
                }
                else
                {
                    return (OrmConnectionType)Enum.Parse(typeof(OrmConnectionType), this.ConnectionType);
                }
            }
        }

        public string ConnectionType { get; set; }
    }
}