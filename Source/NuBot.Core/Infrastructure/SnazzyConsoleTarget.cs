using System;
using System.Collections.Generic;
using System.Linq;
using NLog;
using NLog.Targets;
using NuBot.Abstractions;

namespace NuBot.Infrastructure
{
    public class SnazzyConsoleTarget : TargetWithLayout
    {
        private readonly IConsole _console;

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

        public SnazzyConsoleTarget(IConsole console)
        {
            _console = console;
        }

        protected override void Write(LogEventInfo logEvent)
        {
            // Synchronize the console
            _console.Synchronize(async c =>
            {
                // Get us to the start of a line
                await c.EnsureAtStartOfLineAsync();

                // Get Color Pair colors
                ColorPair pair;
                if (!ColorTable.TryGetValue(logEvent.Level, out pair))
                {
                    pair = new ColorPair();
                }
                if (pair.ForegroundColor == null)
                {
                    pair = new ColorPair(c.Foreground, pair.BackgroundColor);
                }
                if (pair.BackgroundColor == null)
                {
                    pair = new ColorPair(pair.ForegroundColor, c.Background);
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
                var maxWidth = c.BufferWidth - 2;
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
                        await c.WriteLineAsync();
                    }

                    // Write Level
                    await c.WriteAsync(pair.ForegroundColor.Value, pair.BackgroundColor.Value, levelName);

                    // Write the message using the default foreground color, but the specified background color
                    await c.WriteAsync(": ");
                    await c.WriteAsync(c.Foreground, pair.BackgroundColor.Value, message);
                }
            });
        }
    }
}
