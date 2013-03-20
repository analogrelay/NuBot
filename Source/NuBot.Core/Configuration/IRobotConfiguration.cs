using System.Collections.Generic;

namespace NuBot.Configuration
{
    public interface IRobotConfiguration : IKeyValueConfiguration
    {
        IEnumerable<IPartConfiguration> Parts { get; }
    }
}
