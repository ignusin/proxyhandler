using System;
using System.Configuration;

namespace ProxyHandler
{
    public class DefaultConfiguration : IConfiguration
    {
        private static readonly DefaultConfiguration _instance = new DefaultConfiguration();

        public static IConfiguration Current
        {
            get { return _instance; }
        }

        public string TargetUrl
        {
            get
            {
                return ConfigurationManager.AppSettings["ProxyHandler.TargetUrl"];
            }
        }

        public string IISApplicationVirtualPath
        {
            get
            {
                return ConfigurationManager.AppSettings["ProxyHandler.IISApplicationVirtualPath"];
            }
        }

        public bool ParseGZipAndDeflate
        {
            get
            {
                var configValue = ConfigurationManager.AppSettings["ProxyHandler.ParseGZipAndDeflate"];
                return string.Equals(configValue, "true", StringComparison.InvariantCultureIgnoreCase);
            }
        }
    }
}
