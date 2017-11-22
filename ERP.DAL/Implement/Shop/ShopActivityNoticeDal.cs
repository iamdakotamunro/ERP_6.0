using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using ERP.DAL.Interface.IShop;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model.ShopFront;
using Dapper;
using Keede.DAL.RWSplitting;
using ERP.Environment;
using Dapper.Extension;
using System.Transactions;

namespace ERP.DAL.Implement.Shop
{
    public class ShopActivityNoticeDal : IShopActivityNoticeDal
    {
        public ShopActivityNoticeDal(Environment.GlobalConfig.DB.FromType fromType) { }

        public bool InsertShopActivityNoticeInfo(ShopActivityNoticeInfo info)
        {
            const string SQL = @"INSERT INTO [ShopActivityNotice]
           ([NoticeID] ,[NoticeTitle] ,[NoticeContent]  ,[CreateTime] ,[IsNotice] ,[IsShow] ,[OrderIndex])
                VALUES
           (@NoticeID ,@NoticeTitle ,@NoticeContent  ,@CreateTime,@IsNotice ,@IsShow  ,(select ISNULL(MAX(OrderIndex)+1,1) from ShopActivityNotice))";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    NoticeID = info.NoticeID,
                    NoticeTitle = info.NoticeTitle,
                    NoticeContent = info.NoticeContent,
                    CreateTime = info.CreateTime,
                    IsNotice = info.IsNotice,
                    IsShow = info.IsShow,
                }) > 0;
            }
        }

        public bool UpdateShopActivityNoticeInfo(ShopActivityNoticeInfo info)
        {
            const string SQL = @"UPDATE [ShopActivityNotice] SET [NoticeTitle] = @NoticeTitle ,[NoticeContent] = @NoticeContent ,[IsNotice] = @IsNotice   ,[IsShow] = @IsShow  WHERE [NoticeID] = @NoticeID";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    NoticeID = info.NoticeID,
                    NoticeTitle = info.NoticeTitle,
                    NoticeContent = info.NoticeContent,
                    IsNotice = info.IsNotice,
                    IsShow = info.IsShow,
                }) > 0;
            }
        }

        public bool DeleteShopActivityNoticeInfo(Guid noteId)
        {
            const string SQL = @"
            begin tran

            update ShopActivityNotice
            set OrderIndex=OrderIndex-1
            where OrderIndex >(select OrderIndex from ShopActivityNotice WHERE NoticeID=@NoticeID)
            delete ShopActivityNotice where NoticeID=@NoticeID

            if(@@ERROR!=0) 
            rollback 
            else 
            commit";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    NoticeID = noteId,
                }) > 0;
            }
        }

        public ShopActivityNoticeInfo SelectActivityNoticeInfo(Guid noteId)
        {
            const string SQL = @"SELECT [NoticeID]
                                                      ,[NoticeTitle]
                                                      ,[NoticeContent]
                                                      ,[CreateTime]
                                                      ,[IsNotice]
                                                      ,[IsShow]
                                                      ,[OrderIndex]
                                                  FROM [ShopActivityNotice] 
                                                WHERE [NoticeID] = @NoticeID ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.QueryFirstOrDefault<ShopActivityNoticeInfo>(SQL, new
                {
                    NoticeID = noteId,
                });
            }
        }

        public IEnumerable<ShopActivityNoticeInfo> SelectNoticeList()
        {
            const string SQL = @"SELECT [NoticeID]
                                                      ,[NoticeTitle]
                                                      ,[NoticeContent]
                                                      ,[CreateTime]
                                                      ,[IsNotice]
                                                      ,[IsShow]
                                                      ,[OrderIndex]
                                                  FROM [ShopActivityNotice]  ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ShopActivityNoticeInfo>(SQL);
            }
        }

        /// <summary>
        /// 根据条件查询广告---分页
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="isShow">是否显示</param>
        /// <param name="isNotice">是否是广告-显示在首页</param>
        /// <returns></returns>
        public PageItems<ShopActivityNoticeInfo> SelectNoticeListByPage(int pageIndex, int pageSize, bool isShow, bool? isNotice)
        {
             string sql = @"SELECT [NoticeID]
                                                      ,[NoticeTitle]
                                                      ,[NoticeContent]
                                                      ,[CreateTime]
                                                      ,[IsNotice]
                                                      ,[IsShow]
                                                      ,[OrderIndex]
                                                  FROM [ShopActivityNotice]  where  IsShow=@IsShow";
            if (isNotice != null)
            {
                sql += " and IsNotice=@IsNotice  ";
            }

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                var recordCount = conn.ExecuteScalar<int>(string.Format("select count(*) from ({0}) T", sql));
                var data = conn.QueryPaged<ShopActivityNoticeInfo>(sql, pageIndex, pageSize, "CreateTime desc", new
                {
                    IsNotice = isNotice,
                    IsShow = isShow,
                }).AsList();
                return new PageItems<ShopActivityNoticeInfo>(pageIndex, pageSize, recordCount, data);
            }
        }
        /// <summary>
        /// 更新公告管理序号
        /// </summary>
        /// <param name="noticeId">公告管理主键ID</param>
        /// <param name="orderIndex">序号</param>
        /// <returns></returns>
        /// zal 2015-09-22
        public bool UpdateOrderIndex(Guid noticeId, int orderIndex)
        {
            const string SQL = @"
            declare @index int,@Total int
	        select @index=OrderIndex from ShopActivityNotice 
	        where NoticeID=@NoticeID
            select @Total=COUNT(*) from ShopActivityNotice
            
            if(@OrderIndex>@Total)
		        begin
		            set @OrderIndex=@Total
		        end
	
	        begin tran
	
	        if(@OrderIndex<@index)
		        begin
			        --从大序号变到小序号(例如从3到1)
			        update ShopActivityNotice
			        set OrderIndex=OrderIndex+1
			        where OrderIndex between @OrderIndex and @index
		        end
	        else
		        begin
			        --从小序号变到大序号(例如从1到3)
			        update ShopActivityNotice
			        set OrderIndex=OrderIndex-1
			        where OrderIndex between @index and @OrderIndex
		        end
		
	        update ShopActivityNotice
	        set OrderIndex=@OrderIndex
	        where NoticeID=@NoticeID
	
	        if(@@ERROR!=0)
	        rollback
	        else
	        commit";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new {
                    NoticeID = noticeId,
                    OrderIndex = orderIndex
                }) > 0;
            }
        }

        public bool UpdateIsNotice(bool isNote, Guid noteId)
        {
            const string SQL = @"UPDATE [ShopActivityNotice]
                                                   SET                                                      
                                                      [IsNotice] = @IsNotice
                                                 WHERE [NoticeID] = @NoticeID";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    NoticeID = noteId,
                    IsNotice = isNote
                }) > 0;
            }
        }

        public bool UpdateIsShow(bool isShow, Guid noteId)
        {
            const string SQL = @"UPDATE [ShopActivityNotice]
                                                   SET                                                      
                                                      [IsShow] = @IsShow
                                                 WHERE [NoticeID] = @NoticeID";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL, new
                {
                    NoticeID = noteId,
                    IsShow = isShow
                }) > 0;
            }
        }
    }
}
