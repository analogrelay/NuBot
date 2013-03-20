using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NuBot.Abstractions;

namespace NuBot.Configuration
{
    public class JsonRobotConfiguration : JsonKeyValueConfiguration, IRobotConfiguration
    {
        private readonly ITextFile _file;
        private IEnumerable<IPartConfiguration> _parts = Enumerable.Empty<IPartConfiguration>();

        public IEnumerable<IPartConfiguration> Parts { get { return _parts; } }

        public JsonRobotConfiguration(ITextFile file)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            _file = file;

            RefreshConfiguration();
        }

        private void RefreshConfiguration()
        {
            if (_file.Exists)
            {
                try
                {
                    var token = JToken.Parse(_file.ReadAllText());
                    if (token.Type != JTokenType.Object)
                    {
                        throw new InvalidDataException("Incorrect Configuration Document");
                    }
                    var root = (JObject)token;
                    LoadNewSettings(root.Property("Settings"));

                    var partsProperty = root.Property("Parts");
                    var newParts =
                        partsProperty != null ?
                            ParsePartConfigurations(partsProperty) :
                            Enumerable.Empty<IPartConfiguration>();
                    Interlocked.Exchange(ref _parts, newParts);
                }
                catch (JsonException jex)
                {
                    throw new InvalidDataException("Malformed Configuration Document", jex);
                }
            }
        }

        private IEnumerable<IPartConfiguration> ParsePartConfigurations(JProperty partsProperty)
        {
            if (partsProperty.Value.Type != JTokenType.Object)
            {
                throw new InvalidDataException("Incorrect Configuration Document");
            }
            var parts = (JObject)partsProperty.Value;

            return parts.Properties()
                        .Select(ParsePartConfiguration)
                        .ToArray();
        }

        private IPartConfiguration ParsePartConfiguration(JProperty partProperty)
        {
            if (partProperty.Value.Type != JTokenType.Object && partProperty.Value.Type != JTokenType.Boolean)
            {
                throw new InvalidDataException("Incorrect Configuration Document");
            }
            return new JsonPartConfiguration(partProperty);
        }
    }
}
