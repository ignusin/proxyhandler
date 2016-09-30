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
    }
}
