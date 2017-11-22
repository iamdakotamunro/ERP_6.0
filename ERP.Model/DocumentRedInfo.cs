using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/8/11 13:09:00 
     * 描述    : 单据红冲
     * =====================================================================
     * 修改时间：2016/8/11 13:09:00 
     * 修改人  ：  
     * 描述    ：
     */
    public class DocumentRedInfo
    {
        /// <summary>
        /// 记录ID
        /// </summary>
        public Guid RedId { get; set; }

        /// <summary>
        /// 公司ID
        /// </summary>
        public Guid FilialeId { get; set; }

        /// <summary>
        /// 往来单位ID
        /// </summary>
        public Guid ThirdCompanyId { get; set; }

        /// <summary>
        /// 单据编号
        /// </summary>
        public string TradeCode { get; set; }

        /// <summary> 
        /// 创建时间
        /// </summary>
        public DateTime DateCreated { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        public string Transactor { get; set; }

        /// <summary>
        /// 单据描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 总金额
        /// </summary>
        public decimal AccountReceivable { get; set; }

        /// <summary>
        /// 总数量
        /// </summary>
        public decimal SubtotalQuantity { get; set; }

        /// <summary>
        /// 红冲类型，参见<see cref="ERP.Enum.RedType"/>
        /// </summary>
        public int RedType { get; set; }

        /// <summary>
        /// 单据类型，参见<see cref="ERP.Enum.DocumentType"/>
        /// </summary>
        public int DocumentType { get; set; }

        /// <summary>
        /// 状态，参见<see cref="ERP.Enum.DocumentRedState"/>
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// 仓库ID
        /// </summary>
        public Guid WarehouseId { get; set; }

        /// <summary>
        /// 审核时间
        /// </summary>
        public DateTime? AuditTime { get; set; }

        /// <summary>
        /// 入库储
        /// </summary>
        public int StorageType { get; set; }

        /// <summary>
        /// 原始单据ID
        /// </summary>
        public Guid LinkTradeId { get; set; }
        /// <summary>
        /// 原始单据描述
        /// </summary>
        public string LinkDescription { get; set; }
        /// <summary>
        /// 原始单据编号
        /// </summary>
        public string LinkTradeCode { get; set; }

        /// <summary>
        /// 原始单据出入库时间
        /// </summary>
        public DateTime LinkDateCreated { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Memo { get; set; }

        /// <summary>
        /// 是否报税
        /// </summary>
        /// <remarks>目前所有都要报税 By Jerry Bai 2017/6/2</remarks>
        public bool IsOut
        {
            get { return true; }
            set { }
        }
    }
}
