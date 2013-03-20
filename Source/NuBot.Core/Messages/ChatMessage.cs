using System;
using System.Collections.Generic;

namespace NuBot.Messages
{
    public class ChatMessage : RawChatMessage
    {
        public bool DirectedAtRobot { get; private set; }
        public IEnumerable<string> Tokens { get; private set; }

        public ChatMessage(bool directedAtRobot, IEnumerable<string> tokens, RawChatMessage raw) :
            this(directedAtRobot, raw.From, raw.Room, raw.When, raw.Id, raw.Content, tokens, raw.FromRobot)
        {
        }

        public ChatMessage(bool directedAtRobot,
                           string from,
                           string room,
                           DateTimeOffset when,
                           string id,
                           string content,
                           IEnumerable<string> tokens,
                           bool fromRobot) : base(from, room, when, id, content, fromRobot)
        {
            DirectedAtRobot = directedAtRobot;
            Tokens = tokens;
        }
    }
}
