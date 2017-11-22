using System;

namespace ERP.Model
{
    /// <summary>
    /// 活动报备单操作日志
    /// </summary>
    public class ActivityOperateLogModel
    {
        public Guid ID { get; set; }
        public Guid ActivityFilingID { get; set; }
        public Guid OperatePersonnelID { get; set; }
        public string OperatePersonnelName { get; set; }
        public string Description { get; set; }
        public DateTime OperateDate { get; set; }
    }
}
