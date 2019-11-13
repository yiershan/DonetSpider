using System.Collections.Generic;

namespace DonetSpider.config
{
    public class Config
    {
        public string QuerySelectorAll { get; set; }
        public List<QueryItem> QueryItems { get; set; } = new List<QueryItem>();
    }
}
