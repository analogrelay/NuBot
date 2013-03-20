namespace NuBot.Configuration
{
    public interface IPartConfiguration : IKeyValueConfiguration
    {
        string Name { get; }
        bool IsEnabled { get; }
    }
}
