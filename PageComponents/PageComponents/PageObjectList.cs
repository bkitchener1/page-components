using OpenQA.Selenium;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PageComponents
{
    /// <summary>
    /// PageObjectList represents a collection of page objects of a specific type
    /// Useful to be able to use page objects as a container, and enumerate them.
    /// PageObjectList allows the collection to be queried, sorted, and filtered.
    /// THe By locator passed in should return a list of elements, each represents the container
    /// for an item in our list
    /// PageObjectList<SearchResult> searchResults = new PageObjectList<SearchResult>(By.CssSelector(".containers"));
    /// var oneresult = searchResults.First(x=>x.CustomerName.Text == "Joe");
    /// oneresult.CustomerLink.Click();
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageObjectList<T> : IEnumerable<T> where T : BasePageObject
    {
            //The containers represent the list of elements on the page that contain each page object
            private Elements containers;

            public PageObjectList()
            {
            }

            /// <summary>
            /// Insantitate a page object list using a locator.
            /// This locator should represent the container for each page object.
            /// For example, if there are rows in a table, each row can be a page object, and the
            /// locator should represent the outmost container for each row.  
            /// </summary>
            /// <param name="by"></param>
            public PageObjectList(By by)
            {
                this.containers = new Elements(by);
            }

            public PageObjectList(string cssSelector)
            {
                this.containers = new Elements(By.CssSelector(cssSelector));
            }

        /// <summary>
        /// Create a page object list using an iframe and a locator
        /// </summary>
        /// <param name="frame"></param>
        /// <param name="by"></param>
        public PageObjectList(Frame frame, By by)
            {
                this.containers = new Elements(frame, by);
            }

            /// <summary>
            /// Gets the list of page objects, each with an element representing the container
            /// </summary>
            /// <returns></returns>
            public IEnumerator<T> GetEnumerator()
            {
                foreach (var ele in this.containers)
                {
                    var el = Activator.CreateInstance<T>();
                    el.container = ele;
                    el.WrappedElement = ele.WrappedElement;
                    yield return el;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }

            public ElementsVerifications Verify()
            {
                return new ElementsVerifications(this.containers);
            }

            public ElementsVerifications VerifyNot()
            {
                return new ElementsVerifications(this.containers, false);
            }
    }
}
