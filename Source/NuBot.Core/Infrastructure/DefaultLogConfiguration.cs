using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;

namespace NuBot.Infrastructure
{
    public class DefaultLogConfiguration : ILogConfiguration
    {
        private LogFactory _logFactory;

        private LogFactory CreateLogFactory()
        {
            LoggingConfiguration config = new LoggingConfiguration();

            // Targets
            SnazzyConsoleTarget target = new SnazzyConsoleTarget();
            target.Layout = "${message}";
            config.AddTarget("console", target);

            // Rules
            LoggingRule rule = new LoggingRule("*", LogLevel.Trace, target);
            config.LoggingRules.Add(rule);

            // Create factory
            return new LogFactory(config);
        }

        public Logger CreateLogger(string name)
        {
            if (_logFactory == null) { _logFactory = CreateLogFactory(); }
            return _logFactory.GetLogger(name);
        }
    }
}
