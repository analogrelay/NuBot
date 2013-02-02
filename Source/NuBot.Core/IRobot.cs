using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuBot.Configuration;

namespace NuBot
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
        IEnumerable<IPart> Parts { get; }
        IHttpHost HttpHost { get; }
        void Start();
        void Stop();
    }
}
