using System;
using Framework.Core.IO;

namespace ERP.Service.Host
{
    internal class ServiceConfig
    {
        static readonly XmlFileHelper xml = new XmlFileHelper(AppDomain.CurrentDomain.BaseDirectory+ @"\Service.config");

        public static string Get(string xpath)
        {
            var nodes = xml.GetNode(xpath);
            if (nodes != null)
            {
                return nodes.InnerText;
            }
            return string.Empty;
        }

        public static string DisplayName
        {
            get
            {
                return Get(@"/config/service/ServiceDisplayName");
            }
        }

        public static string Description
        {
            get
            {
                return Get(@"/config/service/ServiceDescription");
            }
        }

        public static string ServiceName
        {
            get
            {
                return Get(@"/config/service/ServiceName");
            }
        }
    }
}
