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
using System.ComponentModel.Composition;

namespace NuBot.Core.Parts
{
    [Export(typeof(IPart))]
    public class BeesModule : IPart
    {
        public string Name
        {
            get { return "Bees Module"; }
        }

        public Task Run(IRobot robo, CancellationToken cancelToken)
        {
            MessageProcessor processor = new MessageProcessor();
            robo.Bus.Observe<ChatMessage>()
                .Where(m => processor.ContainsWordsInOrder(m.Tokens, "bees"))
                .Subscribe(msg =>
                {
                    robo.Bus.Send(new SendChatMessage(
                        "http://img37.imageshack.us/img37/7044/oprahbees.gif",
                        msg.Room));
                });
            return Task.FromResult<object>(null);
        }
    }
}
