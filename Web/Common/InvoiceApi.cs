using System;
using System.Net.Http;
using System.Net.Http.Headers;
using ERP.Model.Invoice;
using Config.Keede.Library;

namespace ERP.UI.Web.Common
{
    public class InvoiceApi
    {
        public const string CONTROLLER_NAME = "ExpressManage";
        static readonly string Api_Url = ConfManager.GetAppsetting("B2CUrl");

        public static ApiResultInfo GetInvoiceList(DateTime endDateTime,int invoiceType,Guid saleFilialeId,string invoiceNo,DateTime startDateTime,int pageIndex,int pageSize)
        {
            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.Timeout = new TimeSpan(0, 3, 0);
                HttpResponseMessage message = client.PostAsync(string.Format("{0}{1}?endDateTime={2}&invoiceType={3}&companyId={4}&invoiceNo={5}&startDateTime={6}&pageIndex={7}&pageSize={8}", Api_Url, "GetInvoiceList", endDateTime,
                   invoiceType, saleFilialeId, invoiceNo, startDateTime,pageIndex,pageSize), null).Result;
                var result = message.Content.ReadAsStringAsync().Result;
                if (string.IsNullOrEmpty(result) || result == "null") return null;
                return Newtonsoft.Json.JsonConvert.DeserializeObject<ApiResultInfo>(result);
            }
        }
    }

    

    

    
}