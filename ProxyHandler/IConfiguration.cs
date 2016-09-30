namespace ProxyHandler
{
    public interface IConfiguration
    {
        string TargetUrl { get; }
        string IISApplicationVirtualPath { get; }
        bool ParseGZipAndDeflate { get; }
    }
}
