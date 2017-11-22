using ERP.Model.Report;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.DAL.Interface.IInventory
{
    public interface IWasteBookReport
    {
        /// <summary>
        /// 批量插入交易佣金表
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        bool AddWasteBook(IList<WasteBookInfo> list);

        /// <summary>
        /// 根据创建时间删除未处理的数据
        /// </summary>
        /// <param name="dateCreated">创建时间</param>
        /// <returns></returns>
        bool DelWasteBook(DateTime dateCreated);
    }
}
