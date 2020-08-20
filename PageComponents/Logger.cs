using AventStack.ExtentReports.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace PageComponents
{
    public static class Logger
    {


        private static ThreadLocal<Action<string>> _logMessages = new ThreadLocal<Action<string>>();

        private static Action<string> LogMessage 
        { 
            get
            {
                if(_logMessages.Value == null)
                {
                    _logMessages.Value = new Action<string>(ConsoleLog);
                }
                return _logMessages.Value;
            }

            set
            {
                _logMessages.Value = value;
            }

        }

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
