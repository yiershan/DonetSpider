using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace DonetSpider.http
{
    public class HttpHelper: IHttpHelper
    {
        string _encoding;
        string _contentType;
        CookieContainer cookies;
        HttpWebRequest webRequest;
        public HttpHelper(string encoding = "GB2312", string contentType = "application/x-www-form-urlencoded") {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            this._encoding = encoding;
            this._contentType = contentType;
            cookies = new CookieContainer();
        }
        public void Login(string LOGIN_URL,string postData, string type= "POST", string ContentType = null) {
            webRequest = (HttpWebRequest)WebRequest.Create(LOGIN_URL);
            webRequest.Method = type;
            webRequest.ContentType = ContentType??_contentType;
            webRequest.CookieContainer = cookies;
            StreamWriter requestWriter = new StreamWriter(webRequest.GetRequestStream());
            requestWriter.Write(postData);
            requestWriter.Close();
            StreamReader responseReader = new StreamReader(webRequest.GetResponse().GetResponseStream());
            string responseData = responseReader.ReadToEnd();
            responseReader.Close();
            webRequest.GetResponse().Close();
            CookieCollection cookieheader = webRequest.CookieContainer.GetCookies(new Uri(LOGIN_URL));
            cookies.Add(cookieheader);
        }
        public string GetHTMLByURL(string url, string encoding= null, string ContentType= null)
        {
            try
            {
                webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.ContentType = ContentType??_contentType;
                webRequest.Method = "get";
                webRequest.UseDefaultCredentials = true;
                var task = webRequest.GetResponseAsync();
                System.Net.WebResponse wResp = task.Result;
                System.IO.Stream respStream = wResp.GetResponseStream();
                using (System.IO.StreamReader reader = new System.IO.StreamReader(respStream, Encoding.GetEncoding(encoding??_encoding)))
                {
                    return reader.ReadToEnd();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return string.Empty;
            }
        }
        public string GetHost(string url)
        {
            Uri URL = new Uri(url);
            return URL.Scheme+ "://"+URL.Host;
        }
    }

    public interface IHttpHelper {
        string GetHTMLByURL(string url, string encoding = null, string ContentType = null);
        string GetHost(string url);
    }

}
