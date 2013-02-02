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
        public static string GetRequired(this IKeyValueConfiguration self, string key)
        {
            string val = self.GetSetting(key);
            if (String.IsNullOrEmpty(val))
            {
                throw new InvalidOperationException("Missing required configuration setting: " + key);
            }
            return val;
        }

        public static T GetRequired<T>(this IKeyValueConfiguration self, string key, Func<string, T> converter)
        {
            return InternalGetConverted(self, key, self.GetRequired, converter, default(T));
        }

        public static T GetRequired<T>(this IKeyValueConfiguration self, string key, Func<string, T> converter, T defaultValue)
        {
            return InternalGetConverted(self, key, self.GetRequired, converter, defaultValue);
        }

        public static T GetSetting<T>(this IKeyValueConfiguration self, string key, Func<string, T> converter)
        {
            return InternalGetConverted(self, key, self.GetSetting, converter, default(T));
        }

        public static T GetSetting<T>(this IKeyValueConfiguration self, string key, Func<string, T> converter, T defaultValue)
        {
            return InternalGetConverted(self, key, self.GetSetting, converter, defaultValue);
        }

        private static T InternalGetConverted<T>(IKeyValueConfiguration self, string key, Func<string, string> getter, Func<string, T> converter, T defaultValue)
        {
            string value = getter(key);
            if (String.IsNullOrEmpty(value))
            {
                return defaultValue;
            }
            return converter(value);
        }
    }
}
