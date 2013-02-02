using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
            if (partProperty.Value.Type == JTokenType.Boolean)
            {
                // Simple form: 'Part':true
                IsEnabled = partProperty.Value.ToObject<bool>();
                LoadNewSettings(null);
            }
            else if (partProperty.Value.Type == JTokenType.Object)
            {
                var part = (JObject)partProperty.Value;

                var enabledProp = part.Property("Enabled");
                IsEnabled = enabledProp != null && enabledProp.Value.ToObject<bool>();

                LoadNewSettings(part.Property("Settings"));
            }
            else
            {
                throw new InvalidDataException("Incorrect Configuration Document");
            }
        }
    }
}
