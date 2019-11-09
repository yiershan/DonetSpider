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
        
        private async Task<string> PuppeteerSharpAsync(string url)
        {
            //Enabled headless option
            var launchOptions = new LaunchOptions { Headless = true };
            //Starting headless browser
            var browser = await Puppeteer.LaunchAsync(launchOptions);

            //New tab page
            var page = await browser.NewPageAsync();
            //Request URL to get the page
            
            await page.GoToAsync(url, WaitUntilNavigation.DOMContentLoaded); // 等待页面js加载完成
            
           
            //Get and return the HTML content of the page
            var htmlString = page.GetContentAsync().GetAwaiter().GetResult();

            #region Dispose resources
            //Close tab page
            await page.CloseAsync();

            //Close headless browser, all pages will be closed here.
            await browser.CloseAsync();
            #endregion

            return htmlString;
        }

        public bool SavePhotoFromUrl(string FileName, string Url)
        {
            throw new NotImplementedException();
        }
    }
}
