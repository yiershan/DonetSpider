using DonetSpider.config;
namespace ConsoleApp1
{
    public class SpiderConfig
    {
        public string MainUrl { get; set; }
        public Config Config { get; set; } = new Config();
        public NextPageConfig NextPageConfig { get; set; } = new NextPageConfig();
    }
}
