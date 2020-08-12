using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.Events;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace PageComponents
{
    public class EventedWebDriver : IWebDriver, IWrapsDriver
    {
        public EventFiringWebDriver EventFiringWebDriver { get; set; }
        public IWebDriver WebDriver { get; set; }
        public string Url { get => ((IWebDriver)EventFiringWebDriver).Url; set => ((IWebDriver)EventFiringWebDriver).Url = value; }

        public string Title => ((IWebDriver)EventFiringWebDriver).Title;

        public string PageSource => ((IWebDriver)EventFiringWebDriver).PageSource;

        public string CurrentWindowHandle => ((IWebDriver)EventFiringWebDriver).CurrentWindowHandle;

        public ReadOnlyCollection<string> WindowHandles => ((IWebDriver)EventFiringWebDriver).WindowHandles;

        public IWebDriver WrappedDriver => WebDriver;

        public EventedWebDriver(IWebDriver driver)
        {
            this.WebDriver = driver;
            this.EventFiringWebDriver = new EventFiringWebDriver(driver);
            this.EventFiringWebDriver.Navigating += WebDriver_Navigating;
            this.EventFiringWebDriver.NavigatingBack += WebDriver_NavigatingBack;
            this.EventFiringWebDriver.NavigatedForward += WebDriver_NavigatedForward;
        }

        private void WebDriver_NavigatedForward(object sender, WebDriverNavigationEventArgs e)
        {
            Logger.Log("Navigating forward");
        }

        private void WebDriver_NavigatingBack(object sender, WebDriverNavigationEventArgs e)
        {
            Logger.Log("Navigating Back");
        }

        private void WebDriver_Navigating(object sender, WebDriverNavigationEventArgs e)
        {
            Logger.Log($"Navigating to url {e.Url}");
        }

        public void Close()
        {
            ((IWebDriver)EventFiringWebDriver).Close();
        }

        public void Quit()
        {
            ((IWebDriver)EventFiringWebDriver).Quit();
        }

        public IOptions Manage()
        {
            return ((IWebDriver)EventFiringWebDriver).Manage();
        }

        public INavigation Navigate()
        {
            return ((IWebDriver)EventFiringWebDriver).Navigate();
        }

        public ITargetLocator SwitchTo()
        {
            return ((IWebDriver)EventFiringWebDriver).SwitchTo();
        }

        public IWebElement FindElement(By by)
        {
            return ((IWebDriver)EventFiringWebDriver).FindElement(by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            return ((IWebDriver)EventFiringWebDriver).FindElements(by);
        }

        public void Dispose()
        {
            ((IWebDriver)EventFiringWebDriver).Dispose();
        }
    }
}
