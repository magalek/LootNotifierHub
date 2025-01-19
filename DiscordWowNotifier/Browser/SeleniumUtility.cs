using System.IO;
using DiscordWowNotifier.Data;
using DiscordWowNotifier.Utility;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;

namespace DiscordWowNotifier.Browser;

public static class SeleniumUtility
{
    public const string ImgInfoPng = "img/info.png";
    public const string ImgImagePng = "img/image.png";

    public static async Task<string> GetItemInfo(string itemId)
    {
        var options = new ChromeOptions();
        //options.AddArgument("--disable-blink-features=AutomationControlled");
        options.AddArgument("--headless"); // Enable headless mode
        options.AddArgument("--disable-gpu"); // Optional: Disable GPU for compatibility
        new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
        var service = ChromeDriverService.CreateDefaultService();
        service.HideCommandPromptWindow = true;
        IWebDriver driver = new ChromeDriver(service, options);

        try
        {
            // Load the webpage
            await driver.Navigate().GoToUrlAsync($"https://www.wowauctions.net/auctionHouse/turtle-wow/nordanaar/mergedAh/{itemId}");


            await Task.Delay(600);
            Actions actions = new Actions(driver);
            actions.ScrollByAmount(0, 200).Perform();
                
            var consentElement = driver.FindElement(By.XPath("/html/body/div[3]/div[2]/div[2]/div[2]/div[2]/button[1]/p"));
            consentElement?.Click();

            var divElement = driver.FindElement(By.XPath("//*[@id=\"item_card\"]/div[2]/div[1]"));

            var imgElement = driver.FindElement(By.XPath("//*[@id=\"item_card\"]/div[1]"));

            IWebElement currencyElement;
            try
            {
                currencyElement =
                    driver.FindElement(
                        By.XPath("//*[@id=\"price_stats\"]/div[2]/table/tbody/tr[3]/td[2]/table"));
            }
            catch (Exception e)
            {
                currencyElement =
                    driver.FindElement(
                        By.XPath("//*[@id=\"price_stats\"]/div[3]/table/tbody/tr[3]/td[2]/table"));
            }
            var currencyText = currencyElement.Text;
            Console.WriteLine(currencyText);
            
            // Take a screenshot of the element
            Screenshot infoScreenshot = ((ITakesScreenshot)divElement).GetScreenshot();
            Screenshot imageScreenshot = ((ITakesScreenshot)imgElement).GetScreenshot();

            if (!Directory.Exists("img")) Directory.CreateDirectory("img");
            
            // Save it as a file
            string screenshotPath = ImgInfoPng;
            string screenshot1Path = ImgImagePng;
            infoScreenshot.SaveAsFile(screenshotPath);
            imageScreenshot.SaveAsFile(screenshot1Path);

            
            
            Logger.Log($"Screenshot saved at: {screenshotPath}");
            Logger.Log($"Screenshot saved at: {screenshot1Path}");
            return currencyText;
        }
        finally
        {
            driver.Quit();
        }
    }
}