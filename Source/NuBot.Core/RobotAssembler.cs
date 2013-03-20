using System;
using System.Collections.Generic;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.IO;
using System.Linq;
using System.Reflection;
using NLog;
using NuBot.Abstractions;
using NuBot.Configuration;
using NuBot.Infrastructure;

namespace NuBot
{
    public class RobotAssembler
    {
        public static readonly string DefaultRobotName = "NuBot";

        private readonly IRobotConfiguration _configuration;
        private readonly ILogConfiguration _logConfiguiration;
        private Dictionary<string, IPart> _availableParts;
        private Dictionary<string, IHttpHost> _availableHttpHosts;

        private readonly Logger _log;

        public RobotAssembler(IRobotConfiguration configuration, ILogConfiguration logConfiguration)
            : this(configuration, logConfiguration, Enumerable.Empty<Assembly>(), Enumerable.Empty<string>()) { }
        public RobotAssembler(IRobotConfiguration configuration, ILogConfiguration logConfiguration, IEnumerable<string> partDirectories)
            : this(configuration, logConfiguration, Enumerable.Empty<Assembly>(), partDirectories) { }
        public RobotAssembler(IRobotConfiguration configuration, ILogConfiguration logConfiguration, IEnumerable<Assembly> partAssemblies)
            : this(configuration, logConfiguration, partAssemblies, Enumerable.Empty<string>()) { }
        public RobotAssembler(IRobotConfiguration configuration, ILogConfiguration logConfiguration, IEnumerable<Assembly> partAssemblies, IEnumerable<string> partDirectories) 
            : this(configuration, logConfiguration, BuildCatalogs(partAssemblies, partDirectories)) { }

        public RobotAssembler(IRobotConfiguration configuration, ILogConfiguration logConfiguration, IEnumerable<ComposablePartCatalog> partCatalogs)
        {
            _configuration = configuration;
            _logConfiguiration = logConfiguration;
            _log = _logConfiguiration.CreateLogger("RobotAssembler");

            Compose(partCatalogs);
        }

        public IRobot CreateRobot()
        {
            return CreateRobot(null, new DefaultConsole());
        }

        public IRobot CreateRobot(string name)
        {
            return CreateRobot(name, new DefaultConsole());
        }

        public IRobot CreateRobot(IConsole console)
        {
            return CreateRobot(null, console);
        }

        public IRobot CreateRobot(string name, IConsole console)
        {
            // Get the robot name from config if not specified, use the default if that isn't specified either
            name = (name ?? _configuration.GetSetting("Name")) ?? DefaultRobotName;
            _log.Trace("Creating Robot '{0}'", name);

            // Get the HTTP Host from config
            string httpHostName = _configuration.GetSetting("HttpHost");
            IHttpHost selectedHttpHost = null;
            if (String.IsNullOrEmpty(httpHostName))
            {
                _log.Warn("No HTTP Host is configured. Parts which use HTTP may not function correctly");
            }
            else if (!_availableHttpHosts.TryGetValue(httpHostName, out selectedHttpHost))
            {
                throw new InvalidOperationException(String.Format("Unknown http host: '{0}'", httpHostName));
            }

            // Activate parts
            var selectedParts = _configuration.Parts.Where(p => p.IsEnabled).Select(config =>
            {
                IPart part;
                if (!_availableParts.TryGetValue(config.Name, out part))
                {
                    _log.Warn("Part not found: '{0}'", config.Name);
                    return null;
                }
                else
                {
                    _log.Trace("Configuring Part: {0} - {1} (v{2})", part.Name, part.Title, part.Version());
                    part.Configure(config);
                    return part;
                }
            }).Where(p => p != null).ToArray();

            // Create the robot
            return new Robot(name, _logConfiguiration, _configuration, new MessageBus(), selectedParts, selectedHttpHost, console);
        }

        private void Compose(IEnumerable<ComposablePartCatalog> partCatalogs)
        {
            // Set up a container
            var container = new CompositionContainer(new AggregateCatalog(
                new AssemblyCatalog(typeof(RobotAssembler).Assembly),
                new AggregateCatalog(partCatalogs)));

            foreach (var asmCat in partCatalogs.OfType<AssemblyCatalog>())
            {
                _log.Trace("Using Plugins from {0}", asmCat.Assembly.GetName().Name);
            }

            foreach (var dir in partCatalogs.OfType<TwoLevelDirectoryCatalog>().SelectMany(c => c.SubDirectoryNames))
            {
                _log.Trace("Using Plugins from {0}", dir);
            }

            // Load Parts and HttpHosts
            var allParts = container.GetExportedValues<IPart>();
            var allHttpHosts = container.GetExportedValues<IHttpHost>();
            _availableParts = allParts.ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);
            _availableHttpHosts = allHttpHosts.ToDictionary(h => h.Name, StringComparer.OrdinalIgnoreCase);

            // Report information
            foreach (var plugin in Enumerable.Concat(_availableParts.Values.Cast<IPlugin>(), _availableHttpHosts.Values.Cast<IPlugin>()))
            {
                _log.Trace("Found Plugin: {0} - {1} (v{2})", plugin.Name, plugin.Title, plugin.Version());
            }
        }

        private static IEnumerable<ComposablePartCatalog> BuildCatalogs(IEnumerable<Assembly> partAssemblies, IEnumerable<string> partDirectories)
        {
            return Enumerable.Concat<ComposablePartCatalog>(
                partAssemblies.Select(a => new AssemblyCatalog(a)),
                partDirectories.Where(Directory.Exists).Select(d => new TwoLevelDirectoryCatalog(d)))
                .ToArray();
        }
    }
}
