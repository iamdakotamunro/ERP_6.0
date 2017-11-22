using System;
using System.Configuration;

namespace RepairRealTimeGrossSettlementApp
{
    public class GlobalConfig
    {
        public static string ERPConnnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["ERP_Conn"].ConnectionString;
            }
        }

        public static string WmsDbName
        {
            get
            {
                return ConfigurationManager.AppSettings["WMS_DBName"];
            }
        }

        public static decimal InnerPurchasePriceRate
        {
            get
            {
                return decimal.Parse(ConfigurationManager.AppSettings["InnerPurchasePriceRate"]);
            }
        }

        public static DateTime RealTimeSettlementOccurTimeStartWith
        {
            get
            {
                return DateTime.Parse(ConfigurationManager.AppSettings["RealTimeSettlementOccurTimeStartWith"]);
            }
        }

        //public static bool InitRealTimeSettlementEnabled
        //{
        //    get
        //    {
        //        return ConfigurationManager.AppSettings["InitRealTimeSettlementEnabled"] == "true";
        //    }
        //}
    }
}
