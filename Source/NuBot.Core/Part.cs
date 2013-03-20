using System.Threading;
using NuBot.Configuration;
using Owin;

namespace NuBot
{
    public abstract class Part : IPart
    {
        public abstract string Name { get; }
        public abstract string Title { get; }

        public virtual void Attach(IRobot robo, CancellationToken token)
        {
        }

        public virtual void AttachToHttpApp(IRobot robo, IAppBuilder app)
        {
        }

        public virtual void Configure(IPartConfiguration myConfig)
        {
        }
    }
}
