using System.ComponentModel.Composition;
using System.Threading;

namespace NuBot.Parts
{
    public class ChatTracer : Part
    {
        public override string Name
        {
            get { return "core.trace.chat"; }
        }

        public override string Title
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
