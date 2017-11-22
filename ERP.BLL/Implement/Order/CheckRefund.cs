using System;
using System.Collections.Generic;
using System.Transactions;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IOrder;
using ERP.Environment;
using Keede.Ecsoft.Model;
using System.Linq;

namespace ERP.BLL.Implement.Order
{
    public class CheckRefund : BllInstance<CheckRefund>
    {
        private readonly ICheckRefund _refundDal;

        public CheckRefund(GlobalConfig.DB.FromType fromType)
        {
            _refundDal = OrderInstance.GetCheckRefundDao(fromType);
        }

        public CheckRefund(ICheckRefund checkRefund)
        {
            _refundDal = checkRefund;
        }

        /// <summary>
        /// 删除退换货检查信息
        /// </summary>
        /// <param name="refundId"></param>
        /// <returns></returns>
        public bool DeleteCheckRefundInfo(Guid refundId)
        {
            var info = _refundDal.GetCheckRefundInfo(refundId);
            if (info.RefundId == Guid.Empty)
            {
                return true;
            }
            return _refundDal.DeleteCheckRefundInfo(refundId);
        }

        #region 联盟店退货检查相关

        public Dictionary<Guid, int> GetShopCheckRefundDetailDic(Guid refundId, Guid applyId, int checkState)
        {
            var dataList = _refundDal.GetShopCheckRefundDetailList(refundId, applyId, checkState);
            if (dataList == null)
                return null;
            return dataList.ToDictionary(act => act.GoodsId, ac => ac.Quantity);
        }

        /// <summary>
        /// 修改退货检查明细(for 联盟店退货检查)
        /// </summary>
        /// <param name="checkRefundInfo"></param>
        /// <param name="checkRefundDetailInfos"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool UpdateCheckRefund(CheckRefundInfo checkRefundInfo, IList<CheckRefundDetailInfo> checkRefundDetailInfos, out string msg)
        {
            try
            {
                using (var scope = new TransactionScope())
                {
                    _refundDal.InsertCheckRefund(checkRefundInfo);
                    foreach (var item in checkRefundDetailInfos)
                    {
                        _refundDal.InsertCheckDetails(item);
                    }
                    scope.Complete();
                }
            }
            catch (Exception e)
            {
                msg = e.Message;
                return false;
            }
            msg = string.Empty;
            return true;
        }
        #endregion
    }
}
