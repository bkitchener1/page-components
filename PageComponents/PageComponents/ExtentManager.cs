using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter;
using System;
using System.Collections.Generic;
using System.IO;
using AventStack.ExtentReports.Gherkin.Model;
using System.Threading;
using AventStack.ExtentReports.Reporter.Configuration;

namespace PageComponents
{
    /// <summary>
    /// ExtentManager handles instantiating the extent reports, tracking which test is current, and generating the report.
    /// To start, call CreateTest.  use GetTest() to get access to the current running test, and Flush() after test is complete to generate the report
    /// </summary>
    public class ExtentManager
    {
        private static ExtentHtmlReporter _htmlReporter;
        private static readonly object _synclock = new object();
        private static ThreadLocal<ExtentTest> _currentTest = new ThreadLocal<ExtentTest>();
        private static Dictionary<string, ExtentTest> _tests = new Dictionary<string, ExtentTest>();
        private static ExtentReports _extentReports;

        /// <summary>
        /// Gets the report directorry.  By default it should appear in the bin/debug/reports directory
        /// </summary>
        /// <returns></returns>
        public static string GetReportPath()
        {
            var path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reports\\");
            var timestamp = DateTime.Now.ToString("MMddyyyy_Hmm") + "\\";
            var path2 = Path.Combine(path, timestamp);
            Console.WriteLine($"Extent report generated to : {path2}");
            return path2;
        }

        /// <summary>
        /// Create the ExtentManager, and define CSS/report configuration
        /// </summary>
        static ExtentManager()
        {
            _htmlReporter = new ExtentHtmlReporter(GetReportPath());
            _htmlReporter.Config.Theme = Theme.Dark;
            _extentReports = new ExtentReports();
            _extentReports.AttachReporter(_htmlReporter);
        }

        /// <summary>
        /// Creates a test.  Call before each test
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="description"></param>
        public static void CreateTest(string testName, string description = null)
        {
            lock (_synclock)
            {
                _tests[testName] = _extentReports.CreateTest(testName, description);
                _currentTest.Value = _tests[testName];
            }
        }

        /// <summary>
        /// Creates a test as a feature (for use in specflow).  Call before feature.
        /// </summary>
        /// <param name="testName"></param>
        /// <param name="description"></param>
        public static void CreateFeature(string testName, string description = null) 
        {
            lock (_synclock)
            {
                if (!_tests.ContainsKey(testName))
                {
                    _tests[testName] = _extentReports.CreateTest(testName, description);
                }
                _currentTest.Value = _tests[testName];
            }
        }

        /// <summary>
        /// creates a scenario that is a child of the feature
        /// </summary>
        /// <param name="featureName"></param>
        /// <param name="scenarioName"></param>
        /// <param name="description"></param>
        public static void CreateScenario(string featureName, string scenarioName, string description = null)
        {
            lock (_synclock)
            {
               _tests[scenarioName] = _tests[featureName].CreateNode(scenarioName, description);
               _currentTest.Value = _tests[scenarioName];
            }
        }

        /// <summary>
        /// creates a node, which is a child of the parent test
        /// </summary>
        /// <param name="parentName"></param>
        /// <param name="nodeName"></param>
        /// <param name="description"></param>
        public static void CreateNode(string parentName, string nodeName, string description = null)
        {
            lock (_synclock)
            {
                var parentTest = _tests[parentName];
                _tests[nodeName] = _tests[parentName].CreateNode(nodeName, description);
                _currentTest.Value = _tests[nodeName];
            }
        }

        public static void RemoveTest(string testName)
        {
            lock (_synclock)
            {
                _extentReports.RemoveTest(_tests[testName]);
            }
        }

        /// <summary>
        /// Creates a Step, which is a child of a scenario
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="scenarioName"></param>
        /// <param name="nodeName"></param>
        /// <param name="description"></param>
        public static void CreateStep<T>(string scenarioName, string nodeName, string description = null)
            where T : IGherkinFormatterModel
        {
            lock (_synclock)
            {
                _tests[nodeName] = _tests[scenarioName].CreateNode(nodeName, description);
                _currentTest.Value = _tests[nodeName];
            }
        }

        /// <summary>
        /// Gets the currently running test for the current thread
        /// </summary>
        /// <returns></returns>
        public static ExtentTest GetTest()
        {
            string testName = _currentTest.Value.Model.Name;
            return _currentTest.Value;
        }

        /// <summary>
        /// Generatest the report.  Call after the tests are complete.  
        /// </summary>
        public static void Flush()
        {
            Console.WriteLine("Publishing HTML Report ");
            Console.WriteLine($"Report dir exists {Directory.Exists(GetReportPath())}");
            _extentReports.Flush();
        }
}
}
