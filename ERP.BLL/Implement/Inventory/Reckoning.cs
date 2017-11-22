using System;
using System.Collections.Generic;
using System.Transactions;
using ERP.DAL.Factory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Implement.Inventory
{
    public class ReckoningManager : BllInstance<ReckoningManager>
    {
        static IReckoning _reckoningDao;
        readonly IWasteBook _wasteBookManager;

        public ReckoningManager(Environment.GlobalConfig.DB.FromType fromType)
        {
            _wasteBookManager = new WasteBook(fromType);
            _reckoningDao = InventoryInstance.GetReckoningDao(fromType);
        }

        public ReckoningManager(IReckoning reckoning,IWasteBook wasteBook)
        {
            _wasteBookManager = wasteBook;
            _reckoningDao = reckoning;
        }

        /// <summary>
        /// 对账
        /// </summary>
        /// <param name="lstModify">需要修改状态的往来账列表</param>
        /// <param name="lstAdd">需要增加的往来账列表</param>
        /// <param name="wastBookinfo">需要添加的资金流信息</param>
        public void Checking(IList<ReckoningInfo> lstModify, IList<ReckoningInfo> lstAdd, WasteBookInfo wastBookinfo)
        {
            using (var scop = new TransactionScope())
            {
                try
                {
                    //1.更新被对账往来为已对账
                    if (lstModify.Count > 0)
                        _reckoningDao.UpdateCheckState(lstModify, 1);

                    //2.新增往来账（包括调账）
                    foreach (ReckoningInfo reckoningInfo in lstAdd)
                    {
                        reckoningInfo.AuditingState = (int)AuditingState.Yes;
                        string errorMessage;
                        _reckoningDao.Insert(reckoningInfo, out errorMessage);
                    }

                    //3.资金流 增加一条数据
                    if (wastBookinfo.Income != 0)
                    {
                        _wasteBookManager.Insert(wastBookinfo);
                    }
                    scop.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception("对账失败!", ex);
                }
            }

        }
    }
}
