using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NuBot.Core.Messages;
using System.Text.RegularExpressions;
using NuBot.Core.Services;

namespace NuBot.Core.Parts
{
    public class TrollModule : IPart
    {
        private Random _rand = new Random();

        private string[] trolls = new[] {
            "http://i0.kym-cdn.com/photos/images/newsfeed/000/108/728/Oprah_umad.gif?1301082881",
            "http://t.qkme.me/3oy755.jpg",
            "http://t0.gstatic.com/images?q=tbn:ANd9GcSXI09FJxrT5lm_5o98Zu528sr6Doj0o13ChzPsB5S07aQusQmpOw"
        };

        public string Name
        {
            get { return "Troll Module"; }
        }

        public Task Run(IRobot robo, CancellationToken cancelToken)
        {
            MessageProcessor processor = new MessageProcessor();
            robo.Bus.Observe<ChatMessage>()
                .Where(m => m.DirectedAtRobot && processor.ContainsWordsInOrder(m.Tokens, "troll me"))
                .Subscribe(msg =>
                {
                    int item = _rand.Next(0, 2);
                    robo.Bus.Send(new SendChatMessage(
                        trolls[item],
                        msg.Room));
                });
            return Task.FromResult<object>(null);
        }
    }
}
