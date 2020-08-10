using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;

namespace PageComponents
{
    /// <summary>
    /// Extension methods to IWebDriver and IWebElement
    /// </summary>
    public static class WebExtensions
    {

        /// <summary>
        /// Highlight a web element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="ms"></param>
        /// <param name="color"></param>
        public static void Highlight(this IWebElement element, int ms = 30, string color = "yellow")
        {
            try
            {
                var jsDriver = ((IJavaScriptExecutor)((IWrapsDriver)element).WrappedDriver);
                var originalElementBorder = (string)jsDriver.ExecuteScript("return arguments[0].style.background", element);
                jsDriver.ExecuteScript($"arguments[0].style.background='{color}'; return;", element);
                Thread.Sleep(ms);
                jsDriver.ExecuteScript($"arguments[0].style.background='{originalElementBorder}'; return;", element);
            }
            catch (Exception e)
            {
                throw;
            }
        }

        /// <summary>
        /// Waits for any ajax http requests to finish executing
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="timeoutMs"></param>
        public static void WaitForAjax(this IWebDriver driver, int timeoutMs)
        {
            try
            {
                var wait = new WebDriverWait(driver, TimeSpan.FromMilliseconds(timeoutMs));

                wait.Until(
                    drv => (bool) drv.ExecuteJavaScript("(typeof jQuery === \"undefined\" || jQuery.active==0);"));
                wait.Until(drv =>
                    (bool) drv.ExecuteJavaScript("(typeof Ajax === \"undefined\" || Ajax.activeRequestCount ==0); "));
                wait.Until(drv =>
                    (bool) drv.ExecuteJavaScript(
                        "(typeof dojo === \"undefined\" || dojo.io.XMLHTTPTransport.inFlight.length==0);"));

            }
            catch (Exception e)
            {
                
            }


        }

        /// <summary>
        /// Executes javascript in the browser
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="script"></param>
        /// <returns></returns>
        public static object ExecuteJavaScript(this IWebDriver driver, string script)
        {
            try
            {
                var js = (IJavaScriptExecutor)driver;
                return js.ExecuteScript("return " + script);
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// Finds all descendent webelements, and filters to return only the visible ones
        /// </summary>
        /// <param name="element"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static IEnumerable<IWebElement> FindVisibleElements(this IWebElement element, By by)
        {
            var eles = element.FindElements(by);
            if (eles.Count() == 0)
            {
                return new List<IWebElement>();
            }
            return eles.FilterVisibleElements();
        }

        /// <summary>
        /// Finds a visible element
        /// </summary>
        /// <param name="element"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static IWebElement FindVisibleElement(this IWebElement element, By by)
        {
            var eles = element.FindElements(by);
            if(eles.Count() == 0)
            {
                return null;
            }
            return eles.FilterVisibleElement();
        }

        /// <summary>
        /// Finds all elements that are visible
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static IEnumerable<IWebElement> FindVisibleElements(this IWebDriver driver, By by)
        {
            var eles = driver.FindElements(by);
            if (eles.Count() == 0)
            {
                return new List<IWebElement>();
            }
            return eles.FilterVisibleElements();
        }

        /// <summary>
        /// Finds a single visible element
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="by"></param>
        /// <returns></returns>
        public static IWebElement FindVisibleElement(this IWebDriver driver, By by)
        {
            var eles = driver.FindElements(by);
            if (eles.Count() == 0)
            {
                return null;
            }
            return eles.FilterVisibleElement();
        }

        /// <summary>
        /// Given a collection of webelements, returns all that are visible
        /// Will return a blank list if no visible elements are found
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static IEnumerable<IWebElement> FilterVisibleElements(this IEnumerable<IWebElement> elements)
        {
            return elements.Where(x => x.Displayed);
        }

        /// <summary>
        /// Given a collection of elements, returns the first visible element.
        /// Throws a WebDriverException if no visible elements are found
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        public static IWebElement FilterVisibleElement(this IEnumerable<IWebElement> elements)
        {
            var ele = elements.Where(x => x.Displayed);
            return ele != null && ele.Count() > 0 ? ele.First() : elements.First();
        }

        public static bool IsStale(this IWebElement element)
        {
            try
            {
                if(element == null)
                {
                    return true;
                }
                var enabled = element.Enabled;
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

    }
}
