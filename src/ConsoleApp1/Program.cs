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

            using (SqliteWriter writer = new SqliteWriter("movgg")) {
                new DriectSpider()
                   .SetConfig(config.Config)
                   .SetConfig(config.NextPageConfig)
                   .SetCallBack((a, b) => {
                       writer.Write(a,"wm");
                   })
                   //.SetLogger(new LogHelper())
                   .StartWithUrlAsync(config.MainUrl)
                   .Wait();
            }

        }
    }
}
