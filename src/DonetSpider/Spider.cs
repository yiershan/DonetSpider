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
    public class Spider
    {

        /// <summary>
        /// 访问入口
        /// </summary>
        private string MainUrl { get; set; }

        private IHttpHelper Http;
        private string host;
        private bool removeScripts = false;
        #region 构造函数
        public Spider()
        {
        }
        /// <summary>
        /// 设置访问地址
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public Spider SetUrl(string url) {
            this.MainUrl = url.Trim().ToLower();
            if (!this.MainUrl.StartsWith("http")) {
                this.MainUrl = $"http://{MainUrl}";
            }
            return this;
        }
        public Spider SetHttpHelper(IHttpHelper httpHelper) {
            this.Http = httpHelper;
            return this;
        }
        public Spider RemoveScripts(bool remove)
        {
            this.removeScripts = remove;
            return this;
        }
        #endregion
        /// <summary>
        /// 执行前验证
        /// </summary>
        private void BeforeStart() {
            if (string.IsNullOrEmpty(MainUrl)) throw new Exception("未设置页面地址！");
            if (this.Http == null) this.Http = new HttpHelper();
        }
        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            try
            {
                this.BeforeStart();
                this.host = Http.GetHost(MainUrl);
                string html = Http.GetHTMLByURL(MainUrl);
                var dom = new HtmlParser().Parse(html);
                if (removeScripts) {
                    foreach (var i in dom.Scripts)
                    {
                        i.Remove();
                    }
                }
                this.Parse(dom);
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
        /// <summary>
        /// 解析页面
        /// </summary>
        protected virtual void Parse(IHtmlDocument html) {
            Console.WriteLine(html.Source.Text);
            Console.ReadLine();
        }

        #region
        protected string GetUrl(string url)
        {
            if (url.Trim().ToUpper().StartsWith("HTTP"))
            {
                return url;
            }
            else
            {
                return this.host + url;
            }
        }
        #endregion
    }
}
