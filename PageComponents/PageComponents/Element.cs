﻿using System;
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
        public int Index = 0;

        /// <summary>
        /// The locator associated with this element
        /// </summary>
        public By By
        {
            get { return _by; }
            set { _by = value; }
        }

        /// <summary>
        /// An iframe which the element lives inside.  Frame will be automatically selected prior to finding
        /// </summary>
        public Frame Frame
        {
            get { return _frame; }
        }

        /// <summary>
        /// The root container element.  This element is used for all FindElement() searches, limiting
        /// search scope to descendents.  This helps if there are multiple elements on the page,
        /// or to speed up finding elements
        /// </summary>
        public Element Container
        {
            get { return _container; }
            set { _container = value; }
        }

        /// <summary>
        /// The time in milliseconds the element will be searched for before failing
        /// </summary>
        public int TimeoutMs
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
            get { return _driver != null ? _driver : DriverManager.WebDriver; }
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
        public Element(string cssSelector, int timeoutMs = -1)
        {

            this._by = By.CssSelector(cssSelector);
            this._timeoutMs = timeoutMs == -1 ? WebConfig.ElementTimeoutMs : timeoutMs;
        }

        /// <summary>
        /// Instntiate an Element using a css selector and the default timeout
        /// </summary>
        /// <param name="by"></param>
        public Element(string cssSelector, Frame frame, int timeoutMs = -1)
        {

            this._by = By.CssSelector(cssSelector);
            this._frame = frame;
            this._timeoutMs = timeoutMs == -1 ? WebConfig.ElementTimeoutMs : timeoutMs;
        }

        /// <summary>
        /// Instntiate an Element using a By locator and the default timeout
        /// </summary>
        /// <param name="by"></param>
        public Element(By by, int timeoutMs = -1)
        {

            this._by = by;
            this._timeoutMs = timeoutMs == -1 ? WebConfig.ElementTimeoutMs : timeoutMs;
        }

        /// <summary>
        /// Instantiate an Element with an iframe and a by locator
        /// Frame will be selected before the element is found, so you don't have to 
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="by"></param>
        public Element(By by, Frame frame, int timeoutMs = -1)
        {
            this._frame = frame;
            this._by = by;
            this._timeoutMs = timeoutMs == -1 ? WebConfig.ElementTimeoutMs : timeoutMs;

        }

        /// <summary>
        /// Instntiates an Element using a root container, and a by locator.
        /// The Element will only search in descendents of the root container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="by"></param>
        public Element(Element container, By by, int timeoutMs = -1)
        {
            this._container = container;
            this._by = by;
            this._timeoutMs = timeoutMs == -1 ? WebConfig.ElementTimeoutMs : timeoutMs;

        }

        /// <summary>
        /// Instntiates an Element using a root container, and a css locator.
        /// The Element will only search in descendents of the root container
        /// </summary>
        /// <param name="container"></param>
        /// <param name="by"></param>
        public Element(Element container, string cssSelector, int timeoutMs = -1)
        {
            this._container = container;
            this._by = By.CssSelector(cssSelector);
            this._timeoutMs = timeoutMs == -1 ? WebConfig.ElementTimeoutMs : timeoutMs;

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
            this._container = container;
            this._frame = frame;
            this._by = by;
            this._timeoutMs = timeoutMs == -1 ? WebConfig.ElementTimeoutMs : timeoutMs;

        }

        public Element(IWebElement webElement, By by, int index)
        {
            this._webelement = webElement;
            this._by = by;
            this.Index = index;
        }

        /// <summary>
        /// Overrides the ToString() function to make a nice loggable message
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_container != null)
            {
                return $"Element '{_by}' with container '{_container}'";
            }
            if (Index != 0)
            {
                //Want the log to be clear that this comes from a list
                return $"Element '{_by}' [{Index}]";

            }
            return $"Element '{_by}'";
        }

        public IWebElement WaitForMe()
        {
            var wait = new WebDriverWait(WrappedDriver, TimeSpan.FromMilliseconds(_timeoutMs));
            wait.Message = $"{this} was not found";
            wait.Until(driver => !FindMe().IsStale());
             _webelement.Highlight();
            return _webelement;
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
            WrappedDriver.WaitForAjax(_timeoutMs);

            //look for a non-null reference in the cache            
            if (_webelement == null)
            {
                _webelement = WebElementCache.GetCachedElement(this.ToString(), Index);
            }

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
                        Logger.Log($"Select frame {_frame}");

                        WrappedDriver.SwitchTo().Frame(_frame.WaitForMe());
                    }

                    //if FindHidden is false, only find visible elements
                    if (!FindHidden)
                    {
                        Logger.Log($"Finding visible {this}");

                        _webelement = root.FindVisibleElement(_by);
                    }
                    else
                    {
                        Logger.Log($"Finding {this}");

                        //find any element, even if its hidden
                        _webelement = root.FindElement(_by);
                    }
                }
                else
                {
                    //select the iframe if appropriate
                    if (_frame != null)
                    {
                        Logger.Log($"Selecting frame {_frame}");

                        WrappedDriver.SwitchTo().Frame(_frame.WaitForMe());
                    }
                    //find hidden elements if appropriate
                    if (!FindHidden)
                    {
                        Logger.Log($"Finding visible {this}");

                        _webelement = WrappedDriver.FindVisibleElement(_by);
                    }
                    //find any element including hidden ones
                    else
                    {
                        Logger.Log($"Finding {this}");

                        _webelement = WrappedDriver.FindElement(_by);
                    }


                }
                //save the found element to the cache
                WebElementCache.SaveElementToCache(_webelement, this.ToString(), Index);
            }

            //highlight the element so the user can see what element was found
            //            _webelement.Highlight();
            return _webelement;
        }


        /// <summary>
        /// checks to see if the element is stale or hasn't been found yet
        /// </summary>
        /// <returns></returns>
        public bool IsStale()
        {
            try
            {
                if (_webelement == null)
                {
                    return true;
                }

                //if the element reference is good, this will pass
                var enabled = _webelement.Enabled;
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
                    return _webelement != null;
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
