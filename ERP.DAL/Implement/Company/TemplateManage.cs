using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ERP.DAL.Interface.ICompany;
using Keede.Ecsoft.Model;
using Keede.DAL.RWSplitting;
using ERP.Environment;
using Dapper;
using System.Transactions;

namespace ERP.DAL.Implement.Company
{
    public class TemplateManage : ITemplateManage
    {
        public TemplateManage(Environment.GlobalConfig.DB.FromType fromType) { }

        private const string SQL_INSERT = "insert into lmShop_Template (TemplateID,TemplateCaption,TemplateContent,TemplateType,TemplateState) values (@TemplateID,@TemplateCaption,@TemplateContent,@TemplateType,@TemplateState);";
        private const string SQL_UPDATE = "update lmShop_Template set TemplateCaption=@TemplateCaption,TemplateContent=@TemplateContent,TemplateType=@TemplateType where TemplateID=@TemplateID;";
        private const string SQL_UPDATESTATE = "update lmShop_Template set TemplateState=@TemplateState where TemplateID=@TemplateID;";
        private const string SQL_DELETE = "delete lmShop_Template where TemplateID=@TemplateID;";
        private const string SQL_SELECT_LIST = "select TemplateID,TemplateCaption,TemplateContent,TemplateType,TemplateState from lmShop_Template order by TemplateType;";
        private const string PARM_TEMPLATEID = "@TemplateID";
        private const string PARM_TEMPLATECAPTION = "@TemplateCaption";
        private const string PARM_TEMPLATECONTENT = "@TemplateContent";
        private const string PARM_TEMPLATETYPE = "@TemplateType";
        private const string PARM_TEMPLATESTATE = "@TemplateState";

        private static SqlParameter[] GetTemplateParameters()
        {
            var parms = new[] {
            new SqlParameter(PARM_TEMPLATEID,SqlDbType.UniqueIdentifier),
            new SqlParameter(PARM_TEMPLATECAPTION,SqlDbType.VarChar,32),
            new SqlParameter(PARM_TEMPLATECONTENT,SqlDbType.VarChar,256),
            new SqlParameter(PARM_TEMPLATETYPE,SqlDbType.Int),
            new SqlParameter(PARM_TEMPLATESTATE,SqlDbType.Int)
            };
            return parms;
        }

        /// <summary>
        /// 插入模版
        /// </summary>
        /// <param name="tempInfo"></param>
        public int Insert(TemplateInfo tempInfo)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_INSERT, new
                {
                    TemplateID = tempInfo.TemplateID,
                    TemplateCaption = tempInfo.TemplateCaption,
                    TemplateContent = tempInfo.TemplateContent,
                    TemplateType = tempInfo.TemplateType,
                    TemplateState = tempInfo.TemplateState,
                });
            }
        }

        /// <summary>
        /// 修改模版
        /// </summary>
        /// <param name="tempInfo"></param>
        public int Update(TemplateInfo tempInfo)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_UPDATE, new
                {
                    TemplateID = tempInfo.TemplateID,
                    TemplateCaption = tempInfo.TemplateCaption,
                    TemplateContent = tempInfo.TemplateContent,
                    TemplateType = tempInfo.TemplateType,
                    TemplateState = tempInfo.TemplateState,
                });
            }
        }

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="tempId"></param>
        /// <param name="state"></param>
        public int UpdateState(Guid tempId, int state)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_UPDATESTATE, new
                {
                    TemplateID = tempId,
                    TemplateState = state,
                });
            }
        }

        /// <summary>
        /// 删除模版
        /// </summary>
        /// <param name="tempId"></param>
        public int Delete(Guid tempId)
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                return conn.Execute(SQL_DELETE, new
                {
                    TemplateID = tempId,
                });
            }
        }

        /// <summary>
        /// 获取模版列表
        /// </summary>
        /// <returns></returns>
        public IList<TemplateInfo> GetTemplateList()
        {
            using (SqlConnection conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, Transaction.Current == null))
            {
                return conn.Query<TemplateInfo>(SQL_SELECT_LIST).AsList();
            }
        }
    }
}