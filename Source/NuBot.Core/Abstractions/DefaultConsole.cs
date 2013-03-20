using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace NuBot.Abstractions
{
    public class DefaultConsole : IConsole, IConsoleActor
    {
        private readonly object _syncLock = new object();

        public ConsoleColor Foreground
        {
            get { return Console.ForegroundColor; }
        }

        public ConsoleColor Background
        {
            get { return Console.BackgroundColor; }
        }

        public int BufferWidth
        {
            get { return Console.BufferWidth; }
        }

        public async Task Synchronize(Func<IConsoleActor, Task> action)
        {
            Monitor.Enter(_syncLock);
            await action(new SynchronizedConsole(this));
            Monitor.Exit(_syncLock);
        }

        public async Task WriteAsync(ConsoleColor foreground, ConsoleColor background, string text)
        {
            await Synchronize(a => a.WriteAsync(foreground, background, text));
        }

        public async Task<string> ReadLineAsync()
        {
            string str = null;
            await Synchronize(async a => str = await a.ReadLineAsync());

            // The await above means we know str has a value
            return str; 
        }

        public async Task EnsureAtStartOfLineAsync()
        {
            await Synchronize(a => a.EnsureAtStartOfLineAsync());
        }

        private class SynchronizedConsole : IConsoleActor
        {
            private DefaultConsole _root;

            public SynchronizedConsole(DefaultConsole root)
            {
                _root = root;
            }

            public ConsoleColor Foreground
            {
                get { return _root.Foreground; }
            }

            public ConsoleColor Background
            {
                get { return _root.Background; }
            }

            public int BufferWidth
            {
                get { return _root.BufferWidth; }
            }

            public async Task WriteAsync(ConsoleColor foreground, ConsoleColor background, string text)
            {
                var oldFore = Console.ForegroundColor;
                var oldBack = Console.BackgroundColor;
                Console.ForegroundColor = foreground;
                Console.BackgroundColor = background;

                await Console.Out.WriteAsync(text);

                Console.ForegroundColor = oldFore;
                Console.BackgroundColor = oldBack;
            }

            public Task<string> ReadLineAsync()
            {
                return Console.In.ReadLineAsync();
            }

            public async Task EnsureAtStartOfLineAsync()
            {
                if (Console.CursorLeft != 0)
                {
                    await Console.Out.WriteLineAsync();
                }
            }
        }
    }
}
