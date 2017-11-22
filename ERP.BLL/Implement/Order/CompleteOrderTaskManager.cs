using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Enum;
using ERP.Model;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Implement.Order
{
    public class CompleteOrderTaskManager
    {
        public static IEnumerable<CompleteOrderTaskInfo> GetCompleteOrderTaskExpressByTaskId(Guid taskId)
        {
            return DAL.CompleteOrderTask.Instance.GetCompleteOrderTaskExpressByTaskId(taskId).ToList();
        }

        public static void CreateTask(Guid warehouseId, Guid expressId, Guid operationId, string operationer, out string errorMsg)
        {
            errorMsg = string.Empty;
            var goodsOrderRecordList = DAL.CompleteOrderTask.Instance.GetWaitConsignmentedList(warehouseId, expressId).ToList();
            if (!goodsOrderRecordList.Any())
            {
                errorMsg = "目前没有待完成的订单";
            }
            else
            {
                var completeOrderTaskInfo = new CompleteOrderTaskInfo
                {
                    ID = Guid.NewGuid(),
                    CreateTime = DateTime.Now,
                    WarehouseId = warehouseId,
                    ExpressId = expressId,
                    OperationId = operationId,
                    Operationer = operationer,
                    StartTime = DateTime.Now,
                    EndTime = DateTime.Now,
                    TotalQuantity = goodsOrderRecordList.Count,
                    TaskState = (int)CompleteOrderTaskState.Wait,
                    Description = string.Empty
                };
                var details = goodsOrderRecordList.Select(goodsOrderRecordInfo => new CompleteOrderTaskDetailsInfo
                {
                    TaskId = completeOrderTaskInfo.ID,
                    OrderId = goodsOrderRecordInfo.OrderId,
                    OrderNo = goodsOrderRecordInfo.OrderNo,
                    SaleFilialeId = goodsOrderRecordInfo.SaleFilialeId,
                    SalePlatformId = goodsOrderRecordInfo.SalePlatformId,
                }).ToList();
                var isSuccess = DAL.CompleteOrderTask.Instance.AddCompleteOrderTaskAndDetails(completeOrderTaskInfo, details);
                if (!isSuccess)
                    errorMsg = "生成任务繁忙，请稍后重试";
            }
        }

        public static IEnumerable<CompleteOrderTaskDetailsInfo> GetCompleteOrderTaskDetailsList(Guid taskId)
        {
            return DAL.CompleteOrderTask.Instance.GetCompleteOrderTaskDetailsList(taskId).ToList();
        }

        public static bool RebootTask(Guid taskId)
        {
            return DAL.CompleteOrderTask.Instance.RebootTask(taskId);
        }

        public static bool SetCompleteOrderTaskDetail(Guid orderId)
        {
            return DAL.CompleteOrderTask.Instance.SetCompleteOrderTaskDetail(orderId);
        }

        public static IEnumerable<GoodsOrderInfo> GetExportGoodsOrderList(Guid taskId, Guid expressId)
        {
            return DAL.CompleteOrderTask.Instance.GetExportGoodsOrderList(taskId, expressId);
        }
    }
}
