using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using NuBot.NuGet.Deployment.NuGetSecretStore;

namespace NuBot.NuGet.Deployment
{
    public class AzureConfigGenerator
    {
        const string AzureXmlns = "http://schemas.microsoft.com/ServiceHosting/2008/10/ServiceConfiguration";

        private static Dictionary<string, Action<XElement, EnvironmentsItem>> _handlers = new Dictionary<string, Action<XElement, EnvironmentsItem>>(StringComparer.OrdinalIgnoreCase)
        {
            {"Remote Desktop Settings", HandleRDSetting }
        };

        private Uri _secretStoreUrl;

        public AzureConfigGenerator(Uri secretStoreUrl)
        {
            _secretStoreUrl = secretStoreUrl;
        }

        public XDocument BuildCsCfg(string environment)
        {
            XDocument doc = new XDocument();

            // Create the elements we need to add things to
            XElement configSettings = new XElement("ConfigurationSettings");
            XElement certificates = new XElement("Certificates");

            // Make the OData Query to get all the things
            NuGetTeamSiteDataContext context = new NuGetTeamSiteDataContext(_secretStoreUrl);
            context.Credentials = CredentialCache.DefaultNetworkCredentials;
            
            // Find the environment request
            EnvironmentsItem env = context.Environments.Where(e => e.ContentType == "Folder" && String.Equals(e.Title, environment, StringComparison.OrdinalIgnoreCast)).FirstOrDefault();
            if (env == null)
            {
                throw new InvalidOperationException(String.Format("No such environment: '{0}'", environment));
            }
            var items = context.Environments.Where(e => e.Path == env.Path + "/" env.Title);
            var certs = context.Environments.Where(e => e.ContentType == "Certificates");

            // Fill them
            FillConfigSettings(configSettings, items);
            FillCertificates(certificates, certs);
            XElement instances = GetInstancesElement(environment);

            // Wrap the goop around them
            doc.Add(
                new XElement("ServiceConfiguration",
                    XNamespace.Get(AzureXmlns),
                    new XAttribute("serviceName", "Azure"),
                    new XAttribute("osFamily", 3),
                    new XAttribute("osVersion", "*"),

                    new XElement("Role"),
                        new XAttribute("name", "Website"),

                        instances,
                        configSettings,
                        certificates
                    ));

            return doc;
        }

        private void FillCertificates(XElement certificates, IQueryable<EnvironmentsItem> certs)
        {
            throw new NotImplementedException();
        }

        private void FillConfigSettings(XElement configSettings, IQueryable<EnvironmentsItem> items)
        {
            foreach (var item in items)
            {
                Action<XElement, EnvironmentsItem> handler = null;
                if(_handlers.TryGetValue(item.ContentType, out handler)) {
                    handler(configSettings, item);
                }
            }
        }

        private static void HandleRDSetting(XElement configSettings, EnvironmentsItem item)
        {
            AddConfigSetting(configSettings, "Microsoft.WindowsAzure.Plugins.RemoteAccess.Enabled", "true");
            AddConfigSetting(configSettings, "Microsoft.WindowsAzure.Plugins.RemoteForwarder.Enabled", "true");
            AddConfigSetting(configSettings, "Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountUsername", item.UserName);
            AddConfigSetting(configSettings, "Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountEncryptedPassword", item.EncryptedPassword);
            AddConfigSetting(configSettings, "Microsoft.WindowsAzure.Plugins.RemoteAccess.AccountExpiration", item.Expiration);
        }

        private static void AddConfigSetting(XElement configSettings, string key, string value)
        {
            configSettings.Add(new XElement("Setting",
                new XAttribute("name", key),
                new XAttribute("value", value)));
        }

        private XElement GetInstancesElement(string environment)
        {
            throw new NotImplementedException();
        }
    }
}
