using AventStack.ExtentReports.Model;
using System;
using System.Collections.Generic;
using System.Text;

namespace PageComponents
{
    public static class Logger
    {
        static Logger()
        {
            LogMessage = new Action<string>(ConsoleLog);
        }

        private static Action<string> LogMessage { get; set; }

        public static void SetLogger(Action<string> logFunction)
        {
            LogMessage = logFunction;
        }

        private static void ConsoleLog(string message)
        {
            Console.WriteLine(message);
        }

        public static void Log(string message)
        {
            LogMessage(message);
        }

        public static void Error(string message)
        {
            LogMessage($"ERROR: {message}");
        }
    }
}
