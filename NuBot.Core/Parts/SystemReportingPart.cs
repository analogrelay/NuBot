using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;
using Gate;

namespace NuBot.Parts
{
    [Export(typeof(IPart))]
    public class SystemReportingPart : Part
    {
        public override string Name
        {
            get { return "System Reporting"; }
        }

        private static Version Version
        {
            get
            {
                return typeof(IRobot).Assembly.GetName().Version;
            }
        }

        public override void Attach(IRobot robo, System.Threading.CancellationToken token)
        {
            robo.Respond("version", m => robo.Say(Version.ToString(), m.Room));
            robo.Respond("parts", m => robo.Say(FormatParts(robo), m.Room));
        }

        public override void AttachToHttpApp(IRobot robo, Owin.IAppBuilder app)
        {
            app.Map("/system/version", (IAppBuilder subapp) =>
            {
                subapp.UseGate((req, resp) =>
                {
                    resp.StatusCode = 200;
                    resp.Write(Version.ToString());
                });
            });
            app.Map("/system/parts", (IAppBuilder subapp) =>
            {
                subapp.UseGate((req, resp) =>
                {
                    resp.StatusCode = 200;
                    resp.Write(FormatParts(robo));
                });
            });
        }

        private string FormatParts(IRobot robo)
        {
            StringBuilder builder = new StringBuilder();
            foreach (var part in robo.Parts)
            {
                builder.AppendLine(String.Format("{0} v{1}", part.Name, part.GetType().Assembly.GetName().Version.ToString()));
            }
            if (robo.HttpHost != null)
            {
                builder.AppendLine(String.Format("HTTP Hosting Engine: {0} v{1}", robo.HttpHost.Name, robo.HttpHost.GetType().Assembly.GetName().Version.ToString()));
            }
            return builder.ToString();
        }
    }
}
