using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuBot.Core.Services;
using Xunit;
using Xunit.Extensions;

namespace NuBot.Test
{
    public class MessageProcessorFacts
    {
        public class TheTokenizeMethod
        {
            [Theory]
            [InlineData("Hello, World", new [] { "Hello", ",", " ", "World" })]
            [InlineData("Hey, SmartGuy42. How are you?", new [] { "Hey", ",", " ", "SmartGuy42", ".", " ", "How", " ", "are", " ", "you", "?" })]
            [InlineData("Hyphen-ation Rocks!", new[] { "Hyphen-ation", " ", "Rocks", "!" })]
            public void ProducesTheExpectedTokens(string message, string[] tokens)
            {
                Assert.Equal(tokens, new MessageProcessor().Tokenize(message).ToArray());
            }
        }

        public class TheIsDirectedAtRobotMethod
        {
            const string TestBotName = "NuBot";
            
            [Theory]
            [InlineData("NuBot, can you help me?")]
            [InlineData("nubot, can you help me?")]
            [InlineData("@Nubot, can you help me?")]
            [InlineData("@NuBot, can you help me?")]
            [InlineData("NuBot can you help me?")]
            [InlineData("nubot can you help me?")]
            [InlineData("@NuBot can you help me?")]
            [InlineData("@nubot can you help me?")]
            [InlineData("Hey NuBot can you help me?")]
            [InlineData("Hey nubot can you help me?")]
            [InlineData("Hey @NuBot can you help me?")]
            [InlineData("Hey @nubot can you help me?")]
            public void CorrectlyIdentifiesMessagesSentToTheRobot(string message)
            {
                MessageProcessor processor = new MessageProcessor();
                var tokens = processor.Tokenize(message);
                Assert.True(processor.IsDirectedAtRobot(tokens, "NuBot"));
            }

            [Theory]
            [InlineData("NuBotacular")]
            [InlineData("ILoveNuBot")]
            public void CorrectlyIgnoresMessagesNotSentToTheRobot(string message)
            {
                MessageProcessor processor = new MessageProcessor();
                var tokens = processor.Tokenize(message);
                Assert.False(processor.IsDirectedAtRobot(tokens, "NuBot"));
            }
        }

        public class TheContainsWordsInOrderMethod
        {
            [Theory]
            [InlineData("Troll Me", "troll me")]
            [InlineData("Hey, can you Troll Me?", "troll me")]
            [InlineData("Troll Me Dude", "troll me")]
            [InlineData("Yo,Troll Me Dude", "troll me")]
            [InlineData("Yo.Troll Me Dude", "troll me")]
            [InlineData("Yo!Troll Me Dude", "troll me")]
            [InlineData("ZOMG Bees!", "bees")]
            [InlineData("Bees!", "bees")]
            [InlineData("Bees", "bees")]
            [InlineData("OH NO Bees", "bees")]
            [InlineData("?Bees?", "bees")]
            public void CorrectlyMatchesWordsWhenTheyAreInTheTarget(string target, string words)
            {
                MessageProcessor processor = new MessageProcessor();
                var targetTokens = processor.Tokenize(target);
                var wordTokens = processor.Tokenize(words);
                Assert.True(processor.ContainsWordsInOrder(targetTokens, wordTokens));
                Assert.True(processor.ContainsWordsInOrder(targetTokens, words));
            }
        }
    }
}
