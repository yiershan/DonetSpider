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
    public class PuppeteerSharpHttpHelper : IHttpHelper
    {
        private readonly object _lock = new object();
        private Browser browser = null;
        ~PuppeteerSharpHttpHelper() {
            if (this.browser != null) {
                this.browser.CloseAsync().Wait();
                Console.WriteLine("消亡一个http! 一个browser!");
            }
        }
        public string GetHost(string url)
        {
            Uri URL = new Uri(url);
            return URL.Scheme + "://" + URL.Host;
        }
        public PuppeteerSharpHttpHelper() {
            new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision).Wait();
        }
        public string GetHTMLByURL(string url, string encoding = null, string ContentType = null)
        {
            return PuppeteerSharpAsync(url).GetAwaiter().GetResult();
        }
        private Browser GetBrowser() {
            if (browser == null)
            {
                lock (_lock) {
                    if (browser == null) {
                        var launchOptions = new LaunchOptions { Headless = true };
                        browser = Puppeteer.LaunchAsync(launchOptions).GetAwaiter().GetResult();
                        Console.WriteLine("开启 一个browser!");
                    }
                }
            }
            return browser;
        }
        private async Task<string> PuppeteerSharpAsync(string url)
        {
            string htmlString = "";

            //New tab page
            using (Page page = await GetBrowser().NewPageAsync())
            {
                //Request URL to get the page

                await page.GoToAsync(url, WaitUntilNavigation.DOMContentLoaded); // 等待页面js加载完成
                htmlString = page.GetContentAsync().GetAwaiter().GetResult();

                #region Dispose resources
                //Close tab page
                await page.CloseAsync();
            }
            #endregion

            return htmlString;
        }

        public bool SavePhotoFromUrl(string FileName, string Url)
        {
            using (Page page = GetBrowser().NewPageAsync().GetAwaiter().GetResult())
            {
                page.GoToAsync(Url).Wait();
                var image = page.WaitForSelectorAsync("img").GetAwaiter().GetResult();
                image.ScreenshotAsync(FileName).Wait();
                page.CloseAsync().Wait();
            }
            return true;
        }
    }
}
