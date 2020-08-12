using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace PageComponents
{
    /// <summary>
    /// BasePageObject is an abstract class to be inherited by all page objects
    /// It provides a logger, root uri, and methods to open and instantiate the page object
    /// </summary>
    public abstract class BasePageObject : Element
    {
        public string Uri;
        public string Domain;

        /// <summary>
        /// Instantiate a page object without a root container element.  
        /// </summary>
        public BasePageObject()  : base()
        {
        }

        /// <summary>
        /// OpenPage navigates to the Page object's Uri, and returns a new instance of the object.
        /// Used as a simple way to start a function chain of page objects
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T OpenPage<T>() where T : BasePageObject
        {
            var page = Activator.CreateInstance<T>();
            if (page.Uri == null)
            {
                throw new Exception("Cannot use OpenPage function unless page object Uri is set");
            }

            if (page.Domain == null)
            {
                page.Domain = TestConfig.DefaultUrl;
            }
            page.WrappedDriver.Navigate().GoToUrl(page.Domain + page.Uri);

            return page;
        }
    }
}
