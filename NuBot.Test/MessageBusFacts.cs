using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Threading;

namespace NuBot.Test
{
    public class MessageBusFacts
    {
        public class TheSendMessage
        {
            [Fact]
            public void RequiresNonNullMessage()
            {
                var bus = new MessageBus();
                var ex = Assert.Throws<ArgumentNullException>(() => bus.Send<object>(null));
                Assert.Equal("message", ex.ParamName);
            }

            [Fact]
            public async Task PublishesMessageToSubscriberAsynchronously()
            {
                TaskCompletionSource<object> wait = new TaskCompletionSource<object>();
                TaskCompletionSource<object> done = new TaskCompletionSource<object>();
                
                var bus = new MessageBus();
                object message = new object();
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
