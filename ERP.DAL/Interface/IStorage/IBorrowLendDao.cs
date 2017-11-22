using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Model;

namespace ERP.DAL.Interface.IStorage
{
    public interface IBorrowLendDao
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info"></param>
        /// <param name="list"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        bool AddBorrowLendAndDetailList(BorrowLendInfo info, IList<BorrowLendDetailInfo> list, out string errorMessage);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        BorrowLendInfo GetBorrowLendInfo(Guid stockId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="borrowLendId"></param>
        /// <returns></returns>
        IList<BorrowLendDetailInfo> GetBorrowLendDetailList(Guid borrowLendId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="borrowLendId"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        int DeleteBorrowLendAndDetailList(Guid borrowLendId, out string errorMessage);
    }
}
