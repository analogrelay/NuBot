using System;
using System.Collections.Generic;
using System.Threading;
using NLog;
using NuBot.Configuration;
using NuBot.Infrastructure;

namespace NuBot
{
    public class Robot : IRobot
    {
        private readonly Logger _logger;
        private RobotLog _log;
        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        public IEnumerable<IPart> Parts { get; private set; }
        public string Name { get; private set; }
        public IMessageBus Bus { get; private set; }
        public IRobotConfiguration Configuration { get; private set; }

        public IHttpHost HttpHost { get; set; }

        public IRobotLog Log
        {
            get { return _log ?? (_log = new RobotLog(_logger)); }
        }

        public Robot(string name, ILogConfiguration logConfig, IRobotConfiguration configuration, IMessageBus bus, IEnumerable<IPart> parts, IHttpHost httpHost)
        {
            var loggerName = String.Format("Robot.{0}", name);
            _logger = logConfig.CreateLogger(loggerName);

            Name = name;
            Configuration = configuration;
            Parts = parts;
            HttpHost = httpHost;
            Bus = bus;
        }

        public void Start()
        {
            _logger.Trace("Starting Robot");

            foreach (var part in Parts)
            {
                _logger.Info("Attaching Part: {0}", part.Name);
                part.Attach(this, _cts.Token);
            }

            // Now start the HTTP host (if we have one)
            if (HttpHost != null)
            {
                _logger.Info("Starting HTTP Server: {0}", HttpHost.Name);
                var port = Configuration.GetSetting("HttpPort", Int32.Parse, 8080);
                HttpHost.StartServer(port, this, app =>
                {
                    foreach (var part in Parts)
                    {
                        part.AttachToHttpApp(this, app);
                    }
                });
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
