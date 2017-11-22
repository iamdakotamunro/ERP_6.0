using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Model;
using Keede.DAL.Helper;
using ERP.Environment;

namespace ERP.DAL.Implement.Inventory
{
    public class CheckDataRecord : ICheckDataRecord
    {
        public CheckDataRecord(Environment.GlobalConfig.DB.FromType fromType) { }

        #region  SQL
        /// <summary>
        /// 添加对账记录
        /// </summary>
        protected const string SQL_INSERT = "INSERT INTO ExpressCheckData(CheckId,OriginalFilePath,CheckType,CheckDataState,CheckCompanyId,CheckCreateDate,CheckFilialeId,CheckPersonnelId,CheckUser,ContrastFilePath,ConfirmFilePath,FinishFilePath,CheckDescription) " +
            "VALUES(@CheckId,@OriginalFilePath,@CheckType,@CheckDataState,@CheckCompanyId,@CheckCreateDate,@CheckFilialeId,@CheckPersonnelId,@CheckUser,@ContrastFilePath,@ConfirmFilePath,@FinishFilePath,@CheckDescription)";

        /// <summary>
        /// 对账更新
        /// </summary>
        protected const string SQL_UPDATE_RESULT = @"UPDATE ExpressCheckData SET CheckDataState=@CheckDataState,ContrastFilePath=@ContrastFilePath,ConfirmFilePath=@ConfirmFilePath,FinishFilePath=@FinishFilePath,CheckDescription=@CheckDescription WHERE CheckId=@CheckId ";

        /// <summary>
        /// 
        /// </summary>
        protected const string SQL_UPDATE_STATE = @"UPDATE ExpressCheckData SET CheckDataState=@CheckDataState WHERE CheckId=@CheckId ";

        /// <summary>
        /// 删除对账信息
        /// </summary>
        protected const string SQL_DELETE = "DELETE ExpressCheckData WHERE CheckId=@CheckId ";

        /// <summary>
        /// 判断是否存在同文件名，同对账类型，同快递公司的对账信息
        /// </summary>
        protected const string SQL_ISEXIST = "SELECT 1 FROM ExpressCheckData WHERE OriginalFilePath LIKE '%'+@OriginalFilePath+'' AND CheckType=@CheckType AND CheckCompanyId=@CheckCompanyId AND CheckDataState<>99 ";

        /// <summary>
        /// 获取单条对账信息
        /// </summary>
        protected const string SQL_SELECT_CHECKDATAINFO = "SELECT TOP 1 CheckId,OriginalFilePath,CheckType,CheckDataState,CheckCompanyId,CC.CompanyName,CheckCreateDate,CheckFilialeId,CheckPersonnelId,CheckUser,ContrastFilePath,ConfirmFilePath,FinishFilePath,CheckDescription FROM ExpressCheckData ECD INNER JOIN lmShop_CompanyCussent CC ON ECD.CheckCompanyId=CC.CompanyId AND CC.CompanyType=3 WHERE CheckId=@CheckId ";

        /// <summary>
        /// 获取对账信息列表
        /// </summary>
        protected const string SQL_SELECT_CHECKDATALIST = "SELECT CheckId,OriginalFilePath,CheckType,CheckDataState,CheckCompanyId,CC.CompanyName,CheckCreateDate,CheckFilialeId,CheckPersonnelId,CheckUser,ContrastFilePath,ConfirmFilePath,FinishFilePath,CheckDescription FROM ExpressCheckData ECD INNER JOIN lmShop_CompanyCussent CC ON ECD.CheckCompanyId=CC.CompanyId AND CC.CompanyType=3 ";

        /// <summary>
        /// 插入对账明细
        /// </summary>
        protected const string SQL_INERT_DETAIL = "INSERT INTO ExpressCheckDataDetail(CheckId,ExpressNo,PostMoney,PostWeight,DataState,ConfirmMoney,FinanceConfirmMoney) VALUES(@CheckId,@ExpressNo,@PostMoney,@PostWeight,@DataState,@ConfirmMoney,@FinanceConfirmMoney)";

        /// <summary>
        /// 删除对账详细
        /// </summary>
        protected const string SQL_DELETE_DETAIL = "DELETE ExpressCheckDataDetail WHERE CheckId=@CheckId";

        /// <summary>
        /// 根据对账记录查对账明细
        /// </summary>
        protected const string SQL_SELECT_DETAILIST = "SELECT TOP {0} CheckId,ExpressNo,PostMoney,PostWeight,DataState,ConfirmMoney,FinanceConfirmMoney FROM ExpressCheckDataDetail ";

        /// <summary>
        /// 更新插入明细(用于excel重复数据)
        /// </summary>
        protected const string SQL_UPDATE_DETAILINFO = "UPDATE ExpressCheckDataDetail SET PostMoney=@PostMoney,PostWeight=@PostWeight WHERE CheckId=@CheckId AND ExpressNo=@ExpressNo ";

        /// <summary>
        /// 提交确认文档时
        /// </summary>
        protected const string SQL_UPDATE_CONFIRM = "UPDATE ExpressCheckDataDetail SET ConfirmMoney=@ConfirmMoney,FinanceConfirmMoney=@FinanceConfirmMoney,DataState=@DataState WHERE CheckId=@CheckId ";

        protected const string SQL_UPDATE_DATASTATE = "UPDATE ExpressCheckDataDetail SET DataState=@DataState WHERE CheckId=@CheckId ";

        /// <summary>
        /// 获取对账实际总额
        /// </summary>
        protected const string SQL_SELECT_TOTAL = @"
WITH #Temp AS
(
	SELECT SUM(FinanceConfirmMoney) SumFinanceConfirmMoney,SUM(ConfirmMoney)SumConfirmMoney,FilialeId,Isout FROM BakExpressCheckDataDetail BE WHERE CheckId=@CheckId
	GROUP BY FilialeId,Isout
)
SELECT T.SumFinanceConfirmMoney,T.SumConfirmMoney,T.FilialeId,F.Name AS FilialeName,T.IsOut FROM #Temp T INNER JOIN Filiale F ON T.FilialeId=F.ID";

        /// <summary>
        /// 批量插入备份表中
        /// </summary>
        protected const string SQL_INSERT_TO_TEMP = @"
DELETE BakExpressCheckDataDetail WHERE CheckId=@CheckId;
INSERT INTO BakExpressCheckDataDetail (CheckId,ExpressNo,PostMoney,PostWeight, ConfirmMoney,FinanceConfirmMoney,DataState,FilialeId,FilialeName,IsOut)
 SELECT CheckId,ExpressNo,PostMoney,PostWeight,ConfirmMoney,FinanceConfirmMoney,DataState,FilialeId,FilialeName,IsOut 
 FROM ExpressCheckDataDetail 
     WHERE CheckId=@CheckId";

        /// <summary>获取对比数据
        /// </summary>
        protected const string SQL_GETCONTRASTDATALIST = @"
--获取往来账至临时表
 SELECT ABS(SUM(T.AccountReceivable)) AS ServerMoney,T.LinkTradeCode as ExpressNo 
	into #Reckoning FROM lmShop_Reckoning  T
	WHERE ThirdCompanyID=@CompanyId AND DateCreated BETWEEN @StartTime AND @EndTime 
	AND IsChecked=@IsChecked AND ReckoningCheckType=@ReckoningCheckType
	AND AuditingState=1 GROUP BY LinkTradeCode;
 --异常对账
if(@IsChecked=2)
begin 
 select E.ExpressNo,E.PostMoney,E.PostWeight,G.ServerMoney,G.ServerWeight,G.Province,G.City,
	 CASE  
	  WHEN G.ServerMoney IS NULL THEN NULL   
	  WHEN G.ServerMoney IS NOT NULL THEN ABS(G.ServerMoney-E.PostMoney) 
  END AS DiffMoney  from ExpressCheckDataDetail as  E	    
	inner join 
	(	 
		--获取快递运费信息   	    
		select I1.ExpressNo,I1.ServerMoney,ISNULL(I2.TotalWeight,0) AS ServerWeight,Province,City   from #Reckoning I1
		LEFT JOIN 
		(
			SELECT TotalWeight, ExpressNo, Province, City FROM GoodsOrderDeliver WHERE ExpressId  in
				(
					select ExpressId from lmShop_Express where CompanyId=@CompanyId
				)
		) I2 ON I1.ExpressNo=I2.ExpressNo
    
	) G on E.ExpressNo = G.ExpressNo WHERE E.CheckId=@CheckId
end else begin
--没有对账
 select E.ExpressNo,E.PostMoney,E.PostWeight,G.ServerMoney,G.ServerWeight,G.Province,G.City,
	 CASE  
	  WHEN G.ServerMoney IS NULL THEN NULL   
	  WHEN G.ServerMoney IS NOT NULL THEN ABS(G.ServerMoney-E.PostMoney) 
  END AS DiffMoney  from ExpressCheckDataDetail as  E	    
	left join 
	(	 
		--获取快递运费信息   	    
		select I1.ExpressNo,I1.ServerMoney,ISNULL(I2.TotalWeight,0) AS ServerWeight,Province,City   from #Reckoning I1
		LEFT JOIN 
		(
			SELECT TotalWeight, ExpressNo, Province, City FROM GoodsOrderDeliver WHERE ExpressId in
				(
					select ExpressId from lmShop_Express where CompanyId=@CompanyId
				)
		) I2 ON I1.ExpressNo=I2.ExpressNo
    
	) G on E.ExpressNo = G.ExpressNo WHERE E.CheckId=@CheckId
end
DROP TABLE #Reckoning";

        #endregion

        #region  SQL参数
        protected const string PARM_CHECKID = "@CheckId";
        protected const string PARM_ORIGINALFILEPATH = "@OriginalFilePath";
        protected const string PARM_CHECKTYPE = "@CheckType";
        protected const string PARM_CHECKDATASTATE = "@CheckDataState";
        protected const string PARM_CHECKCOMPANYID = "@CheckCompanyId";
        protected const string PARM_CHECKCREATEDATE = "@CheckCreateDate";
        protected const string PARM_CHECKFILIALEID = "@CheckFilialeId";
        protected const string PARM_CHECKPERSONNELID = "@CheckPersonnelId";
        protected const string PARM_CHECKUSER = "@CheckUser";
        protected const string PARM_CONTRASTFILEPATH = "@ContrastFilePath";
        protected const string PARM_CONFIRMFILEPATH = "@ConfirmFilePath";
        protected const string PARM_FINISHFILEPATH = "@FinishFilePath";
        protected const string PARM_CHECKDESCRIPTION = "@CheckDescription";
        protected const string PARM_EXPRESSNO = "@ExpressNo";
        protected const string PARM_POSTMONEY = "@PostMoney";
        protected const string PARM_POSTWEIGHT = "@PostWeight";
        protected const string PARM_CONFIRMMONEY = "@ConfirmMoney";
        protected const string PARM_FINANCECONFIRM_MONEY = "@FinanceConfirmMoney";
        protected const string PARM_DIFFMONEY = "@DiffMoney";
        protected const string PARM_DATASTATE = "@DataState";
        #endregion

        #region[声明参数]
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static SqlParameter[] GetCheckDataRecordParameters()
        {
            var parms = new[] {
                new SqlParameter(PARM_CHECKID, SqlDbType.UniqueIdentifier),
				new SqlParameter(PARM_ORIGINALFILEPATH, SqlDbType.VarChar ,256),
                new SqlParameter(PARM_CHECKTYPE, SqlDbType.Int),
                new SqlParameter(PARM_CHECKDATASTATE,SqlDbType.Int), 
                new SqlParameter(PARM_CHECKCOMPANYID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_CHECKCREATEDATE, SqlDbType.DateTime),
                new SqlParameter(PARM_CHECKFILIALEID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_CHECKPERSONNELID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_CHECKUSER, SqlDbType.VarChar, 6),
                new SqlParameter(PARM_CONTRASTFILEPATH, SqlDbType.VarChar, 256),
                new SqlParameter(PARM_CONFIRMFILEPATH,SqlDbType.VarChar, 256),
                new SqlParameter(PARM_FINISHFILEPATH, SqlDbType.VarChar, 256),
                new SqlParameter(PARM_CHECKDESCRIPTION,SqlDbType.VarChar, 512)
            };
            return parms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private static SqlParameter[] GetCheckDataDetailParameters()
        {
            var parms = new[]
                            {
                                new SqlParameter(PARM_CHECKID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_EXPRESSNO, SqlDbType.VarChar, 15),
                                new SqlParameter(PARM_POSTMONEY, SqlDbType.Decimal),
                                new SqlParameter(PARM_POSTWEIGHT, SqlDbType.Float),
                                new SqlParameter(PARM_DATASTATE,SqlDbType.Int),
                                new SqlParameter(PARM_CONFIRMMONEY,SqlDbType.Decimal),
                                new SqlParameter(PARM_FINANCECONFIRM_MONEY,SqlDbType.Decimal)
                            };
            return parms;
        }

        #endregion

        public void InsertData(CheckDataRecordInfo dataInfo)
        {
            SqlParameter[] parms = GetCheckDataRecordParameters();
            parms[0].Value = dataInfo.CheckId;
            parms[1].Value = dataInfo.OriginalFilePath;
            parms[2].Value = dataInfo.CheckType;
            parms[3].Value = dataInfo.CheckDataState;
            parms[4].Value = dataInfo.CheckCompanyId;
            parms[5].Value = dataInfo.CheckCreateDate;
            parms[6].Value = dataInfo.CheckFilialeId;
            parms[7].Value = dataInfo.CheckPersonnelId;
            parms[8].Value = dataInfo.CheckUser;
            parms[9].Value = string.IsNullOrEmpty(dataInfo.ContrastFilePath) ? string.Empty : dataInfo.ContrastFilePath;
            parms[10].Value = string.IsNullOrEmpty(dataInfo.ConfirmFilePath) ? string.Empty : dataInfo.ConfirmFilePath;
            parms[11].Value = string.IsNullOrEmpty(dataInfo.FinishFilePath) ? string.Empty : dataInfo.FinishFilePath;
            parms[12].Value = dataInfo.CheckDescription;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("对账记录插入失败!", ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataDetailInfo"></param>
        public void InsertDataDetail(CheckDataDetailInfo dataDetailInfo)
        {
            SqlParameter[] parms = GetCheckDataDetailParameters();
            parms[0].Value = dataDetailInfo.CheckId;
            parms[1].Value = dataDetailInfo.ExpressNo;
            parms[2].Value = dataDetailInfo.PostMoney;
            parms[3].Value = dataDetailInfo.PostWeight;
            parms[4].Value = dataDetailInfo.DataState;
            parms[5].Value = dataDetailInfo.SystemConfirmMoney;
            parms[6].Value = dataDetailInfo.FinanceConfirmMoney;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INERT_DETAIL, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("对账明细记录插入失败!", ex);
            }
        }

        /// <summary>
        /// 用于excel有重复数据时
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="expressNo"></param>
        /// <param name="postMoney"></param>
        /// <param name="postWeight"></param>
        public void UpdateDataDetail(Guid checkId, string expressNo, decimal postMoney, decimal postWeight)
        {
            var parms = new[]
                            {
                                new SqlParameter(PARM_POSTMONEY, postMoney),
                                new SqlParameter(PARM_POSTWEIGHT, postWeight),
                                new SqlParameter(PARM_CHECKID, checkId),
                                new SqlParameter(PARM_EXPRESSNO,expressNo)
                            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_DETAILINFO, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("更新对账明细记录失败!", ex);
            }
        }

        public void DeleteDataDetail(Guid checkId)
        {
            var parms = new[] { new SqlParameter(PARM_CHECKID, checkId) };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE_DETAIL, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("删除对账明细失败!", ex);
            }
        }

        public void UpdateResult(CheckDataRecordInfo dataInfo)
        {
            var parms = new[]
                            {
                                new SqlParameter(PARM_CHECKDATASTATE,dataInfo.CheckDataState), 
                                new SqlParameter(PARM_CONTRASTFILEPATH, string.IsNullOrEmpty(dataInfo.ContrastFilePath)?string.Empty:dataInfo.ContrastFilePath),
                                new SqlParameter(PARM_CONFIRMFILEPATH, string.IsNullOrEmpty(dataInfo.ConfirmFilePath)?string.Empty:dataInfo.ConfirmFilePath),
                                new SqlParameter(PARM_FINISHFILEPATH, string.IsNullOrEmpty(dataInfo.FinishFilePath)?string.Empty:dataInfo.FinishFilePath),
                                new SqlParameter(PARM_CHECKDESCRIPTION, dataInfo.CheckDescription),
                                new SqlParameter(PARM_CHECKID, dataInfo.CheckId)
                            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_RESULT, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("更新对账记录相关文件路径失败!", ex);
            }
        }

        public void DeleteData(Guid checkId)
        {
            var parms = new[] { new SqlParameter(PARM_CHECKID, checkId) };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("删除对账记录失败!", ex);
            }
        }

        public bool IsExistCheckData(Guid companyId, int checkType, string filePath)
        {
            var parms = new[]
                            {
                                new SqlParameter(PARM_CHECKCOMPANYID, companyId),
                                new SqlParameter(PARM_CHECKTYPE, checkType),
                                new SqlParameter(PARM_ORIGINALFILEPATH, filePath)
                            };
            try
            {
                var obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_ISEXIST, parms);
                return obj != DBNull.Value;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("根据快递公司、类型、对账文件判断是否存在对账记录失败!", ex);
            }
        }

        public CheckDataRecordInfo GetCheckDataInfoById(Guid id)
        {
            var recordInfo = new CheckDataRecordInfo();
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_CHECKDATAINFO, new SqlParameter(PARM_CHECKID, id)))
            {
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        recordInfo = new CheckDataRecordInfo
                        {
                            CheckId = dr.GetGuid(0),
                            OriginalFilePath = dr.GetString(1),
                            CheckType = dr.GetInt32(2),
                            CheckDataState = dr.GetInt32(3),
                            CheckCompanyId = dr.GetGuid(4),
                            CompanyName = dr.GetString(5),
                            CheckCreateDate = dr.GetDateTime(6),
                            CheckFilialeId = dr.GetGuid(7),
                            CheckPersonnelId = dr.GetGuid(8),
                            CheckUser = dr.GetString(9),
                            ContrastFilePath = dr[10] == DBNull.Value ? string.Empty : dr.GetString(10),
                            ConfirmFilePath = dr[11] == DBNull.Value ? string.Empty : dr.GetString(11),
                            FinishFilePath = dr[12] == DBNull.Value ? string.Empty : dr.GetString(12),
                            CheckDescription = dr[13] == DBNull.Value ? string.Empty : dr.GetString(13)
                        };
                    }
                }
            }
            return recordInfo;
        }

        public IList<CheckDataDetailInfo> GetCheckDataDetailList(IList<Guid> checkIds, int[] dataStates, int count)
        {
            IList<CheckDataDetailInfo> list = new List<CheckDataDetailInfo>();
            var builder = new StringBuilder(SQL_SELECT_DETAILIST);
            if (checkIds.Count > 1)
            {
                string whereStr = "'" + string.Join("','", checkIds.ToArray()) + "'";
                builder.Append(" WHERE CheckId in(" + whereStr + ")");
            }
            else
            {
                builder.Append(" WHERE CheckId ='" + checkIds[0] + "'");
            }
            if (dataStates.Length > 0)
            {
                if (dataStates.Length == 1)
                {
                    builder.Append(" AND DataState=" + dataStates[0]);
                }
                else
                {
                    string whereStr = "";
                    for (var i = 0; i < dataStates.Length; i++)
                    {
                        whereStr += dataStates.Length - 1 == i ? dataStates[i] + "" : dataStates[i] + ",";
                    }
                    builder.Append(" AND DataState in (" + whereStr + ")");
                }
            }
            string sqlStr = builder.ToString();
            sqlStr = count == 0 ? sqlStr.Replace(" TOP {0} ", " ") : string.Format(sqlStr, count);
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sqlStr, null))
            {
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        list.Add(new CheckDataDetailInfo { CheckId = dr.GetGuid(0), ExpressNo = dr.GetString(1), PostMoney = dr.GetDecimal(2), PostWeight = dr[3] == DBNull.Value ? 0 : dr.GetDouble(3), DataState = dr.GetInt32(4), SystemConfirmMoney = dr.GetDecimal(5), FinanceConfirmMoney = dr.GetDecimal(6) });
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="checkType"></param>
        /// <param name="searchKey"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="states"></param>
        /// <returns></returns>
        public IList<CheckDataRecordInfo> GetCheckDataList(Guid companyId, int checkType, string searchKey, DateTime startTime, DateTime endTime, int[] states)
        {
            var resultList = new List<CheckDataRecordInfo>();
            var builder = new StringBuilder(SQL_SELECT_CHECKDATALIST);
            builder.Append(" WHERE ECD.CheckDataState<>99 AND ECD.CheckCreateDate >= '" + startTime + "' AND ECD.CheckCreateDate < '" + endTime + "' ");
            var list = new List<SqlParameter>();
            if (companyId != Guid.Empty)
            {
                builder.Append(" AND ECD.CheckCompanyId=@CheckCompanyId ");
                list.Add(new SqlParameter(PARM_CHECKCOMPANYID, companyId));
            }
            if (checkType != -1)
            {
                builder.Append(" AND ECD.CheckType=@CheckType ");
                list.Add(new SqlParameter(PARM_CHECKTYPE, checkType));
            }
            if (states.Length > 0)
            {
                if (states.Length == 1)
                {
                    builder.Append(" AND ECD.CheckDataState=@CheckDataState ");
                    list.Add(new SqlParameter(PARM_CHECKDATASTATE, states[0]));
                }
                else
                {
                    string str = "";
                    for (int i = 0; i < states.Length; i++)
                    {
                        str += i == states.Length - 1 ? states[i] + "" : states[i] + ",";
                    }
                    builder.Append(" AND ECD.CheckDataState in(" + str + ") ");
                }
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                builder.Append(" AND ECD.CheckUser like '%" + searchKey + "%'");
            }
            builder.Append(" ORDER BY ECD.CheckCreateDate DESC;");
            using (var dr = list.Count > 0 ? SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, builder.ToString(), list.ToArray()) : SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, builder.ToString()))
            {
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        resultList.Add(new CheckDataRecordInfo
                        {
                            CheckId = dr.GetGuid(0),
                            OriginalFilePath = dr.GetString(1),
                            CheckType = dr.GetInt32(2),
                            CheckDataState = dr.GetInt32(3),
                            CheckCompanyId = dr.GetGuid(4),
                            CompanyName = dr.GetString(5),
                            CheckCreateDate = dr.GetDateTime(6),
                            CheckFilialeId = dr.GetGuid(7),
                            CheckPersonnelId = dr.GetGuid(8),
                            CheckUser = dr.GetString(9),
                            ContrastFilePath = dr[10] == DBNull.Value ? string.Empty : dr.GetString(10),
                            ConfirmFilePath = dr[11] == DBNull.Value ? string.Empty : dr.GetString(11),
                            FinishFilePath = dr[12] == DBNull.Value ? string.Empty : dr.GetString(12),
                            CheckDescription = dr[13] == DBNull.Value ? string.Empty : dr.GetString(13)
                        });
                    }
                }
            }
            return resultList;
        }

        /// <summary>
        /// 更改对账状态
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="checkDataState"></param>
        public void UpdateState(Guid checkId, int checkDataState)
        {
            var parms = new[]
                            {
                                new SqlParameter(PARM_CHECKDATASTATE,checkDataState), 
                                new SqlParameter(PARM_CHECKID, checkId)
                            };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_STATE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("更新对账记录状态失败!", ex);
            }
        }

        /// <summary>
        /// 更改详细状态
        /// </summary>
        /// <param name="checkId"></param>
        /// <param name="dataState"></param>
        /// <param name="oldDataState"> </param>
        /// <param name="expressNo"></param>
        /// <param name="confirmMoney"> </param>
        /// <param name="financeConfirmMoney"> </param>
        public void UpdateDataState(Guid checkId, int dataState, int oldDataState, string expressNo, decimal confirmMoney, decimal financeConfirmMoney)
        {
            var builder = new StringBuilder(string.IsNullOrEmpty(expressNo) ? SQL_UPDATE_DATASTATE : SQL_UPDATE_CONFIRM);
            var parmsList = new List<SqlParameter>
                                {
                                    new SqlParameter(PARM_DATASTATE, dataState),
                                    new SqlParameter(PARM_CHECKID, checkId)
                                };
            if (!string.IsNullOrEmpty(expressNo))
            {
                builder.Append(" AND ExpressNo=@ExpressNo ");
                parmsList.Add(new SqlParameter(PARM_CONFIRMMONEY, confirmMoney));
                parmsList.Add(new SqlParameter(PARM_FINANCECONFIRM_MONEY, financeConfirmMoney));
                parmsList.Add(new SqlParameter(PARM_EXPRESSNO, expressNo));
            }
            else
            {
                if (oldDataState != -1)
                {
                    builder.Append(" AND DataState=@OldDataState ");
                    parmsList.Add(new SqlParameter("@OldDataState", oldDataState));
                }
            }

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, builder.ToString(), parmsList.ToArray());
            }
            catch (Exception ex)
            {
                throw new ApplicationException("更新对账明细记录状态失败!", ex);
            }
        }

        /// <summary>
        /// 多条对账记录
        /// </summary>
        /// <param name="checkIds"></param>
        /// <returns></returns>
        public IList<CheckDataRecordInfo> GetCheckDataRecordList(IList<Guid> checkIds)
        {
            var builder = new StringBuilder(SQL_SELECT_CHECKDATALIST);
            var list = new List<CheckDataRecordInfo>();
            if (checkIds.Count > 1)
            {
                string whereStr = "'" + string.Join("','", checkIds.ToArray()) + "'";
                builder.Append(" WHERE CheckId in(" + whereStr + ")");
            }
            else
            {
                builder.Append(" WHERE CheckId ='" + checkIds[0] + "'");
            }
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, builder.ToString()))
            {
                if (dr != null)
                {
                    while (dr.Read())
                    {
                        list.Add(new CheckDataRecordInfo
                                     {
                                         CheckId = dr.GetGuid(0),
                                         OriginalFilePath = dr.GetString(1),
                                         CheckType = dr.GetInt32(2),
                                         CheckDataState = dr.GetInt32(3),
                                         CheckCompanyId = dr.GetGuid(4),
                                         CompanyName = dr.GetString(5),
                                         CheckCreateDate = dr.GetDateTime(6),
                                         CheckFilialeId = dr.GetGuid(7),
                                         CheckPersonnelId = dr.GetGuid(8),
                                         CheckUser = dr.GetString(9),
                                         ContrastFilePath = dr[10] == DBNull.Value ? string.Empty : dr.GetString(10),
                                         ConfirmFilePath = dr[11] == DBNull.Value ? string.Empty : dr.GetString(11),
                                         FinishFilePath = dr[12] == DBNull.Value ? string.Empty : dr.GetString(12),
                                         CheckDescription = dr[13] == DBNull.Value ? string.Empty : dr.GetString(13)
                                     });
                    }
                }
            }
            return list;
        }

        /// <summary>获取对账的实际总金额
        /// </summary>
        /// <param name="checkId"></param>
        /// <returns></returns>
        public IList<ExceptionReckoningInfo> GetTotalMoney(Guid checkId)
        {
            var parm = new SqlParameter(PARM_CHECKID, SqlDbType.UniqueIdentifier) { Value = checkId };
            IList<ExceptionReckoningInfo> exceptionReckoningList = new List<ExceptionReckoningInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_TOTAL, parm))
            {
                while (rdr.Read())
                {
                    var reckoning = new ExceptionReckoningInfo
                    {
                        FilialeId = rdr["FilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["FilialeId"].ToString()),
                        FilialeName = rdr["FilialeName"] == DBNull.Value ? String.Empty : rdr["FilialeName"].ToString(),
                        DiffMoney = 0,
                        SumConfirmMoney = rdr["SumConfirmMoney"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["SumConfirmMoney"]),
                        SumFinanceConfirmMoney = rdr["SumFinanceConfirmMoney"] == DBNull.Value ? 0 : Convert.ToDecimal(rdr["SumFinanceConfirmMoney"]),
                        IsOut = rdr["IsOut"] != DBNull.Value && Convert.ToBoolean(rdr["IsOut"])
                    };
                    exceptionReckoningList.Add(reckoning);
                }
            }
            return exceptionReckoningList;
        }

        /// <summary>
        /// 批量将数据表中的数据插入的备份表中
        /// </summary>
        ///<param name="checkId"></param>
        public void DataToTemp(Guid checkId)
        {
            var parms = new[] { new SqlParameter(PARM_CHECKID, checkId) };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT_TO_TEMP, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("将数据转移到备份表中失败!", ex);
            }
        }

        //批量插入到确认表中，再表联合修改

        /// <summary>
        /// 批量插入数据
        /// </summary>
        /// <param name="addList"></param>
        /// <param name="tableName"></param>
        /// <param name="dics"></param>
        /// <returns></returns>
        public int BitchInsert(IList<CheckDataDetailInfo> addList, string tableName, Dictionary<string, string> dics)
        {
            return SqlHelper.BatchInsert(GlobalConfig.ERP_DB_NAME, addList, tableName, dics);

        }

        /// <summary>获取对比数据  2015-04-03  陈重文
        /// </summary>  
        /// <param name="recordInfo"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="checkType">对账状态</param>
        /// <returns></returns>
        public IList<CheckDataDetailInfo> GetContrastDataList(CheckDataRecordInfo recordInfo, DateTime startTime, DateTime endTime, CheckType checkType)
        {
            var list = new List<CheckDataDetailInfo>();
            var builder = new StringBuilder(SQL_GETCONTRASTDATALIST);
            var sqlparams = new[]{
                new SqlParameter("@CompanyId",recordInfo.CheckCompanyId),
                new SqlParameter("@StartTime",startTime),
                new SqlParameter("@EndTime",endTime),
                new SqlParameter("@IsChecked",(int)checkType),
                new SqlParameter("@ReckoningCheckType",recordInfo.CheckType),
                new SqlParameter("@CheckId",recordInfo.CheckId)
            };
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, builder.ToString(), sqlparams))
            {
                if (dr == null) return list;
                while (dr.Read())
                {
                    var errmsg = String.Empty;
                    if (dr["ServerMoney"] == DBNull.Value && dr["ServerWeight"] == DBNull.Value)
                    {
                        errmsg = "未找到往来帐记录！";
                    }
                    list.Add(new CheckDataDetailInfo
                    {
                        CheckId = recordInfo.CheckId,
                        ExpressNo = dr["ExpressNo"] == DBNull.Value ? String.Empty : dr["ExpressNo"].ToString(),
                        PostMoney = dr["PostMoney"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PostMoney"]),
                        PostWeight = dr["PostWeight"] == DBNull.Value ? 0 : Convert.ToDouble(dr["PostWeight"]),
                        ServerMoney = dr["ServerMoney"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ServerMoney"]),
                        ServerWeight = dr["ServerWeight"] == DBNull.Value ? 0 : Convert.ToDouble(dr["ServerWeight"]),
                        DiffMoney = dr["DiffMoney"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["DiffMoney"]),
                        DataState = (int)DataState.Contrasted,
                        SystemConfirmMoney = dr["ServerMoney"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ServerMoney"]),
                        FinanceConfirmMoney = dr["PostMoney"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["PostMoney"]),
                        ExceptionMessage = String.Empty,
                        ErrorMessage = errmsg,
                        ProvinceName = dr["Province"] == DBNull.Value ? String.Empty : dr["Province"].ToString(),
                        CityName = dr["City"] == DBNull.Value ? String.Empty : dr["City"].ToString(),
                    });
                }
            }
            return list;
        }

        public void DeleteConfirmData(Guid checkId)
        {
            const string SQL = "DELETE BakExpressCheckDataDetail WHERE CheckId=@CheckId";
            var parms = new[] { new SqlParameter(PARM_CHECKID, checkId) };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("删除备份文件中临时记录失败!", ex);
            }
        }

        public void TransferConfirmData(Guid checkId)
        {
            const string SQL = @"UPDATE ExpressCheckDataDetail set ConfirmMoney=B.ConfirmMoney,FinanceConfirmMoney=B.FinanceConfirmMoney,DataState=B.DataState
  FROM ExpressCheckDataDetail E INNER JOIN BakExpressCheckDataDetail B ON E.CheckId=B.CheckId AND E.ExpressNo=B.ExpressNo WHERE E.CheckId=@CheckId";
            var parms = new[] { new SqlParameter(PARM_CHECKID, checkId) };
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("财务确认数据转移失败!", ex);
            }
        }
    }
}
