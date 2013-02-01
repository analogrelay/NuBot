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
                Assert.Equal(tokens, MessageProcessor.Tokenize(message).ToArray());
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
                var tokens = MessageProcessor.Tokenize(message);
                Assert.True(MessageProcessor.IsDirectedAtRobot(tokens, "NuBot"));
            }

            [Theory]
            [InlineData("NuBotacular")]
            [InlineData("ILoveNuBot")]
            public void CorrectlyIgnoresMessagesNotSentToTheRobot(string message)
            {
                var tokens = MessageProcessor.Tokenize(message);
                Assert.False(MessageProcessor.IsDirectedAtRobot(tokens, "NuBot"));
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
                var targetTokens = MessageProcessor.Tokenize(target);
                var wordTokens = MessageProcessor.Tokenize(words);
                Assert.True(MessageProcessor.ContainsWordsInOrder(targetTokens, wordTokens));
                Assert.True(MessageProcessor.ContainsWordsInOrder(targetTokens, words));
            }
        }
    }
}
