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
    public static class WebElementCache
    {
        private static ThreadLocal<Dictionary<string, IWebElement>> ThreadSafeElementCache = new ThreadLocal<Dictionary<string, IWebElement>>();

        private static Dictionary<string, IWebElement> ElementCache
        {
            get
            {
                return ThreadSafeElementCache.Value ??
                       (ThreadSafeElementCache.Value = new Dictionary<string, IWebElement>());
            }
        }

        /// <summary>
        /// Saves a WebElement to the cache, using the By locator as a key
        /// </summary>
        /// <param name="element"></param>
        /// <param name="by"></param>
        public static void SaveElementToCache(IWebElement element, string key, int index = 0)
        {
            string name = key + index;
            ElementCache[name] = element;
        }

        /// <summary>
        /// Returns an WebElement from the cache, or null if not found
        /// </summary>
        /// <param name="by"></param>
        /// <returns>IWebElement or null</returns>
        public static IWebElement GetCachedElement(string key, int index = 0)
        {
            string name = key + index;
            if (ElementCache.ContainsKey(name))
            {
                var ele = ElementCache[name];
                if (!ele.IsStale())
                {
                    //WebElement exists in element cache
                    return ele;
                }
                else
                {
                    //Element exists in cache but is stale
                    return null;
                }
            }
            else
            {
                //WebElement not found in element cache
                return null;
            }
        }
    }
}
