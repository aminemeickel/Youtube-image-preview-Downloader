
using ScrapySharp.Network;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Youtube_Downloader
{
    public static class DataController
    {
        [DllImport("wininet.dll")]
        private extern static bool InternetGetConnectedState(out int description, int reservedValue);

        public static bool IsInternetAvailable()
                => InternetGetConnectedState(out _, 0);

       public static bool IsAlink(string link) =>
            string.IsNullOrEmpty(link) ? false : Regex.IsMatch(link, @"^https://www.youtube.com/watch\?v=.{5,}$");

       public static async Task<WebPage> BrowserAsync(string link)
        {

            var url = link;
            ScrapingBrowser scrapingBrowser = new ScrapingBrowser();
            scrapingBrowser.Timeout = TimeSpan.FromSeconds(40);
            try
            {
               return await Task.Run(() => scrapingBrowser.NavigateToPage(new Uri(url)));
               
            }
            catch (Exception e)
            {
                MessageBox.Show(e.StackTrace);
            }
            return null;
        }

       public static string FetchImage(WebPage htmlPage)
        => htmlPage.Html.Descendants("link")
               .ToList()
               .Where(node => node.GetAttributeValue("rel", "not found") == "image_src")
               .First()
               .GetAttributeValue("href", "not found");
    }
}
