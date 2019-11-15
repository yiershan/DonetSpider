using AngleSharp.Dom.Html;
using DonetSpider;
using DonetSpider.http;
using PuppeteerSharpHttp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    /// <summary>
    /// 一级数据
    /// </summary>
    public class m : Spider
    {
        protected override void Parse(IHtmlDocument html)
        {
            var m11 = new m1()
                .SetLogger(_log)
                .SetHttpHelper(_Http);

            var result = html.QuerySelectorAll(".preview-box").ToList();
            //var m1 = new m1();
            //foreach (var r in result)
            //{
            //    Console.WriteLine(r.c);
            //    Console.WriteLine("--------------------------------");
            //    //m11.StartWithUrlAsync(r.a).Wait();
            //}
        }
    }
    /// <summary>
    /// 二级数据
    /// </summary>
    public class m1 : Spider
    {
        protected override void Parse(IHtmlDocument html)
        {
            var m2 = new m2()
                .SetLogger(_log)
                .SetHttpHelper(_Http);
            var result = html.QuerySelectorAll("#content .p-thumb .lazyload")
                .Select(m => new
                {
                    a = m.GetAttribute("title"),
                    b = m.GetAttribute("href"),
                    c = m.QuerySelector("img").GetAttribute("data-original"),
                }).ToList();
            foreach (var r in result)
            {
                Console.WriteLine(r.a);
                Console.WriteLine(r.c);
                Console.WriteLine("…………………………………………");
                m2.StartWithUrlAsync(r.b).Wait();
            }
        }
    }
    /// <summary>
    /// 三级数据
    /// </summary>
    public class m2 : Spider
    {
        protected override void Parse(IHtmlDocument html)
        {
            var m3 = new m3()
                     .SetLogger(_log)
                     .SetHttpHelper(_Http);

            IHttpHelper http = new PuppeteerSharpHttpHelper();
            var dbc = html.QuerySelector(".dbc").TextContent.Trim();
            var ll = html.QuerySelector("#video-list").TextContent.Trim();
            var imgs = html.QuerySelectorAll(".slides img").Select(m => m.GetAttribute("src"));
            var down = html.QuerySelector(".download_dz a").GetAttribute("href");
            Console.WriteLine(dbc);
            Console.WriteLine(ll);
            foreach (var i in imgs)
            {
                Console.WriteLine(i);
            }
            m3.StartWithUrlAsync(down).Wait();
        }
    }
    /// <summary>
    /// 四级数据
    /// </summary>
    public class m3 : Spider
    {
        protected override void Parse(IHtmlDocument html)
        {
            var download = html.QuerySelectorAll("#download li a")
                .Select(m => m.GetAttribute("href"));

            foreach (var d in download)
            {
                Console.WriteLine(d);
            }
        }
    }
}
