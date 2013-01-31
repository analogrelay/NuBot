using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuBot.Core
{
    public interface IRobotConfiguration
    {
        string GetSetting(string key);
    }

    public static class RobotConfigurationExtensions
    {
        public static T GetSetting<T>(this IRobotConfiguration self, string key, Func<string, T> conversionThunk)
        {
            return conversionThunk(self.GetSetting(key));
        }
    }
}
