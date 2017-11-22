using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using ERP.Model.Finance;
using ERP.Model.Finance.Payment;
using ERP.Model.Finance.Gathering;

namespace ERP.Service.Contract
{
    /// <summary>
    /// 财务API
    /// </summary>
    public partial interface IService
    {
        #region 付款单

        /// <summary>
        /// 获取付款单：往来单位收付款（收付款类型=付款单）
        /// 功能点：ERP往来单位收/付款=》完成
        /// 业务点：往来付款
        /// 数据库：lmshop_CompanyFundReceipt
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<CompanyFundReceiptDTO> GetPayment_CompanyFundReceipt(DateTime StartTime, DateTime EndTime);

        /// <summary>
        /// 获取付款单：资金流（转账）。上海百秀旗舰店》百秀兰溪转账。
        /// 功能点：ERP资金流=》转账
        /// 业务点：上海百秀旗舰店天猫向百秀兰溪转账
        /// 数据库：lmShop_WasteBook
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        IList<WasteBookDTO> GetPayment_WasteBook(DateTime StartTime, DateTime EndTime);

        #endregion 付款单
    }
}