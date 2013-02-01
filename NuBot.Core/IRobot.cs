using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuBot.Core
{
    public interface IRobotFactory
    {
        IRobot CreateRobot(string name);
    }

    public interface IRobot
    {
        string Name { get; }
        IMessageBus Bus { get; }
        IRobotLog Log { get; }
        IRobotConfiguration Configuration { get; }
        IList<IPart> Parts { get; }
        void Start();
        void Stop();
    }
}
