using NLog;
using NLog.Config;
using NLog.Targets;

namespace NuBot.Test
{
    public static class TestLogging
    {
        internal static LogFactory CreateTestLogFactory(TargetWithLayout target)
        {
            var config = new LoggingConfiguration();

            target.Layout = "${level:lowercase=true}: [${logger}] ${message}";
            config.AddTarget("test", target);

            var rule = new LoggingRule("*", LogLevel.Trace, target);
            config.LoggingRules.Add(rule);

            return new LogFactory(config);
        }
    }
}
