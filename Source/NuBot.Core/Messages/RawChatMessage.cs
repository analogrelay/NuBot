using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuBot.Messages
{
    public class RawChatMessage
    {
        public string From { get; private set; }
        public DateTimeOffset When { get; private set; }
        public string Room { get; private set; }
        public string Content { get; private set; }
        public string Id { get; private set; }
        public bool FromRobot { get; private set; }
        public string[] AdditionalRobotNames { get; private set; }

        public RawChatMessage(string from,
                              string room,
                              DateTimeOffset when,
                              string id,
                              string content,
                              bool fromRobot) : this(from, room, when, id, content, fromRobot, new string[0])
        {
        }

        public RawChatMessage(string from,
                              string room,
                              DateTimeOffset when,
                              string id,
                              string content,
                              bool fromRobot, 
                              string[] additionalRobotNames)
        {
            From = from;
            Room = room;
            When = when;
            Content = content;
            Id = id;
            FromRobot = fromRobot;
            AdditionalRobotNames = additionalRobotNames;
        }
    }
}
