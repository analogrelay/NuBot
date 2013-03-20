using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JabbR.Client;
using JabbR.Client.Models;
using NuBot.Messages;
using NuBot.Services;

namespace NuBot.Chat.JabbR
{
    public class JabbrListenerWorker
    {
        private readonly IRobot _robo;
        private readonly string[] _rooms;
        private readonly string _userName;
        private readonly JabbRClient _client;
        private readonly LogOnInfo _logOnInfo;
        
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
                foreach (var room in extraRooms)
                {
                    await _client.LeaveRoom(room);
                }
            }

            // Join rooms we're not in
            extraRooms = _rooms.Except(_logOnInfo.Rooms.Select(r => r.Name));
            if (extraRooms.Any())
            {
                _robo.Log.Trace("Joining rooms: {0}", String.Join(",", extraRooms));
                foreach (var room in extraRooms)
                {
                    await _client.JoinRoom(room);
                }
            }

            await _client.SetFlag("CA");

            // Attach events
            _client.MessageReceived += _client_MessageReceived;

            // Attach to the bus
            _robo.Bus.On<SendChatMessage>(SendMessage);
            
            // Wait until terminated and disconnect
            token.Register(() => {
                Task.WhenAll(_rooms.Select(s => _client.LeaveRoom(s)).ToArray()).ContinueWith(t => _client.Disconnect());
            });
        }

        private void SendMessage(SendChatMessage msg)
        {
            if (msg.MeMessage)
            {
                _client.Send("/me " + msg.Message, msg.Room);
            }
            else
            {
                _client.Send(msg.Message, msg.Room);
            }
        }

        void _client_MessageReceived(Message message, string room)
        {
            _robo.Bus.Send(new RawChatMessage(
                message.User.Name,
                room,
                message.When,
                message.Id,
                message.Content, 
                String.Equals(message.User.Name, _userName, StringComparison.OrdinalIgnoreCase),
                new[] { _userName }));
        }
    }
}
