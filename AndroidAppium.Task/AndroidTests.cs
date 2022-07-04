using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Support.UI;
using System;
using System.Threading;

namespace Appium_Android_Tests
{
    public class AndroidTests
    {
        private const string Url = "http://127.0.0.1:4723/wd/hub";
        private const string realUrl = "https://taskboard.nakov.repl.co/api";
        private const string appLocation = @"C:\Exam.19.06\taskboard-androidclient.apk";
        private AndroidDriver<AndroidElement> driver;
        private AppiumOptions options;

        [SetUp]
        public void Setup()
        {
            options = new AppiumOptions() { PlatformName = "Android" };
            options.AddAdditionalCapability("app", appLocation);
            driver = new AndroidDriver<AndroidElement>(new Uri(Url), options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(7);
        }

        [TearDown]
        public void ShutDown()
        {
            driver.Quit();
        }

        [Test]
        public void TestAddTaskValid()
        {
            var conectionField = driver.FindElementById("taskboard.androidclient:id/editTextApiUrl");
            conectionField.Clear();
            conectionField.SendKeys(realUrl);
            var connectButton = driver.FindElementById("taskboard.androidclient:id/buttonConnect");
            connectButton.Click();

            var title = driver.FindElementById("taskboard.androidclient:id/textViewTitle");
            Assert.AreEqual("Project skeleton", title.Text);

            var taskTitle = "Kris" + DateTime.Now.Ticks; ;
            var taskDescription = "Description" + DateTime.Now.Ticks;

            driver.FindElementById("taskboard.androidclient:id/buttonAdd").Click();
            driver.FindElementById("taskboard.androidclient:id/editTextTitle").SendKeys(taskTitle);
            driver.FindElementById("taskboard.androidclient:id/editTextDescription").SendKeys(taskDescription);
            driver.FindElementById("taskboard.androidclient:id/buttonCreate").Click();

            var wait = new WebDriverWait(driver, TimeSpan.FromSeconds(7));
            var result = driver.FindElementById("taskboard.androidclient:id/textViewStatus");
            wait.Until(t => result.Text.Contains("created"));

            Assert.That(result.Text.Contains("created."));

            var textBoxSearch = driver.FindElementById("taskboard.androidclient:id/editTextKeyword");
            textBoxSearch.SendKeys(taskTitle);
            driver.FindElementById("taskboard.androidclient:id/buttonSearch").Click();

            Thread.Sleep(3000);

            Assert.That(taskTitle, Is.EqualTo(title.Text));

        }

    }
}