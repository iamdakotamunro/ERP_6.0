using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.ICompany
{
    /// <summary>
    /// 功   能:Excel 模板接口
    /// 时   间:2010-11-18
    /// 作   者:蒋赛标
    /// </summary>
    public interface IExcelTemplate
    {
        /// <summary>
        /// 功   能:插入Excel模板
        /// 时   间:2010-11-18
        /// 作   者:蒋赛标
        /// </summary>
        /// <param name="tInfo">Excel模板实体</param>
        int Insert(ExcelTemplateInfo tInfo);

        /// <summary>
        /// 功   能:修改Excel模板信息
        /// 时   间:2010-11-18
        /// 作   者:蒋赛标
        /// </summary>
        /// <param name="tInfo">Excel模板实体</param>
        int Update(ExcelTemplateInfo tInfo);

        /// <summary>
        /// 功   能:删除Excel模板信息
        /// 时   间:2010-11-18
        /// 作   者:蒋赛标
        /// </summary>
        /// <param name="tempId">模板id</param>
        void Delete(Guid tempId);

        /// <summary>
        /// 获取Excel模板的集合
        /// </summary>
        /// <returns></returns>
        IList<ExcelTemplateInfo> GetExcelTemplateList();

        /// <summary>
        /// 根据仓库Id获取Excel模板的集合
        /// </summary>
        /// <returns></returns>
        IList<ExcelTemplateInfo> GetExcelTemplateListByWarehouseId(Guid warehouseId);

        /// <summary>
        /// 获取指定模版
        /// </summary>
        /// <param name="tempId"></param>
        /// <returns></returns>
        ExcelTemplateInfo GetExcelTemplateInfo(Guid tempId);

    }
}
