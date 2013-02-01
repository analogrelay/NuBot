using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Owin;

namespace NuBot
{
    public abstract class Part : IPart
    {
        public abstract string Name { get; }

        public virtual void Attach(IRobot robo, CancellationToken token)
        {
        }

        public virtual void AttachToHttpApp(IRobot robo, IAppBuilder app)
        {
        }
    }
}
