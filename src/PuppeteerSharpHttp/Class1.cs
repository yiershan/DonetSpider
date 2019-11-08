using PuppeteerSharp;
using System;
using System.Threading.Tasks;

namespace PuppeteerSharpHttp
{
    public class Class1
    {
        private const string Url = "https://store.mall.autohome.com.cn/83106681.html";
        private const int ChromiumRevision = BrowserFetcher.DefaultRevision;

        private static void Main(string[] args)
        {
            //Download chromium browser revision package
            new BrowserFetcher().DownloadAsync(ChromiumRevision).Wait();

            var htmlString = TestPuppeteerSharpAsync().Result;
            Console.ReadKey();
        }

        private static async Task<string> TestPuppeteerSharpAsync()
        {
            //Enabled headless option
            var launchOptions = new LaunchOptions { Headless = true };
            //Starting headless browser
            var browser = await Puppeteer.LaunchAsync(launchOptions);

            //New tab page
            var page = await browser.NewPageAsync();
            //Request URL to get the page
            await page.GoToAsync(Url);

            //Get and return the HTML content of the page
            var htmlString = await page.GetContentAsync();

            #region Dispose resources
            //Close tab page
            await page.CloseAsync();

            //Close headless browser, all pages will be closed here.
            await browser.CloseAsync();
            #endregion

            return htmlString;
        }
    }
}
