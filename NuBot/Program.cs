using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;

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

            log.Info("Suiting up.");
            var composer = new Composer();
            var robot = composer.ComposeRobot("NuBot", factory, config);
            
            robot.Start();
            try
            {
                log.Info("Press ESC to shut down the robot");
                await Task.Factory.StartNew(() => SpinWait.SpinUntil(() => Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Escape));
                robot.Stop();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message);
            }
            log.Info("Robot shut down.");
        }

        private static LogFactory ConfigureLogging()
        {
            LoggingConfiguration config = new LoggingConfiguration();

            // Targets
            SnazzyConsoleTarget target = new SnazzyConsoleTarget();
            target.Layout = "${message}";
            config.AddTarget("console", target);

            // Rules
            LoggingRule rule = new LoggingRule("*", LogLevel.Info, target);
            config.LoggingRules.Add(rule);

            // Create factory
            return new LogFactory(config);
        }
    }
}
