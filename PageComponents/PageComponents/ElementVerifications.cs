using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Support.UI;

namespace PageComponents
{
    /// <summary>
    /// This class acts as an assertion helper.
    /// It provides a mechanism to avoid having to write custom assertion messages, and automatically
    /// waits up until the timeout for the assertion to fail.  This helps when DOM elements are updated
    /// dynamically, after they have been found.   Verifications are much more stabhle than Assertions.
    /// </summary>
    public class ElementVerifications
    {
        private Element _element;
        private bool _isTrue;
        private int _timeoutMs;
        private WebDriverWait _wait;

        public ElementVerifications(Element element)
        {
            this._element = element;
            _timeoutMs = element.timeoutMs;
            _isTrue = true;
            _wait = new WebDriverWait(_element.WrappedDriver, TimeSpan.FromMilliseconds(_timeoutMs));
        }

        public ElementVerifications(Element element, bool isTrue)
        {
            this._element = element;
            this._isTrue = isTrue;
            _timeoutMs = element.timeoutMs;
            _wait = new WebDriverWait(_element.WrappedDriver, TimeSpan.FromMilliseconds(_timeoutMs));

        }

        public Element IsDisplayed(int timeoutMs)
        {
            _wait.Timeout = TimeSpan.FromMilliseconds(timeoutMs);
            return IsDisplayed();
        }

        public Element IsDisplayed()
        {
            _wait.Message = $"{_element} {GetCondition()} displayed";
            _wait.Until(drv => _element.Displayed == _isTrue);
            return _element;
        }

        public Element IsPresent(int timeoutMs)
        {
            _wait.Timeout = TimeSpan.FromMilliseconds(timeoutMs);
            return IsPresent();
        }
        public Element IsPresent()
        {
            _wait.Message = $"{_element} {GetCondition()} present";
            _wait.Until(drv => _element.Present == _isTrue);
            return _element;
        }

        public Element TextIs(string value, int timeoutMs)
        {
            _wait.Timeout = TimeSpan.FromMilliseconds(timeoutMs);
            return TextIs(value);
        }

        public Element TextIs(string value)
        {
            _wait.Message = $"{_element} expected text '{value}' {GetCondition()} actual text '{_element.Text}' ";
            _wait.Until(drv => (_element.Text == value) == _isTrue);
            return _element;
        }

        public Element AttributeIs(string attributeName, string expectedValue, int timeoutMs)
        {
            _wait.Timeout = TimeSpan.FromMilliseconds(timeoutMs);
            return AttributeIs(attributeName, expectedValue);
        }

        public Element AttributeIs(string attributeName, string expectedValue)
        {
            _wait.Message = $"{_element} attribute '{attributeName}' expected value '{expectedValue}' {GetCondition()} actual value '{_element.GetAttribute(attributeName)}'";
            _wait.Until(drv => (_element.GetAttribute(attributeName) == expectedValue) == _isTrue);
            return _element;
        }

        public Element ValueIs(string expectedValue)
        {
            return AttributeIs("value", expectedValue);
        }

        public Element ValueIs(string expectedValue, int timeoutMs)
        {
            return AttributeIs("value", expectedValue, timeoutMs);
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
