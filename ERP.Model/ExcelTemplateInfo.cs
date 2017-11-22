using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// 功   能:导出Excel模板的实体类
    /// 时   间:2010-11-18
    /// 作   者:蒋赛标
    [Serializable]
    [DataContract]
    public class ExcelTemplateInfo
    {
        #region 模板id

        /// <summary>
        /// 模板id
        /// </summary>
        [DataMember]
        public Guid TempId { get; set; }

        #endregion

        #region     客户

        /// <summary>
        /// 客户
        /// </summary>
        [DataMember]
        public string Customer { get; set; }

        #endregion

        #region     模板名

        /// <summary>
        /// 模板名
        /// </summary>
        [DataMember]
        public string TemplateName { get; set; }

        #endregion

        #region     说货人信息

        /// <summary>
        /// 收货人信息
        /// </summary>
        [DataMember]
        public string Shipper { get; set; }

        #endregion

        #region     联系人信息

        /// <summary>
        /// 联系人信息
        /// </summary>
        [DataMember]
        public string ContactPerson { get; set; }

        #endregion

        #region     联系地址

        /// <summary>
        /// 联系地址
        /// </summary>
        [DataMember]
        public string ContactAddress { get; set; }

        #endregion

        #region     备注

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        public string Remarks { get; set; }

        #endregion

        #region 仓库id

        /// <summary>
        /// 仓库id
        /// </summary>
        [DataMember]
        public Guid WarehouseId { get; set; }

        #endregion

    }
}
