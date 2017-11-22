using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using ERP.Model;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    ///门店管理模型
    /// </summary>
    [Serializable]
    public class ShopFilialeInfo
    {
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public ShopFilialeInfo()
        {
        }

        //公司基本属性
        /// <summary>
        /// 公司编号
        /// </summary>
        [DataMember]
        public Guid FilialeId
        {
            get;
            set;
        }
        /// <summary>
        /// 公司名称
        /// </summary>
        [DataMember]
        public string FilialeName
        {
            get;
            set;
        }
        /// <summary>
        ///
        /// </summary>
        [DataMember]
        public string FilialeCode
        {
            get;
            set;
        }

        /// <summary>
        /// 新建日期
        /// </summary>
        [DataMember]
        public DateTime DateCreated
        {
            get;
            set;
        }
        /// <summary>
        /// 状态 1 启用 0 搁置
        /// </summary>
        [DataMember]
        public int State
        {
            get;
            set;
        }
        /// <summary>
        /// 描述
        /// </summary>
        [DataMember]
        public string Description
        {
            get;
            set;
        }
        /// <summary>
        /// 公司类型
        /// </summary>
        [DataMember]
        public int FilialeType
        {
            get;
            set;
        }
        /// <summary>
        /// 父公司ID
        /// </summary>
        [DataMember]
        public Guid ParentFilialeId
        {
            get;
            set;
        }
        /// <summary>
        /// 主公司ID
        /// </summary>
        [DataMember]
        public Guid MainFilialeId
        {
            get;
            set;
        }
        /// <summary>
        /// 主公司名称
        /// </summary>
        [DataMember]
        public string MainFilialeName
        {
            get;
            set;
        }
        /// <summary>
        /// 主仓库ID
        /// </summary>
        [DataMember]
        public Guid MainWarehouseId
        {
            get;
            set;
        }
        /// <summary>
        /// 主仓库名称
        /// </summary>
        [DataMember]
        public string MainWarehouseName
        {
            get;
            set;
        }
        /// <summary>
        /// 城市ID
        /// </summary>
        [DataMember]
        public Guid CityId
        {
            get;
            set;
        }
        /// <summary>
        /// 省份ID
        /// </summary>
        [DataMember]
        public Guid ProvinceId
        {
            get;
            set;
        }
        /// <summary>
        /// 国家ID
        /// </summary>
        [DataMember]
        public Guid CountryId
        {
            get;
            set;
        }
        /// <summary>
        /// 加盟方式
        /// </summary>
        [DataMember]
        public int JoinType
        {
            get;
            set;
        }
        /// <summary>
        /// 刷卡账户ID
        /// </summary>
        [DataMember]
        public Guid SwingCardAccountId
        {
            get;
            set;
        }
        /// <summary>
        /// 现金账户ID
        /// </summary>
        [DataMember]
        public Guid CashAccountId
        {
            get;
            set;
        }
        /// <summary>
        /// 是否有加工功能
        /// </summary>
        [DataMember]
        public Boolean IsProcess
        {
            get;
            set;
        }
    }
}
