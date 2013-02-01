using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace NuBot.Core.Services
{
    public class MessageScanner
    {
        private Regex[] _messageScanners;
        private string[] _robotNames;

        public MessageScanner(params string[] robotNames)
        {
            _robotNames = robotNames;
            _messageScanners = robotNames.SelectMany(GenerateRegexesForName).ToArray();
        }

        /// <summary>
        /// Checks if the provided message is directed at the robot
        /// </summary>
        /// <param name="message">The message recieved</param>
        /// <returns></returns>
        public bool IsForRobot(string message)
        {
            return _messageScanners.Any(r => r.IsMatch(message));
        }

        private IEnumerable<Regex> GenerateRegexesForName(string name)
        {
            yield return new Regex(String.Format(@"^@?{0}\,?\s+.*", name), RegexOptions.IgnoreCase);
            yield return new Regex(String.Format(@".*\s+@?{0}\,?$", name), RegexOptions.IgnoreCase);
            yield return new Regex(String.Format(@".*\s+@?{0}\,?\s+.*", name), RegexOptions.IgnoreCase);
        }
    }
}
