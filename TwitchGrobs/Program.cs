using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
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
        const string title = "TwitchGrobs-0.2";

        static List<string> onlineList = new List<string>();
        static List<int> exclusion = new List<int>();

        static void Main()
        {
            Console.Title = title;
            Console.WriteLine("Google Chrome is going to be closed. Make sure you okay with that, otherwise press 'N' (program will be closed)");

            ConsoleKeyInfo cki = Console.ReadKey();

            if (cki.Key.ToString().ToLower() == "n")
                return;

            foreach (var process in Process.GetProcessesByName("chrome"))
                process.Kill();

            var options = new ChromeOptions();
            options.AddArgument("user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
            options.AddArgument("--log-level=3");
            //ChromeDriverService service = ChromeDriverService.CreateDefaultService(); // might use later
            //service.SuppressInitialDiagnosticInformation = true; // might use later

            using (IWebDriver driver = new ChromeDriver(options))
            {
                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                int currentStreamer = 0;

                if (!CustomList())
                {
                    StreamerCheck(driver);
                    Exclusion();
                }

                while (true)
                {
                    if (currentStreamer < onlineList.Count)
                    {
                        driver.Navigate().GoToUrl("https://twitch.tv/" + onlineList[currentStreamer]);
                        
                        System.Threading.Thread.Sleep(5000);
                        try
                        {
                            driver.FindElement(By.XPath("/html/body/div[1]/div/div[2]/nav/div/div[3]/div[6]/div/div/div/div/button")).Click(); // Clicking on profile button to get % of drop
                            System.Threading.Thread.Sleep(1000);
                            var percent = driver.FindElement(By.XPath("/html/body/div[5]/div/div/div/div/div/div/div/div/div/div/div/div[3]/div/div/div[1]/div[9]/a/div/div[2]/p[2]")).GetAttribute("textContent"); // percentage xpath that being cut from whole text. Its different on other languages, thats why english twitch is needed.
                            var perName = percent.Substring(percent.LastIndexOf('/') + 1);
                            if (perName != onlineList[currentStreamer].ToLowerInvariant()) // checks if streamer page is the same as progressing one
                            {
                                Console.WriteLine("Watching the wrong streamer. Switching...");
                                currentStreamer++;
                            }
                            else
                            {
                                Console.Clear();
                                Console.WriteLine("Currently watching " + onlineList[currentStreamer]);

                                Stopwatch sw = new Stopwatch();
                                sw.Start();
                                while (sw.Elapsed < TimeSpan.FromMinutes(15)) // while cycle for 15 munutes, after that we're getting the list of streamers again. Also shows the % of drop in real time and if its 100% breaks cycle and claim the drop
                                {
                                    System.Threading.Thread.Sleep(10); // reducing CPU use
                                    percent = driver.FindElement(By.XPath("/html/body/div[5]/div/div/div/div/div/div/div/div/div/div/div/div[3]/div/div/div[1]/div[9]/a/div/div[2]/p[2]")).GetAttribute("textContent").GetUntilOrEmpty();
                                    Console.Write("\rPercentage of drop: {0}    " , percent);
                                    if (percent == "100")
                                    {
                                        Console.WriteLine("100% on one of drops. Claiming and switching streamer.");
                                        ClaimDrop(driver);
                                        currentStreamer++;
                                        break;
                                    }
                                }

                                if(!CustomList())
                                    StreamerCheck(driver); // checking streamers after 15 minutes, incase the one we were watching went off.
                            }
                        }
                        catch
                        {
                            Console.WriteLine("No drops progression... Refreshing streamer list and switching in a minute.");
                            currentStreamer++;
                            System.Threading.Thread.Sleep(60000); // need to change later
                            if (!CustomList())
                                StreamerCheck(driver);
                        }
                    }
                    else
                    {
                        currentStreamer = 0;
                    }
                    System.Threading.Thread.Sleep(10); // Less CPU usage
                }
            }
        }

        static void ClaimDrop(IWebDriver driver)
        {
            driver.Navigate().GoToUrl("https://www.twitch.tv/drops/inventory");
            System.Threading.Thread.Sleep(5000);

            driver.FindElement(By.XPath("//button[@data-test-selector ='DropsCampaignInProgressRewardPresentation-claim-button']")).Click();
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
                    var streamerName = driver.FindElement(By.XPath(streamerHeader)).FindElement(By.ClassName("drop-item__header-streamer")).FindElement(By.ClassName("username"));

                    string statusHeader = $"/html/body/div[1]/div[2]/div[{y}]/a[{x}]/div[1]";
                    var status = driver.FindElement(By.XPath(statusHeader)).FindElement(By.ClassName("drop-item__header-status")).GetAttribute("textContent");
                    status = string.Join("", status.Split(default(string[]), StringSplitOptions.RemoveEmptyEntries));

                    if (status == "Live")
                    {
                        onlineList.Add(streamerName.GetAttribute("textContent"));
                    }
                }
            }

            foreach (var number in exclusion.OrderByDescending(v => v))
            {
                onlineList.RemoveAt(number);
            }

            Console.Clear();
            foreach (var a in onlineList)
                Console.WriteLine(a + " is live.");
            Console.WriteLine();
        }

        static void Exclusion()
        {
            Console.Clear();
            for (int i = 0; i < onlineList.Count; i++)
            {
                Console.WriteLine("(" + i + ") " + onlineList[i]);
            }
            while (true)
                try
                {
                    Console.WriteLine("Write down the numbers of streamers you want to exclude, then press Enter. (For example: '0, 1, 3') Or press 'N' to skip.");
                    string kb = Console.ReadLine();
                    if (kb.ToLower() == "n")
                        break;
                    exclusion = kb.Split(',').Select(Int32.Parse).ToList();
                    break;
                }
                catch
                {
                    Console.WriteLine("Incorrect input!");
                }
            foreach (var number in exclusion.OrderByDescending(v => v))
            {
                onlineList.RemoveAt(number);
            }
            Console.Clear();
        }

        static bool CustomList()
        {
            if(File.Exists(@".\streamers.txt"))
            {
                try
                {
                    if (onlineList.Count == 0)
                    {
                        Console.Clear();
                        Console.WriteLine("Custom streamers list found.");
                        var logFile = File.ReadAllLines(@".\streamers.txt");
                        for (int i = 0; i < logFile.Length; i++)
                        {
                            logFile[i] = logFile[i].Remove(0, 22);
                        }
                        onlineList = new List<string>(logFile);
                    }
                    return true;
                }
                catch
                {
                    Console.WriteLine("The list has wrong format.");
                    return false;
                }
            }
            return false;
        }
    }
}
