using NLog;
using NLog.Config;
using NuBot.Abstractions;

namespace NuBot.Infrastructure
{
    public class DefaultLogConfiguration : ILogConfiguration
    {
        private LogFactory _logFactory;
        private readonly IConsole _console;

        public DefaultLogConfiguration(IConsole console)
        {
            _console = console;
        }

        private LogFactory CreateLogFactory()
        {
            var config = new LoggingConfiguration();

            // Targets
            var target = new SnazzyConsoleTarget(_console)
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
