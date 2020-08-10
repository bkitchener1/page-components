using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace PageComponents
{
    /// <summary>
    /// Frame represents an IFrame on the page.  It is passed into an Element class to support
    /// automatically selecting the appropriate iframe before an element is found.
    /// </summary>
    public class Frame : Element
    {
        public Frame(By by) : base(by)
        {

        }
        public Frame(string cssSelector) : base(cssSelector)
        {

        }

        /// <summary>
        /// Overrides the ToString() function to make a nice loggable message
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (_container != null)
            {
                return $"Frame '{_by}' with parent '{_container}'";
            }
            if (Index != 0)
            {
                //Want the log to be clear that this comes from a list
                return $"Frame '{_by}' [{Index + 1}]";

            }
            return $"Frame '{_by}'";
        }
    }
}
