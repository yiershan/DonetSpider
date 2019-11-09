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

        Stream inStream = null;
        Stream outStream = null;
        StreamReader reader = null;
        WebResponse response = null;
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
        public bool SavePhotoFromUrl(string FileName, string Url)
        {
            bool Value = false;

            try
            {
                webRequest = (HttpWebRequest)WebRequest.Create(Url);

                response = webRequest.GetResponse();

                if (!response.ContentType.ToLower().StartsWith("text/"))
                {
                    Value = SaveBinaryFile(response,FileName);
                }
                response.Close();

            }
            catch (Exception err)
            {
                string aa = err.ToString();
            }
            return Value;
            }


        /// <summary>
        /// Save a binary file to disk.
        /// </summary>
        /// <param name="response">The response used to save the file</param>
        // 将二进制文件保存到磁盘
        private bool SaveBinaryFile(WebResponse response, string FileName)
        {
            bool Value = true;
            byte[] buffer = new byte[1024];

            try
            {
                if (File.Exists(FileName))
                    File.Delete(FileName);
                inStream = response.GetResponseStream();
                outStream = System.IO.File.Create(FileName);
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
            catch
            {
                Value = false;
            }
            return Value;
        }
        public string GetHTMLByURL(string url, string encoding = null, string ContentType = null)
        {
            try
            {
                webRequest = (HttpWebRequest)WebRequest.Create(url);
                webRequest.ContentType = ContentType ?? _contentType;
                webRequest.Method = "get";
                webRequest.UseDefaultCredentials = true;
                var task = webRequest.GetResponseAsync();
                response = task.Result;
                inStream = response.GetResponseStream();
                reader = encoding != null ? new StreamReader(inStream, Encoding.GetEncoding(encoding)) : new System.IO.StreamReader(inStream);
                var result = reader.ReadToEnd();
                response.Close();
                return result;
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
        bool SavePhotoFromUrl(string FileName, string Url);
    }

}
