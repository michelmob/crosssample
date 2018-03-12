using System;
using System.Collections.Generic;
using Gravity.Diagnostics;

namespace Gravity.Manager.Web.Tests.Controllers
{
    public class TestLogger : ILogger
    {
        private string _category;
        
        public List<Entry> Entries { get; private set; } = new List<Entry>();

        public void Log(LogLevel logLevel, string message)
        {
            Log(logLevel, message, null);
        }

        public void Log(LogLevel logLevel, string message, Exception exception)
        {
            Entries.Add(new Entry(_category, message, logLevel, exception));
        }

        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        public void Warn(string message)
        {
            Log(LogLevel.Warning, message);
        }

        public void Error(string message, Exception exception)
        {
            Log(LogLevel.Error, message, exception);
        }

        public void Fatal(string message)
        {
            Log(LogLevel.Fatal, message);
        }

        public void Fatal(string message, Exception exception)
        {
            Log(LogLevel.Fatal, message, exception);
        }

        public ILogger GetLogger(string name)
        {
            return new TestLogger {_category = name, Entries = Entries};
        }

        public ILogger GetLogger(Type type)
        {
            return GetLogger(type.FullName);
        }

        public class Entry
        {
            public Entry(string category, string message, LogLevel level, Exception exception)
            {
                Message = message;
                Level = level;
                Exception = exception;
                Category = category;
            }

            public string Message { get; }
            public LogLevel Level { get; }
            public Exception Exception { get; }
            public string Category { get; }
        }
    }
}
