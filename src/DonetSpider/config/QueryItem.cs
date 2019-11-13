using AngleSharp.Dom;

namespace DonetSpider.config
{
    public class QueryItem
    {
        public string KeyName { get; set; }
        public SelectorType SelectorType { get; set; }
        public string Attribute { get; set; }
        public string QuerySelector {get;set;}
    }
}
