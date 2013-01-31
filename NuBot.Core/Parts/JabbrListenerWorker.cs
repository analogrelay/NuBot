using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client.Http;
using Microsoft.AspNet.SignalR.Client.Hubs;
using Microsoft.AspNet.SignalR.Client.Transports;

namespace NuBot.Core.Parts
{
    public class JabbrListenerWorker
    {
        private Uri _host;
        private IRobot _robo;
        private string[] _rooms;
        private CancellationToken _cancellationToken;
        private CookieContainer _cookieJar;

        public JabbrListenerWorker(Uri host, string[] rooms, IRobot robo, CancellationToken cancellationToken, CookieContainer cookieJar)
        {
            _host = host;
            _robo = robo;
            _rooms = rooms;
            _cancellationToken = cancellationToken;
            _cookieJar = cookieJar;
        }

        public async Task Run()
        {
            _robo.Log.Trace("Connecting to JabbR");
            // Connect to the chat hub
            var hubConnection = new HubConnection(_host.AbsoluteUri)
            {
                CookieContainer = _cookieJar
            };
            var chatHub = hubConnection.CreateHubProxy("chat");
            hubConnection.RegisterCallback(hr =>
            {
            });

            try
            {
                await hubConnection.Start();

                await chatHub.Invoke("Join");

                _robo.Log.Trace("Connected!");

                // Set up listener
                chatHub.On("addUser", HandleAddUser);

                while (!_cancellationToken.IsCancellationRequested)
                {
                    // Yield the task
                    await Task.Yield();
                }
            }
            finally
            {
                hubConnection.Stop();
            }
        }

        private void HandleAddUser(dynamic data)
        {

        }
    }
}
