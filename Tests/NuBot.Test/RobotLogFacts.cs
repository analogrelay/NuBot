using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLog.Targets;
using NuBot;
using Xunit;

namespace NuBot.Test
{
    public class RobotLogFacts
    {
        public class TheInfoMethod
        {
            [Fact]
            public void ShouldLogInfoLevelMessageToLogger()
            {
                MemoryTarget target = new MemoryTarget();
                var factory = TestLogging.CreateTestLogFactory(target);
                var log = new RobotLog(factory.GetLogger("Test"));
                log.Info("Hello World!");
                Assert.Equal(
                    new[] { "info: [Test] Hello World!" },
                    target.Logs.ToArray());
            }
        }

        public class TheErrorMethod
        {
            [Fact]
            public void ShouldLogErrorLevelMessageToLogger()
            {
                MemoryTarget target = new MemoryTarget();
                var factory = TestLogging.CreateTestLogFactory(target);
                var log = new RobotLog(factory.GetLogger("Test"));
                log.Error("Hello World!");
                Assert.Equal(
                    new[] { "error: [Test] Hello World!" },
                    target.Logs.ToArray());
            }
        }

        public class TheTraceMethod
        {
            [Fact]
            public void ShouldLogTraceLevelMessageToLogger()
            {
                MemoryTarget target = new MemoryTarget();
                var factory = TestLogging.CreateTestLogFactory(target);
                var log = new RobotLog(factory.GetLogger("Test"));
                log.Trace("Hello World!");
                Assert.Equal(
                    new[] { "trace: [Test] Hello World!" },
                    target.Logs.ToArray());
            }
        }
    }
}
