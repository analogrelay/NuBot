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
    public class MessageScannerFacts
    {
        public class TheIsForRobotMethod
        {
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
                MessageScanner scanner = new MessageScanner("NuBot");
                Assert.True(scanner.IsForRobot(message));
            }

            [Theory]
            [InlineData("NuBotacular")]
            [InlineData("ILoveNuBot")]
            public void CorrectlyIgnoresMessagesNotSentToTheRobot(string message)
            {
                MessageScanner scanner = new MessageScanner("NuBot");
                Assert.False(scanner.IsForRobot(message));
            }
        }
    }
}
