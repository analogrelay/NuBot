using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using NLog.Config;
using NLog.Targets;
using NuBot.Abstractions;
using NuBot.Configuration;
using NuBot.Infrastructure;

namespace NuBot
{
    class Program
    {
        static void Main(string[] args)
        {
            // Entrypoint cannot be async :(
            AsyncMain(args).Wait();
        }

        const string ConfigFileName = "NuBot.config.json";
        static async Task AsyncMain(string[] args)
        {
            // Look for the configuration file
            string configPath = null;
            if (args.Length > 0)
            {
                configPath = args[0];
            } 
            else 
            {
                var userConfig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "NuBot", ConfigFileName);
                var appConfig = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "NuBot", ConfigFileName);
                var localConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);
                if (File.Exists(userConfig))
                {
                    configPath = userConfig;
                }
                else if (File.Exists(appConfig))
                {
                    configPath = appConfig;
                }
                else
                {
                    configPath = localConfig;
                }
            }

            // Set up logging
            var logConfig = new DefaultLogConfiguration();

            // Load config file
            ITextFile configFile = new PhysicalTextFile(configPath);
            var config = new JsonRobotConfiguration(configFile);

            // Set up the assembler
            var log = logConfig.CreateLogger("Program");
            log.Info("Assembling NuBot.");
            var assembler = new RobotAssembler(config, logConfig,
                partAssemblies: new[] {
                    typeof(Program).Assembly
                },
                partDirectories: new[] {
                    Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Parts")
                });

            // Make a NuBot!
            var robot = assembler.CreateRobot("NuBot");

            // Start the NuBot and wait for shutdown
            try
            {
                robot.Start();
                log.Info("Press ESC to shut down the robot");
                await Task.Factory.StartNew(() => SpinWait.SpinUntil(() => Console.KeyAvailable && Console.ReadKey().Key == ConsoleKey.Escape));
                robot.Stop();
            }
            catch (Exception ex)
            {
                while (ex != null)
                {
                    log.Error("{0} - {1}", ex.GetType().FullName, ex.Message);
                    ex = ex.InnerException;
                }
            }
            log.Info("Robot shut down.");
        }
    }
}
