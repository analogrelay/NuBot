using System.Collections.Generic;
using NuBot.Abstractions;

namespace NuBot
{
    public interface IRobotFactory
    {
        IRobot CreateRobot(string name);
    }

    public interface IRobot
    {
        string Name { get; }
        IConsole Console { get; }
        IMessageBus Bus { get; }
        IRobotLog Log { get; }
        IEnumerable<IPart> Parts { get; }
        IHttpHost HttpHost { get; }
        void Start();
        void Stop();
    }
}
