using System;
using GravityLogLevel = Gravity.Diagnostics.LogLevel;
using NLogLogLevel = NLog.LogLevel;

namespace Gravity.Diagnostics.NLog
{
    public class NLogLogger : ILogger
    {
        private readonly global::NLog.ILogger _logger;

        public NLogLogger(global::NLog.ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public void Log(GravityLogLevel logLevel, string message)
        {
            _logger.Log(ConvertLogLevel(logLevel), message);
        }

        public void Log(GravityLogLevel logLevel, string message, Exception exception)
        {
            _logger.Log(ConvertLogLevel(logLevel), exception, message);
        }

        public void Debug(string message)
        {
            _logger.Debug(message);
        }

        public void Info(string message)
        {
            _logger.Info(message);
        }

        public void Warn(string message)
        {
            _logger.Warn(message);
        }

        public void Error(string message, Exception exception)
        {
            _logger.Error(exception, message);
        }

        public void Fatal(string message)
        {
            _logger.Fatal(message);
        }

        public void Fatal(string message, Exception exception)
        {
            _logger.Fatal(exception, message);
        }

        public ILogger GetLogger(string name)
        {
            return new NLogLogger(_logger.Factory.GetLogger(name));
        }

        public ILogger GetLogger(Type type)
        {
            return GetLogger((type ?? throw new ArgumentNullException(nameof(type))).FullName);
        }

        private static NLogLogLevel ConvertLogLevel(GravityLogLevel level)
        {
            switch (level)
            {
                case GravityLogLevel.Debug:
                    return NLogLogLevel.Debug;
                case GravityLogLevel.Info:
                    return NLogLogLevel.Info;
                case GravityLogLevel.Warning:
                    return NLogLogLevel.Warn;
                case GravityLogLevel.Error:
                    return NLogLogLevel.Error;
                case GravityLogLevel.Fatal:
                    return NLogLogLevel.Fatal;
                default:
                    throw new ArgumentOutOfRangeException("level", level, "Invalid Gravity LogLevel.");
            }
        }
    }
}
