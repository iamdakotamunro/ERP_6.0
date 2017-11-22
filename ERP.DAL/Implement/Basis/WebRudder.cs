using System.Data.SqlClient;
using ERP.DAL.Interface.IBasis;
using Keede.Ecsoft.Model;
using ERP.Environment;
using Keede.DAL.RWSplitting;
using Dapper;
using System.Transactions;

namespace ERP.DAL.Implement.Basis
{
    /// <summary>
    /// 
    /// </summary>
    public class WebRudder : IWebRudder
    {
        public WebRudder(GlobalConfig.DB.FromType fromType) { }

        /// <summary>
        /// 获取网站基本设置
        /// </summary>
        /// <returns></returns>
        public WebRudderInfo GetWebRudder()
        {
            WebRudderInfo webRudderInfo;
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                const string SQL_SELECT_WEBRUDDER = "SELECT WebName,WebHolder,WebTitle,WebUrl,DefaultGoodsType,DefaultUnitsId,DefaultCurrencyId,CurrencyDecimalType,CurrencyDecimalDigits,DefaultLanguage,METADescription,METAKeywords,WorkingDay,WorkingHoursBegin,WorkingHoursEnd FROM lmShop_Webrudder";
                webRudderInfo = conn.QueryFirstOrDefault<WebRudderInfo>(SQL_SELECT_WEBRUDDER);
            }
            if (webRudderInfo == null)
            {
                webRudderInfo = new WebRudderInfo();
            }
            return webRudderInfo;
        }
    }
}