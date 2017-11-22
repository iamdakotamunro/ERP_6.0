using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;
using ERP.Model;

namespace ERP.BLL.Implement.Inventory
{
    /// <summary>
    /// 此类是对账服务在使用
    /// </summary>
    public class MediumReckoning : BllInstance<MediumReckoning>
    {
        readonly IMediumReckoning _dao;
        readonly ICheckDataRecord _checkDao;

        public MediumReckoning(Environment.GlobalConfig.DB.FromType fromType)
        {
            _dao = InventoryInstance.GetMediumReckoningDao(fromType);
            _checkDao = InventoryInstance.GetCheckDataRecordDao(fromType);
        }

        /// <summary>
        /// 插入往来账
        /// </summary>
        /// <param name="info"></param>
        public void Insert(MediumReckoningInfo info)
        {
            _dao.Insert(info);
        }

        /// <summary>
        /// 删除往来账,并将完成的数据转移
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="reckoningId"> </param>
        /// <param name="handleType"> </param>
        /// <param name="count"> </param>
        public void Delete(Guid checkId, Guid reckoningId, int handleType, int count)
        {
            _dao.Delete(checkId, reckoningId, handleType, count);
        }

        /// <summary>
        /// 将往来账更新到
        /// </summary>
        /// <param name="checkId"></param>
        public void UpdateData(Guid checkId)
        {
            _dao.UpdateData(checkId);
        }

        /// <summary>
        /// 将新增的往来账添加到往来账记录中
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="companyId">对账往来单位ID</param>
        public void TransferData(Guid checkId, Guid companyId)
        {
            _dao.TransferData(checkId, companyId);
        }

        ///// <summary>
        ///// 对账处理，将临时往来帐中修改和新增的往来帐更新到往来帐中  暂时注释
        ///// </summary>
        ///// <param name="checkId"></param>
        //public void Checking(Guid checkId)
        //{
        //    using (var scop = new System.Transactions.TransactionScope())
        //    {
        //        try
        //        {
        //            Dao.UpdateData(checkId);
        //            Dao.TransferData(checkId);
        //            scop.Complete();
        //        }
        //        catch (Exception ex)
        //        {
        //            throw new Exception(ex.Message);
        //        }
        //    }
        //}

        /// <summary>
        /// 获取应收(应付)款总调账金额
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="isReceivable">是否应收，(true：应收；false：应付) </param>
        /// <returns></returns>
        public IList<ExceptionReckoningInfo> GetReceivable(Guid checkId, Boolean isReceivable)
        {
            var list = _dao.GetAccountReceivableByFilialeId(checkId, isReceivable);
            var info = _dao.GetAccountReceivableByElseFilialeId(checkId, isReceivable);
            if (info != null)
            {
                info.FilialeId = new Guid("7AE62AF0-EB1F-49C6-8FD1-128D77C84698");
                list.Add(info);
            }
            return list.Where(ent => ent.DiffMoney != 0).ToList();
        }


        /// <summary>
        /// 获取对应对账记录的往来帐记录
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="handleType"></param>
        /// <param name="count"> </param>
        /// <returns></returns>
        public IList<MediumReckoningInfo> GetList(Guid checkId, int handleType, int count)
        {
            return _dao.GetList(checkId, handleType, count);
        }

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="addList"></param>
        /// <param name="tableName"></param>
        /// <param name="dics"></param>
        /// <returns></returns>
        public int BitchInsert(IList<MediumReckoningInfo> addList, string tableName, Dictionary<string, string> dics)
        {
            return _dao.BitchInsert(addList, tableName, dics);
        }

        /// <summary>对账完成删除临时往来帐数据（对账记录生成收付款后）  2015-04-16  陈重文
        /// </summary>
        /// <param name="checkId"></param>
        public void DeleteTempReckoning(Guid checkId)
        {
            _dao.DeleteTempReckoning(checkId);
        }
    }
}
