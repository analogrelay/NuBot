using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using JabbR.Client;
using JabbR.Client.Models;
using Microsoft.AspNet.SignalR.Client.Http;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client.Transports;
using NuBot.Core.Services;
using NuBot.Core.Messages;

namespace NuBot.Core.Parts
{
    public class JabbrListenerWorker
    {
        private IRobot _robo;
        private string[] _rooms;
        private string _userName;
        private JabbRClient _client;
        private LogOnInfo _logOnInfo;
        private MessageProcessor _processor;
        private string[] _robotNames;

        public JabbrListenerWorker(MessageProcessor processor, LogOnInfo logOnInfo, JabbRClient client, string[] rooms, IRobot robo, string[] robotNames)
        {
            _robo = robo;
            _rooms = rooms;
            _client = client;
            _logOnInfo = logOnInfo;
            _processor = processor;
            _robotNames = robotNames;
        }

        public async Task Run(CancellationToken token)
        {
            // Leave rooms we're not supposed to be in
            var extraRooms = _logOnInfo.Rooms.Select(r => r.Name).Except(_rooms);
            if (extraRooms.Any())
            {
                await _client.LeaveRooms(extraRooms);
            }

            // Join rooms we're not in
            extraRooms = _rooms.Except(_logOnInfo.Rooms.Select(r => r.Name));
            if (extraRooms.Any())
            {
                _robo.Log.Trace("Joining rooms: {0}", String.Join(",", extraRooms));
                await _client.JoinRooms(extraRooms);
            }

            // Attach events
            _client.MessageReceived += _client_MessageReceived;

            // Attach to the bus
            _robo.Bus.Observe<SendChatMessage>().Subscribe(SendMessage);
            
            // Wait until terminated and disconnect
            token.Register(() => {
                _client.LeaveRooms(_rooms).ContinueWith(t =>
                {
                    // Always disconnect as gracefully as possible
                    _client.Disconnect();
                });
            });
        }

        private void SendMessage(SendChatMessage msg)
        {
            _client.Send(msg.Message, msg.Room);
        }

        void _client_MessageReceived(Message message, string room)
        {
            // Process the message and put it on the bus
            var tokens = _processor.Tokenize(message.Content);
            var directedAtRobot = _processor.IsDirectedAtRobot(tokens, _robotNames);
            _robo.Bus.Send(new ChatMessage(
                directedAtRobot,
                message.User.Name,
                room,
                message.When,
                message.Id,
                message.Content, 
                tokens));
        }
    }
}
