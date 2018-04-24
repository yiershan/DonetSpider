using AngleSharp.Parser.Html;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

namespace DonetSpider
{
    public class Spider
    {
        SpiderConfig Config;
        HtmlParser htmlParser;
        SaveMessage saveMessage;
        AngleSharp.Dom.Html.IHtmlDocument dom;
        public string Url { get; protected set; }
        public string ErrorMessage { get; protected set; }

        public Spider(SpiderConfig Config, SaveMessage saveMessage = null)
        {
            this.Config = Config;
            htmlParser = new HtmlParser();
            this.saveMessage = saveMessage;
        }
        public void Start()
        {
            if (Config.Select == null) throw new Exception("没有选择抓取内容");
            Url = Config.MainUrl;
            doTask();

        }
        void doTask() {
            string url = Url;
            if (string.IsNullOrEmpty(Url)) return;
            var html = GetHTMLByURL(Url);
            var data = explain(html);
            foreach (var d in data)
            {
                select(d);
            }
            nextPage();
        }
        void nextPage() {
            if (Config.NextPage == null) return;
            if (Config.NextPage.param != null) {

                return;
            }
            if (Config.NextPage.next != null)
            {
                if (Config.NextPage.next.Select == null) return;
                var url = select(dom.QuerySelector("body"), Config.NextPage.next.Select);
                if (url == Url) return;
                doTask();
                return;
            }
        }
        void select(AngleSharp.Dom.IElement element) {
            Dictionary<string, string> message = new Dictionary<string, string>();
            foreach (var s in Config.Select)
            {
                message.Add(s.ResultKey, select(element, s));
            }
            saveMessage?.Invoke(message);
        }
       string select(AngleSharp.Dom.IElement element, SelectQuery query) {
            string value = "";
            var msg = explain(element, query.Query).FirstOrDefault();

            if (msg != null)
            {
                value = query.Attribute.ToUpper() == "HTML" ? msg.InnerHtml : msg.GetAttribute(query.Attribute);
            }
            return value;

        }
        IList<AngleSharp.Dom.IElement>  explain(AngleSharp.Dom.IElement element, HtmlQuery query)
        {
            if (query == null) return new List<AngleSharp.Dom.IElement> { element } ;
            var result = element.QuerySelectorAll(query.Query).ToList();
            if (query.Where != null) {
                result = result.Where(m=>m.GetAttribute(query.Where.Key)==query.Where.Value).ToList();
            }
            if(result==null||result.Count()==0) return new List<AngleSharp.Dom.IElement>();
            return explain(result,query.Children);

        }
        IList<AngleSharp.Dom.IElement> explain(IList<AngleSharp.Dom.IElement> element, HtmlQuery query)
        {
            if (query == null) return element;
            var result = new List<AngleSharp.Dom.IElement>();
            foreach (var e in element)
            {
                var data = e.QuerySelectorAll(query.Query).ToList();
                var select = query.Where;
                if (select != null)
                {
                   data = data.Where(m => m.GetAttribute(select.Key) == select.Value).ToList();
                }
                foreach (var s in data)
                {
                    result.Add(s);
                }
            }
            if (query.Children != null)
            {
                return explain(result, query.Children);
            }
            return result;
        }
        IList<AngleSharp.Dom.IElement> explain(string html)
        {
            dom = htmlParser.Parse(html);
            if (Config.Query == null) throw new Exception("Config配置里Query不能为空");
            var result = dom.QuerySelectorAll(Config.Query.Query).ToList();
            if (Config.Query.Where != null)
            {
                var select = Config.Query.Where;
                result = result.Where(m => m.GetAttribute(select.Key) == select.Value).ToList();
            }
            return explain(result, Config.Query.Children);
        }
        /// <summary>
        /// 访问页面
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string httpGet(string url)
        {
            string result = "";
            using (HttpClient client = new HttpClient())
            {
                if (Config.HttpConfig != null && Config.HttpConfig.Timeout.HasValue)
                {
                    client.Timeout = new TimeSpan(0, 0, Config.HttpConfig.Timeout.Value);
                }
                Byte[] resultBytes = client.GetByteArrayAsync(url).Result;
                result = Encoding.GetEncoding("GB2312").GetString(resultBytes);
            }
            return result;
        }
        string GetHTMLByURL(string url)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            try
            {
                System.Net.WebRequest wRequest = System.Net.WebRequest.Create(url);
                wRequest.ContentType = "text/html; charset=gb2312";

                wRequest.Method = "get";
                wRequest.UseDefaultCredentials = true;
                // Get the response instance.
                var task = wRequest.GetResponseAsync();
                System.Net.WebResponse wResp = task.Result;
                System.IO.Stream respStream = wResp.GetResponseStream();
                //dy2018这个网站编码方式是GB2312,
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding("GB2312")))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }
        }
    }

    public delegate void SaveMessage(Dictionary<string, string> message);
}
