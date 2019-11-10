using AngleSharp.Dom;
using AngleSharp.Dom.Html;
using AngleSharp.Parser.Html;
using DonetSpider.http;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace DonetSpider
{
    public class Spider
    {
        //private IHtmlDocument dom;
        //private string html;
        public bool isFirst { get; protected set; } = true;
        public virtual string savePath { get; private set; } = @"F:\hf\";
        public List<string> urls { get; private set; }
        /// <summary>
        /// 访问入口
        /// </summary>
        public virtual string MainUrl { get;private set; }

        protected IHttpHelper Http { get; private set; }
        public string host { get;private set; }
        public bool removeScripts { get; private set; } = false;
        #region 构造函数
        public Spider()
        {
        }
        public Spider SetSavePath(string path) {
            this.savePath = path;
            return this;
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
        protected virtual  bool BeforeStart() {
            if (string.IsNullOrEmpty(MainUrl)) throw new Exception("未设置页面地址！");
            if (this.Http == null) this.Http = new HttpHelper();
            if (this.urls == null) this.urls = new List<string>();
            if (this.urls.Contains(this.MainUrl)) {
                return false;
            }

            if (Directory.Exists(savePath) == false)
            {
                Directory.CreateDirectory(savePath);
            }
            return true;


        }
        /// <summary>
        /// 开始
        /// </summary>
        public void Start()
        {
            try
            {
                bool canStart = true;
                if (isFirst) {
                    canStart =  this.BeforeStart();
                }

                if (canStart) {
                    Console.WriteLine($"访问页面：{MainUrl}");
                    this.host = Http.GetHost(MainUrl);
                    var html = Http.GetHTMLByURL(MainUrl);
                    using (IHtmlDocument dom = new HtmlParser().Parse(html)) {
                        if (removeScripts)
                        {
                            foreach (var i in dom.Scripts)
                            {
                                i.Remove();
                            }
                        }
                        this.Parse(dom);
                    }
                    this.urls.Add(this.MainUrl);
                };

                this.End();
            }
            catch(Exception e) {
                Console.WriteLine(e.Message);
                Console.ReadLine();
            }
        }
        protected virtual void End() {

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

        public bool DownPic(string FileName, string Url) {
            return this.Http.SavePhotoFromUrl(FileName, Url);
        }
        #endregion
    }
}
