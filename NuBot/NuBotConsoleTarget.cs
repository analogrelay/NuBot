using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NLog;
using NLog.Targets;

namespace NuBot
{
    public class NuBotConsoleTarget : TargetWithLayout
    {
        private static Dictionary<LogLevel, ColorPair> _colorTable = new Dictionary<LogLevel, ColorPair>()
        {
            { LogLevel.Debug, ColorPair.Foreground(ConsoleColor.Magenta) },
            { LogLevel.Error, ColorPair.Foreground(ConsoleColor.Red) },
            { LogLevel.Fatal, new ColorPair(ConsoleColor.White, ConsoleColor.Red) },
            { LogLevel.Info, ColorPair.Foreground(ConsoleColor.Green) },
            { LogLevel.Trace, ColorPair.Foreground(ConsoleColor.Gray) },
            { LogLevel.Warn, new ColorPair(ConsoleColor.Black, ConsoleColor.Yellow) }
        };

        private static Dictionary<LogLevel, string> _levelNames = new Dictionary<LogLevel, string>() {
            { LogLevel.Debug, "debug" },
            { LogLevel.Error, "error" },
            { LogLevel.Fatal, "fatal" },
            { LogLevel.Info, "info" },
            { LogLevel.Trace, "trace" },
            { LogLevel.Warn, "warn" },
        };

        private static int _levelLength = _levelNames.Values.Max(s => s.Length);

        protected override void Write(LogEventInfo logEvent)
        {
            // Capture current colors
            ConsoleColor originalForeground = Console.ForegroundColor;
            ConsoleColor originalBackground = Console.BackgroundColor;

            // Set colors
            ColorPair pair;
            if (_colorTable.TryGetValue(logEvent.Level, out pair))
            {
                if (pair.BackgroundColor != null)
                {
                    Console.BackgroundColor = pair.BackgroundColor.Value;
                }
                if (pair.ForegroundColor != null)
                {
                    Console.ForegroundColor = pair.ForegroundColor.Value;
                }
            }

            // Get level string
            string levelName;
            if (!_levelNames.TryGetValue(logEvent.Level, out levelName))
            {
                levelName = logEvent.Level.ToString();
            }
            levelName = levelName.PadRight(_levelLength).Substring(0, _levelLength);

            // Write Level
            Console.Write(levelName);

            // Reset foreground color
            Console.ForegroundColor = originalForeground;
            
            // Write message
            Console.Write(": ");
            Console.Write(Layout.Render(logEvent));

            // Reset colors and end the line
            Console.ForegroundColor = originalForeground;
            Console.BackgroundColor = originalBackground;

            Console.WriteLine();
        }
    }
}
