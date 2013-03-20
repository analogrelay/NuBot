using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuBot.Services
{
    public static class MessageHelper
    {
        public static IEnumerable<string> Tokenize(string input)
        {
            var tokenBuilder = new StringBuilder();
            char? tokenStart = null;
            foreach (char chr in input)
            {
                if (tokenStart == null)
                {
                    tokenStart = chr;
                    tokenBuilder.Append(chr);
                }
                else if (AreSimilar(chr, tokenStart.Value))
                {
                    tokenBuilder.Append(chr);
                }
                else if(tokenBuilder.Length > 0)
                {
                    yield return tokenBuilder.ToString();
                    tokenBuilder.Length = 0;
                    tokenStart = chr;
                    tokenBuilder.Append(chr);
                }
            }

            if (tokenBuilder.Length > 0)
            {
                yield return tokenBuilder.ToString();
            }
        }

        public static bool IsDirectedAtRobot(IEnumerable<string> tokens, IEnumerable<string> robotNames)
        {
            return tokens.Intersect(robotNames, StringComparer.OrdinalIgnoreCase).Any();
        }

        public static bool ContainsWordsInOrder(IEnumerable<string> tokens, string words)
        {
            return ContainsWordsInOrder(tokens, Tokenize(words));
        }

        public static bool ContainsWordsInOrder(IEnumerable<string> tokens, IEnumerable<string> words)
        {
            // Strip out uninteresting tokens
            var tokenArray = tokens.Where(str => IsWord(str)).ToArray();
            var wordArray = words.Where(str => IsWord(str)).ToArray();

            int matching = 0;
            for (int i = 0; i < tokenArray.Length; i++)
            {
                if (matching >= wordArray.Length)
                {
                    break;
                }

                if (String.Equals(wordArray[matching], tokenArray[i], StringComparison.OrdinalIgnoreCase))
                {
                    matching++;
                }
                else if (matching > 0)
                {
                    // We matched the first word but not the second, reset the matching area
                    matching = 0;
                }
            }

            return matching >= wordArray.Length;
        }

        private static bool IsWord(string str)
        {
            return str.Length > 0 && (Char.IsLetterOrDigit(str[0]) || str[0] == '-');
        }

        private static bool AreSimilar(char x, char y)
        {
            return ((Char.IsLetterOrDigit(x) || x == '-') && (Char.IsLetterOrDigit(y) || y == '-')) ||
                   (Char.IsWhiteSpace(x) && Char.IsWhiteSpace(y)) ||
                   (Char.IsPunctuation(x) && Char.IsPunctuation(y));
        }
    }
}
