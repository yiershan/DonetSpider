using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DonetSpider.http
{

    public interface IHttpHelper:IDisposable
    {
        /// <summary>
        /// 获取页面
        /// </summary>
        /// <param name="url"></param>
        /// <param name="encoding"></param>
        /// <param name="ContentType"></param>
        /// <returns></returns>
        Task<string> GetHTMLByURLAsync(string url, string encoding = null, string ContentType = null);
        string GetHTMLByURL(string url, string encoding = null, string ContentType = null);
        /// <summary>
        /// 获取域名
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        string GetHost(string url);
        /// <summary>
        /// 下载图片
        /// </summary>
        /// <param name="FileName"></param>
        /// <param name="Url"></param>
        /// <returns></returns>
        Task SavePhotoFromUrlAsync(string FileName, string Url);
        void SavePhotoFromUrl(string FileName, string Url);
    }
}
