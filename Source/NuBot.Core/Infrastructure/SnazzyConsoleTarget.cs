using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NLog.Targets;

namespace NuBot.Infrastructure
{
    public class SnazzyConsoleTarget : TargetWithLayout
    {
        private static readonly Dictionary<LogLevel, ColorPair> ColorTable = new Dictionary<LogLevel, ColorPair>()
        {
            { LogLevel.Debug, ColorPair.ForegroundOnly(ConsoleColor.Magenta) },
            { LogLevel.Error, ColorPair.ForegroundOnly(ConsoleColor.Red) },
            { LogLevel.Fatal, new ColorPair(ConsoleColor.White, ConsoleColor.Red) },
            { LogLevel.Info, ColorPair.ForegroundOnly(ConsoleColor.Green) },
            { LogLevel.Trace, ColorPair.ForegroundOnly(ConsoleColor.DarkGray) },
            { LogLevel.Warn, new ColorPair(ConsoleColor.Black, ConsoleColor.Yellow) }
        };

        private static readonly Dictionary<LogLevel, string> LevelNames = new Dictionary<LogLevel, string>() {
            { LogLevel.Debug, "debug" },
            { LogLevel.Error, "error" },
            { LogLevel.Fatal, "fatal" },
            { LogLevel.Info, "info" },
            { LogLevel.Trace, "trace" },
            { LogLevel.Warn, "warn" },
        };

        private static readonly int LevelLength = LevelNames.Values.Max(s => s.Length);

        protected override void Write(LogEventInfo logEvent)
        {
            // Get us to the start of a line
            if (Console.CursorLeft != 0)
            {
                Console.WriteLine();
            }

            // Capture current colors
            ConsoleColor originalForeground = Console.ForegroundColor;
            ConsoleColor originalBackground = Console.BackgroundColor;

            // Set colors
            ColorPair pair;
            if (ColorTable.TryGetValue(logEvent.Level, out pair))
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
            if (!LevelNames.TryGetValue(logEvent.Level, out levelName))
            {
                levelName = logEvent.Level.ToString();
            }
            levelName = levelName.PadRight(LevelLength).Substring(0, LevelLength);

            // Break the message in to lines as necessary
            var message = Layout.Render(logEvent);
            var lines = new List<string>();
            var prefix = levelName + ": ";
            var fullMessage = prefix + message;
            var maxWidth = Console.BufferWidth - 2;
            while (fullMessage.Length > maxWidth)
            {
                int end = maxWidth - prefix.Length;
                int spaceIndex = message.LastIndexOf(' ', Math.Min(end, message.Length - 1));
                if (spaceIndex < 10)
                {
                    spaceIndex = end;
                }
                lines.Add(message.Substring(0, spaceIndex).Trim());
                message = message.Substring(spaceIndex).Trim();
                fullMessage = prefix + message;
            }
            lines.Add(message);

            // Write lines
            bool first = true;
            foreach (var line in lines.Where(l => !String.IsNullOrWhiteSpace(l)))
            {
                if (first)
                {
                    first = false;
                }
                else
                {
                    Console.WriteLine();
                }

                // Set Foreground
                if (pair.ForegroundColor != null)
                {
                    Console.ForegroundColor = pair.ForegroundColor.Value;
                }

                // Write Level
                Console.Write(levelName);

                // Reset foreground color, but only if there's no background color.
                // Why? Because if there's a background color, there's a foreground color that was specified explicitly and it would be good to preserve that
                if (pair.BackgroundColor == null)
                {
                    Console.ForegroundColor = originalForeground;
                }

                // Write message
                Console.Write(": ");
                Console.Write(line);
            }

            // Reset colors and end the line
            Console.ForegroundColor = originalForeground;
            Console.BackgroundColor = originalBackground;

            Console.WriteLine();
        }
    }
}
