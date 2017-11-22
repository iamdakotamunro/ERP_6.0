using System;
using System.Collections.Generic;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.ICompany
{
    /// <summary>
    /// 模板管理接口
    /// code: lxm 20101221
    /// </summary>
    public interface ITemplateManage
    {
        /// <summary>
        /// 新建模板
        /// </summary>
        /// <param name="tempInfo"></param>
        int Insert(TemplateInfo tempInfo);

        /// <summary>
        /// 修改模板
        /// </summary>
        /// <param name="tempInfo"></param>
        int Update(TemplateInfo tempInfo);
        
        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="tempId"></param>
        /// <param name="state"></param>
        int UpdateState(Guid tempId, int state);

        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="tempId"></param>
        int Delete(Guid tempId);

        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <returns></returns>
        IList<TemplateInfo> GetTemplateList();
    }
}