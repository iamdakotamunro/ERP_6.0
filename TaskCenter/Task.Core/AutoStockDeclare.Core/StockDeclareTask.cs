using System;
using Framework.Common;
using System.Text;
using ERP.BLL.Implement.Purchasing;
using Config.Keede.Library;

namespace AutoStockDeclare.Core
{
    public class StockDeclareTask
    {
        #region --> Config

        public static string FirstStockDeclareTime
        {
            get { return ConfManager.GetAppsetting("FirstStockDeclareTime"); }
        }

        public static string SecondDeclareStockTime
        {
            get { return ConfManager.GetAppsetting("SecondStockDeclareTime"); }
        }

        #endregion

        private static readonly StockDeclareManager _stockDeclareManager = StockDeclareManager.WriteInstance;

        public static void RunStockDeclare()
        {
            var currentHour = DateTime.Now.ToShortTimeString();
            if (currentHour == FirstStockDeclareTime || currentHour == SecondDeclareStockTime)
            {
                try
                {
                    var msg = new StringBuilder();
                    _stockDeclareManager.AutoDeclare(DateTime.Now, ref msg);
                    ERP.SAL.LogCenter.LogService.LogInfo(msg.ToString(), "进货申报");
                }
                catch (Exception exp)
                {
                    ERP.SAL.LogCenter.LogService.LogError("执行进货申报出错", "进货申报", exp);
                }
            }
        }
    }
}
