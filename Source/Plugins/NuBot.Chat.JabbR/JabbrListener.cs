using System;
using System.ComponentModel.Composition;
using System.Security;
using System.Threading;
using JabbR.Client;
using NuBot.Configuration;

namespace NuBot.Chat.JabbR
{
    [Export(typeof(IPart))]
    public class JabbrListener : Part
    {
        public static readonly string HostConfigKey = "Host";
        public static readonly string UserNameConfigKey = "UserName";
        public static readonly string PasswordConfigKey = "Password";
        public static readonly string RoomsConfigKey = "Rooms";

        public override string Name { get { return "chat.jabbr"; } }
        public override string Title { get { return "JabbR Listener"; } }
        
        public Uri Host { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string[] Rooms { get; private set; }

        public override void Configure(IPartConfiguration myConfig)
        {
            // Get data from config
            Host = Host ?? myConfig.GetRequired(HostConfigKey, s => String.IsNullOrEmpty(s) ? null : new Uri(s));
            UserName = UserName ?? myConfig.GetRequired(UserNameConfigKey);
            Password = Password ?? myConfig.GetRequired(PasswordConfigKey);
            Rooms = Rooms ?? myConfig.GetRequired(RoomsConfigKey, s => String.IsNullOrEmpty(s) ? null : s.Split(','));
        }

        public override async void Attach(IRobot robo, CancellationToken token)
        {
            // Connect JabbR Client
            var client = new JabbRClient(Host.AbsoluteUri);
            try
            {
                robo.Log.Trace("Connecting to JabbR");
                var logOnInfo = await client.Connect(UserName, Password);
                robo.Log.Trace("Connection Established");
                robo.Log.Info("Jabbin in JabbR");
                await new JabbrListenerWorker(logOnInfo, client, Rooms, robo, new[] { robo.Name, UserName }, UserName).Run(token);
            }
            catch (SecurityException)
            {
                robo.Log.Error("Invalid User Name or Password");
            }
            catch (Exception ex)
            {
                robo.Log.Error(ex.Message);
            }
        }
    }
}
