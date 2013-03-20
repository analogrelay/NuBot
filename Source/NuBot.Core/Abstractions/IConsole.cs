using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NuBot.Abstractions
{
    public interface IConsoleActor
    {
        ConsoleColor Foreground { get; }
        ConsoleColor Background { get; }
        int BufferWidth { get; }

        Task WriteAsync(ConsoleColor foreground, ConsoleColor background, string text);
        Task<string> ReadLineAsync();
        Task EnsureAtStartOfLineAsync();
    }

    public interface IConsole : IConsoleActor
    {
        Task Synchronize(Func<IConsoleActor, Task> action);
    }

    public static class ConsoleExtensions
    {
        public static Task WriteLineAsync(this IConsoleActor self,
                                          ConsoleColor foreground,
                                          ConsoleColor background,
                                          string text)
        {
            return self.WriteAsync(foreground, background, text + Environment.NewLine);
        }

        public static Task WriteLineAsync(this IConsoleActor self,
                                          ConsoleColor foreground,
                                          string text)
        {
            return WriteLineAsync(self, foreground, self.Background, text);
        }

        public static Task WriteLineAsync(this IConsoleActor self,
                                          string text)
        {
            return WriteLineAsync(self, self.Foreground, self.Background, text);
        }

        public static Task WriteLineAsync(this IConsoleActor self)
        {
            return self.WriteAsync(self.Foreground, self.Background, Environment.NewLine);
        }

        public static Task WriteAsync(this IConsoleActor self,
                                      ConsoleColor foreground,
                                      string text)
        {
            return self.WriteAsync(foreground, self.Background, text);
        }

        public static Task WriteAsync(this IConsoleActor self,
                                      string text)
        {
            return self.WriteAsync(self.Foreground, self.Background, text);
        }
    }
}
