using NLog;
using NLog.Config;

namespace NuBot.Infrastructure
{
    public class DefaultLogConfiguration : ILogConfiguration
    {
        private LogFactory _logFactory;

        private LogFactory CreateLogFactory()
        {
            var config = new LoggingConfiguration();

            // Targets
            var target = new SnazzyConsoleTarget
            {
                Layout = "${message}"
            };
            config.AddTarget("console", target);

            // Rules
            var rule = new LoggingRule("*", LogLevel.Trace, target);
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
