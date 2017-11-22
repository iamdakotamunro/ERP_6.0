using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.Model;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 折扣/差额说明数据处理
    /// </summary>
    public class CompanySubjectDiscountDal
    {
        public CompanySubjectDiscountDal(Environment.GlobalConfig.DB.FromType fromType) { }

        public const string SQL_INSERT = @"INSERT INTO CompanySubjectDiscount(ID,CompanyId,FilialeId,DateCreated,PersonnelName,Income,Memo,MemoType) 
  VALUES(@ID,@CompanyId,@FilialeId,@DateCreated,@PersonnelName,@Income,@Memo,@MemoType)";
        public const string SQL_UPDATE = @"UPDATE CompanySubjectDiscount SET Memo=Memo+@Memo WHERE ID=@ID";
        public const string SQL_DELETE = @"DELETE CompanySubjectDiscount WHERE ID=@ID";
        public const string SQL_SELECT = @"SELECT CS.ID,CS.CompanyId,CC.CompanyName,CS.FilialeId,CS.DateCreated,CS.PersonnelName,CS.Income,CS.Memo,CS.MemoType FROM CompanySubjectDiscount CS
  INNER JOIN lmShop_CompanyCussent CC ON CS.CompanyId=CC.CompanyId
  WHERE CS.CompanyId=@CompanyId  ";

        /// <summary>
        /// 添加折扣/差额说明
        /// </summary>
        /// <param name="subjectDiscountInfo"></param>
        /// <returns></returns>
        public bool Insert(CompanySubjectDiscountInfo subjectDiscountInfo)
        {
            var parms = new[]
            {
                new Parameter("ID", subjectDiscountInfo.ID), 
                new Parameter("CompanyId", subjectDiscountInfo.CompanyId),
                new Parameter("FilialeId", subjectDiscountInfo.FilialeId),
                new Parameter("DateCreated", subjectDiscountInfo.Datecreated),
                new Parameter("PersonnelName", subjectDiscountInfo.PersonnelName),
                new Parameter("Income", subjectDiscountInfo.Income),
                new Parameter("Memo", subjectDiscountInfo.Memo),
                new Parameter("MemoType", subjectDiscountInfo.MemoType)
            };
            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false, SQL_INSERT, parms);
            }
        }

        /// <summary>
        /// 添加折扣/差额说明
        /// </summary>
        /// <param name="id"></param>
        /// <param name="memo"></param>
        /// <returns></returns>
        public bool Update(Guid id,string memo)
        {
            var parms = new[]
            {
                new Parameter("ID", id), 
                new Parameter("Memo", memo)
            };
            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false, SQL_UPDATE, parms);
            }
        }

        /// <summary>
        /// 删除折扣/差额说明
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(Guid id)
        {
            var parms = new[]
            {
                new Parameter("ID", id)
            };
            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false, SQL_DELETE, parms);
            }
        }

        /// <summary>
        /// 获取折扣/说明列表
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="filialeId"></param>
        /// <param name="memoType"></param>
        /// <returns></returns>
        public IList<CompanySubjectDiscountInfo> GetCompanySubjectDiscountInfos(Guid companyId,Guid filialeId,int memoType)
        {
            var builder=new StringBuilder(SQL_SELECT);
            if (filialeId!=Guid.Empty)
            {
                builder.AppendFormat(" AND CS.FilialeId='{0}'", filialeId);
            }
            if (memoType!=0)
            {
                builder.AppendFormat(" AND CS.MemoType={0}", memoType);
            }
            builder.Append(" ORDER BY CS.DateCreated DESC ");
            var parms = new[]
            {
                new Parameter("CompanyId", companyId)
            };
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<CompanySubjectDiscountInfo>(true, builder.ToString(), parms).ToList();
            }
        }
    }
}
