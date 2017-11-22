using System;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Implement.Inventory
{
    /// <summary>
    /// 待盘点商品建议处理 add by dinghq 2011-06-29
    /// </summary>
    public class WaitCheckStockGoods:BllInstance<WaitCheckStockGoods>
    {
        private readonly IWaitCheckStockGoods _waitCheckStockGoodsDao;

        public WaitCheckStockGoods(Environment.GlobalConfig.DB.FromType fromType)
        {
            _waitCheckStockGoodsDao = InventoryInstance.GetWaitCheckStockGoodsDao(fromType);
        }

        /// <summary>
        /// 添加或是修改待盘点商品信息(需要捕获异常)
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public int AdOrUpWaitCheckGoodsInfo(WaitCheckGoodsInfo info)
        {
            try
            {
                WaitCheckGoodsInfo checkInfo = _waitCheckStockGoodsDao.GetCheckGoodsInfo(info.GoodsId, info.WarehouseId);

                if (checkInfo == null)
                {
                    return _waitCheckStockGoodsDao.InsertWaitCheckGoodsInfo(info);
                }

                if (checkInfo.State == (int)WaitCheckGoodsState.Checked)
                {
                    return _waitCheckStockGoodsDao.UpdateWaitCheckGoodsState(info.GoodsId, info.WarehouseId, WaitCheckGoodsState.WaitCheck);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return 1;
        }
    }
}
