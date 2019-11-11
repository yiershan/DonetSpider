using DonetSpider.Log;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace DonetSpider.http
{
    public class HttpHelper: HttpHelperBase
    {
        CookieContainer cookies;
        #region 
        public HttpHelper() {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            cookies = new CookieContainer();
        }

        #endregion
        protected override async Task _SavePhotoFromUrlAsync(string FileName, string Url)
        {
            WebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);
            webRequest.Method = "get";
            webRequest.Headers.Add("UserAgent", "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/75.0.3770.100 Safari/537.36");
            webRequest.Headers.Add("Upgrade-Insecure-Requests", "1");
            webRequest.UseDefaultCredentials = false;
            using (WebResponse response = await webRequest.GetResponseAsync())
            {
                if (!response.ContentType.ToLower().StartsWith("text/"))
                {
                    SaveBinaryFile(response, FileName);
                }
                else
                {
                    Waring($"下载{Url}失败！");
                }
                response.Close();
            }
        }
        /// <summary>
        /// 将二进制文件保存到磁盘
        /// </summary>
        /// <param name="response"></param>
        /// <param name="FileName"></param>
        /// <returns></returns>
        private void SaveBinaryFile(WebResponse response, string FileName)
        {
            byte[] buffer = new byte[1024];
            if (File.Exists(FileName))
                File.Delete(FileName);
            using (Stream inStream = response.GetResponseStream())
            {
                using (Stream outStream = File.Create(FileName))
                {
                    int l;
                    do
                    {
                        l = inStream.Read(buffer, 0, buffer.Length);
                        if (l > 0)
                            outStream.Write(buffer, 0, l);
                    }
                    while (l > 0);
                    outStream.Close();
                    inStream.Close();
                }
            }
        }
        protected override async Task<string> _GetHTMLByURLAsync(string url, string encoding = null, string ContentType = null)
        {
            string result = "";
            WebRequest webRequest = (HttpWebRequest)WebRequest.Create(url);
            if (ContentType != null) webRequest.ContentType = ContentType;
            webRequest.Method = "get";
            webRequest.UseDefaultCredentials = true;
            using (WebResponse response = await webRequest.GetResponseAsync())
            {
                using (Stream inStream = response.GetResponseStream())
                {
                    using (StreamReader reader = encoding != null ? new StreamReader(inStream, Encoding.GetEncoding(encoding)) : new System.IO.StreamReader(inStream))
                    {
                        result = reader.ReadToEnd();
                        reader.Close();
                    }
                }
                response.Close();
            }
            return result;
        }
        #region private
        #endregion
    }
}
