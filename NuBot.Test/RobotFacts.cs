using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NLog;
using NLog.Targets;
using NuBot;
using Xunit;

namespace NuBot.Test
{
    public class RobotFacts
    {
        public class TheConstructor
        {
            [Fact]
            public void SetsDefaultNameIfNoneSpecified()
            {
                Assert.Equal("NuBot", new Robot().Name);
            }

            [Fact]
            public void InitializesLogToNonNullValue()
            {
                Assert.NotNull(new Robot().Log);
            }
        }

        public class TheRunMethod
        {
            [Fact]
            public void InitializesAllParts()
            {
                // Arrage
                var robot = new Robot();
                var mockPart = new Mock<IPart>();
                mockPart.Setup(p => p.Attach(It.IsAny<IRobot>(), It.IsAny<CancellationToken>()));
                robot.Parts.Add(mockPart.Object);
                
                // Act
                robot.Start();

                // Assert
                mockPart.Verify(p => p.Attach(robot, It.IsAny<CancellationToken>()));
            }
        }
    }
}
