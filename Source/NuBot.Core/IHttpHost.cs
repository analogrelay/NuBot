using System;
using Owin;

namespace NuBot
{
    public interface IHttpHost : IPlugin
    {
        void StartServer(int port, IRobot robo, Action<IAppBuilder> app);
    }
}
