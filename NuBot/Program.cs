using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using NuBot.Core;
using NuBot.Core.Parts;

namespace NuBot
{
    class Program
    {
        static void Main(string[] args)
        {
            // Entrypoint cannot be async :(
            AsyncMain(args).Wait();
        }

        static async Task AsyncMain(string[] args)
        {
            var config = new DefaultRobotConfiguration("NuBot.", args);
            var factory = ConfigureLogging();
            var log = factory.GetLogger("Main");
            var robot = new Robot("NuBot", factory, config);
            robot.Parts.Add(new JabbrListener());

            var robotTask = robot.Run();
            try
            {
                log.Info("Robot is running, press ESC to stop");
                await Task.WhenAny(Task.Factory.StartNew(() =>
                    SpinWait.SpinUntil(() => Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Escape)),
                    robotTask);
                robot.Stop();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            if (robotTask.IsFaulted)
            {
                log.Error(robotTask.Exception.GetBaseException().Message);
            }
            log.Info("Robot shut down.");
        }

        private static LogFactory ConfigureLogging()
        {
            LoggingConfiguration config = new LoggingConfiguration();

            // Targets
            NuBotConsoleTarget target = new NuBotConsoleTarget();
            target.Layout = "${message}";
            config.AddTarget("console", target);

            // Rules
            LoggingRule rule = new LoggingRule("*", LogLevel.Trace, target);
            config.LoggingRules.Add(rule);

            // Create factory
            return new LogFactory(config);
        }
    }
}
