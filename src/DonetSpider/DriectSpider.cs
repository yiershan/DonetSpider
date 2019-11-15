using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AngleSharp.Dom.Html;
using DonetSpider.config;

namespace DonetSpider
{
    public class DriectSpider : Spider
    {
        Action<List<Dictionary<string, string>>, string> _save { get; set; }

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
        public DriectSpider SetCallBack(Action<List<Dictionary<string,string>>,string> action) {
            _save = action;
            return this;
        }
        protected override void Parse(IHtmlDocument html)
        {
            if (_config != null)
            {
                var result = _config._queryItems(html);
                Save(result);
            }
            else {
                Info(html.Source.Text);
            }
        }

        protected override string GetNextPage(IHtmlDocument dom)
        {
            return _nexPageConfig?._querySelector(dom,_currentPage);
        }
        private void Save(List<Dictionary<string, string>> msg) {
            if (_save == null) {
                Info(ToJson(msg));
            } else {
                _save.Invoke(msg,_currentPage);
            }
        }

    }
}
