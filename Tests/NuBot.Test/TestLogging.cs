using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace NuBot.Test
{
    public static class TestLogging
    {
        internal static LogFactory CreateTestLogFactory(TargetWithLayout target)
        {
            LoggingConfiguration config = new LoggingConfiguration();

            target.Layout = "${level:lowercase=true}: [${logger}] ${message}";
            config.AddTarget("test", target);

            LoggingRule rule = new LoggingRule("*", LogLevel.Trace, target);
            config.LoggingRules.Add(rule);

            return new LogFactory(config);
        }
    }
}
