using System;

namespace Keede.Ecsoft.Model
{
	/// <summary>
    ///lmshop_CheckStock 盘点计划表
	/// </summary>
	[Serializable]
	public class CheckStockInfo
	{
        /// <summary>
        /// 
        /// </summary>
        public CheckStockInfo() { }

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="checkID">盘点计划ID</param>
        /// <param name="checkNo">盘点计划编号</param>
        /// <param name="warehouseID">盘点仓库ID</param>
        /// <param name="stockFrozenTime">库存冻结时间</param>
        /// <param name="showStock">是否显示库存</param>
        /// <param name="state">状态</param>
        /// <param name="firstCount">初盘人</param>
        /// <param name="firstCountTime">初盘时间</param>
        /// <param name="secondCount">复盘人</param>
        /// <param name="secondCountTime">复盘时间</param>
        /// <param name="secondCheck">审核人</param>
        /// <param name="secondCheckTime">审核时间</param>
        /// <param name="finishTime">完成时间</param>
        public CheckStockInfo(Guid checkID, String checkNo, Guid warehouseID, DateTime stockFrozenTime, Boolean showStock,
            Int32 state, Guid firstCount, DateTime firstCountTime, Guid secondCount, DateTime secondCountTime, Guid secondCheck,
            DateTime secondCheckTime, DateTime finishTime) 
        {
            CheckID = checkID;
            CheckNo = checkNo;
            WarehouseID = warehouseID;
            StockFrozenTime = stockFrozenTime;
            ShowStock = showStock;
            State = state;
            FirstCount = firstCount;
            FirstCountTime = firstCountTime;
            SecondCount = secondCount;
            SecondCountTime = secondCountTime;
            SecondCheck = secondCheck;
            SecondCheckTime = secondCheckTime;
            FinishTime = finishTime;
        }

        /// <summary>
        /// 构造函数（添加用）
        /// </summary>
        /// <param name="checkID">盘点计划ID</param>
        /// <param name="checkNo">盘点计划编号</param>
        /// <param name="warehouseID">盘点仓库ID</param>
        /// <param name="stockFrozenTime">库存冻结时间</param>
        /// <param name="showStock">是否显示库存</param>
        /// <param name="state">状态</param>
        public CheckStockInfo(Guid checkID, String checkNo, Guid warehouseID, DateTime stockFrozenTime, Boolean showStock,Int32 state)
        {
            CheckID = checkID;
            CheckNo = checkNo;
            WarehouseID = warehouseID;
            StockFrozenTime = stockFrozenTime;
            ShowStock = showStock;
            State = state;
        }

		/// <summary>
		///盘点计划ID
		/// </summary>
		public Guid CheckID { get; set; }

		/// <summary>
		///盘点计划编号
		/// </summary>
		public String CheckNo { get; set; }

		/// <summary>
		///盘点仓库
		/// </summary>
		public Guid WarehouseID { get; set; }

		/// <summary>
		///库存冻结时间
		/// </summary>
		public DateTime StockFrozenTime { get; set; }

		/// <summary>
		///是否显示库存
		/// </summary>
		public Boolean ShowStock { get; set; }

		/// <summary>
		///状态
		/// </summary>
		public Int32 State { get; set; }

		/// <summary>
		///初盘人
		/// </summary>
		public Guid FirstCount { get; set; }

		/// <summary>
		///初盘时间
		/// </summary>
		public DateTime FirstCountTime { get; set; }

		/// <summary>
		///复盘人
		/// </summary>
		public Guid SecondCount { get; set; }

		/// <summary>
		///复盘时间
		/// </summary>
		public DateTime SecondCountTime { get; set; }

		/// <summary>
		///审核人
		/// </summary>
		public Guid SecondCheck { get; set; }

		/// <summary>
		///审核时间
		/// </summary>
		public DateTime SecondCheckTime { get; set; }

		/// <summary>
		///完成时间
		/// </summary>
		public DateTime FinishTime { get; set; }

	}
}
