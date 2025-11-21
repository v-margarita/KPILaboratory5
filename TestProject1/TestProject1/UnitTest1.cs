using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI; 
using SeleniumExtras.WaitHelpers;
using System;

namespace LabWork5_Variant4
{
    public class SeleniumTests
    {
        private IWebDriver driver;
        private WebDriverWait wait;
        private const string BaseUrl = "https://the-internet.herokuapp.com";

        [SetUp]
        public void Setup()
        {
            var options = new ChromeOptions();
            options.AddUserProfilePreference("profile.default_content_setting_values.geolocation", 1);

            driver = new ChromeDriver(options);
            driver.Manage().Window.Maximize();
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
        }

        [TearDown]
        public void Teardown()
        {
            driver.Quit();
            driver.Dispose();
        }

        [Test]
        public void Test1_AddRemoveElements()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/add_remove_elements/");
            var addButton = driver.FindElement(By.XPath("//button[text()='Add Element']"));
            addButton.Click();
            var deleteButton = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("added-manually")));
            Assert.IsTrue(deleteButton.Displayed, "Кнопка Delete не з'явилася");
            deleteButton.Click();
            wait.Until(ExpectedConditions.InvisibilityOfElementLocated(By.ClassName("added-manually")));
        }

        [Test]
        public void Test2_Checkboxes()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/checkboxes");
            var checkboxes = driver.FindElements(By.CssSelector("#checkboxes input"));
            if (!checkboxes[0].Selected) checkboxes[0].Click();
            Assert.IsTrue(checkboxes[0].Selected);
            if (checkboxes[1].Selected) checkboxes[1].Click();
            Assert.IsFalse(checkboxes[1].Selected);
        }

        [Test]
        public void Test3_Dropdown()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/dropdown");
            var dropdownElement = driver.FindElement(By.Id("dropdown"));
            var selectElement = new SelectElement(dropdownElement);
            selectElement.SelectByText("Option 1");
            Assert.AreEqual("Option 1", selectElement.SelectedOption.Text);
            selectElement.SelectByValue("2");
            Assert.AreEqual("Option 2", selectElement.SelectedOption.Text);
        }

        [Test]
        public void Test4_Inputs()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/inputs");
            var inputField = driver.FindElement(By.TagName("input"));
            inputField.SendKeys("12345");
            string value = inputField.GetAttribute("value");
            Assert.AreEqual("12345", value);
        }

        [Test]
        public void Test5_StatusCodes()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/status_codes");
            driver.FindElement(By.LinkText("200")).Click();
            wait.Until(ExpectedConditions.UrlContains("200"));

            string pageSource = driver.PageSource;
            Assert.IsTrue(pageSource.Contains("This page returned a 200 status code"), "Не знайдено текст про 200 статус");
            driver.Navigate().GoToUrl($"{BaseUrl}/status_codes");

            driver.FindElement(By.LinkText("500")).Click();
            wait.Until(ExpectedConditions.UrlContains("500"));

            pageSource = driver.PageSource;
            Assert.IsTrue(pageSource.Contains("This page returned a 500 status code"), "Не знайдено текст про 500 статус");
        }

        [Test]
        public void Test6_DragAndDrop()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/drag_and_drop");
            var columnA = driver.FindElement(By.Id("column-a"));
            var columnB = driver.FindElement(By.Id("column-b"));
            Actions action = new Actions(driver);
            action.DragAndDrop(columnA, columnB).Build().Perform();
            Assert.AreEqual("B", columnA.Text);
        }

        [Test]
        public void Test7_ShiftingContent()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/shifting_content/menu");
            var menuItems = driver.FindElements(By.CssSelector("ul li a"));
            Assert.AreEqual(5, menuItems.Count);
            Assert.IsTrue(menuItems[0].GetAttribute("href").Contains("/"));
        }

        [Test]
        public void Test8_Geolocation()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/geolocation");
            driver.FindElement(By.XPath("//button[text()='Where am I?']")).Click();
            var latValue = wait.Until(ExpectedConditions.ElementIsVisible(By.Id("lat-value")));
            var longValue = driver.FindElement(By.Id("long-value"));
            Assert.IsNotEmpty(latValue.Text);
            Assert.IsNotEmpty(longValue.Text);
        }

        [Test]
        public void Test9_JavaScriptErrors()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/javascript_error");
            var logs = driver.Manage().Logs.GetLog(LogType.Browser);
            bool hasError = logs.Any(log => log.Level == LogLevel.Severe);
            Assert.IsTrue(hasError, "У консолі браузера не знайдено JS помилок!");

            foreach (var log in logs)
            {
                Console.WriteLine($"Console log: {log.Message}");
            }
        }

        [Test]
        public void Test10_ExitIntent()
        {
            driver.Navigate().GoToUrl($"{BaseUrl}/exit_intent");

            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript(@"
                const event = new MouseEvent('mouseleave', { 
                    'view': window, 
                    'bubbles': true, 
                    'cancelable': true, 
                    'clientY': -100 
                }); 
                document.dispatchEvent(event);
            ");

            var modal = wait.Until(ExpectedConditions.ElementIsVisible(By.ClassName("modal")));
            Assert.IsTrue(modal.Displayed, "Модальне вікно не з'явилося");
            driver.FindElement(By.XPath("//div[@class='modal-footer']/p")).Click();
        }
    }
}