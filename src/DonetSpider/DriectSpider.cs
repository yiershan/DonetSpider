using System;
using System.Collections.Generic;
using System.Text;
using AngleSharp.Dom.Html;

namespace DonetSpider
{
    public class DriectSpider : Spider
    {
        Action<IHtmlDocument> _parse;
        Func<IHtmlDocument, string> _getNextPage;
        public DriectSpider() {

        }

        protected override void Parse(IHtmlDocument html)
        {
            _parse(html);
        }

        protected override string GetNextPage(IHtmlDocument dom)
        {
            return _getNextPage(dom);
        }


    }
}
