using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NuBot.Messages;
using NuBot.Services;

namespace NuBot.Parts
{
    public class ChatParser : Part
    {
        public override string Name
        {
            get { return "chat.parser"; }
        }

        public override string Title
        {
            get { return "Chat Parser"; }
        }

        public override void Attach(IRobot robo, System.Threading.CancellationToken token)
        {
            robo.Bus.On<RawChatMessage>(msg =>
            {
                var tokens = MessageHelper.Tokenize(msg.Content).ToList();
                var directedAtRobot = MessageHelper.IsDirectedAtRobot(tokens, Enumerable.Concat(
                    new [] { robo.Name },
                    msg.AdditionalRobotNames ?? new string[0]));
                robo.Bus.Send(new ChatMessage(directedAtRobot, tokens, msg));
            });
        }
    }
}
