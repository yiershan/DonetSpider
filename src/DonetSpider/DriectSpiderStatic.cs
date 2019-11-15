using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using DonetSpider.config;
using System;
using System.Collections.Generic;
using System.Text;

namespace DonetSpider
{
    public static class DriectSpiderStatic
    {
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
                if(itemResult.Count >0) result.Add(itemResult);
            }
            return result;
        }
        public static string _queryItems(this QueryItem item, IElement e)
        {
            var temp = string.IsNullOrEmpty(item.QuerySelector) ? e : e.QuerySelector(item.QuerySelector);
            if (temp == null) temp = e;
            string result;
            switch (item.SelectorType)
            {
                case SelectorType.TextContent:
                    result = temp.TextContent.Replace("\n", "").Replace(" ", "").Replace("\t", "").Replace("\r", ""); ;
                    break;
                case SelectorType.Attribute:
                default:
                    result = temp.GetAttribute(string.IsNullOrEmpty(item.Attribute) ? "href" : item.Attribute);
                    break;
            }
            return result;
        }

        public static string _querySelector(this NextPageConfig next, IHtmlDocument dom, string currentPage)
        {
            if (next.NextPageRule != null)
            {
                return next.NextPageRule._querySelector(dom, currentPage);
            };
            // 动态获取
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
        public static string _querySelector(this NextPageRule rule, IHtmlDocument dom, string currentPage)
        {
            // 规则 如 page=1,page=2  page={0}
            if (!string.IsNullOrEmpty(rule.PageRule))
            {
                var splits = rule.PageRule.Split("{0}");
                var indexStr = currentPage;
                foreach (var s in splits)
                {
                    if (string.IsNullOrEmpty(s)) continue;
                    indexStr = indexStr.Replace(s, "");
                }
                int.TryParse(indexStr, out int index);
                index++;
                if (index < rule.MinPage) index = rule.MinPage;
                if (index > rule.MaxPage) return "";
                return string.Format(rule.PageRule, index);
            }
            return "";
        }
    }
}
