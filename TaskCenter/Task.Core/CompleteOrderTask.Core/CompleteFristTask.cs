using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using ERP.BLL.Implement.Inventory;
using ERP.BLL.Implement.Order;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using Keede.Ecsoft.Model;
using MIS.Enum;

namespace CompleteOrderTask.Core
{
    /// <summary>
    /// 完成订单的第一步骤
    /// </summary>
    public class CompleteFristTask
    {
        private readonly WarehouseManager _warehouseManager = new WarehouseManager();
        private readonly IList<MIS.Model.View.FilialeInfo> _filialeList;
        private readonly OrderManager _orderManager=OrderManager.WriteInstance;
        private readonly ICompanyCussent _companyCussent = new CompanyCussent(ERP.Environment.GlobalConfig.DB.FromType.Read);
        public CompleteFristTask()
        {
            using (var client = new Framework.WCF.ServiceClient<MIS.Service.Contract.IService>("Group.MIS"))
            {
                _filialeList = client.Instance.GetAllFiliale();
            }
        }

        public void RunFinish(DateTime finishDate, Guid personnelId, string operationer, Guid warehouseId, Guid expressId)
        {
            var taskDate = GetTaskDate(finishDate);
            var isFinish = DAL.OrderDao.ExistsFinish(warehouseId);
            while (!isFinish)
            {
                ERP.SAL.LogCenter.LogService.Log(LogCenter.Logger.LogLevel.Info, "任务还在继续...", "完成订单", new LogCenter.Logger.Models.MethodInfo("CompleteOrderTask.Core", "CompleteFristTask", "RunFinish"), finishDate, personnelId, operationer, warehouseId, expressId);
                Thread.Sleep(60000);
                isFinish = DAL.OrderDao.ExistsFinish(warehouseId);
            }
            var taskId = DAL.OrderDao.GetTaskId(taskDate, warehouseId, expressId);
            if (taskId == Guid.Empty)
            {
                taskId = Guid.NewGuid();
                DAL.OrderDao.AddTask(taskId, taskDate, warehouseId, expressId, operationer);
            }

            var warehouseInfo = _warehouseManager.GetWarehouse(warehouseId);
            var memberCompanyInfo = _companyCussent.GetMemberGeneralLedger();
            if (warehouseInfo != null)
            {
                WriteLog(taskId,
                         "=================================================================================================");
                WriteLog(taskId, ">> 仓库：" + warehouseInfo.WarehouseName + "，开始进行完成订单任务");
                if (memberCompanyInfo == null)
                {
                    WriteLog(taskId, ">> 尚未绑定会员往来账，不允许发货处理！");
                }
                else
                {
                    var filiales = _filialeList.Where(ent => ent.Rank == (int)FilialeRank.Head && (ent.Type == (int)FilialeType.SaleCompany || ent.Type == (int)FilialeType.EntityShop));
                    foreach (var filialeInfo in filiales)
                    {
                        var fInfo = filialeInfo;
                        var count = DAL.OrderDao.CountByState((int)OrderState.WaitConsignmented, expressId, filialeInfo.ID);
                        if (expressId == Guid.Empty)
                        {
                            WriteLog(taskId, ">> 销售公司：" + filialeInfo.Name + "，待发货数:" + count);
                        }
                        else
                        {
                            var expressInfo = _express.GetExpress(expressId);
                            WriteLog(taskId, ">> 销售公司：" + filialeInfo.Name + "，快递公司:" + expressInfo.ExpressName + "，待发货数:" + count);
                        }
                        int s = 0; //成功数
                        int f = 0; //失败数量
                        IList<GoodsOrderInfo> orderList = DAL.OrderDao.GetOrderListByState((int)OrderState.WaitConsignmented,
                                                                                         expressId, filialeInfo.ID, warehouseId);
                        foreach (GoodsOrderInfo goodsOrderInfo in orderList)
                        {
                            var orderInfo = goodsOrderInfo;
                            string errorMsg;
                            var success = _orderManager.JoinWaitConsignmentOrder(personnelId, orderInfo, operationer, out errorMsg);
                            if (success)
                            {
                                s++;
                            }
                            else
                            {
                                WriteLog(taskId, ">> 销售公司：" + fInfo.Name + "，订单号：" + orderInfo.OrderNo + "，完成订单发生异常，" + errorMsg);
                                f++;
                            }
                        }
                        WriteLog(taskId, ">> 销售公司：" + filialeInfo.Name + "成功完成订单：" + s + "，失败：" + f + "----------------");
                    }
                    DAL.OrderDao.FinishOver(taskId, warehouseId, expressId);
                }
            }
        }

        void WriteLog(Guid taskId, string message)
        {
            DAL.OrderDao.LogMessage(taskId, message);
        }

        string GetTaskDate(DateTime date)
        {
            return date.ToString("yyyy.MM.dd HH") + ":00:00";
        }
    }
}
