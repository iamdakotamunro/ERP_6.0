using System;
using System.Transactions;
using ERP.BLL.Implement.Organization;
using ERP.DAL.Interface.IShop;
using ERP.Enum.ShopFront;
using ERP.SAL;

namespace ERP.BLL.Implement.Shop
{
    public class ShopRefundMessageBll 
    {
        private readonly IShopRefundMessage _shopRefund;

        public ShopRefundMessageBll(IShopRefundMessage shopRefund)
        {
            _shopRefund = shopRefund;
        }

        /// <summary>
        /// 设置联盟店为可退货状态
        /// </summary>
        /// <param name="shopId"></param>
        /// <param name="isAllow"></param>
        /// <param name="remark"> </param>
        /// <param name="msg"></param>
        /// <param name="msgId"></param>
        /// <returns></returns>
        public bool SetIsAllowReturnGoods(Guid shopId,Guid msgId,bool isAllow,string remark,out string msg)
        {
            bool flag;
            Guid parentId=FilialeManager.GetShopHeadFilialeId(shopId);
            if(parentId==Guid.Empty)
            {
                msg = "未找到对应总公司信息!";
                return false;
            }
            using (var scope = new TransactionScope())
            {
                //修改联盟店退货次数
                flag = ShopSao.SetCanReturnCount(parentId,shopId, isAllow, out msg);
                if(flag)
                {
                    //修改ERP退货留言
                    var row = _shopRefund.SetMessageState(msgId, (int)ReturnMsgState.Pass, remark);
                    if(row>=0)
                    {
                        scope.Complete();
                    }
                    else
                    {
                        flag = false;
                        msg = "留言审核失败!";
                    }    
                }
            }
            return flag;
        }
    }
}
