using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using DonetSpider.http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DonetSpider
{
    public class SpiderBase
    {
        protected IHttpHelper Http;
        protected SpiderConfig Config;
        protected HtmlParser htmlParser;
        protected SaveMessage SaveMessage;
        private string host;

        public SpiderBase(IHttpHelper http, SpiderConfig config, SaveMessage saveMessage) {
            this.Http = http;
            this.htmlParser = new HtmlParser();
            this.Config = config;
            this.SaveMessage = saveMessage;
        }
        public void Start() {
            this.host = Http.GetHost(Config.MainUrl);
            var data = DellUrl(Config.MainUrl,Config.Select);
            if (SaveMessage != null) {
                    this.SaveMessage(data);
            } 
        }

        protected ResultMessage DellUrl(string url, List<SelectQuery> Select) {
            var result = new ResultMessage();
            string html = Http.GetHTMLByURL(url);
            var dom = htmlParser.Parse(html);
            foreach (var s in Select) {
               result.Add(string.IsNullOrEmpty(s.Name)?result.Count.ToString():s.Name, GetValues(dom,s)) ;
            }

            return result;
        }

        protected IList<IElement> GetElements(IHtmlDocument html,HtmlQuery query) {
            var result = html.QuerySelectorAll(query.Query).ToList();
            if (query.Where != null)
            {
                result = result.Where(m => m.GetAttribute(query.Where.Key) == query.Where.Value).ToList();
            }
            return result;
        }
        protected ResultMessage GetValues(IHtmlDocument html, SelectQuery select)
        {
            var result = new ResultMessage();
            if (select.Query != null)
            {
                var ele = GetElements(html,select.Query);
                foreach (var e in ele)
                {
                    if (select.Select!=null) {
                        result.Add(result.Count.ToString(), GetValues(e, select.Select));
                    }
                }
            }
            return result;

        }
        protected ResultMessage GetValue(IElement element, HtmlSelect select) {
            var result = new ResultMessage();
            var e = select.Query == null? element: element.QuerySelector(select.Query);

            if (e != null && select.Attribute != null)
            {
                string value = select.Attribute.ToUpper() == "HTML" ? e.TextContent.Trim() : e.GetAttribute(select.Attribute);
                if (select.Url != null)
                {
                    var values = DellUrl(GetUrl(value) ,select.Url);
                    if (values != null)
                    {
                        foreach (var v in values)
                        {
                            result.Add(v.Key, v.Value);
                        }
                    }
                }
                else
                {
                    result.Add(select.ResultKey != null ? select.ResultKey : select.Attribute, new ResultMessage { Result = value});
                }
            }
            return result;
        }
        protected ResultMessage GetValues(IElement element, IList<HtmlSelect> select) {
            var result = new ResultMessage();
            foreach (var s in select)
            {
                result.Add(result.Count.ToString(),GetValue(element,s));
            }
            return result;
        }
        protected string GetUrl(string url) {
            if (url.Trim().ToUpper().StartsWith("HTTP"))
            {
                return url;
            }
            else
            {
                return this.host + url;
            }
        }
    }

    public delegate void SaveMessage(ResultMessage message);
}
