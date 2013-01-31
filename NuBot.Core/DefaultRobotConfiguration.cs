using System;
using System.Collections.Generic;
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
        public DefaultRobotConfiguration() : this(false, null) { }

        /// <summary>
        /// Sets up a default configuration which does use Environment Variables as a fallback store
        /// </summary>
        public DefaultRobotConfiguration(string environmentVariablePrefix) : this(true, environmentVariablePrefix) {}

        private DefaultRobotConfiguration(bool useEnvironmentVariables, string environmentVariablePrefix)
        {
            if (useEnvironmentVariables && String.IsNullOrEmpty(environmentVariablePrefix))
            {
                throw new ArgumentNullException("environmentVariablePrefix");
            }

            Settings = new Dictionary<string, string>();
            UseEnvironmentVariables = useEnvironmentVariables;
            EnvironmentVariablePrefix = environmentVariablePrefix;
        }

        public string GetSetting(string key)
        {
            string value;
            if (!Settings.TryGetValue(key, out value))
            {
                value = Environment.GetEnvironmentVariable(String.Concat(EnvironmentVariablePrefix, key));
            }
            return value;
        }
    }
}
