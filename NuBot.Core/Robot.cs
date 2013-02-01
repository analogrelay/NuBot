using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;

namespace NuBot
{
    [Export(typeof(IRobotFactory))]
    public class RobotFactory : IRobotFactory
    {
        private LogFactory _logFactory;
        private IRobotConfiguration _robotConfiguration;
        private IMessageBus _messageBus;

        [ImportingConstructor]
        public RobotFactory(LogFactory logFactory, IRobotConfiguration config, IMessageBus bus)
        {
            _logFactory = logFactory;
            _robotConfiguration = config;
            _messageBus = bus;
        }

        public IRobot CreateRobot(string name)
        {
            return new Robot(name, _logFactory, _robotConfiguration, _messageBus);
        }
    }

    public class Robot : IRobot
    {
        private Logger _logger;
        private RobotLog _log;
        private CancellationTokenSource _cts = new CancellationTokenSource();

        public IList<IPart> Parts { get; private set; }
        public string Name { get; private set; }
        public IMessageBus Bus { get; private set; }
        public IRobotConfiguration Configuration { get; private set; }

        public IRobotLog Log
        {
            get { return _log ?? (_log = new RobotLog(_logger)); }
        }

        public Robot() : this("NuBot", null, new DefaultRobotConfiguration(), new MessageBus()) { }
        public Robot(string name, LogFactory factory, IRobotConfiguration configuration, IMessageBus bus)
        {
            var loggerName = String.Format("Robot.{0}", name);
            _logger = factory == null ?
                LogManager.GetLogger(loggerName) :
                factory.GetLogger(loggerName);

            Name = name;
            Configuration = configuration;
            Parts = new List<IPart>();
            Bus = bus;
        }

        public void Start()
        {
            _logger.Trace("Starting Robot");

            var tasks = Parts.Select(async part =>
            {
                _logger.Info("Attaching Part: {0}", part.Name);
                try
                {
                    await part.Run(this, _cts.Token);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message);
                }
            }).ToArray();
            
            _logger.Trace("Started Robot");
            _logger.Info("{0} has suited up and is ready to go", Name);
        }

        public void Stop()
        {
            _cts.Cancel();
        }
    }
}
