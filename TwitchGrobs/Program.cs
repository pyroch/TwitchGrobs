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
        const string title = "TwitchGrobs-0.5.3";

        static List<string> onlineList = new List<string>();
        static List<string> alreadyWatched = new List<string>();

        static IWebDriver driver;

        //xpaths to elements
        const string livePath = "/html/body/div[1]/div/div[2]/div/main/div[2]/div[3]/div/div/div[2]/div[1]/div[2]/div/div[1]/div/div/div/div[1]/a/div/div/div/div[2]/div/div/div/p";
        const string profileButton = "/html/body/div[1]/div/div[2]/nav/div/div[3]/div[6]/div/div/div/div/button";
        const string dropProgress = "/html/body/div[5]/div/div/div/div/div/div/div/div/div/div/div/div[3]/div/div/div[1]/div[9]/a/div/div[2]/p[2]";
        //

        static void Init()
        {
            Console.Title = title;
            Console.WriteLine("Google Chrome will be closed. Make sure you OK with that, otherwise press 'N'.");

            ConsoleKeyInfo cki = Console.ReadKey();

            if (cki.Key.ToString().ToLower() == "n")
                Environment.Exit(0);

            foreach (var process in Process.GetProcessesByName("chrome"))
                process.Kill();


            ChromeOptions options = new ChromeOptions();
            options.AddArgument("user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
            options.AddArgument("--log-level=3");

            driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            Console.Clear();
        }

        static void Main()
        {
            GetCustomList();
            Console.WriteLine("List of streamers found.\n");
            Init();
            CustomListChecks();
            BrowseLogic();
        }

        static void BrowseLogic()
        {
            int currentStreamer = 0;
            while (true)
            {
                if (onlineList.Count == 0)
                {
                    Console.WriteLine("Nothing to watch... Waiting 15 minutes.");
                    driver.Navigate().GoToUrl("https://www.twitch.tv/drops/inventory");
                    System.Threading.Thread.Sleep(900000);
                    CustomListChecks();
                }

                if (currentStreamer < onlineList.Count)
                {
                    driver.Navigate().GoToUrl("https://twitch.tv/" + onlineList[currentStreamer]);
                    try
                    {
                        driver.FindElement(By.XPath(profileButton)).Click(); // Clicking on profile button to get % of drop
                        System.Threading.Thread.Sleep(3000);
                        var percent = driver.FindElement(By.XPath(dropProgress)).GetAttribute("textContent"); // percentage xpath that being cut from whole text. Its different on other languages, thats why english twitch is needed.
                        var perName = percent.Substring(percent.LastIndexOf('/') + 1);
                        Console.WriteLine(perName);
                        if (perName != onlineList[currentStreamer].ToLower()) // checks if streamer page is the same as progressing one
                        {
                            Console.WriteLine("Wrong streamer. Switching...");
                            for(int i = 0; i < onlineList.Count; i++)
                            {
                                if (onlineList[i].ToLower() == perName)
                                {
                                    currentStreamer = i;
                                }
                            }
                        }
                        else
                        {
                            Console.Clear();
                            Console.WriteLine("Currently watching " + onlineList[currentStreamer]);

                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            while (sw.Elapsed < TimeSpan.FromMinutes(15)) // while cycle for 15 munutes, after that we're getting the list of streamers again. Also shows the % of drop in real time and if its 100% breaks cycle and claim the drop
                            {
                                System.Threading.Thread.Sleep(10); // reducing CPU usage
                                percent = driver.FindElement(By.XPath(dropProgress)).GetAttribute("textContent").GetUntilOrEmpty();
                                Console.Write("\rPercentage of drop: {0}    ", percent);
                                if (percent == "100")
                                {
                                    Console.WriteLine();
                                    Console.WriteLine("100% on one of drops. Claiming and switching streamer.");
                                    ClaimDrop();
                                    alreadyWatched.Add(onlineList[currentStreamer]);
                                    currentStreamer++;
                                    break;
                                }
                            }
                            Console.WriteLine();
                            CustomListChecks(); // Looking through all streamers list every 15 to make sure they're still online.
                            Console.WriteLine();
                        }
                    }
                    catch
                    {
                        Console.WriteLine("No drops here now... Switching in a minute.");
                        currentStreamer++;
                        System.Threading.Thread.Sleep(60000);
                    }
                }
                else if(onlineList.Count != 0)
                {
                    currentStreamer = 0;
                    CustomListChecks();
                }
                System.Threading.Thread.Sleep(10); // reducing CPU usage
            }
        }

        static void ClaimDrop()
        {
            driver.Navigate().GoToUrl("https://www.twitch.tv/drops/inventory");
            System.Threading.Thread.Sleep(3000);
            var buttons = driver.FindElements(By.XPath("//button[@data-test-selector ='DropsCampaignInProgressRewardPresentation-claim-button']"));
            foreach(var button in buttons)
            {
                button.Click();
                System.Threading.Thread.Sleep(1000);
            }
        }

        static void GetCustomList()
        {
            if(File.Exists(@".\streamers.txt"))
            {
                try
                {
                    if (onlineList.Count == 0)
                    {
                        var logFile = File.ReadAllLines(@".\streamers.txt");
                        for (int i = 0; i < logFile.Length; i++)
                        {
                            logFile[i] = logFile[i].Remove(0, 22);
                        }
                        onlineList = new List<string>(logFile);
                        onlineList = onlineList.Distinct().ToList(); //remove duplicates if there is any in file for some reason
                    }
                    return;
                }
                catch
                {
                    Console.Clear();
                    Console.WriteLine("The list has wrong format. Program will be closed.");
                    Console.ReadKey();
                    Environment.Exit(0);
                }
            }
            Console.Clear();
            Console.WriteLine("There is no streamers.txt file with list of streamers. Program will be closed.");
            Console.ReadKey();
            Environment.Exit(0);
        }

        static void CustomListChecks()
        {
            onlineList.Clear();
            GetCustomList();
            Console.Clear();

            Console.WriteLine("Checking streamers status.");

            List<string> offlineList = new List<string>();

            onlineList.RemoveAll(item => alreadyWatched.Contains(item)); // removing the ones that was already watched
            foreach (var guy in onlineList)
            {
                driver.Navigate().GoToUrl("https://twitch.tv/" + guy);
                try
                {
                    if (driver.FindElement(By.XPath(livePath)).Displayed)
                        Console.WriteLine(guy + " is Live.");
                }
                catch
                {
                    Console.WriteLine(guy + " is Offline.");
                    offlineList.Add(guy);
                }
            }
            onlineList.RemoveAll(item => offlineList.Contains(item)); // removing all items from main list that contained in offlineList
        }
    }
}
