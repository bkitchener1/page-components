using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;

namespace PageComponents
{
    /// <summary>
    /// This class acts as an assertion helper.
    /// It provides a mechanism to avoid having to write custom assertion messages, and automatically
    /// waits up until the timeout for the assertion to fail.  This helps when DOM elements are updated
    /// dynamically, after they have been found.   Verifications are much more stabhle than Assertions.
    /// This class supports verifications for Elements (collections of IWebElement)
    /// </summary>
    public class ElementsVerifications
    {
        private Elements _elements;
        private bool _isTrue;
        private int _timeoutMs = WebConfig.ElementTimeoutMs;
        private WebDriverWait _wait;

        public ElementsVerifications(Elements elements)
        {
            this._elements = elements;
            _isTrue = true;
            _wait = new WebDriverWait(_elements.WrappedDriver, TimeSpan.FromMilliseconds(_timeoutMs));
        }

        public ElementsVerifications(Elements elements, bool isTrue)
        {
            this._elements = elements;
            this._isTrue = isTrue;
            _wait = new WebDriverWait(_elements.WrappedDriver, TimeSpan.FromMilliseconds(_timeoutMs));

        }


        public Elements Count(int num, int timeoutMs)
        {
            _wait.Timeout = TimeSpan.FromMilliseconds(timeoutMs);
            return Count(num);
        }

        public Elements Count(int num)
        {
            _wait.Message = $"Count of {_elements} expected '{num}' {GetCondition()} equal to actual '{_elements.Count()}'";
            _wait.Until(x => _elements.Count() == num);
            return _elements;
        }

        public Elements CountGreaterThan(int num)
        {
            _wait.Message = $"Count of {_elements} expected '{num}' {GetCondition()} greater than actual '{_elements.Count()}'";
            _wait.Until(x => _elements.Count() > num);
            return _elements;
        }

        public Elements IsPresent(int timeoutMs)
        {
            _wait.Timeout = TimeSpan.FromMilliseconds(timeoutMs);
            return IsPresent();
        }

        public Elements IsPresent()
        {
            _wait.Message = $"{_elements} {GetCondition()} found";
            _wait.Until(x => _elements.Count() > 0);
            return _elements;
        }


        private string GetCondition()
        {
            if (_isTrue)
            {
                return "was not";
            }
            else
            {
                return "was still";
            }
        }
    }
}
