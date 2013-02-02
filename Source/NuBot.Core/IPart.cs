using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NuBot.Configuration;
using Owin;

namespace NuBot
{
    public interface IPart : IPlugin
    {
        void Configure(IPartConfiguration myConfig);
        void Attach(IRobot robo, CancellationToken token);
        void AttachToHttpApp(IRobot robo, IAppBuilder app);
    }
}
