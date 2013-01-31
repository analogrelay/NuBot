using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NuBot.Core
{
    public interface IPart
    {
        string Name { get; }
        Task Run(IRobot robo, CancellationToken cancelToken);
    }
}
