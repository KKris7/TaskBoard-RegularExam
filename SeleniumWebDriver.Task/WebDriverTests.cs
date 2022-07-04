using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace SeleniumWebDriver.Task
{
    public class WebDriverTests
    {
        private WebDriver driver;
        //private const string URL = "http://localhost:8080/";
        private const string URL = "https://taskboard.nakov.repl.co/";
        private static Random random = new Random();


        [OneTimeSetUp]
        public void OpenBrowser()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(15);
        }

        [OneTimeTearDown]
        public void CloseBrowser()
        {
            driver.Quit();
        }

        [Test]
        public void TestFirstTaskFromDone()
        {
            // arrange
            driver.Navigate().GoToUrl(URL);
            driver.FindElement(By.LinkText("Task Board")).Click();

            //act
            var title = driver.FindElement(By.CssSelector("#task1 > tbody > tr.title > td")).Text;

            // assert
            Assert.That(title, Is.EqualTo("Project skeleton"));
        }

        [Test]
        public void TestSearchTask()
        {
            // arrange
            driver.Navigate().GoToUrl(URL);
            driver.FindElement(By.LinkText("Search")).Click();

            //act
            var keywordField = driver.FindElement(By.Id("keyword"));
            keywordField.Click();
            keywordField.SendKeys("home");
            driver.FindElement(By.Id("search")).Click();
            var title = driver.FindElement(By.CssSelector("tbody > tr.title > td")).Text;

            // assert
            Assert.That(title, Is.EqualTo("Home page"));
        }

        [Test]
        public void TestSearchMissingTask()
        {
            // arrange
            driver.Navigate().GoToUrl(URL);
            driver.FindElement(By.LinkText("Search")).Click();

            //act
            var keywordField = driver.FindElement(By.Id("keyword"));
            keywordField.Click();

            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string randnumString = new string(Enumerable.Repeat(chars, 10).Select(s => s[random.Next(s.Length)]).ToArray());
            keywordField.SendKeys(randnumString);
           
            driver.FindElement(By.Id("search")).Click();
            var searchResult = driver.FindElement(By.CssSelector("#searchResult"));

            // assert
            Assert.That(searchResult.Text, Is.EqualTo("No tasks found."));
        }


        [Test]
        public void TestCreateTaskInvalidData()
        {
            // arrange
            driver.Navigate().GoToUrl(URL);
            driver.FindElement(By.LinkText("Create")).Click();

            //act
            driver.FindElement(By.Id("create")).Click();
            var errorMessage = driver.FindElement(By.CssSelector("body>main>div")).Text;

            // assert
            Assert.IsNotEmpty(errorMessage);
            Assert.That(errorMessage, Is.EqualTo("Error: Title cannot be empty!"));
        }

        [Test]
        public void TestCreateNew()
        {
            // arrange
            driver.Navigate().GoToUrl(URL);
            driver.FindElement(By.LinkText("Create")).Click();

            var title = "Kris" + DateTime.Now.Ticks;
            var description = "Description" + DateTime.Now.Ticks;

            //act
            var firstNameField = driver.FindElement(By.Id("title"));
            firstNameField.SendKeys(title);

            var lastNameField = driver.FindElement(By.Id("description"));
            lastNameField.SendKeys(description);

            var boardName = driver.FindElement(By.Id("boardName"));
            boardName.SendKeys("Done");
            boardName.Click();


            driver.FindElement(By.Id("create")).Click();

            
            var allTasks = driver.FindElements(By.CssSelector("table.task-entry"));
            var lastTask = allTasks.Last();

            var lastTaskTitle = lastTask.FindElement(By.CssSelector("tr.title>td")).Text;

            // assert
            Assert.That(lastTaskTitle, Is.EqualTo(title));
        }
    }
}