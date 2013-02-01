using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NuBot.Parts
{
    [Export(typeof(IPart))]
    public class ChatTracer : Part
    {
        public override string Name
        {
            get { return "Chat Tracer"; }
        }

        public override void Attach(IRobot robo, CancellationToken token)
        {
            robo.Hear(msg =>
            {
                string type = msg.DirectedAtRobot ? "DM" : "OH";
                robo.Log.Trace("[{0} In {1}] {2}: {3}", type, msg.Room, msg.From, msg.Content);
            });
        }
    }
}
