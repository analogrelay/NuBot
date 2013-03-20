using System.IO;
using Newtonsoft.Json.Linq;

namespace NuBot.Configuration
{
    public class JsonPartConfiguration : JsonKeyValueConfiguration, IPartConfiguration
    {
        public string Name { get; private set; }
        public bool IsEnabled { get; private set; }

        public JsonPartConfiguration(JProperty partProperty)
        {
            LoadConfiguration(partProperty);
        }

        private void LoadConfiguration(JProperty partProperty)
        {
            Name = partProperty.Name;
            switch (partProperty.Value.Type)
            {
                case JTokenType.Boolean:
                    IsEnabled = partProperty.Value.ToObject<bool>();
                    LoadNewSettings(null);
                    break;
                case JTokenType.Object:
                {
                    var part = (JObject)partProperty.Value;

                    var enabledProp = part.Property("Enabled");
                    IsEnabled = enabledProp != null && enabledProp.Value.ToObject<bool>();

                    LoadNewSettings(part.Property("Settings"));
                }
                    break;
                default:
                    throw new InvalidDataException("Incorrect Configuration Document");
            }
        }
    }
}
