using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
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
        protected By _by;
        protected Frame _frame;
        protected Element _container;
        protected int _timeoutMs;
        protected IWebElement _webelement;
        protected IWebDriver _driver;
        protected bool _findHidden = false;
        protected ExtentTest test => ExtentManager.GetTest();

        /// <summary>
        /// The locator associated with this element
        /// </summary>
        public By by
        {
            get { return _by; }
        }

        /// <summary>
        /// An iframe which the element lives inside.  Frame will be automatically selected prior to finding
        /// </summary>
        public Frame frame
        {
            get { return _frame; }
        }

        /// <summary>
        /// The root container element.  This element is used for all FindElement() searches, limiting
        /// search scope to descendents.  This helps if there are multiple elements on the page,
        /// or to speed up finding elements
        /// </summary>
        public Element container
        {
            get { return _container; }
            set { _container = value; }
        }

        /// <summary>
        /// The time in milliseconds the element will be searched for before failing
        /// </summary>
        public int timeoutMs
        {
            get { return _timeoutMs; }
            set { _timeoutMs = value; }
        }

        /// <summary>
        /// The IWebDriver instance associated with this element
        /// By default it will use the driver associated with the test
        /// </summary>
        public IWebDriver WrappedDriver
        {
            get { return _driver != null ? _driver : DriverManager.GetDriver(); }
            set { _driver = value; }
        }

        /// <summary>
        /// THe actual IWebElement that is found using selenium.  
        /// </summary>
        public IWebElement WrappedElement
        {
            get { return _webelement; }
            set { _webelement = value; }
        }

        /// <summary>
        /// Determines whether or not hidden elements should be found.
        /// By default, the Element class will exclude non-visible elements
        /// To find hidden elements, set this to be false for the element.
        /// </summary>
        public bool FindHidden
        {
            get { return _findHidden; }
            set { _findHidden = value; }
        }

//        /// <summary>
//        /// Instantiates an element without a by locator 
//        /// </summary>
        public Element()
        {
            this._timeoutMs = WebConfig.ElementTimeoutMs;
        }

        /// <summary>
        /// Instntiate an Element using a css selector and the default timeout
        /// </summary>
        /// <param name="by"></param>
        public Element(string cssSelector)
        {

            this._by = By.CssSelector(cssSelector);
            this._timeoutMs = WebConfig.ElementTimeoutMs;
        }

        /// <summary>
        /// Instntiate an Element using a By locator and the default timeout
        /// </summary>
        /// <param name="by"></param>
        public Element(By by)
        {
            
            this._by = by;
            this._timeoutMs = WebConfig.ElementTimeoutMs;
        }

        /// <summary>
        /// Instantiate an Element with an iframe and a by locator
        /// Frame will be selected before the element is found, so you don't have to 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="by"></param>
        public Element(Frame frame, By by)
        {
            this._frame = frame;
            this._by = by;
            this._timeoutMs = WebConfig.ElementTimeoutMs;
        }

        /// <summary>
        /// Instntiates an Element using a root container, and a by locator.
        /// The Element will only search in descendents of the root container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="by"></param>
        public Element(Element container, By by)
        {
            this._container = container;
            this._by = by;
            this._timeoutMs = WebConfig.ElementTimeoutMs;
        }

        /// <summary>
        /// Instntiates an Element using a root container, and a css locator.
        /// The Element will only search in descendents of the root container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="by"></param>
        public Element(Element container, string cssSelector)
        {
            this._container = container;
            this._by = By.CssSelector(cssSelector);
            this._timeoutMs = WebConfig.ElementTimeoutMs;
        }

        /// <summary>
        /// Instntiates an Element using a root container, an iframe, and a by locator.
        /// The container will be found first, then the iframe selected, then the element found
        /// </summary>
        /// <param name="container"></param>
        /// <param name="frame"></param>
        /// <param name="by"></param>
        public Element(Element container,  Frame frame, By by)
        {
            this._container = container;
            this._frame = frame;
            this._by = by;
            this._timeoutMs = WebConfig.ElementTimeoutMs;
        }

        /// <summary>
        /// Overrides the ToString() function to make a nice loggable message
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"Element '{_by}'";
        }

        /// <summary>
        /// FindMe() selects the iframe, and waits for the element to appear.  Once found the IWebElement
        /// is stored in the WrappedElement for future use
        /// Waits an amount of time specified by the timeoutMs value.
        /// </summary>
        /// <returns></returns>
        public IWebElement FindMe()
        {
            var wait = new WebDriverWait(WrappedDriver, TimeSpan.FromMilliseconds(_timeoutMs));
            wait.Message = $"{this} was not found";

            //wait for all ajax requests to finish
            WrappedDriver.WaitForAjax(_timeoutMs);

            //If the element was already found, and has not gone stale, return the previously found IWebElement
            if (IsStale())
            {
                WrappedDriver.SwitchTo().DefaultContent();
                //if the container is not null, use it as the root element (search in descendents)
                if (_container != null)
                {
                    //find the root element using the container's findMe
                    var root = _container.FindMe();
                    //select an iframe if necessary
                    if (_frame != null)
                    {
                        WrappedDriver.SwitchTo().Frame(_frame.FindMe());
                    }

                    //if FindHidden is false, only find visible elements
                    if (!FindHidden)
                    {
                        _webelement = wait.Until(drv => root.FindVisibleElement(_by));
                    }
                    else
                    {
                        //find any element, even if its hidden
                        _webelement = wait.Until(drv => root.FindElement(_by));
                    }
                    

                }
                else
                {
                    //select the iframe if appropriate
                    if (_frame != null)
                    {
                        WrappedDriver.SwitchTo().Frame(_frame.FindMe());
                    }
                    //find hidden elements if appropriate
                    if (!FindHidden)
                    {
                        _webelement = wait.Until(drv => drv.FindVisibleElement(_by));
                    }
                    //find any element including hidden ones
                    else
                    {
                        _webelement = wait.Until(drv => drv.FindElement(_by));
                    }
                    

                }
            }
            //highlight the element so the user can see what element was found
            _webelement.Highlight();
            return _webelement;
        }


        public IWebElement WaitForMe()
        {
            WebDriverWait wait = new WebDriverWait(this.WrappedDriver,TimeSpan.FromMilliseconds(timeoutMs));
            wait.Until(x => FindMe().Displayed);
            return WrappedElement;
        }
        /// <summary>
        /// checks to see if the element is stale or hasn't been found yet
        /// </summary>
        /// <returns></returns>
        public bool IsStale()
        {
            try
            {
                //if the element reference is good, this will pass
                var enabled = _webelement.Enabled;
                return false;
            }
            catch (Exception e)
            {
                //if the element is stale or null an esception will be thrown
                return true;
            }
        }

        public string TagName {
            get { return FindMe().TagName; }    
        }

        public string Text
        {
            get { return FindMe().Text; }
        }

        public bool Enabled
        {
            get { return FindMe().Enabled; }
        }

        public bool Selected
        {
            get { return FindMe().Selected; }
        }

        public Point Location
        {
            get { return FindMe().Location; }
        }

        public Size Size
        {
            get { return FindMe().Size; }
        }

        public bool Displayed
        {
            get { return Present && FindMe().Displayed;  }
        }

        public bool Present
        {
            get
            {
                try
                {
                    FindMe();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
                
            }
        }
        public Element Clear()
        {
            test.Info($"Clearing {this}");
            FindMe().Clear();
           return this;
        }

        public Element Click()
        {
            test.Info($"Clicking {this}");
            FindMe().Click();
            return this;
        }

        public Element Hover()
        {
            test.Info($"Mouse Hover over {this}");
            Actions().MoveToElement(FindMe()).Build().Perform();
            return this;
        }

        public Actions Actions()
        {
            return new Actions(WrappedDriver);
        }

        public IWebElement FindElement(By by)
        {
            test.Info($"Finding child of {this} with '{by}'");
            return FindMe().FindElement(by);
        }

        public ReadOnlyCollection<IWebElement> FindElements(By by)
        {
            test.Info($"Finding children of {this} with '{by}'");
            return FindMe().FindElements(by);
        }

        public string GetAttribute(string attributeName)
        {
            return FindMe().GetAttribute(attributeName);
        }

        public string GetCssValue(string propertyName)
        {
            return FindMe().GetCssValue(propertyName);
        }

        public string GetProperty(string propertyName)
        {
            return FindMe().GetProperty(propertyName);
        }

        public Element SendKeys(string text)
        {
            test.Info($"Sending keys '{text}' into {this}");

            FindMe().SendKeys(text);
            return this;
        }

        public Element Submit()
        {
            test.Info($"Submitting {this}");

            FindMe().Submit();
            return this;
        }

        public Element SetText(string value)
        {
            test.Info($"Setting {this} text to {value}");
            FindMe().Clear();
            FindMe().SendKeys(value);
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
            FindMe();
            return new SelectElement(WrappedElement);
        }

        
    }
}
