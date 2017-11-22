using System;
using System.Collections.Generic;
using ERP.Model;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IOrder
{
    public interface ICompleteOrderTask
    {
        IEnumerable<CompleteOrderTaskRecordInfo> GetWaitConsignmentedList(Guid warehouseId, Guid expressId);
        
        IEnumerable<CompleteOrderTaskInfo> GetCompleteOrderTaskExpressByTaskId(Guid taskId);

        bool AddCompleteOrderTaskAndDetails(CompleteOrderTaskInfo info, List<CompleteOrderTaskDetailsInfo> detailsList);

        IEnumerable<CompleteOrderTaskDetailsInfo> GetCompleteOrderTaskDetailsList(Guid taskId);

        bool RebootTask(Guid taskId);

        bool SetCompleteOrderTaskDetail(Guid orderId);

        IEnumerable<GoodsOrderInfo> GetExportGoodsOrderList(Guid taskId, Guid expressId);
    }
}
