using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace PageComponents
{
    /// <summary>
    /// BasePageObject is an abstract class to be inherited by all page components
    /// It extends the element class so it can be built using a By locator that becomes the root container for the class
    /// </summary>
    public abstract class BaseComponent : Element
    {

        /// <summary>
        /// Instantiate a page object without a root container element.  
        /// </summary>
        public BaseComponent()  : base()
        {
        }

        /// <summary>
        /// instntiates a page component using a by locator.  The by locator bedcomes an element container
        /// that represenets the root element in the dom for this page object.
        /// </summary>
        /// <param name="by"></param>
        public BaseComponent(By by) : base(by)
        {
        }

        /// <summary>
        /// Instantiate a page object inside an IFrame, with a By locator that becomes the
        /// root element for the page component
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="by"></param>
        public BaseComponent(Frame frame, By by) : base(frame, by)
        {
        }

        /// <summary>
        /// Instantiates a page component using another page object as a root container, and a by locator
        /// </summary>
        /// <param name="container"></param>
        /// <param name="by"></param>
        public BaseComponent(BaseComponent container, By by) : base(container, by)
        {
        }

        /// <summary>
        /// Instntiate a page component using a container, an iframe, and a by locator.
        /// </summary>
        /// <param name="container"></param>
        /// <param name="frame"></param>
        /// <param name="by"></param>
        public BaseComponent(BaseComponent container, By by, Frame frame ) : base(container, by, frame)
        {
        }

        /// <summary>
        /// Overrides the ToString() function to make a nice loggable message
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if(_by == null && Container != null)
            {
                return _container.ToString();
            }
            if (_by != null && _container != null)
            {
                return $"Element '{_by}' with parent '{_container}'";
            }
            if (Index != 0)
            {
                //Want the log to be clear that this comes from a list
                return $"Element '{_by}' [{Index}]";

            }
            return $"Element '{_by}'";
        }

        /// <summary>
        /// Instantiate a new Element using this component as a root element
        /// </summary>
        /// <param name="cssSelector"></param>
        /// <returns></returns>
        public Element Element(string cssSelector)
        {
            return new Element(this, cssSelector);
        }

        /// <summary>
        /// Instantiate a new Element using this component as a root element
        /// </summary>
        /// <param name="cssSelector"></param>
        /// <returns></returns>
        public Element Element(By by)
        {
            return new Element(this, by);
        }

        /// <summary>
        /// Instantiate a new Element using this component as a root element
        /// </summary>
        /// <param name="cssSelector"></param>
        /// <returns></returns>
        public Element Element(string cssSelector, Frame frame)
        {
            return new Element(this,  By.CssSelector(cssSelector), frame);
        }

        /// <summary>
        /// Instantiate a new Element using this component as a root element
        /// </summary>
        /// <param name="cssSelector"></param>
        /// <returns></returns>
        public Element Element( By by, Frame frame)
        {
            return new Element(this, by, frame);
        }

        /// <summary>
        /// Instantiate a new Elements using this component as a root element
        /// </summary>
        /// <param name="cssSelector"></param>
        /// <returns></returns>
        public Elements Elements(string cssSelector)
        {
            return new Elements(this, By.CssSelector(cssSelector));
        }

        /// <summary>
        /// Instantiate a new Element using this component as a root element
        /// </summary>
        /// <param name="cssSelector"></param>
        /// <returns></returns>
        public Elements Elements(By by)
        {
            return new Elements(this, by);
        }

        /// <summary>
        /// Instantiate a new Element using this component as a root element
        /// </summary>
        /// <param name="cssSelector"></param>
        /// <returns></returns>
        public Elements Elements(string cssSelector, Frame frame)
        {
            return new Elements(this, By.CssSelector(cssSelector), frame);
        }

        /// <summary>
        /// Instantiate a new Element using this component as a root element
        /// </summary>
        /// <param name="cssSelector"></param>
        /// <returns></returns>
        public Elements Elements( By by, Frame frame)
        {
            return new Elements(this, by, frame);
        }
    }
}
