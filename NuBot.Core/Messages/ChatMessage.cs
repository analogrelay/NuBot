using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuBot.Core.Messages
{
    public class ChatMessage
    {
        public bool DirectedAtRobot { get; private set; }
        public string From { get; private set; }
        public DateTimeOffset When { get; private set; }
        public string Room { get; private set; }
        public string Content { get; private set; }
        public string Id { get; private set; }
        public IEnumerable<string> Tokens { get; private set; }

        public ChatMessage(bool directedAtRobot, string from, string room, DateTimeOffset when, string id, string content, IEnumerable<string> tokens)
        {
            DirectedAtRobot = directedAtRobot;
            From = from;
            Room = room;
            When = when;
            Content = content;
            Id = id;
            Tokens = tokens;
        }
    }
}
