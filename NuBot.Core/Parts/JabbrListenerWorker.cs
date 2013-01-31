using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
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
        private JabbRClient _client;
        private LogOnInfo _logOnInfo;

        public JabbrListenerWorker(LogOnInfo logOnInfo, JabbRClient client, string[] rooms, IRobot robo)
        {
            _robo = robo;
            _rooms = rooms;
            _client = client;
            _logOnInfo = logOnInfo;
        }

        public async Task Run()
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

            // Say hi in all our rooms
            foreach (var room in _rooms)
            {
                await _client.Send("Hello World!", room);
            }

            // Leave :)
            _client.Disconnect();
        }

        private void HandleAddUser(dynamic data)
        {

        }
    }
}
