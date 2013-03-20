using System;
using System.ComponentModel.Composition;
using System.Threading;
using System.Threading.Tasks;
using NuBot.Abstractions;
using NuBot.Messages;

namespace NuBot.Parts
{
    public class ConsoleChatListener : Part
    {
        private int _nextId;
        public override string Name
        {
            get { return "chat.console"; }
        }

        public override string Title
        {
            get { return "Console Chat Listener"; }
        }

        public override async void Attach(IRobot robo, CancellationToken token)
        {
            robo.Bus.On<SendChatMessage>(msg =>
            {
                robo.Console.Synchronize(async c =>
                {
                    await c.EnsureAtStartOfLineAsync();
                    await c.WriteLineAsync((msg.MeMessage ? "/me " : "") + msg.Message);
                });
            });

            await Task.Factory.StartNew(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    string line = await robo.Console.ReadLineAsync();
                    robo.Bus.Send(new RawChatMessage(
                                      from: "user",
                                      room: "Console",
                                      when: DateTimeOffset.UtcNow,
                                      id: (_nextId++).ToString(),
                                      content: line,
                                      fromRobot: false));
                }
            });
        }
    }
}
