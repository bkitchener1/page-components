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
            get { return _driver != null ? _driver : DriverManager.GetDriver(); }
            set { _driver = value; }
        }

        public bool FindHidden
        {
            get { return _findHidden; }
            set { _findHidden = value; }
        }

        public Elements()
        {
            this._timeoutMs = WebConfig.ElementTimeoutMs;
        }

        public Elements(By by)
        {
            this._by = by;
            this._timeoutMs = WebConfig.ElementTimeoutMs;
        }

        public Elements(Frame frame, By by)
        {
            this._frame = frame;
            this._by = by;
            this._timeoutMs = WebConfig.ElementTimeoutMs;
        }

        public Elements(Element container, By by)
        {
            this._container = container;
            this._by = by;
            this._timeoutMs = WebConfig.ElementTimeoutMs;
        }

        public Elements(Element container, Frame frame, By by)
        {
            this._container = container;
            this._frame = frame;
            this._by = by;
            this._timeoutMs = WebConfig.ElementTimeoutMs;
        }

        public override string ToString()
        {
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

        public IEnumerable<IWebElement> FindMe()
        {
            var wait = new WebDriverWait(WrappedDriver, TimeSpan.FromMilliseconds(_timeoutMs));
            wait.Message = $"{this} was not found";
            WrappedDriver.WaitForAjax(_timeoutMs);
            if (IsStale())
            {
                if (_container != null)
                {
                    var root = _container.FindMe();
                    if (_frame != null)
                    {
                        WrappedDriver.SwitchTo().Frame(_frame.FindMe());
                    }

                    if (FindHidden)
                    {
                        _webElements = wait.Until(drv => root.FindElements(_by));
                    }
                    else
                    {
                        _webElements = wait.Until(drv => root.FindVisibleElements(_by));
                    }


                }
                else
                {
                    if (_frame != null)
                    {
                        WrappedDriver.SwitchTo().Frame(_frame.FindMe());
                    }

                    if (FindHidden)
                    {
                        _webElements = wait.Until(_driver => _driver.FindElements(_by));

                    }
                    else
                    {
                        _webElements = wait.Until(_driver => _driver.FindVisibleElements(_by));
                    }

                }
            }

            _elements = new List<Element>();
            foreach (var webele in _webElements)
            {
                webele.Highlight();
                var elem = new Element(by);
                elem.WrappedElement = webele;
                _elements.Add(elem);
            }
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
