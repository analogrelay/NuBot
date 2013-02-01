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
    public class ChatTracer : SimplePart
    {
        public override string Name
        {
            get { return "Chat Tracer"; }
        }

        public override void Run(IRobot robo)
        {
            robo.Hear(msg =>
            {
                string type = msg.DirectedAtRobot ? "DM" : "OH";
                robo.Log.Trace("[{0} In {1}] {2}: {3}", type, msg.Room, msg.From, msg.Content);
            });
        }
    }
}
