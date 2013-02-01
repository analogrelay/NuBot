using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NuBot.Core.Messages;

namespace NuBot.Core.Parts
{
    [Export(typeof(IPart))]
    public class ChatTracer : IPart
    {
        public string Name
        {
            get { return "Chat Tracer"; }
        }

        public Task Run(IRobot robo, CancellationToken cancelToken)
        {
            robo.Bus.Observe<ChatMessage>()
                .Subscribe(msg =>
                {
                    string type = msg.DirectedAtRobot ? "DM" : "OH";
                    robo.Log.Trace("[{0} In {1}] {2}: {3}", type, msg.Room, msg.From, msg.Content);
                });
            return Task.FromResult<object>(null);
        }
    }
}
