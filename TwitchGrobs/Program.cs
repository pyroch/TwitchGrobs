using System;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;

namespace TwitchGrobs
{
    static class Helper
    {
        public static string GetUntilOrEmpty(this string text, string stopAt = "%")
        {
            if (!String.IsNullOrWhiteSpace(text))
            {
                int charLocation = text.IndexOf(stopAt, StringComparison.Ordinal);

                if (charLocation > 0)
                {
                    return text.Substring(0, charLocation);
                }
            }
            return String.Empty;
        }
    }

    class Program
    {
        static List<string> onlineList = new List<string>();
        static int currentOnline = 0;

        static void Main(string[] args)
        {
            var options = new ChromeOptions();

            options.AddArgument("user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
            using (IWebDriver driver = new ChromeDriver(options))
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                int currentStreamer = 0;

                StreamerCheck(driver);

                
                while (true)
                {
                    if (currentStreamer < currentOnline)
                    {
                        driver.Navigate().GoToUrl("https://twitch.tv/" + onlineList[currentStreamer]);
                        System.Threading.Thread.Sleep(5000);
                        try
                        {
                        var button = driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/nav/div/div[3]/div[6]/div/div/div/div/button"));
                        button.Click();
                        System.Threading.Thread.Sleep(1000);
                        
                            var percent = driver.FindElement(By.XPath("/html/body/div[5]/div/div/div/div/div/div/div/div/div/div/div/div[3]/div/div/div[1]/div[9]/a/div/div[2]/p[2]")).GetAttribute("textContent");
                            Console.WriteLine(percent.GetUntilOrEmpty());

                            System.Threading.Thread.Sleep(900000); // Timer between checks streamers that live and how much percent is done.
                            //System.Threading.Thread.Sleep(10000);
                            if (percent == "100")
                            {
                                ClaimDrop(driver);
                                currentStreamer++;
                            }
                            else
                            {
                                StreamerCheck(driver);
                            }
                        }
                        catch
                        {
                            currentStreamer++;
                            System.Threading.Thread.Sleep(10000);
                        }
                        
                    }
                    else
                    {
                        currentStreamer = 0;
                    }
                }
               
            }
        }

        static void ClaimDrop(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://www.twitch.tv/drops/inventory");
            System.Threading.Thread.Sleep(5000);

            //var claimButton = driver.FindElement(By.LinkText("Claim Now"));
            //driver.FindElement(By.XPath("//button[contains(text(), 'Claim')]")).Click();
            driver.FindElement(By.XPath("//button[@data-test-selector ='DropsCampaignInProgressRewardPresentation-claim-button']")).Click();

            //claimButton.Click();
        }

        static void StreamerCheck(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://twitch.facepunch.com/");
            onlineList.Clear();
            System.Threading.Thread.Sleep(5000);
            for (int x = 1; x <= 3; x++)
            {
                for (int y = 2; y <= 4; y++)
                {
                    string streamerHeader = $"/html/body/div[1]/div[2]/div[{y}]/a[{x}]/div[1]";
                    string statusHeader = $"/html/body/div[1]/div[2]/div[{y}]/a[{x}]/div[1]";
                    var a = driver.FindElement(By.XPath(streamerHeader));
                    a = a.FindElement(By.ClassName("drop-item__header-streamer"));
                    a = a.FindElement(By.ClassName("username"));

                    var status = driver.FindElement(By.XPath(statusHeader)).FindElement(By.ClassName("drop-item__header-status")).GetAttribute("textContent");

                    status = string.Join("", status.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                    if (status == "Live")
                    {
                        onlineList.Add(a.GetAttribute("textContent"));
                        currentOnline++;
                    }

                    //Console.WriteLine(a.GetAttribute("textContent") + status.GetAttribute("textContent"));
                }
            }
            foreach (var a in onlineList)
                Console.WriteLine("Live: " + a);
        }
    }
}
