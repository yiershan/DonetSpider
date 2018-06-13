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

        public SpiderBase(IHttpHelper http, SpiderConfig config) {
            this.Http = http;
            this.htmlParser = new HtmlParser();
            this.Config = config;
        }
        public void Start() {
            DellUrl(Config.MainUrl);
        }

        protected void DellUrl(string url) {
            string html = Http.GetHTMLByURL(url);
            var dom = htmlParser.Parse(html);
            var data = dom.QuerySelectorAll(Config.Query.Query);
            foreach (var d in data) {

            }
        }

        protected IList<IElement> GetElements(IHtmlDocument html,HtmlQuery query) {
            return html.QuerySelectorAll(query.Query).ToList();
        }
        protected IElement GetElement(IElement html, HtmlQuery query)
        {
            return html.QuerySelector(query.Query);
        }
        protected string GetValues(IElement element, SelectQuery select) {
            if (select.Query != null) {
                element = GetElement(element,select.Query);
            }
            string value = "";
            if (element != null&&select.Attribute!=null)
            {
                value = select.Attribute.ToUpper() == "HTML" ? element.InnerHtml : element.GetAttribute(select.Attribute);
            }
            return value;
        }
    }
}
