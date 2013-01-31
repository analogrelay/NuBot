using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuBot.Core
{
    public class DefaultRobotConfiguration : IRobotConfiguration
    {
        public bool UseEnvironmentVariables { get; private set; }
        public string EnvironmentVariablePrefix { get; private set; }
        public IDictionary<string, string> Settings { get; private set; }

        /// <summary>
        /// Sets up a default configuration which does NOT use Environment Variables as a fallback store
        /// </summary>
        public DefaultRobotConfiguration() : this(false, null, null) { }

        /// <summary>
        /// Sets up a default configuration which does use Environment Variables as a fallback store
        /// </summary>
        public DefaultRobotConfiguration(string environmentVariablePrefix) : this(true, environmentVariablePrefix, null) {}

        public DefaultRobotConfiguration(string environmentVariablePrefix, string[] commandLine) : this(true, environmentVariablePrefix, commandLine) { }

        private DefaultRobotConfiguration(bool useEnvironmentVariables, string environmentVariablePrefix, string[] commandLine)
        {
            if (useEnvironmentVariables && String.IsNullOrEmpty(environmentVariablePrefix))
            {
                throw new ArgumentNullException("environmentVariablePrefix");
            }

            Settings = new Dictionary<string, string>();
            UseEnvironmentVariables = useEnvironmentVariables;
            EnvironmentVariablePrefix = environmentVariablePrefix;

            if (commandLine != null)
            {
                foreach (var arg in commandLine)
                {
                    string[] splitted = arg.Split('=');
                    if (splitted.Length == 1)
                    {
                        Settings[splitted[0]] = Boolean.TrueString;
                    }
                    else if (splitted.Length == 2)
                    {
                        Settings[splitted[0]] = splitted[1];
                    }
                    else
                    {
                        throw new InvalidDataException("Argument format invalid. Expected key=value pairs");
                    }
                }
            }
        }

        public string GetSetting(string key)
        {
            string value;
            if (!Settings.TryGetValue(key, out value))
            {
                value = Environment.GetEnvironmentVariable(String.Concat(EnvironmentVariablePrefix, key).Replace('.', '_'));
            }
            return value;
        }
    }
}
