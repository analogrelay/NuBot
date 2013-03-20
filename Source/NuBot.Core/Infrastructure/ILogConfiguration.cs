using NLog;

namespace NuBot.Infrastructure
{
    public interface ILogConfiguration
    {
        Logger CreateLogger(string name);
    }
}
