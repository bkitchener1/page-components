using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PageComponents
{
    public class TestContext
    {

        private static ThreadLocal<TestContext> _currentTestContext = new ThreadLocal<TestContext>();
        private IDictionary<string, object> _contextDict { get; set; } = new Dictionary<string, object>();

        public string TestName { get; internal set; }
        public TestConfig TestConfig { get; internal set; }

        TestContext()
        {
            TestConfig = new TestConfig();
        }

        public static TestContext SuiteContext { get; set; }

        public static TestContext CurrentContext
        {
            get
            {
                if (_currentTestContext.Value == null)
                {
                    _currentTestContext.Value = SuiteContext.CopyToNewContext();
                }
                return _currentTestContext.Value;
            }
            set => _currentTestContext.Value = value;
        }


        public TestContext CopyToNewContext()
        {
            var newContext = (TestContext)MemberwiseClone();
            newContext.TestName = NUnit.Framework.TestContext.CurrentContext.Test.FullName;
            return newContext;
        }

        public static void SaveContext<T>(string key, T obj)
        {
            CurrentContext._contextDict[key] = obj;
        }

        protected static T GetContext<T>(string key)
        {
            return (T)CurrentContext._contextDict[key];
        }
    }
}
