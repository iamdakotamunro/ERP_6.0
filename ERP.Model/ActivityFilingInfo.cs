using System;

namespace ERP.Model
{
    /// <summary>
    /// 活动报备类
    /// </summary>
    [Serializable]
    public class ActivityFilingInfo
    {
        public Guid ID { get; set; }

        /// <summary>
        /// 活动申报标题
        /// </summary>
        public string ActivityFilingTitle { get; set; }

        /// <summary>
        /// 申报公司ID
        /// </summary>
        public Guid FilingCompanyID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FilingCompanyName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ActivityFilingState { get; set; }

        public string ActivityFilingStateName {
            get
            {
                  return   Enum.Attribute.EnumAttribute.GetKeyName((Enum.ActivityFilingState) ActivityFilingState);
            }
            set { ActivityFilingStateName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public Guid GoodsID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string GoodsCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string GoodsName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime ActivityStateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime ActivityEndDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ProspectSaleNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ProspectReadyNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int ActualSaleNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int NormalSaleNumber { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid PurchasePersonnelID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string PurchasePersonnelName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Guid FilingTerraceID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string FilingTerraceName { get; set; }

        public Guid OperatePersonnelID { get; set; }

        public string OperatePersonnelName { get; set; }

        public decimal ErrorProbability { get; set; }

        public Guid WarehouseID { get; set; }
    }
}
