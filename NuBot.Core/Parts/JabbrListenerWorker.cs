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
        private MessageScanner _scanner;

        public JabbrListenerWorker(MessageScanner scanner, LogOnInfo logOnInfo, JabbRClient client, string[] rooms, IRobot robo, string userName)
        {
            _robo = robo;
            _rooms = rooms;
            _client = client;
            _logOnInfo = logOnInfo;
            _userName = userName;
            _scanner = scanner;
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
                await _client.JoinRooms(extraRooms);
            }

            // Attach events
            _client.MessageReceived += _client_MessageReceived;
            
            // Wait until terminated and disconnect
            token.Register(() => {
                _client.LeaveRooms(_rooms).ContinueWith(t =>
                {
                    // Always disconnect as gracefully as possible
                    _client.Disconnect();
                });
            });
        }

        void _client_MessageReceived(Message message, string room)
        {
            // Process the message and put it on the bus
            var directedAtRobot = _scanner.IsForRobot(message.Content);
            _robo.Bus.Send(new ChatMessage(
                directedAtRobot,
                message.User.Name,
                room,
                message.When,
                message.Content, 
                message.Id));
        }
    }
}
