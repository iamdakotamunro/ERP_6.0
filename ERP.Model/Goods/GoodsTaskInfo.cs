using System;

namespace ERP.Model.Goods
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class GoodsTaskInfo
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public Guid GoodsId { get; set; }

        /// <summary>
        /// 商品名
        /// </summary>
        public string GoodsName { get; set; }

        /// <summary>
        /// 分公司ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 分公司名
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 任务ID
        /// </summary>
        public Guid TaskId { get; set; }

        /// <summary>
        /// 任务名
        /// </summary>
        public string TaskName { get; set; }

        /// <summary>
        /// 采购组ID
        /// </summary>
        public Guid PmId { get; set; }

        /// <summary>
        /// 采购组名
        /// </summary>
        public string PmName { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid WareHouseId { get; set; }

        /// <summary>
        /// 仓库名字
        /// </summary>
        public string WareHouseName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public GoodsTaskInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId">商品ID</param>
        /// <param name="goodsName">商品名</param>
        /// <param name="companyId">分公司ID</param>
        /// <param name="companyName">分公司名</param>
        /// <param name="taskId">任务ID</param>
        /// <param name="taskName">任务名</param>
        /// <param name="pmId">采购组ID</param>
        /// <param name="pmName">采购组名</param>
        /// <param name="wareHouseId">仓库ID</param>
        /// <param name="wareHouseName">仓库名</param>
        public GoodsTaskInfo(Guid goodsId, string goodsName, Guid companyId, string companyName, Guid taskId, string taskName, Guid pmId, string pmName, Guid wareHouseId, string wareHouseName)
        {
            GoodsId = goodsId;
            GoodsName = goodsName;
            CompanyId = companyId;
            CompanyName = companyName;
            TaskId = taskId;
            TaskName = taskName;
            PmId = pmId;
            PmName = pmName;
            WareHouseId = wareHouseId;
            WareHouseName = wareHouseName;
        }
    }
}
