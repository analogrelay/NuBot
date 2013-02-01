using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Net;
using System.Security;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using JabbR.Client;
using Microsoft.AspNet.SignalR.Client.Hubs;
using NuBot.Configuration;

namespace NuBot.Chat.JabbR
{
    [Export(typeof(IPart))]
    public class JabbrListener : Part
    {
        public static readonly string HostConfigKey = "Jabbr.Host";
        public static readonly string UserNameConfigKey = "Jabbr.UserName";
        public static readonly string PasswordConfigKey = "Jabbr.Password";
        public static readonly string RoomsConfigKey = "Jabbr.Rooms";

        private CookieContainer _cookieJar = new CookieContainer();
        
        public override string Name { get { return "JabbR Listener"; } }
        
        public Uri Host { get; private set; }
        public string UserName { get; private set; }
        public string Password { get; private set; }
        public string[] Rooms { get; private set; }
        
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

        public override async void Attach(IRobot robo, CancellationToken token)
        {
            // Get data from config if not specified
            Host = Host ?? robo.Configuration.GetSetting(HostConfigKey, s => String.IsNullOrEmpty(s) ? null : new Uri(s));
            UserName = UserName ?? robo.Configuration.GetSetting(UserNameConfigKey);
            Password = Password ?? robo.Configuration.GetSetting(PasswordConfigKey);
            Rooms = Rooms ?? robo.Configuration.GetSetting(RoomsConfigKey, s => String.IsNullOrEmpty(s) ? null : s.Split(','));
            
            // Validate data
            //  This is a rare case in which we DON'T want short-circuiting since we want to print as many "blah is required" messages as possible
            if (!Require(robo, Host, HostConfigKey) |
               !Require(robo, UserName, UserNameConfigKey) |
               !Require(robo, Password, PasswordConfigKey) |
               !Require(robo, Rooms, RoomsConfigKey))
            {
                return;
            }

            // Connect JabbR Client
            var client = new JabbRClient(Host);
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

        private bool Require(IRobot robo, string value, string settingName)
        {
            return Require(robo, String.IsNullOrEmpty(value), settingName);
        }

        private bool Require(IRobot robo, object value, string settingName)
        {
            return Require(robo, value == null, settingName);
        }

        private bool Require(IRobot robo, bool invalidCondition, string settingName)
        {
            if (invalidCondition)
            {
                robo.Log.Error("The {0} setting is required", settingName);
            }
            return !invalidCondition;
        }
    }
}
