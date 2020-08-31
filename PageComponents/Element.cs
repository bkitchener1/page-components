using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AventStack.ExtentReports;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Internal;
using OpenQA.Selenium.Support.UI;

namespace PageComponents
{
    /// <summary>
    /// The Element class represents a single IWebElement.
    /// It allows us to instantiate it in the class header of a page object or test class
    /// It wraps around the IWebElement, and implements the same functions, but automatically finds,
    /// waits, and caches the IWebElement so duplicate findElement() calls aren't needed.
    /// If needed the IWebElement can be gotten from the WrappedElement.
    /// It handles selecting frames automatically, and can be instntiated using a root container Elemenent,
    /// which limits the search scope to descendents.   

    /// </summary>
    public class Element : IWrapsElement, IWrapsDriver
    {
        public By By { get; set; }
        public Frame Frame { get; set; }
        public Element Container { get; set; }
        protected string Name { get; set; }
        public int TimeoutMs { get; set; }
        protected IWebElement WebElement { get; set; }
        protected IWebDriver Driver { get; set; }
        public bool FindHidden { get; set; } = false;
        public int Index { get; set; } = 0;

        /// <summary>
        /// The IWebDriver instance associated with this element
        /// By default it will use the driver associated with the test
        /// </summary>
        public IWebDriver WrappedDriver
        {
            get { return Driver != null ? Driver : DriverManager.WebDriver; }
            set { Driver = value; }
        }

        /// <summary>
        /// THe actual IWebElement that is found using selenium.  
        /// </summary>
        public IWebElement WrappedElement
        {
            get { return WebElement; }
            set { WebElement = value; }
        }


        //        /// <summary>
        //        /// Instantiates an element without a by locator 
        //        /// </summary>
        public Element()
        {
            this.TimeoutMs = TestContext.CurrentContext.TestConfig.ElementTimeoutMs;
            this.Name = GetNameFromStackTrace();
        }

        /// <summary>
        /// Instntiate an Element using a css selector and the default timeout
        /// </summary>
        /// <param name="by"></param>
        public Element(string cssSelector, int timeoutMs = -1)
        {

            this.By = By.CssSelector(cssSelector);
            this.TimeoutMs = timeoutMs == -1 ? TestContext.CurrentContext.TestConfig.ElementTimeoutMs : timeoutMs;
            this.Name = GetNameFromStackTrace();
        }

        /// <summary>
        /// Instntiate an Element using a css selector and the default timeout
        /// </summary>
        /// <param name="by"></param>
        public Element(string cssSelector, Frame frame, int timeoutMs = -1)
        {

            this.By = By.CssSelector(cssSelector);
            this.Frame = frame;
            this.TimeoutMs = timeoutMs == -1 ? TestContext.CurrentContext.TestConfig.ElementTimeoutMs : timeoutMs;
            this.Name = GetNameFromStackTrace();
        }

        /// <summary>
        /// Instntiate an Element using a By locator and the default timeout
        /// </summary>
        /// <param name="by"></param>
        public Element(By by, int timeoutMs = -1)
        {

            this.By = by;
            this.TimeoutMs = timeoutMs == -1 ? TestContext.CurrentContext.TestConfig.ElementTimeoutMs : timeoutMs;
            this.Name = GetNameFromStackTrace();
        }

        /// <summary>
        /// Instantiate an Element with an iframe and a by locator
        /// Frame will be selected before the element is found, so you don't have to 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="by"></param>
        public Element(By by, Frame frame, int timeoutMs = -1)
        {
            this.Frame = frame;
            this.By = by;
            this.TimeoutMs = timeoutMs == -1 ? TestContext.CurrentContext.TestConfig.ElementTimeoutMs : timeoutMs;
            this.Name = GetNameFromStackTrace();
        }

        /// <summary>
        /// Instntiates an Element using a root container, and a by locator.
        /// The Element will only search in descendents of the root container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="by"></param>
        public Element(Element container, By by, int timeoutMs = -1)
        {
            this.Container = container;
            this.By = by;
            this.TimeoutMs = timeoutMs == -1 ? TestContext.CurrentContext.TestConfig.ElementTimeoutMs : timeoutMs;
            this.Name = GetNameFromStackTrace();
        }

        /// <summary>
        /// Instntiates an Element using a root container, and a css locator.
        /// The Element will only search in descendents of the root container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="by"></param>
        public Element(Element container, string cssSelector, int timeoutMs = -1)
        {
            this.Container = container;
            this.By = By.CssSelector(cssSelector);
            this.TimeoutMs = timeoutMs == -1 ? TestContext.CurrentContext.TestConfig.ElementTimeoutMs : timeoutMs;
            this.Name = GetNameFromStackTrace();
        }


        /// <summary>
        /// Instntiates an Element using a root container, an iframe, and a by locator.
        /// The container will be found first, then the iframe selected, then the element found
        /// </summary>
        /// <param name="container"></param>
        /// <param name="frame"></param>
        /// <param name="by"></param>
        public Element(Element container, By by, Frame frame, int timeoutMs = -1)
        {
            this.Container = container;
            this.Frame = frame;
            this.By = by;
            this.TimeoutMs = timeoutMs == -1 ? TestContext.CurrentContext.TestConfig.ElementTimeoutMs : timeoutMs;
            this.Name = GetNameFromStackTrace();
        }

        public Element(IWebElement webElement, By by, int index)
        {
            this.WebElement = webElement;
            this.By = by;
            this.Index = index;
            this.Name = GetNameFromStackTrace();
        }

        private string GetNameFromStackTrace()
        {
            StackTrace stackTrace = new StackTrace();
            foreach (var frame in stackTrace.GetFrames())
            {
                var type = frame.GetMethod().ReflectedType;
                var name = frame.GetMethod().Name;
                if (name != "Element" && (type.IsSubclassOf(typeof(BaseComponent)) || type.IsSubclassOf(typeof(PageComponentList<>)) || type.IsSubclassOf(typeof(BasePageObject))))
                {
                    return $"{type.Name}.{name.Replace("get_", "")}";
                }
            }
            return "Element";
        }

        /// <summary>
        /// Overrides the ToString() function to make a nice loggable message
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (Container != null)
            {
                return $"{Name} '{By}' with container '{Container}'";
            }
            if (Index != 0)
            {
                //Want the log to be clear that this comes from a list
                return $"{Name} '{By}' [{Index}]";

            }
            return $"{Name} '{By}'";
        }

        public IWebElement WaitForMe()
        {
            var wait = new WebDriverWait(WrappedDriver, TimeSpan.FromMilliseconds(TimeoutMs));
            wait.Message = $"{this} was not found";
            wait.Until(driver => !FindMe().IsStale());
            if (TestContext.CurrentContext.TestConfig.HighlightElements)
            {
                WebElement.Highlight();
            }
             
            return WebElement;
        }

        /// <summary>
        /// FindMe() selects the iframe, and searches for the element.  Once found the IWebElement
        /// is stored in the WrappedElement for future use
        /// Waits an amount of time specified by the timeoutMs value.
        /// </summary>
        /// <returns></returns>
        public IWebElement FindMe()
        {

            //wait for all ajax requests to finish
            WrappedDriver.WaitForAjax(TimeoutMs);

            //look for a non-null reference in the cache            
            if (WebElement == null)
            {
                WebElement = WebElementCache.GetCachedElement(this.ToString(), Index);
            }

            //If the element was already found, and has not gone stale, return the previously found IWebElement

            if (IsStale())
            {
                var stopwatch = Stopwatch.StartNew();
                WrappedDriver.SwitchTo().DefaultContent();
                //if the container is not null, use it as the root element (search in descendents)
                if (Container != null)
                {
                    //find the root element using the container's findMe
                    var root = Container.FindMe();
                    //select an iframe if necessary
                    if (Frame != null)
                    {
                        Logger.Log($"Select frame {Frame}");

                        WrappedDriver.SwitchTo().Frame(Frame.WaitForMe());
                    }

                    //if FindHidden is false, only find visible elements
                    if (!FindHidden)
                    {
                        Logger.Log($"Finding visible {this}");

                        WebElement = root.FindVisibleElement(By);
                    }
                    else
                    {
                        Logger.Log($"Finding {this}");

                        //find any element, even if its hidden
                        WebElement = root.FindElement(By);
                    }
                }
                else
                {
                    //select the iframe if appropriate
                    if (Frame != null)
                    {
                        Logger.Log($"Selecting frame {Frame}");

                        WrappedDriver.SwitchTo().Frame(Frame.WaitForMe());
                    }
                    //find hidden elements if appropriate
                    if (!FindHidden)
                    {
                        Logger.Log($"Finding visible {this}");

                        WebElement = WrappedDriver.FindVisibleElement(By);
                    }
                    //find any element including hidden ones
                    else
                    {
                        Logger.Log($"Finding {this}");

                        WebElement = WrappedDriver.FindElement(By);
                    }


                }
                stopwatch.Stop();
                Logger.Log($"{this} found after {stopwatch.ElapsedMilliseconds} ms");
                //save the found element to the cache
                WebElementCache.SaveElementToCache(WebElement, this.ToString(), Index);
            }

            return WebElement;
        }


        /// <summary>
        /// checks to see if the element is stale or hasn't been found yet
        /// </summary>
        /// <returns></returns>
        public bool IsStale()
        {
            try
            {
                if (WebElement == null)
                {
                    return true;
                }

                //if the element reference is good, this will pass
                var enabled = WebElement.Enabled;
                return false;
            }
            catch (Exception e)
            {
                //if the element is stale or null an exception will be thrown
                return true;
            }
        }

        public string TagName {
            get { return WaitForMe().TagName; }
        }

        public string Text
        {
            get { return WaitForMe().Text; }
        }

        public bool Enabled
        {
            get { return WaitForMe().Enabled; }
        }

        public bool Selected
        {
            get { return WaitForMe().Selected; }
        }

        public Point Location
        {
            get { return WaitForMe().Location; }
        }

        public Size Size
        {
            get { return WaitForMe().Size; }
        }

        public bool Displayed
        {
            get { return Present && WaitForMe().Displayed; }
        }

        public string Value
        {
            get { return GetAttribute("value"); }
        }

        public bool Present
        {
            get
            {
                try
                {
                    FindMe();
                    return WebElement != null;
                }
                catch (Exception e)
                {
                    return false;
                }
                
            }
        }
        public Element Clear()
        {
            WaitForMe().Clear();
            Logger.Log($"Clear {this}");

            return this;
        }

        public Element Click()
        {
            WaitForMe().Click();
            Logger.Log($"Click {this}");

            return this;
        }

        public Element Hover()
        {
            Actions().MoveToElement(WaitForMe()).Build().Perform();
            Logger.Log($"Mouse Hover over {this}");

            return this;
        }

        public Actions Actions()
        {
            return new Actions(WrappedDriver);
        }

        public IWebElement FindElement(By by)
        {
            //Logger.Log($"Finding child of {this} with '{by}'");
            return WaitForMe().FindElement(by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            //Logger.Log($"Finding children of {this} with '{by}'");
            return WaitForMe().FindElements(by);
        }

        public string GetAttribute(string attributeName)
        {
            return WaitForMe().GetAttribute(attributeName);
        }

        public string GetCssValue(string propertyName)
        {
            return WaitForMe().GetCssValue(propertyName);
        }

        public string GetProperty(string propertyName)
        {
            return WaitForMe().GetProperty(propertyName);
        }

        public Element SendKeys(string text)
        {
            WaitForMe().SendKeys(text);
            Logger.Log($"Send keys '{text}' into {this}");
            return this;
        }

        public Element Submit()
        {
            WaitForMe().Submit();
            Logger.Log($"Submit {this}");
            return this;
        }

        public Element SetText(string value)
        {
            WaitForMe().Clear();
            WaitForMe().SendKeys(value);
            Logger.Log($"Set {this} text to {value}");
            return this;

        }

        /// <summary>
        /// returns a new instance of the ElementVerifications class to verify a positive condition.
        /// is used in place of assertions
        /// element.Verify().IsDisplayed();
        /// </summary>
        /// <returns></returns>
        public ElementVerifications Verify()
        {
            return new ElementVerifications(this);
        }

        /// <summary>
        /// Returns an instance of the ElementVerifications class to verify a negative condition
        /// element.VerifyNot().IsDisplayed();
        /// </summary>
        /// <returns></returns>
        public ElementVerifications VerifyNot()
        {
            return new ElementVerifications(this, false);
        }

        /// <summary>
        /// returns an instance of the SelectElement class, used to select dropdown items
        /// </summary>
        /// <returns></returns>
        public SelectElement Select()
        {
            WaitForMe();
            return new SelectElement(WrappedElement);
        }

        
    }
}
