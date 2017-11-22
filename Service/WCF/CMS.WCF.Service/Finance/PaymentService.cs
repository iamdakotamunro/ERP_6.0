using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ERP.Enum;
using ERP.Model.Finance;
using ERP.DAL.Finance.Interface;
using ERP.Environment;
using ERP.DAL.Finance;
using ERP.Model.Finance.Payment;
using ERP.Model.Finance.Gathering;
using ERP.BLL.Implement;

namespace ERP.Service.Implement
{
    public partial class Service
    {
        private readonly static IPaymentDAL _paymentDAL = new PaymentDAL(GlobalConfig.DB.FromType.Read);

        #region 付款单

        /// <summary>
        /// 获取付款单：往来单位收付款（收付款类型=付款单）
        /// 功能点：ERP往来单位收/付款=》完成
        /// 业务点：往来付款
        /// 数据库：lmshop_CompanyFundReceipt
        /// </summary>
        /// <returns></returns>
        public IList<CompanyFundReceiptDTO> GetPayment_CompanyFundReceipt(DateTime StartTime, DateTime EndTime)
        {
            return _paymentDAL.GetPayment_CompanyFundReceipt(StartTime, EndTime);
        }

        /// <summary>
        /// 获取付款单：资金流（转账）。上海百秀旗舰店》百秀兰溪转账。
        /// 功能点：ERP资金流=》转账
        /// 业务点：上海百秀旗舰店天猫向百秀兰溪转账
        /// 数据库：lmShop_WasteBook
        /// </summary>
        /// <returns></returns>
        public IList<WasteBookDTO> GetPayment_WasteBook(DateTime StartTime, DateTime EndTime)
        {
            //转出公司（物流公司）：上海百秀 444e0c93-1146-4386-bae2-cb352da70499
            //转入公司（销售公司）：百秀兰溪 75621b55-2fa3-4fcf-b68a-039c28f560b6

            string LogisticsCompany = ConfigManage.GetConfigValue("BaiXiu_Shanghai_ID");//物流公司
            string SalesCompany = ConfigManage.GetConfigValue("BaiXiu_Lanxi_ID");//销售公司
            return _paymentDAL.GetPayment_WasteBook(StartTime, EndTime, LogisticsCompany, SalesCompany);
        }

        #endregion 付款单
    }
}