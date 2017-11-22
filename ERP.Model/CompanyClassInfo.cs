//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2006年4月27日
// 文件创建人:马力
// 最后修改时间:2006年4月27日
// 最后一次修改人:马力
//================================================
using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 往来单位分类
    /// </summary>
    [Serializable]
    [DataContract]
    public class CompanyClassInfo
    {
        //基本字段

        /// <summary>
        /// 默认构造函数
        /// </summary>
        public CompanyClassInfo() { }

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        /// <param name="companyClassId">往来单位分类Id</param>
        /// <param name="parentCompanyClassId">往来单位父分类编号</param>
        /// <param name="companyClassCode">往来单位编号</param>
        /// <param name="companyClassName">往来单位名称</param>
        public CompanyClassInfo(Guid companyClassId, Guid parentCompanyClassId, string companyClassCode, string companyClassName)
        {
            CompanyClassId = companyClassId;
            ParentCompanyClassId = parentCompanyClassId;
            CompanyClassCode = companyClassCode;
            CompanyClassName = companyClassName;
        }

        /// <summary>
        /// 往来单位分类编号
        /// </summary>
        [DataMember]
        public Guid CompanyClassId { get; private set; }

        /// <summary>
        /// 往来单位父分类编号
        /// </summary>
        [DataMember]
        public Guid ParentCompanyClassId { get; set; }

        /// <summary>
        /// 往来单位分类编码
        /// </summary>
        [DataMember]
        public string CompanyClassCode { get; set; }

        /// <summary>
        /// 往来单位分类名称
        /// </summary>
        [DataMember]
        public string CompanyClassName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public override bool Equals(object compareObj)
        {
            if (compareObj is CompanyClassInfo)
                return (compareObj as CompanyClassInfo).CompanyClassId == CompanyClassId;
            return base.Equals(compareObj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (CompanyClassId == Guid.Empty) return base.GetHashCode();
            string stringRepresentation = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "#" + CompanyClassId.ToString();
            return stringRepresentation.GetHashCode();
        }
    }
}
