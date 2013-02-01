using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Owin.Host.HttpListener;
using Microsoft.Owin.Hosting;
using Microsoft.Owin.Hosting.Settings;
using Owin;

namespace NuBot.Http.Katana
{
    [Export(typeof(IHttpHost))]
    public class KatanaHttpHost : IHttpHost
    {
        public string Name
        {
            get { return "Katana"; }
        }

        public void StartServer(IRobot robo, Action<IAppBuilder> app)
        {
            //try
            //{
                robo.Log.Info("Sharpening the Katana...");
                var port = robo.Configuration.GetSetting("Http.Port", s => String.IsNullOrEmpty(s) ? 8080 : Int32.Parse(s));
                KatanaSettings settings = new KatanaSettings()
                {
                    DefaultPort = port,
                    LoaderFactory = () => _ => app
                };
                KatanaEngine engine = new KatanaEngine(settings);
                engine.Start(new StartContext());
                robo.Log.Info("Katana is listening on port {0}", port);
            //} catch() {
            //}
        }
    }
}
