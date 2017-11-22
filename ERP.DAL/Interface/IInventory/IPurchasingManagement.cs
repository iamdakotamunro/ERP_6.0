using System;
using System.Collections.Generic;
using ERP.Model;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface IPurchasingManagement
    {
        /// <summary>
        /// 根据日期获取进入需调拨统计的业务数据
        /// zhangfan added at 2013-Sep-26th
        /// </summary>
        /// <returns></returns>
        IList<NeedToAllocateInfo> GetNeedToAllocateDataInfos(DateTime startTime, DateTime endTime);

        ///// <summary>
        ///// 根据时间段和商品负责人员获取当月销量 ADD 文雯 2016-05-12
        ///// </summary>
        ///// <returns></returns>
        //int GetGoodsOrderDetailInQuantity(Guid personResponsible, DateTime startTime, DateTime endTime);
    }
}
