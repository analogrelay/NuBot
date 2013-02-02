using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Owin;

namespace NuBot
{
    public interface IHttpHost : IPlugin
    {
        void StartServer(int port, IRobot robo, Action<IAppBuilder> app);
    }
}
