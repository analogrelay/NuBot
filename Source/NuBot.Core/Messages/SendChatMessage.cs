using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuBot.Messages
{
    public class SendChatMessage
    {
        public string Message { get; private set; }
        public string Room { get; private set; }
        public bool MeMessage { get; private set; }

        public SendChatMessage(string message, string room) : this(message, room, meMessage: false) { }

        public SendChatMessage(string message, string room, bool meMessage)
        {
            Message = message;
            Room = room;
            MeMessage = meMessage;
        }
    }
}
