using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.ComponentModel.Composition.Primitives;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog;

namespace NuBot
{
    public class Composer
    {
        public IList<ComposablePartCatalog> AdditionalCatalogs { get; private set; }

        public Composer() : this(Enumerable.Empty<ComposablePartCatalog>()) { }
        public Composer(IEnumerable<ComposablePartCatalog> additionalCatalogs)
        {
            AdditionalCatalogs = additionalCatalogs.ToList();
        }

        public IRobot ComposeRobot(string name, LogFactory logFactory, IRobotConfiguration config)
        {
            var container = new CompositionContainer(new AggregateCatalog(
                new DirectoryCatalog(AppDomain.CurrentDomain.BaseDirectory),
                new AggregateCatalog(AdditionalCatalogs)));

            container.ComposeExportedValue(logFactory);
            container.ComposeExportedValue(config);
            
            var factory = container.GetExportedValue<IRobotFactory>();
            var robot = factory.CreateRobot(name);

            // Attach parts
            foreach (var part in container.GetExportedValues<IPart>())
            {
                robot.Parts.Add(part);
            }

            return robot;
        }
    }
}
