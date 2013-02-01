using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using Owin;
using Owin.Builder;

namespace NuBot
{
    [Export(typeof(IRobotFactory))]
    public class RobotFactory : IRobotFactory
    {
        private LogFactory _logFactory;
        private IRobotConfiguration _robotConfiguration;
        private IMessageBus _messageBus;
        private IHttpHost _httpHost;

        [ImportingConstructor]
        public RobotFactory(LogFactory logFactory, IRobotConfiguration config, IMessageBus bus, [Import(AllowDefault=true)] IHttpHost httpHost)
        {
            _logFactory = logFactory;
            _robotConfiguration = config;
            _messageBus = bus;
            _httpHost = httpHost;
        }

        public IRobot CreateRobot(string name)
        {
            return new Robot(name, _logFactory, _robotConfiguration, _messageBus, _httpHost);
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

        public IHttpHost HttpHost { get; private set; }

        public IRobotLog Log
        {
            get { return _log ?? (_log = new RobotLog(_logger)); }
        }

        public Robot() : this("NuBot", null, new DefaultRobotConfiguration(), new MessageBus(), null) { }

        public Robot(string name, LogFactory factory, IRobotConfiguration configuration, IMessageBus bus, IHttpHost httpHost)
        {
            var loggerName = String.Format("Robot.{0}", name);
            _logger = factory == null ?
                LogManager.GetLogger(loggerName) :
                factory.GetLogger(loggerName);

            Name = name;
            Configuration = configuration;
            Parts = new List<IPart>();
            Bus = bus;
            HttpHost = httpHost;
        }

        public void Start()
        {
            _logger.Trace("Starting Robot");

            AppBuilder httpApp = new AppBuilder();
            foreach (var part in Parts)
            {
                _logger.Info("Attaching Part: {0}", part.Name);
                part.Attach(this, _cts.Token);
            }

            // Now start the HTTP host (if we have one)
            if (HttpHost != null)
            {
                _logger.Info("Starting HTTP Server: {0}", HttpHost.Name);
                HttpHost.StartServer(this, app =>
                {
                    foreach (var part in Parts)
                    {
                        part.AttachToHttpApp(this, app);
                    }
                });
            }
            else
            {
                _logger.Warn("No HTTP Host Plugin Found. Parts which create HTTP endpoints may not function correctly");
            }
            
            _logger.Trace("Started Robot");
            _logger.Info("{0} has suited up and is ready to go", Name);
        }

        public void Stop()
        {
            _cts.Cancel();
        }
    }
}
