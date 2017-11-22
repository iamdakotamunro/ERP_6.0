using System;
using System.Collections.Generic;

namespace ERP.UI.Web.Common
{
    public class ResouceServiceManager
    {
        private static readonly Dictionary<Guid, ResourceFileServerClient> _dicServiceClient = new Dictionary<Guid, ResourceFileServerClient>();
        public static ResourceFileServerClient Client
        {
            get
            {
                // var id = CurrentSession.SalePlatform.Get().ID;
                var id = CurrentSession.System.ShopWebSiteId;
                if (_dicServiceClient.ContainsKey(id))
                {
                    var client = _dicServiceClient[id];
                    return client ?? GetClient(id);
                }
                return GetClient(id);
            }
        }

        private static ResourceFileServerClient GetClient(Guid id)
        {
            var server = new ResourceFileServerClient(id);
            // server.Login(CurrentSession.SalePlatform.ID, id);
            _dicServiceClient.Add(id, server);
            return server;
        }

        //public static void ClearClient(Guid personnelId)
        //{
        //    if (DicServiceClient.ContainsKey(personnelId))
        //    {
        //        DicServiceClient.Remove(personnelId);
        //    }
        //}

        public static void Wake()
        {
            new FileClient().Wake();
        }

        public static bool UploadFile(string targetPath, string fileName, byte[] data)
        {
            using (var client = new UploadClient(CurrentSession.System.ShopWebSiteId))
            {
                return client.UploadFile(targetPath, fileName, data);
            }
        }
    }
}