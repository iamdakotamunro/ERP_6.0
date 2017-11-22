using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ERP.DAL.Interface.ICompany;
using Keede.Ecsoft.Model;
using Keede.DAL.RWSplitting;
using Dapper;
using ERP.Environment;
using System.Transactions;

namespace ERP.DAL.Implement.Company
{
    /// <summary>
    /// 功   能:Excel模板数据库操作类
    /// 时   间:2010-11-18
    /// 作   者:蒋赛标
    /// </summary>
    public class ExcelTemplate : IExcelTemplate
    {
        public ExcelTemplate(Environment.GlobalConfig.DB.FromType fromType) { }

        #region   变量
        private const string PARM_TEMP_ID = "@TempId";
        private const string PARM_CUSTOMER = "@Customer";
        private const string PARM_TEMPLATE_NAME = "@TemplateName";
        private const string PARM_SHIPPER = "@Shipper";
        private const string PARM_CONTACT_PERSON = "@ContactPerson";
        private const string PARM_CONTACT_ADDRESS = "@ContactAddress";
        private const string PARM_REMARKS = "@Remarks";
        private const string PARM_WAREHOUSEID = "@WarehouseId";
        #endregion
        
        #region IExcelTemplate接口内容
        /// <summary>
        /// 功   能:插入Excel模板
        /// 时   间:2010-11-18
        /// 作   者:蒋赛标
        /// </summary>
        /// <param name="tInfo">Excel模板实体</param>
        public int Insert(ExcelTemplateInfo tInfo)
        {
            const string SQL_EXCEL_TEMPATE_INSERT = @"Insert into lmshop_ExcelTemplate (TempId,Customer,TemplateName,Shipper,ContactPerson,ContactAddress,Remarks,WarehouseId)
                                             Values(@TempId,@Customer,@TemplateName,@Shipper,@ContactPerson,@ContactAddress,@Remarks,@WarehouseId)";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_EXCEL_TEMPATE_INSERT, new
                {
                    TempId = Guid.NewGuid(),
                    Customer = tInfo.Customer,
                    TemplateName = tInfo.TemplateName,
                    Shipper = tInfo.Shipper,
                    ContactPerson = tInfo.ContactPerson,
                    ContactAddress = tInfo.ContactAddress,
                    Remarks = tInfo.Remarks,
                    WarehouseId = tInfo.WarehouseId,
                });
            }
        }

        /// <summary>
        /// 功   能:修改Excel模板信息
        /// 时   间:2010-11-18
        /// 作   者:蒋赛标
        /// </summary>
        /// <param name="tInfo">Excel模板实体</param>
        public int Update(ExcelTemplateInfo tInfo)
        {
            const string SQL_EXCEL_TEMPLATE_UPDATE = @"Update lmshop_ExcelTemplate Set Customer=@Customer, TemplateName=@TemplateName,Shipper=@Shipper,
                                              ContactPerson=@ContactPerson,ContactAddress=@ContactAddress,Remarks=@Remarks ,WarehouseId=@WarehouseId
                                              Where TempId=@TempId ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_EXCEL_TEMPLATE_UPDATE, new
                {
                    TempId = Guid.NewGuid(),
                    Customer = tInfo.Customer,
                    TemplateName = tInfo.TemplateName,
                    Shipper = tInfo.Shipper,
                    ContactPerson = tInfo.ContactPerson,
                    ContactAddress = tInfo.ContactAddress,
                    Remarks = tInfo.Remarks,
                    WarehouseId = tInfo.WarehouseId,
                });
            }
        }

        /// <summary>
        /// 功   能:删除Excel模板信息
        /// 时   间:2010-11-18
        /// 作   者:蒋赛标
        /// </summary>
        /// <param name="tempId">模板id</param>
        public void Delete(Guid tempId)
        {
            const string SQL_EXCEL_TEMPLATE_DELETE = @"Delete From lmshop_ExcelTemplate Where TempId=@TempId";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Execute(SQL_EXCEL_TEMPLATE_DELETE, new
                {
                    TempId = tempId,
                });
            }
        }

        /// <summary>
        /// 获取Excel模板的集合
        /// </summary>
        /// <returns></returns>
        public IList<ExcelTemplateInfo> GetExcelTemplateList()
        {
            const string SQL_TEMPLATE_SELECT = @"SELECT  [TempId]
                                          ,[Customer]
                                          ,[TemplateName]
                                          ,[Shipper]
                                          ,[ContactPerson]
                                          ,[ContactAddress]
                                          ,[Remarks]
                                          ,[WarehouseId]
                                      FROM [lmshop_ExcelTemplate]";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ExcelTemplateInfo>(SQL_TEMPLATE_SELECT).AsList();
            }
        }

        /// <summary>
        /// 根据仓库Id获取Excel模板的集合
        /// </summary>
        /// <returns></returns>
        public IList<ExcelTemplateInfo> GetExcelTemplateListByWarehouseId(Guid warehouseId)
        {
            IList<ExcelTemplateInfo> tempList = new List<ExcelTemplateInfo>();
            const string SQL_TEMPLATE_SELECT = @"SELECT  [TempId]
                                          ,[Customer]
                                          ,[TemplateName]
                                          ,[Shipper]
                                          ,[ContactPerson]
                                          ,[ContactAddress]
                                          ,[Remarks]
                                          ,[WarehouseId]
                                      FROM [lmshop_ExcelTemplate] WHERE WarehouseId=@WarehouseId";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<ExcelTemplateInfo>(SQL_TEMPLATE_SELECT, new { WarehouseId = warehouseId }).AsList();
            }
        }


        /// <summary>
        /// 根据仓库Id集合获取Excel模板的集合
        /// </summary>
        /// <returns></returns>
        public IList<ExcelTemplateInfo> GetExcelTemplateListByWarehouseIdList(List<Guid> warehouseIdList)
        {
            if (warehouseIdList != null && warehouseIdList.Count != 0)
            {
                var strWarehouseId = "'" + string.Join("','", warehouseIdList) + "'";
                string sqlTemplateSelect = @"SELECT  [TempId]
                                          ,[Customer]
                                          ,[TemplateName]
                                          ,[Shipper]
                                          ,[ContactPerson]
                                          ,[ContactAddress]
                                          ,[Remarks]
                                          ,[WarehouseId]
                                      FROM [lmshop_ExcelTemplate] WHERE WarehouseId in(" +
                                           strWarehouseId + ")";


                using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
                {
                    return conn.Query<ExcelTemplateInfo>(sqlTemplateSelect).AsList();
                }
            }
            return null;
        }

        /// <summary>
        /// 获取指定模版
        /// </summary>
        /// <param name="tempId"></param>
        /// <returns></returns>
        public ExcelTemplateInfo GetExcelTemplateInfo(Guid tempId)
        {
            const string SQL_TEMPLATE_SELECT = @"SELECT TOP 1 [TempId]
                                          ,[Customer]
                                          ,[TemplateName]
                                          ,[Shipper]
                                          ,[ContactPerson]
                                          ,[ContactAddress]
                                          ,[Remarks],[WarehouseId]
                                      FROM [lmshop_ExcelTemplate] WHERE TempId=@TempId ";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.QueryFirstOrDefault<ExcelTemplateInfo>(SQL_TEMPLATE_SELECT, new { TempId = tempId });
            }
        }
        #endregion


        /// <summary>
        /// 功   能:插入Excel模板
        /// 时   间:2016-7-20
        /// 作   者:文雯
        /// </summary>
        /// <param name="tInfo">Excel模板实体</param>
        public bool Add(ExcelTemplateInfo tInfo)
        {
            const string SQL_EXCEL_TEMPATE_INSERT = @"Insert into lmshop_ExcelTemplate (TempId,Customer,TemplateName,Shipper,ContactPerson,ContactAddress,Remarks,WarehouseId)
                                             Values(@TempId,@Customer,@TemplateName,@Shipper,@ContactPerson,@ContactAddress,@Remarks,@WarehouseId)";
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_EXCEL_TEMPATE_INSERT, new
                {
                    TempId = Guid.NewGuid(),
                    Customer = tInfo.Customer,
                    TemplateName = tInfo.TemplateName,
                    Shipper = tInfo.Shipper,
                    ContactPerson = tInfo.ContactPerson,
                    ContactAddress = tInfo.ContactAddress,
                    Remarks = tInfo.Remarks,
                    WarehouseId = tInfo.WarehouseId,
                }) > 0;
            }
        }

        /// <summary>
        /// 根据模板名称和仓库id判断是否存在
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public bool ExcelTemplateIsExists(string templateName, Guid warehouseId)
        {
            const string SQL = @"SELECT COUNT(*) FROM lmshop_ExcelTemplate WHERE TemplateName=@TemplateName AND WarehouseId=@WarehouseId";

            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.ExecuteScalar<int>(SQL, new
                {
                    TemplateName = templateName,
                    WarehouseId = warehouseId,
                }) > 0;
            }
        }
    }
}
