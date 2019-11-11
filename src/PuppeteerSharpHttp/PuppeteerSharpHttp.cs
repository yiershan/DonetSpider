using DonetSpider.http;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PuppeteerSharpHttp
{
    /// <summary>
    /// 据说可以获取js执行完成后的页面数据
    /// </summary>
    public class PuppeteerSharpHttpHelper : HttpHelperBase
    {
        private Browser _browser = null;
        public PuppeteerSharpHttpHelper() {
            new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision).Wait();
            var launchOptions = new LaunchOptions { Headless = true };
            _browser = Puppeteer.LaunchAsync(launchOptions).GetAwaiter().GetResult();
        }
        protected override async Task<string> _GetHTMLByURLAsync(string url, string encoding, string ContentType)
        {
            string htmlString = "";
            using (Page page = await _browser.NewPageAsync())
            {
                await page.GoToAsync(url, WaitUntilNavigation.Networkidle2); // 等待页面js加载完成
                htmlString = await page.GetContentAsync();
                await page.CloseAsync();
            }
            return htmlString;
        }

        protected override async Task _SavePhotoFromUrlAsync(string FileName, string Url)
        {
            using (Page page = await _browser.NewPageAsync())
            {
                await page.GoToAsync(Url);
                var image = await page.WaitForSelectorAsync("img");
                await image.ScreenshotAsync(FileName);
                await page.CloseAsync();
            }
        }
        #region IDisposable Support

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。
                if (_browser != null)
                {
                    _browser.CloseAsync().Wait();
                    _browser.Dispose();
                }
                disposedValue = true;
            }
        }
        #endregion
    }
}
