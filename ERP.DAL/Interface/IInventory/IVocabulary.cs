using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Model;

namespace ERP.DAL.Interface.IInventory
{
    public interface IVocabulary
    {
        /// <summary>
        /// 批量插入词汇
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool AddBatchVocabulary(IList<VocabularyInfo> list);

        /// <summary>
        /// 根据id更新状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="state">状态(0：禁用,1：启用)</param>
        /// <returns></returns>
        bool UpdateStateById(Guid id, int state);

        /// <summary>
        /// 删除词汇
        /// </summary>
        /// <param name="arryId">主键Id</param>
        /// <returns></returns>
        bool DelById(string[] arryId);

        /// <summary>
        /// 根据词汇名称查询
        /// </summary>
        /// <param name="vocabularyName">词汇名称</param>
        /// <returns></returns>
        List<VocabularyInfo> GetVocabularyListByVocabularyName(string vocabularyName);

        /// <summary>
        /// 根据词汇状态查询
        /// </summary>
        /// <param name="state">状态(0：禁用,1：启用)</param>
        /// <returns></returns>
        List<VocabularyInfo> GetVocabularyListByState(int state);

        /// <summary>
        /// 根据词汇状态查询
        /// </summary>
        /// <param name="state">状态(0：禁用,1：启用)</param>
        /// <returns></returns>
        List<string> GetVocabularyNameListByState(int state);
    }
}
