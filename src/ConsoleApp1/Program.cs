using AngleSharp.Dom.Html;
using DonetSpider;
using DonetSpider.config;
using DonetSpider.http;
using DonetSpider.Log;
using Microsoft.Extensions.Configuration;
using PuppeteerSharpHttp;
using System;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            var builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
            var configuration = builder.Build();
            SpiderConfig config = new SpiderConfig();
            configuration.GetSection("SpiderConfig").Bind(config);
            new DriectSpider()
               .SetConfig(config.Config)
               .SetConfig(config.NextPageConfig)
               .SetLogger(new LogHelper())
               .StartWithUrlAsync(config.MainUrl)
               .Wait();
        }
    }
    public class SpiderConfig {
        public string MainUrl { get; set; }
        public Config Config { get; set; } = new Config();
        public NextPageConfig NextPageConfig { get; set; } = new NextPageConfig();
    }
}
