using AngleSharp.Dom.Html;
using DonetSpider;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    public class HJD: Spider
    {
        private string startPage = "http://hjd.he2048.com/2048/thread.php?";
        public override string savePath => @"F:\fid\page\";
        public int current = 837;
        public string currentPage => $"fid-5-page-{current}.html";
        public string saveText => $"{savePath}{current}.txt";
        protected override bool BeforeStart()
        {
            this.SetUrl(startPage+currentPage);
            return base.BeforeStart();

        }
        protected override void Parse(IHtmlDocument html)
        {

            var result = html.QuerySelectorAll(".t_one .subject")
                .Select(m => new {
                    a = m.TextContent.Trim(),
                    b = GetUrl(m.GetAttribute("href")),
                });
            if (!result.Any()) current = -1;
            var data = new Dictionary<string, string>();

            foreach (var r in result) {
                if (!data.ContainsKey(r.a)) data.Add(r.a,r.b);
            }
            var str = ToJson(data);

            File.WriteAllText(saveText, str);
        }
        protected override void End()
        {
            if (this.current > 0) {
                this.current += 1;
                Start();
            } 

        }
    }
}
