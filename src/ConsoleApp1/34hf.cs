using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using AngleSharp.Dom.Html;

namespace ConsoleApp1
{

    public class _34hfMain : DonetSpider.Spider
    {
        private Dictionary<string,string> data = new Dictionary<string, string>();
        protected string savePath;
        private  string saveText => "index.txt";
        protected override bool BeforeStart()
        {
            var result = base.BeforeStart();
            if (!result) {
                if (File.Exists(saveText))
                {
                    var str = File.ReadAllText(saveText);
                    data = GetJson<Dictionary<string, string>>(str);
                    return false;
                }
            }
            return result;
        }
        protected override void Parse(IHtmlDocument html)
        {
            var data = html.QuerySelectorAll(".fenlei li a")
                .Select(m => new 
                {
                    a = m.TextContent.Trim(),
                    b = GetUrl(m.GetAttribute("href"))
                }).ToDictionary(m=>m.a,m=>m.b);

            var str = ToJson(data);
            File.WriteAllText(saveText,str);
        }

        protected override void End()
        {
            foreach (var d in data) {
                new _34hfPage()
                    .SetSavePath($@"{savePath}{d.Key}\")
                    .StartWithUrl(d.Key);
            }
            base.End();
        }

    }
    public class _34hfPage : DonetSpider.Spider
    {
        protected string savePath;
        private string nextPageFile => savePath + "next.txt";

        public _34hfPage SetSavePath(string path) {
            this.savePath = path;
            return this;
        }

        protected override bool BeforeStart()
        {
            if (File.Exists(nextPageFile)) {
                var url = File.ReadAllText(nextPageFile);
                if (!string.IsNullOrEmpty(url))
                {
                    this._currentPage = url;
                }
                else {
                    return false;
                }
                
            }
            return base.BeforeStart();;
        }
        protected override void Parse(IHtmlDocument html)
        {
            var result = html.QuerySelectorAll("article a")
                .Select(m =>
                {
                    var url = GetUrl(m.GetAttribute("href"));
                    var imgd = m.QuerySelector("img");
                    string img = imgd == null ? "" : imgd.GetAttribute("data-src");
                    return (url, img);
                }).Where(m => !string.IsNullOrEmpty(m.img));
            Debugger($"{DateTime.Now.ToString()}:成功获取{result.Count()}个数据");
            foreach (var r in result)
            {
                var name = r.url.GetHashCode().ToString();
                var txtName = $"{savePath}{name}.txt";
                var picName = $"{savePath}{name}.png";
                if (File.Exists(txtName)) continue;
                File.WriteAllText(txtName, r.url, Encoding.UTF8);
                DownPic(picName,r.img);
            }
            File.WriteAllText(nextPageFile, string.Empty);
        }

        protected override string GetNextPage(IHtmlDocument dom)
        {
            string nexUrl = string.Empty;
            var nextPage = dom.QuerySelector(".next-page a");
            if (nextPage != null)
            {
                nexUrl = nextPage.GetAttribute("href").Trim();
            }
            File.WriteAllText(nextPageFile, nexUrl);
            return nexUrl;
        }
    }

    public class _34hfData : DonetSpider.Spider {
        protected override void Parse(IHtmlDocument html)
        {
        }
    }
}
