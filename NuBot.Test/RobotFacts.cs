using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NLog;
using NLog.Targets;
using NuBot.Core;
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
            public void SetsProvidedNameIfSpecified()
            {
                Assert.Equal("Steve", new Robot("Steve").Name);
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
                var robot = new Robot("Steve");
                var mockPart = new Mock<IPart>();
                mockPart.Setup(p => p.Run(It.IsAny<IRobot>(), It.IsAny<CancellationToken>())).Returns(Task.FromResult(""));
                robot.Parts.Add(mockPart.Object);
                
                // Act
                robot.Run();

                // Assert
                mockPart.Verify(p => p.Run(robot, It.IsAny<CancellationToken>()));
            }

            [Fact]
            public void ReturnsTaskWhichDoesNotCompleteUntilAllTasksDone()
            {
                // Arrange
                TaskCompletionSource<object> tcs1 = new TaskCompletionSource<object>();
                var mockPart1 = new Mock<IPart>();
                mockPart1.Setup(p => p.Run(It.IsAny<IRobot>(), It.IsAny<CancellationToken>())).Returns(tcs1.Task);

                TaskCompletionSource<object> tcs2 = new TaskCompletionSource<object>();
                var mockPart2 = new Mock<IPart>();
                mockPart2.Setup(p => p.Run(It.IsAny<IRobot>(), It.IsAny<CancellationToken>())).Returns(tcs2.Task);
                
                var robot = new Robot("Steve");
                robot.Parts.Add(mockPart1.Object);
                robot.Parts.Add(mockPart2.Object);
                
                // Act
                var ret = robot.Run();

                // Assert
                Assert.False(ret.IsCompleted);
                tcs1.TrySetResult(null);
                Assert.False(ret.IsCompleted);
                tcs2.TrySetResult(null);
                Assert.True(ret.IsCompleted);
            }
        }
    }
}
