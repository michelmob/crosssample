using System;
using System.Linq;
using NLog;
using NLog.Config;
using NLog.Layouts;
using NLog.Targets;
using Xunit;

namespace Gravity.Diagnostics.NLog.Tests
{
    public class NLogLoggerTests
    {
        [Fact]
        public void Logger_Log_DelegatesToNLog()
        {
            var target = GetLogMemoryTarget();
            ILogger log = new NLogLogger(LogManager.GetLogger("cat"));
            
            log.Info("msg");
            Assert.Equal("cat|Info|msg||", target.Logs.Last());
            log.Log(LogLevel.Info, "msg");
            Assert.Equal("cat|Info|msg||", target.Logs.Last());

            log.Debug("msg");
            Assert.Equal("cat|Debug|msg||", target.Logs.Last());
            log.Log(LogLevel.Debug, "msg");
            Assert.Equal("cat|Debug|msg||", target.Logs.Last());
            
            log.Warn("msg");
            Assert.Equal("cat|Warn|msg||", target.Logs.Last());
            log.Log(LogLevel.Warning, "msg");
            Assert.Equal("cat|Warn|msg||", target.Logs.Last());
            
            log.Error("msg", new Exception("ex"));
            Assert.Equal("cat|Error|msg|ex|", target.Logs.Last());
            log.Log(LogLevel.Error, "msg", new Exception("ex"));
            Assert.Equal("cat|Error|msg|ex|", target.Logs.Last());
            
            log.Fatal("msg");
            Assert.Equal("cat|Fatal|msg||", target.Logs.Last());
            log.Fatal("msg", new Exception("ex"));
            Assert.Equal("cat|Fatal|msg|ex|", target.Logs.Last());
            log.Log(LogLevel.Fatal, "msg", new Exception("ex"));
            Assert.Equal("cat|Fatal|msg|ex|", target.Logs.Last());
        }

        [Fact]
        public void Logger_GetLogger_ChangesCategory()
        {
            var target = GetLogMemoryTarget();
            ILogger log = new NLogLogger(LogManager.GetLogger("category1"));
            
            log.Info("msg");
            Assert.Equal("category1|Info|msg||", target.Logs.Last());

            log = log.GetLogger("cat2");
            log.Info("msg");
            Assert.Equal("cat2|Info|msg||", target.Logs.Last());

            log = log.GetLogger(GetType());
            log.Info("msg");
            Assert.Equal($"{GetType().FullName}|Info|msg||", target.Logs.Last());
        }

        private static MemoryTarget GetLogMemoryTarget()
        {
            var cfg = new LoggingConfiguration();

            var target = new MemoryTarget("mem")
            {
                Layout = new SimpleLayout("${Logger}|${Level}|${Message}|${exception}|${all-event-properties}")
            };
            
            cfg.AddTarget(target);

            cfg.AddRule(global::NLog.LogLevel.Trace, global::NLog.LogLevel.Fatal, target);

            LogManager.Configuration = cfg;
            
            return target;
        }
    }
}