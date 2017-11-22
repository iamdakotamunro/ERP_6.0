//*****************************//
//** Func : 费用分类
//** Date : 2009-6-18
//** Code : dyy
//*****************************//

using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 费用类型
    /// </summary>
    [Serializable]
    public class CostCompanyClassInfo
    {

        /// <summary>
        /// 
        /// </summary>
        public CostCompanyClassInfo()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ccid">分类ID</param>
        /// <param name="pcid">上级分类ID</param>
        /// <param name="classcode">分类编码</param>
        /// <param name="classname">分类名称</param>
        public CostCompanyClassInfo(Guid ccid, Guid pcid, string classcode, string classname)
        {
            CompanyClassId = ccid;
            ParentCompanyClassId = pcid;
            CompanyClassCode = classcode;
            CompanyClassName = classname;
        }

        /// <summary>
        /// 分类ID
        /// </summary>
        public Guid CompanyClassId { get; set; }

        /// <summary>
        /// 上级分类ID
        /// </summary>
        public Guid ParentCompanyClassId { get; set; }

        /// <summary>
        /// 分类编码
        /// </summary>
        public string CompanyClassCode { get; set; }

        /// <summary>
        /// 分类名称
        /// </summary>
        public string CompanyClassName { get; set; }
    }
}
