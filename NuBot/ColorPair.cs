using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuBot
{
    public struct ColorPair
    {
        public ConsoleColor? ForegroundColor { get; private set; }
        public ConsoleColor? BackgroundColor { get; private set; }

        public ColorPair(ConsoleColor? foreground, ConsoleColor? background)
            : this()
        {
            ForegroundColor = foreground;
            BackgroundColor = background;
        }

        public static ColorPair Foreground(ConsoleColor color)
        {
            return new ColorPair(color, null);
        }

        public static ColorPair Background(ConsoleColor color)
        {
            return new ColorPair(null, color);
        }
    }
}
