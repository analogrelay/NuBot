using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NuBot.Core.Messages;
using System.Text.RegularExpressions;

namespace NuBot.Core.Parts
{
    public class BeesModule : IPart
    {
        private Regex[] _bees = new[] {
            new Regex(@".*(^|\s+)bees(\?|\!|\,|\.)?($|\s+.*)")
        };

        public string Name
        {
            get { return "Bees Module"; }
        }

        public Task Run(IRobot robo, CancellationToken cancelToken)
        {
            robo.Bus.Observe<ChatMessage>()
                .Where(m => IsBees(m))
                .Subscribe(msg =>
                {
                    robo.Bus.Send(new SendChatMessage(
                        "http://img37.imageshack.us/img37/7044/oprahbees.gif",
                        msg.Room));
                });
            return Task.FromResult<object>(null);
        }

        private bool IsBees(ChatMessage m)
        {
            return _bees.Any(r => r.IsMatch(m.Content));
        }
    }
}
