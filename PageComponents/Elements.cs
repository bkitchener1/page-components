using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using OpenQA.Selenium.Internal;


namespace PageComponents
{
    /// <summary>
    /// This class operates exactly like the Element class, except it represents a list of IWebElement
    /// instead of a single one.  Can be used in place of driver.FindElements() to autojmatically wait, log, and cache the results
    /// </summary>
    public class Elements : IEnumerable<Element>, IWrapsDriver
    {
        protected By _by;
        protected Frame _frame;
        protected Element _container;
        protected int _timeoutMs;
        private IEnumerable<IWebElement> _webElements;
        private List<Element> _elements;
        protected IWebDriver _driver;
        protected bool _findHidden = false;
        
        public List<Element> elements
        {
            get { return _elements; }
        }

        public IEnumerable<IWebElement> WrappedElements
        {
            get { return _webElements; }
        }

        public By by
        {
            get { return _by; }
        }

        public Frame frame
        {
            get { return _frame; }
        }

        public Element container
        {
            get { return _container; }
        }

        public int timeoutMs
        {
            get { return _timeoutMs; }
        }

        public IWebDriver WrappedDriver
        {
            get { return _driver != null ? _driver : DriverManager.WebDriver; }
            set { _driver = value; }
        }

        public bool FindHidden
        {
            get { return _findHidden; }
            set { _findHidden = value; }
        }

        public Elements()
        {
            this._timeoutMs = TestConfig.ElementTimeoutMs;
        }

        public Elements(string cssSelector, int timeoutMs = -1)
        {
            this._by = By.CssSelector(cssSelector);
            this._timeoutMs = timeoutMs == -1 ? TestConfig.ElementTimeoutMs : timeoutMs;
        }

        public Elements(By by, int timeoutMs = -1)
        {
            this._by = by;
            this._timeoutMs = timeoutMs == -1 ? TestConfig.ElementTimeoutMs : timeoutMs;
        }

        public Elements(By by, Frame frame, int timeoutMs = -1)
        {
            this._frame = frame;
            this._by = by;
            this._timeoutMs = timeoutMs == -1 ? TestConfig.ElementTimeoutMs : timeoutMs;
        }

        public Elements(Element container, By by, int timeoutMs = -1)
        {
            this._container = container;
            this._by = by;
            this._timeoutMs = timeoutMs == -1 ? TestConfig.ElementTimeoutMs : timeoutMs;
        }

        public Elements(Element container, By by, Frame frame, int timeoutMs = -1)
        {
            this._container = container;
            this._frame = frame;
            this._by = by;
            this._timeoutMs = timeoutMs == -1 ? TestConfig.ElementTimeoutMs : timeoutMs;
        }

        public override string ToString()
        {
            if (_container != null)
            {
                return $"Elements '{_by}' with container '{_container}'";
            }
            return $"Elements '{_by}'";
        }

        public bool IsStale()
        {
            try
            {
                if (_webElements == null || _webElements.Count() == 0)
                {
                    return true;
                }
                foreach (var ele in _webElements)
                {
                  var enabled = ele.Enabled;
                }
                return false;
            }
            catch (Exception e)
            {
                return true;
            }
        }

        private IEnumerable<IWebElement> FindMe()
        {
            WrappedDriver.WaitForAjax(_timeoutMs);
            _webElements = WebElementsCache.GetCachedElements(this.ToString());
            if (IsStale())
            {

                if (_container != null)
                {
                    var root = _container.FindMe();
                    if (_frame != null)
                    {
                        Logger.Log($"Select frame {_frame}");
                        WrappedDriver.SwitchTo().Frame(_frame.FindMe());
                    }

                    if (FindHidden)
                    {
                        Logger.Log($"Find {this}");
                        _webElements =  root.FindElements(_by);
                    }
                    else
                    {
                        Logger.Log($"Find visible {this}");
                        _webElements = root.FindVisibleElements(_by);
                    }


                }
                else
                {
                    if (_frame != null)
                    {
                        Logger.Log($"Select frame {_frame}");
                        WrappedDriver.SwitchTo().Frame(_frame.FindMe());
                    }

                    if (FindHidden)
                    {
                        Logger.Log($"Find {this}");
                        _webElements = WrappedDriver.FindElements(_by);

                    }
                    else
                    {
                        Logger.Log($"Find visible {this}");
                        _webElements = WrappedDriver.FindVisibleElements(_by);
                    }

                }
                foreach(var ele in _webElements)
                {
                    ele.Highlight(50, "red");
                }
            }


             Logger.Log($"Found {_webElements.Count()} Elements {_by}");

            _elements = new List<Element>();
            int i = 1;
            foreach (var webele in _webElements)
            {
                var elem = new Element(webele, by, i);
                elem.WrappedElement = webele;
                _elements.Add(elem);
                WebElementCache.SaveElementToCache(webele, this.ToString(), i);
                i++;
            }
            WebElementsCache.SaveElementsToCache(_webElements.ToList(), this.ToString());
            return _webElements;
        }

        public IEnumerable<IWebElement> WaitForMe()
        {
            var wait = new WebDriverWait(WrappedDriver, TimeSpan.FromMilliseconds(_timeoutMs));
            wait.Message = $"{this} was not found";
            wait.Until(drv => FindMe().Count() > 0);
            return _webElements;
        }

        public IEnumerator<Element> GetEnumerator()
        {
            FindMe();
            foreach (var ele in _elements.AsEnumerable())
            {
                yield return ele;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public ElementsVerifications Verify()
        {
            return new ElementsVerifications(this, true);
        }

        public ElementsVerifications VerifyNot()
        {
            return new ElementsVerifications(this, false);
        }
    }

}
