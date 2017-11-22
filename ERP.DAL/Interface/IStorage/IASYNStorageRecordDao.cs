using System;
using System.Collections.Generic;
using ERP.Model.ASYN;

namespace ERP.DAL.Interface.IStorage
{
    /// <summary>
    /// 
    /// </summary>
    public interface IASYNStorageRecordDao
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="asynStorageRecordInfo"></param>
        /// <returns></returns>
        bool Insert(ASYNStorageRecordInfo asynStorageRecordInfo);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        IList<ASYNStorageRecordInfo> GetList(int top);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        bool Delete(Guid id);
    }
}
