using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenQA.Selenium;

namespace PageComponents
{
    /// <summary>
    /// Since Element and Elements are instantiated in the class headers using properties, we need a static thread safe way to cache webelements
    /// </summary>
    public static class WebElementsCache
    {
        private static ThreadLocal<Dictionary<string, List<IWebElement>>> ThreadSafeElementsCache = new ThreadLocal<Dictionary<string, List<IWebElement>>>();

        private static Dictionary<string, List<IWebElement>> ElementsCache
        {
            get
            {
                return ThreadSafeElementsCache.Value ??
                       (ThreadSafeElementsCache.Value = new Dictionary<string, List<IWebElement>> ());
            }
        }

        /// <summary>
        /// Saves a WebElement to the cache, using the By locator as a key
        /// </summary>
        /// <param name="element"></param>
        /// <param name="by"></param>
        public static void SaveElementsToCache(List<IWebElement> elements, string key)
        {
            string name = key;
            ElementsCache[name] = elements;
        }

        /// <summary>
        /// Returns an WebElement from the cache, or null if not found
        /// </summary>
        /// <param name="by"></param>
        /// <returns>IWebElement or null</returns>
        public static List<IWebElement> GetCachedElements(string key)
        {
            string name = key;
            if (ElementsCache.ContainsKey(name))
            {
                var eles = ElementsCache[name];
                foreach(var ele in eles)
                {
                    //if even one element is stale, the cache is invalid
                    if (ele.IsStale())
                    {
                        ElementsCache.Remove(name);
                        return null;
                    }
                }
                return eles;
            }
            else
            {
                //WebElement not found in element cache
                return null;
            }
        }
    }
}
