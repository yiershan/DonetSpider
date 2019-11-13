using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using DonetSpider.config;

namespace DonetSpider
{
    public class DriectSpider : Spider
    {
        Action<string,string> _save { get; set; }

        Config _config = new Config();
        NextPageConfig _nexPageConfig = new NextPageConfig();
        public DriectSpider SetConfig(Config config) {
            _config = config;
            return this;
        }
        public DriectSpider SetConfig(NextPageConfig config)
        {
            _nexPageConfig = config;
            return this;
        }
        public DriectSpider SetCallBack(Action<string,string> action) {
            _save = action;
            return this;
        }
        protected override void Parse(IHtmlDocument html)
        {
            var result = _config == null ?html.Source.Text: ToJson( _config._queryItems(html));
            Save(result);
        }

        protected override string GetNextPage(IHtmlDocument dom)
        {
            return _nexPageConfig?._querySelector(dom);
        }
        private void Save(string msg) {
            if (_save == null) {
                Info(msg);
            } else {
                _save.Invoke(msg,_currentPage);
            }
        }

    }

    public static class DriectSpiderStatic {
        public static List<Dictionary<string, string>> _queryItems(this Config config, IHtmlDocument dom)
        {
            List<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
            var elements = dom.QuerySelectorAll(string.IsNullOrEmpty(config.QuerySelectorAll) ? "body" : config.QuerySelectorAll);
            foreach (var e in elements)
            {
                Dictionary<string, string> itemResult = new Dictionary<string, string>();
                if (config.QueryItems != null)
                {
                    foreach (var q in config.QueryItems)
                    {
                        if (string.IsNullOrEmpty(q.KeyName)) continue;
                        var data = q._queryItems(e);
                        itemResult.Add(q.KeyName, data?.Trim());
                    }
                }
                result.Add(itemResult);
            }
            return result;
        }
        public static string _queryItems(this QueryItem item, IElement e)
        {
            var temp = string.IsNullOrEmpty(item.QuerySelector) ? e : e.QuerySelector(item.QuerySelector);
            if (temp == null) _ = e;
            var result = "";
            switch (item.SelectorType)
            {
                case SelectorType.TextContent:
                    result = temp.TextContent.Trim();
                    break;
                case SelectorType.Attribute:
                default:
                    result = temp.GetAttribute(string.IsNullOrEmpty(item.Attribute) ? "href" : item.Attribute);
                    break;
            }
            return result;
        }

        public static string _querySelector(this NextPageConfig next, IHtmlDocument dom)
        {
            if (string.IsNullOrEmpty(next.QuerySelector))
            {
                return "";
            }
            if (next.QueryItem == null)
            {
                next.QueryItem = new QueryItem();
            }
            var element = dom.QuerySelector(next.QuerySelector);
            if (element == null) return "";
            return next.QueryItem._queryItems(element);
        }
    }
}
