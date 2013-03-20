using System;
using System.Reactive.Linq;
using System.Threading.Tasks;
using Xunit;

namespace NuBot.Test
{
    public class MessageBusFacts
    {
        public class TheSendMessage
        {
            [Fact]
            public void ShouldRequireNonNullMessage()
            {
                var bus = new MessageBus();
                var ex = Assert.Throws<ArgumentNullException>(() => bus.Send<object>(null));
                Assert.Equal("message", ex.ParamName);
            }

            [Fact]
            public async Task ShouldPublishMessageToSubscriberAsynchronously()
            {
                var wait = new TaskCompletionSource<object>();
                var done = new TaskCompletionSource<object>();
                
                var bus = new MessageBus();
                var message = new object();
                bool recieved = false;
                var current = Task.CurrentId;
                bus.Observe<object>()
                   .Where(o => Object.ReferenceEquals(message, o))
                   .Subscribe(async o =>
                   {
                       await wait.Task;
                       recieved = true; 
                       done.TrySetResult(null);
                   });
                Assert.False(recieved);
                bus.Send(message);
                Assert.False(recieved);
                wait.TrySetResult(null);
                await done.Task;
                Assert.True(recieved);
            }
        }
    }
}
