using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Owin;

namespace NuBot
{
    public interface IPart
    {
        string Name { get; }
        void Attach(IRobot robo, CancellationToken token);
        void AttachToHttpApp(IRobot robo, IAppBuilder app);
    }
}
