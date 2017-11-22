using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Interface
{
    public interface IReckoningManager
    {
        /// <summary>
        /// 完成订单业务后，添加往来帐
        /// </summary>
        /// <param name="orderInfo"></param>
        /// <param name="orderDetailList"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool AddByCompleteOrder(GoodsOrderInfo orderInfo, IList<GoodsOrderDetailInfo> orderDetailList,
            out string errorMessage);

        /// <summary>
        /// 运行异步插入往来帐数据
        /// </summary>
        /// <param name="readCount"></param>
        void RunAsynAddTask(int readCount);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="asynInfo"></param>
        /// <returns></returns>
        bool AddAsynInfo(Model.ASYN.ASYNReckoningInfo asynInfo);
    }
}
