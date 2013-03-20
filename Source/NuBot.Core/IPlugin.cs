using System;

namespace NuBot
{
    public interface IPlugin
    {
        string Name { get; }
        string Title { get; }
    }

    public static class PluginExtensions
    {
        public static Version Version(this IPlugin self)
        {
            return self.GetType().Assembly.GetName().Version;
        }
    }
}
