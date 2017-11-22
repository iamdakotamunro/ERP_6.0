using System;
using System.Linq;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IOrder;
using ERP.SAL;
using ERP.SAL.Interface;
using Framework.Core.Utility;

namespace GoodsStockRecordTask.Core
{
    /// <summary>
    /// 商品毛利存档
    /// </summary>
    public class GoodsGrossProfitManager
    {
        static readonly IGoodsOrderDetail _goodsOrderDetail = new GoodsOrderDetail(ERP.Environment.GlobalConfig.DB.FromType.Write);

        static readonly IGoodsGrossProfit _goodsGrossProfit = new GoodsGrossProfitDao();

        static readonly IPromotionSao _promotionSao = new PromotionSao();

        static readonly IGoodsGrossProfitRecordDetail _goodsGrossProfitRecordDetail = new GoodsGrossProfitRecordDetailDao();

        static int _excuteday;

        private static readonly GoodsGrossProfitBll _goodsGrossProfitBll = new GoodsGrossProfitBll(_goodsGrossProfit, _goodsOrderDetail, _promotionSao, _goodsGrossProfitRecordDetail);

        /// <summary>
        /// 生成商品毛利记录存档
        /// </summary>
        public static void RunGoodsGrossProfitTask()
        {
            try
            {
                var date = DateTime.Now;
                if (date.Day == _excuteday) return;
                var createStockHour = Configuration.AppSettings["CreateStockHour"];
                var hours = createStockHour.Split(',');
                if (!hours.Contains(string.Format("{0}", date.Hour))) return;

                string errorMsg;
                //10月份算9月份存档
                var result = _goodsGrossProfitBll.AddGoodsGrossProfitRecord(date, out errorMsg);
                if (!result)
                {
                    ERP.SAL.LogCenter.LogService.LogError("生成商品毛利记录失败", "商品毛利记录", new Exception(errorMsg));
                }
                else
                {
                    _excuteday = date.Day;
                }
            }
            catch (Exception exp)
            {
                ERP.SAL.LogCenter.LogService.LogError("生成商品毛利异常", "商品毛利记录", exp);
            }
        }
    }
}
