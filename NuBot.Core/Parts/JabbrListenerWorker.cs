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

namespace NuBot.Core.Parts
{
    public class JabbrListenerWorker
    {
        private IRobot _robo;
        private string[] _rooms;
        private string _userName;
        private JabbRClient _client;
        private LogOnInfo _logOnInfo;

        public JabbrListenerWorker(LogOnInfo logOnInfo, JabbRClient client, string[] rooms, IRobot robo, string userName)
        {
            _robo = robo;
            _rooms = rooms;
            _client = client;
            _logOnInfo = logOnInfo;
            _userName = userName;
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
            _robo.Log.Trace("[{0}] {1}: {2}", room, message.User.Name, message.Content);
            if (Regex.IsMatch(message.Content, ".*" + _userName + ".*") || Regex.IsMatch(message.Content, ".*" + _robo.Name + ".*"))
            {
                // Send a dumb response
                _client.Send(String.Format("Hi {0}, what can I do for you?", message.User.Name), room);
            }
        }
    }
}
