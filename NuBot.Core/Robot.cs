using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace NuBot.Core
{
    public class Robot : IRobot
    {
        private Logger _logger;
        private RobotLog _log;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public static readonly string DefaultRobotName = "NuBot";
        public static readonly string DefaultEnvironmentVariablePrefix = "NuBot.";

        public IList<IPart> Parts { get; private set; }
        public string Name { get; private set; }
        public IRobotConfiguration Configuration { get; private set; }

        public IRobotLog Log
        {
            get { return _log ?? (_log = new RobotLog(_logger)); }
        }

        public Robot() : this(DefaultRobotName) { }
        public Robot(string name) : this(name, null) { }
        public Robot(string name, LogFactory factory) : this(name, factory, new DefaultRobotConfiguration(DefaultEnvironmentVariablePrefix)) { }
        public Robot(string name, LogFactory factory, IRobotConfiguration configuration)
        {
            var loggerName = String.Format("Robot.{0}", name);
            _logger = factory == null ?
                LogManager.GetLogger(loggerName) :
                factory.GetLogger(loggerName);

            Name = name;
            Configuration = configuration;
            Parts = new List<IPart>();
        }

        public async Task Run()
        {
            _logger.Trace("Starting Robot");

            var tasks = Parts.Select(async part =>
            {
                _logger.Trace("Attaching Part: {0}", part.Name);
                await part.Run(this, _cts.Token);
            }).Concat(new[] { Task.Factory.StartNew(() => SpinWait.SpinUntil(() => _cts.IsCancellationRequested)) }).ToArray();
            
            _logger.Trace("Started Robot");

            // Wait until all tasks shut down
            await Task.WhenAll(tasks);
        }

        public void Stop()
        {
            _cts.Cancel();
        }
    }
}
