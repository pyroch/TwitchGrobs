using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TwitchGrobs
{

    public partial class TwitchGrobsForm : Form
    {
        static IWebDriver driver;

        static List<string> onlineList = new List<string>();
        static List<string> alreadyWatched = new List<string>();

        const string livePath = "/html/body/div[1]/div/div[2]/div/main/div[2]/div[3]/div/div/div[1]/div[1]/div[2]/div/div[1]/div/div/div/div[1]/div/div/a/div[2]/div/div/div/div/p";
        const string offPath = "/html/body/div[1]/div/div[2]/div/main/div[2]/div[3]/div/div/div[1]/div[1]/div[1]/div[2]/div/div/div/div[2]/div[1]/div[1]/div/div[1]/div/p";
        const string profileButton = "/html/body/div[1]/div/div[2]/nav/div/div[3]/div[6]/div/div/div/div/button";
        const string dropProgress = "/html/body/div[5]/div/div/div/div/div/div/div/div/div/div/div/div[3]/div/div/div[1]/div[9]/a/div/div[2]/p[2]";

        Thread browseThread;

        public TwitchGrobsForm()
        {

            InitializeComponent();
            Init();
            browseThread = new Thread(BrowseLogic);
            browseThread.IsBackground = true;
            browseThread.Start();
        }

        void Init()
        {
            GetCustomList();

            var procs = Process.GetProcessesByName("chrome");
            if (procs.Length != 0)
            {
                if (MessageBox.Show("Google Chrome will be closed. Make sure you OK with that.", "TwitchGrobs", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    foreach (var process in Process.GetProcessesByName("chrome"))
                        process.Kill();
                }
                else
                {
                    Environment.Exit(0);
                }
            }

            ChromeOptions options = new ChromeOptions();
            options.AddArgument("user-data-dir=C:\\Users\\" + Environment.UserName + "\\AppData\\Local\\Google\\Chrome\\User Data");
            options.AddArgument("--log-level=off");
            options.AddArgument("--no-sandbox");
            options.AddArgument("--disable-gpu");
            var chromeDriverService = ChromeDriverService.CreateDefaultService();
            chromeDriverService.HideCommandPromptWindow = true;
            driver = new ChromeDriver(chromeDriverService, options, TimeSpan.FromMinutes(5));
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);

            //CustomListChecks();
        }

        void GetCustomList()
        {
            if (File.Exists(@".\streamers.txt"))
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
                    MessageBox.Show("The list has wrong format. Program will be closed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Environment.Exit(0);
                }
            }
            MessageBox.Show("There is no streamers.txt file with list of streamers. Program will be closed.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Environment.Exit(0);
        }

        void CustomListChecks()
        {
            onlineList.Clear();
            GetCustomList();

            StatusLog("Checking streamers status...");
            StatusLog("------------------");

            List<string> offlineList = new List<string>();

            onlineList.RemoveAll(item => alreadyWatched.Contains(item)); // removing the ones that was already watched
            foreach (var guy in onlineList)
            {
                driver.Navigate().GoToUrl("https://twitch.tv/" + guy);
                try
                {
                    var status = driver.FindElement(By.XPath(livePath + " | " + offPath));
                    if (status.Displayed)
                    {
                        if (status.Text == "OFFLINE" || status.Text == "HOSTING")
                        {
                            StatusLog(guy + " is Offline.");
                            offlineList.Add(guy);
                        }
                        if (status.Text == "LIVE")
                            StatusLog(guy + " is Live.");
                    }
                }
                catch
                {
                    StatusLog(guy + " incorrect/offline.");
                    offlineList.Add(guy);
                }
            }
            onlineList.RemoveAll(item => offlineList.Contains(item)); // removing all items from main list that contained in offlineList
            StatusLog("------------------");
        }

        void BrowseLogic()
        {
            int currentStreamer = 0;
            while (true)
            {
                if (onlineList.Count == 0)
                {
                    StatusLog("Nothing to watch... Waiting 15 minutes.");
                    StatusLog("Nothing to watch... Waiting 15 minutes.");
                    driver.Navigate().GoToUrl("https://www.twitch.tv/drops/inventory");
                    System.Threading.Thread.Sleep(900000);
                    CustomListChecks();
                }

                if (currentStreamer < onlineList.Count)
                {
                    StreamerLog(onlineList[currentStreamer]);
                    driver.Navigate().GoToUrl("https://twitch.tv/" + onlineList[currentStreamer]);
                    try
                    {
                        driver.FindElement(By.XPath(profileButton)).Click(); // Clicking on profile button to get % of drop
                        var percent = driver.FindElement(By.XPath(dropProgress)).GetAttribute("textContent"); // percentage xpath that being cut from whole text. Its different on other languages, thats why english twitch is needed.
                        var perName = percent.Substring(percent.LastIndexOf('/') + 1);
                        if (perName == onlineList[currentStreamer].ToLower()) // checks if streamer page is the same as progressing one
                        {
                            StatusLog("Currently watching " + onlineList[currentStreamer]);

                            Stopwatch sw = new Stopwatch();
                            sw.Start();
                            while (sw.Elapsed < TimeSpan.FromMinutes(15)) // while cycle for 15 munutes, after that we're getting the list of streamers again. Also shows the % of drop in real time and if its 100% breaks cycle and claim the drop
                            {
                                System.Threading.Thread.Sleep(100); // reducing CPU usage
                                percent = driver.FindElement(By.XPath(dropProgress)).GetAttribute("textContent").GetUntilOrEmpty();
                                StatusLog("Percentage of drop: " + percent.ToString());
                                if (percent == "100")
                                {
                                    //StatusLog();
                                    StatusLog("100% on one of drops. Claiming and switching streamer.");
                                    ClaimDrop();
                                    alreadyWatched.Add(onlineList[currentStreamer]);
                                    currentStreamer++;
                                    break;
                                }
                            }
                            //StatusLog();
                            CustomListChecks(); // Looking through all streamers list every 15 to make sure they're still online.
                            //StatusLog();
                        }
                        else
                        {
                            if (!onlineList.Contains(perName, StringComparer.OrdinalIgnoreCase))
                            {
                                StatusLog("No drops here now... Switching in a minute.");
                                currentStreamer++;
                                System.Threading.Thread.Sleep(60000);
                            }
                            else
                            {
                                StatusLog("Wrong streamer. Switching...");
                                for (int i = 0; i < onlineList.Count; i++)
                                {
                                    if (onlineList[i].ToLower() == perName)
                                    {
                                        currentStreamer = i;
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {
                        StatusLog("No drops here now... Switching in a minute.");
                        currentStreamer++;
                        System.Threading.Thread.Sleep(60000);
                    }
                }
                else if (onlineList.Count != 0)
                {
                    currentStreamer = 0;
                    CustomListChecks();
                }
                System.Threading.Thread.Sleep(10); // reducing CPU usage
            }
        }

        void ClaimDrop()
        {
            driver.Navigate().GoToUrl("https://www.twitch.tv/drops/inventory");
            System.Threading.Thread.Sleep(3000);
            var buttons = driver.FindElements(By.XPath("//button[@data-test-selector ='DropsCampaignInProgressRewardPresentation-claim-button']"));
            foreach (var button in buttons)
            {
                button.Click();
                System.Threading.Thread.Sleep(1000);
            }
        }


        void StatusLog(string text)
        {
            if (InvokeRequired)
                Invoke((Action<string>)StatusLog, text);
            else
                status.Text = text;
        }

        void StreamerLog(string text)
        {
            if (InvokeRequired)
                Invoke((Action<string>)StreamerLog, text);
            else
                currStreamer.Text = text;
        }

        void TwitchGrobsForm_UnLoad(object sender, EventArgs e)
        {
            browseThread.Abort();
        }
    }

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
}
