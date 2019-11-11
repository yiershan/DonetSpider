﻿using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using DonetSpider.http;
using DonetSpider.Log;
using Newtonsoft.Json;
using System;
using System.Security.Cryptography;
using System.Text;

namespace DonetSpider
{
    public abstract class Spider: HasLog
    {
        public string _nextPage { get; private set; }
        public string _currentPage { get; set; }
        private IHttpHelper _Http;
        public string _host { get;private set; }
        public bool _removeScripts { get; private set; } = false;
        #region 构造函数
        public Spider SetHttpHelper(IHttpHelper httpHelper) {
            this._Http = httpHelper;
            return this;
        }
        public Spider RemoveScripts(bool remove)
        {
            this._removeScripts = remove;
            return this;
        }
        public Spider SetLogger(ILog log) {
            this._log = log;
            return this;
        }
        public void StartWithUrl(string url)
        {
            this._currentPage = url;
            if (this.BeforeStart())
            {
                this.WithUrl(_currentPage);
            }
            this.End();
        }

        #endregion
        protected virtual void End()
        {
            Debugger("任务结束！");
        }
        protected virtual bool BeforeStart() {
            Debugger("任务开始！");
            return true;
        }
        /// <summary>
        /// 解析网页
        /// </summary>
        protected void WithUrl(string url)
        {
            try
            {
                Check();
                var html = _Http.GetHTMLByURL(url);
                if (!string.IsNullOrEmpty(html)) {
                    Debugger($"解析{url}页面开始！");
                    using (IHtmlDocument dom = new HtmlParser().Parse(html))
                    {
                        this.PreParse(dom);
                        _nextPage = GetNextPage(dom);
                        this.Parse(dom);
                        Debugger($"解析{url}页面结束！");
                        NextPage();
                    }
                }
            }
            catch(Exception e) {
                Error($"解析{url}页面出错：{e.Message}");
            }
        }
        private void NextPage() {
            if (!string.IsNullOrEmpty(_nextPage)) {
                WithUrl(_nextPage);
            }
        }
        /// <summary>
        /// 获取下一页
        /// </summary>
        /// <param name="dom"></param>
        /// <returns></returns>
        protected virtual string GetNextPage(IHtmlDocument dom) {
            return string.Empty;
        }
        /// <summary>
        /// 网页文档预处理
        /// </summary>
        /// <param name="dom"></param>
        protected virtual void PreParse(IHtmlDocument dom) {
            if (_removeScripts)
            {
                foreach (var i in dom.Scripts)
                {
                    i.Remove();
                }
            }
        }
        /// <summary>
        /// 开始前校验
        /// </summary>
        protected virtual void Check()
        {
            if (string.IsNullOrEmpty(this._currentPage))
            {
                throw new Exception("地址无效！");
            }
            if (this._Http == null)
            {
                this._Http = new HttpHelper().SetLogger(_log);
            }
        }
        /// <summary>
        /// 解析页面内容
        /// </summary>
        protected abstract void Parse(IHtmlDocument html);

        #region
        protected string GetUrl(string url)
        {
            if (url.Trim().ToUpper().StartsWith("HTTP"))
            {
                return url;
            }
            else
            {
                return this._host + url;
            }
        }

        public string CalculateMD5Hash(string input)
        {

            // step 1, calculate MD5 hash from input

            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);

            byte[] hash = md5.ComputeHash(inputBytes);


            // step 2, convert byte array to hex string

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();

        }

        public string ToJson(object obj) {
            return JsonConvert.SerializeObject(obj);
        }

        public T GetJson<T>(string str) {
            return JsonConvert.DeserializeObject<T>(str);
        
        }
        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Url"></param>
        public virtual void DownPic(string FileName, string Url) {
            this._Http.SavePhotoFromUrl(FileName, Url);
        }
        #endregion
    }
}
