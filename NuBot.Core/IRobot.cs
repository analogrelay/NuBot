using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuBot.Core
{
    public interface IRobot
    {
        IRobotLog Log { get; }
        IRobotConfiguration Configuration { get; }
        Task Run();
        void Stop();
    }
}
