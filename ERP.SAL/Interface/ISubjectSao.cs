using System;
using System.Collections.Generic;
using ERP.Model;
using ERP.Model.Goods;

namespace ERP.SAL.Interface
{
    /// <summary>
    /// 票据处理——会计科目
    /// </summary>
    public interface ISubjectSao
    {
        /// <summary>
        /// 获取所有会计科目分类
        /// </summary>
        /// <returns></returns>
        List<SubjectClassInfo> GetAllSubjectClassList();

        /// <summary>
        /// 添加会计科目
        /// </summary>
        /// <param name="subjectClassInfo"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool Add(SubjectInfo subjectClassInfo, out string errorMessage);

        /// <summary>
        /// 删除会计科目
        /// </summary>
        /// <param name="subjectID"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool Delete(int subjectID, out string errorMessage);

        /// <summary>
        /// 更新会计科目
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool Update(SubjectInfo model, out string errorMessage);
    }
}