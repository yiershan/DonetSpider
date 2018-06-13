using AngleSharp.Parser.Html;
using DonetSpider.http;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DonetSpider
{
    public class Spider
    {
        HttpHelper http;


        protected SpiderConfig Config;
        protected HtmlParser htmlParser;
        protected SaveMessage saveMessage;
        protected AngleSharp.Dom.Html.IHtmlDocument dom;

        public string Url { get; protected set; }
        public string ErrorMessage { get; protected set; }
        public Spider() {
            htmlParser = new HtmlParser();
        }
        public virtual void Init(SpiderConfig Config, SaveMessage saveMessage = null) {
            this.Config = Config;
            this.saveMessage = saveMessage;
            this.http = new HttpHelper();
        }
        public void Start()
        {
            if (Config.Select == null) throw new Exception("没有选择抓取内容");
            Url = Config.MainUrl;
            doTask();
        }

        protected virtual void doTask() {
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
        protected virtual void nextPage() {
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
        protected void select(AngleSharp.Dom.IElement element) {
            Dictionary<string, string> message = new Dictionary<string, string>();
            foreach (var s in Config.Select)
            {
                message.Add(s.ResultKey, select(element, s));
            }
            saveMessage?.Invoke(message);
        }
       protected string select(AngleSharp.Dom.IElement element, SelectQuery query) {
            string value = "";
            var msg = explain(element, query.Query).FirstOrDefault();

            if (msg != null)
            {
                value = query.Attribute.ToUpper() == "HTML" ? msg.InnerHtml : msg.GetAttribute(query.Attribute);
            }
            return value;

        }
        protected IList<AngleSharp.Dom.IElement>  explain(AngleSharp.Dom.IElement element, HtmlQuery query)
        {
            if (query == null) return new List<AngleSharp.Dom.IElement> { element } ;
            var result = element.QuerySelectorAll(query.Query).ToList();
            if (query.Where != null) {
                result = result.Where(m=>m.GetAttribute(query.Where.Key)==query.Where.Value).ToList();
            }
            if(result==null||result.Count()==0) return new List<AngleSharp.Dom.IElement>();
            return explain(result,query.Children);

        }
        protected IList<AngleSharp.Dom.IElement> explain(IList<AngleSharp.Dom.IElement> element, HtmlQuery query)
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
        protected virtual IList<AngleSharp.Dom.IElement> explain(string html)
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
        protected virtual string GetHTMLByURL(string url) {
           return http.GetHTMLByURL(url);
        }


    }

    public delegate void SaveMessage(Dictionary<string, string> message);
}
