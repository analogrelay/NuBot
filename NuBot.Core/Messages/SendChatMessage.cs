using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NuBot.Core.Messages
{
    public class SendChatMessage
    {
        public string Message { get; private set; }
        public string Room { get; private set; }

        public SendChatMessage(string message, string room)
        {
            Message = message;
            Room = room;
        }
    }
}
