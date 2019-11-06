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
    }
}
