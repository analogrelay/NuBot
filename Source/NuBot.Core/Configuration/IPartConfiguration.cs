using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuBot.Configuration
{
    public interface IPartConfiguration : IKeyValueConfiguration
    {
        string Name { get; }
        bool IsEnabled { get; }
    }
}
