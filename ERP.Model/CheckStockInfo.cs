using System;

namespace Keede.Ecsoft.Model
{
	/// <summary>
    ///lmshop_CheckStock �̵�ƻ���
	/// </summary>
	[Serializable]
	public class CheckStockInfo
	{
        /// <summary>
        /// 
        /// </summary>
        public CheckStockInfo() { }

        /// <summary>
        /// ���캯��
        /// </summary>
        /// <param name="checkID">�̵�ƻ�ID</param>
        /// <param name="checkNo">�̵�ƻ����</param>
        /// <param name="warehouseID">�̵�ֿ�ID</param>
        /// <param name="stockFrozenTime">��涳��ʱ��</param>
        /// <param name="showStock">�Ƿ���ʾ���</param>
        /// <param name="state">״̬</param>
        /// <param name="firstCount">������</param>
        /// <param name="firstCountTime">����ʱ��</param>
        /// <param name="secondCount">������</param>
        /// <param name="secondCountTime">����ʱ��</param>
        /// <param name="secondCheck">�����</param>
        /// <param name="secondCheckTime">���ʱ��</param>
        /// <param name="finishTime">���ʱ��</param>
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
        /// ���캯��������ã�
        /// </summary>
        /// <param name="checkID">�̵�ƻ�ID</param>
        /// <param name="checkNo">�̵�ƻ����</param>
        /// <param name="warehouseID">�̵�ֿ�ID</param>
        /// <param name="stockFrozenTime">��涳��ʱ��</param>
        /// <param name="showStock">�Ƿ���ʾ���</param>
        /// <param name="state">״̬</param>
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
		///�̵�ƻ�ID
		/// </summary>
		public Guid CheckID { get; set; }

		/// <summary>
		///�̵�ƻ����
		/// </summary>
		public String CheckNo { get; set; }

		/// <summary>
		///�̵�ֿ�
		/// </summary>
		public Guid WarehouseID { get; set; }

		/// <summary>
		///��涳��ʱ��
		/// </summary>
		public DateTime StockFrozenTime { get; set; }

		/// <summary>
		///�Ƿ���ʾ���
		/// </summary>
		public Boolean ShowStock { get; set; }

		/// <summary>
		///״̬
		/// </summary>
		public Int32 State { get; set; }

		/// <summary>
		///������
		/// </summary>
		public Guid FirstCount { get; set; }

		/// <summary>
		///����ʱ��
		/// </summary>
		public DateTime FirstCountTime { get; set; }

		/// <summary>
		///������
		/// </summary>
		public Guid SecondCount { get; set; }

		/// <summary>
		///����ʱ��
		/// </summary>
		public DateTime SecondCountTime { get; set; }

		/// <summary>
		///�����
		/// </summary>
		public Guid SecondCheck { get; set; }

		/// <summary>
		///���ʱ��
		/// </summary>
		public DateTime SecondCheckTime { get; set; }

		/// <summary>
		///���ʱ��
		/// </summary>
		public DateTime FinishTime { get; set; }

	}
}
