using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NuBot
{
    public abstract class SimplePart : IPart
    {
        public abstract string Name { get; }

        public abstract void Run(IRobot robo);

        Task IPart.Run(IRobot robo, CancellationToken cancelToken)
        {
            Run(robo);
            return Task.FromResult<object>(null);
        }
    }
}
