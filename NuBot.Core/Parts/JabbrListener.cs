using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR.Client.Hubs;

namespace NuBot.Core.Parts
{
    public class JabbrListener : IPart
    {
        private const string JabbrCookieName = "jabbr.userToken";
        private const string UserNameParamName = "username";
        private const string PasswordParamName = "password";

        public static readonly string HostConfigKey = "Jabbr.Host";
        public static readonly string UserNameConfigKey = "Jabbr.UserName";
        public static readonly string PasswordConfigKey = "Jabbr.Password";
        public static readonly string RoomsConfigKey = "Jabbr.Rooms";

        private CookieContainer _cookieJar = new CookieContainer();
        
        public string Name { get { return "JabbR Listener"; } }
        
        public Uri Host { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string[] Rooms { get; private set; }

        private HttpClient _client;
        private HttpClient Client
        {
            get
            {
                return _client ?? (_client = new HttpClient(new WebRequestHandler()
                {
                    CookieContainer = _cookieJar,
                    UseCookies = true
                }));
            }
        }

        public JabbrListener()
        {
        }

        public JabbrListener(Uri host, string username, string password, string[] rooms)
        {
            Host = host;
            UserName = username;
            Password = password;
            Rooms = rooms;
        }

        public async Task Run(IRobot robo, CancellationToken token)
        {
            // Get data from config if not specified
            Host = Host ?? robo.Configuration.GetSetting(HostConfigKey, s => new Uri(s));
            UserName = UserName ?? robo.Configuration.GetSetting(UserNameConfigKey);
            Password = Password ?? robo.Configuration.GetSetting(PasswordConfigKey);
            Rooms = Rooms ?? robo.Configuration.GetSetting(RoomsConfigKey, s => s.Split(','));

            // Authenticate with JabbR and get a cookie
            var url = Host.AbsoluteUri + "account/login";
            robo.Log.Trace("http POST {0}", url);
            var response = await Client.PostAsync(
                url,
                new FormUrlEncodedContent(new[] {
                    new KeyValuePair<string, string>(UserNameParamName, UserName),
                    new KeyValuePair<string, string>(PasswordParamName, Password)
                }));

            robo.Log.Trace("http {0} {1}", (int)response.StatusCode, url);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var cookies = _cookieJar.GetCookies(Host);
                if (cookies[JabbrCookieName] == null)
                {
                    robo.Log.Error("Didn't get a cookie from JabbR. Is your user name and password correct?");
                }
                else
                {
                    // Start the worker loop
                    await new JabbrListenerWorker(Host, Rooms, robo, token, _cookieJar).Run();
                }
            }
        }
    }
}
