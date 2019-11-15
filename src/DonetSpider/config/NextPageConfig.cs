namespace DonetSpider.config
{
    public class NextPageConfig
    {
        public NextPageRule NextPageRule { get; set; }
        public string QuerySelector { get; set; }
        public QueryItem QueryItem { get; set; }
    }

    public class NextPageRule {
        public string PageRule { get; set; }
        public int MinPage { get; set; } = 1;
        public int MaxPage { get; set; } = 10000;
    }
}
