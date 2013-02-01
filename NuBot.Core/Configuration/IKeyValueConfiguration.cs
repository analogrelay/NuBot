using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuBot.Configuration
{
    public interface IKeyValueConfiguration
    {
        string GetSetting(string key);
    }

    public static class KeyValueConfigurationExtensions
    {
        public static T GetSetting<T>(this IKeyValueConfiguration self, string key, Func<string, T> converter)
        {
            return GetSetting(self, key, converter, default(T));
        }

        public static T GetSetting<T>(this IKeyValueConfiguration self, string key, Func<string, T> converter, T defaultValue)
        {
            string value = self.GetSetting(key);
            if (String.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            return converter(value);
        }
    }
}
