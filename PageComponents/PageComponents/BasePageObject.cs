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
    /// It provides a logger, root uri, and extends the element class
    /// It can be built using a By locator that becomes the root container for the class
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
        /// instntiates a page object using a by locator.  The by locator bedcomes an element container
        /// that represenets the root element in the dom for this page object.
        /// </summary>
        /// <param name="by"></param>
        public BasePageObject(By by) : base(by)
        {
        }

        /// <summary>
        /// Instantiate a page object inside an IFrame, with a By locator that becomes the
        /// root element for the page object
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="by"></param>
        public BasePageObject(Frame frame, By by) : base(frame, by)
        {
        }

        /// <summary>
        /// Instantiates a page object using another page object as a root container, and a by locator
        /// </summary>
        /// <param name="container"></param>
        /// <param name="by"></param>
        public BasePageObject(BasePageObject container, By by) : base(container, by)
        {
        }

        /// <summary>
        /// Instntiate a page object using a container, an iframe, and a by locator.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="frame"></param>
        /// <param name="by"></param>
        public BasePageObject(BasePageObject container, Frame frame, By by) : base(container, frame, by)
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
                page.Domain = WebConfig.DefaultUrl;
            }
            page.WrappedDriver.Navigate().GoToUrl(page.Domain + page.Uri);

            return page;
        }
    }
}
