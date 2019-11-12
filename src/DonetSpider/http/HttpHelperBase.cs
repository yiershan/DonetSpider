using DonetSpider.Log;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DonetSpider.http
{
    public abstract class HttpHelperBase: HasLog,IHttpHelper
    {
       

        public IHttpHelper SetLogger(ILog log)
        {
            this._log = log;
            return this;
        }
        public string GetHost(string url)
        {
            Uri URL = new Uri(url);
            return URL.Scheme + "://" + URL.Host;
        }
        #region IDisposable Support
        protected bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        ~HttpHelperBase()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose();
        }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            GC.SuppressFinalize(this);
        }
        #endregion
        public async Task<string> GetHTMLByURLAsync(string url, string encoding = null, string ContentType = null) {
            var result = "";
            try
            {
                Debugger($"访问页面：{url}");
                result = await  _GetHTMLByURLAsync(url, encoding, ContentType);
                Debugger($"页面{url}加载完成！");
            }
            catch (Exception ex) {
                Error($"访问页面{url}出错：{ex.Message}");
            }
            return result;
        }
        public async Task SavePhotoFromUrlAsync(string FileName, string Url) {
            try
            {
                Debugger($"开始下载：{Url}");
                await _SavePhotoFromUrlAsync(FileName, Url);
                Debugger($"下载{Url}完成！");
            }
            catch (Exception ex)
            {
                Error($"下载{Url}报错：{ex.Message}");
            }
        }
        public virtual void SavePhotoFromUrl(string FileName, string Url)
        {
            this.SavePhotoFromUrlAsync(FileName, Url).Wait();
        }
        public virtual string GetHTMLByURL(string url, string encoding = null, string ContentType = null)
        {
            return this.GetHTMLByURLAsync(url, encoding, ContentType).GetAwaiter().GetResult();
        }

        protected abstract Task<string> _GetHTMLByURLAsync(string url, string encoding = null, string ContentType = null);
        protected abstract Task _SavePhotoFromUrlAsync(string FileName, string Url);
    }
}
