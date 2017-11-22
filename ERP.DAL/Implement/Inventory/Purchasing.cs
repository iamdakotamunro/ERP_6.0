using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;
using System.Transactions;
using Keede.DAL.RWSplitting;

namespace ERP.DAL.Implement.Inventory
{
    /// <summary>
    /// 采购数据层 2011-03-22 by jiang
    /// </summary>
    public class Purchasing : IPurchasing
    {
        static Dictionary<string, string> _purchasingDetailMappings = new Dictionary<string, string>
        {
            { "PurchasingGoodsID","PurchasingGoodsID"},
            { "PurchasingID","PurchasingID"},
            { "GoodsID","GoodsID"},
            { "GoodsName","GoodsName"},
            { "Units","Units"},
            { "GoodsCode","GoodsCode"},
            { "Specification","Specification"},
            { "CompanyID","CompanyID"},
            { "Price","Price"},
            { "PlanQuantity","PlanQuantity"},
            { "RealityQuantity","RealityQuantity"},
            { "State","State"},
            { "Description","Description"},
            { "PurchasingGoodsType","PurchasingGoodsType"},
            { "SixtyDaySales","SixtyDaySales"},
            { "ThirtyDaySales","ThirtyDaySales"},
            { "ElevenDaySales","ElevenDaySales"},
            { "CPrice","CPrice"},
        };
        public Purchasing(GlobalConfig.DB.FromType fromType)
        {

        }

        #region [SQL语句]
        private const string SQL_INSERT_PURCHASING = @"INSERT INTO [lmShop_Purchasing]([PurchasingID],[PurchasingNo],[CompanyID],[CompanyName],[FilialeID],[WarehouseID],[PurchasingState],[PurchasingType],[StartTime],[Description],PmId,ArrivalTime,Director,PersonResponsible,PurchasingFilialeId,PurchaseGroupId,IsOut,PurchasingPersonName) VALUES(@PurchasingID,@PurchasingNo,@CompanyID,@CompanyName,@FilialeID,@WarehouseID,@PurchasingState,@PurchasingType,@StartTime,@Description,@PmId,@ArrivalTime,@Director,@PersonResponsible,@PurchasingFilialeId,@PurchaseGroupId,@IsOut,@PurchasingPersonName)";
        private const string SQL_UPDATE_PURCHASING = @"UPDATE [lmShop_Purchasing]
   SET [PurchasingNo] = @PurchasingNo 
      ,[CompanyID] = @CompanyID
      ,[CompanyName] = @CompanyName
      ,[WarehouseID] = @WarehouseID
      ,[PurchasingState] = @PurchasingState
      ,[PurchasingType] = @PurchasingType
      ,[StartTime] = @StartTime
      ,[EndTime] = @EndTime
      ,[Description] = @Description
      ,[FilialeId] = @FilialeId
      ,[PmId] = @PmId
      ,[ArrivalTime] = @ArrivalTime
      ,[Director] = @Director
      ,[PersonResponsible] = @PersonResponsible
      ,[PurchasingFilialeId] = @PurchasingFilialeId
      ,[PurchaseGroupId] = @PurchaseGroupId
      ,[IsOut] = @IsOut
 WHERE [PurchasingID] = @PurchasingID";
        private const string SQL_DELETE_PURCHASING = @"Delete From lmShop_Purchasing Where PurchasingState<=1 And PurchasingID=@PurchasingID";
        private const string SQL_SELECT_BY_COMPANYID = "SELECT [PurchasingID],[PurchasingNo],[CompanyID],[CompanyName],[FilialeID],[WarehouseID],[PurchasingState],[PurchasingType],[StartTime],[EndTime],[Description],[PersonResponsible],[PurchasingFilialeId],[IsOut] FROM [lmShop_Purchasing] WHERE CompanyID=@CompanyID and PurchasingState=@PurchasingState and PurchasingType=@PurchasingType and WarehouseID=@WarehouseID";

        private const string SQL_SELECT_PURCHASING_COUNT_BY_PMID = "SELECT COUNT([PurchasingID]) AS PURCHASINGCOUNT FROM [lmShop_Purchasing] WHERE [PmId]=@PmId AND [PurchasingState]=1";
        /// <summary>
        /// 查询一段时间的采购次数
        /// </summary>
        private const string SQL_SELECT_PURCHASING_COUNT = @"
Select distinct b.GoodsName,count(b.PurchasingID) as pCount,SUM(b.PlanQuantity) as totalquantity  from 
(
	select  p.PurchasingID,GoodsName, sum(d.PlanQuantity) as PlanQuantity from
	(
		Select PurchasingID  from lmShop_Purchasing WITH(NOLOCK) 
		where PurchasingType=0 and PurchasingState=4 and EndTime >= @StartTime and EndTime < @EndTime 
	) p
	left join lmShop_PurchasingDetail d  WITH(NOLOCK) on p.PurchasingID=d.PurchasingID 
	group by p.PurchasingID,GoodsName
) b
group by b.GoodsName Order  By  totalquantity Desc";
        #endregion

        #region [参数]
        /// <summary>
        /// 采购id
        /// </summary>
        private const string PARM_PURCHASING_ID = "@PurchasingID";
        /// <summary>
        /// 采购单号
        /// </summary>
        private const string PARM_PURCHASING_NO = "@PurchasingNo";
        /// <summary>
        /// 供应商id
        /// </summary>
        private const string PARM_COMPANY_ID = "@CompanyID";
        /// <summary>
        /// 供应商编号
        /// </summary>
        private const string PARM_COMPANY_NAME = "@CompanyName";
        /// <summary>
        /// 采购公司
        /// </summary>
        private const string PARM_FILIALEID = "@FilialeID";
        /// <summary>
        /// 存储仓库
        /// </summary>
        private const string PARM_WAREHOUSE_ID = "@WarehouseID";
        /// <summary>
        /// 采购单状态
        /// </summary>
        private const string PARM_PURCHASING_STATE = "@PurchasingState";
        /// <summary>
        /// 采购类型
        /// </summary>
        private const string PARM_PURCHASING_TYPE = "@PurchasingType";
        /// <summary>
        /// 采购开始时间
        /// </summary>
        private const string PARM_START_TIME = "@StartTime";
        /// <summary>
        /// 采购结束时间
        /// </summary>
        private const string PARM_END_TIME = "@EndTime";
        /// <summary>
        /// 采购描述
        /// </summary>
        private const string PARM_DESCRIPTION = "@Description";
        /// <summary>
        /// 商品Id
        /// </summary>
        private const string PARM_GOODS_ID = "@GoodsId";
        /// <summary>
        /// 采购组
        /// </summary>
        private const string PARM_PM_ID = "@PmId";
        /// <summary>
        /// 登录人
        /// </summary>
        private const string PARM_PERSON_ID = "@PersonId";
        /// <summary>
        /// 到货时间
        /// </summary>
        private const string PARM_ARRIVALTIME = "@ArrivalTime";
        /// <summary>
        /// 负责人
        /// </summary>
        private const string PARM_DIRECTOR = "@Director";
        /// <summary>
        /// 负责人
        /// </summary>
        private const string PARM_PERSONRESPONSIBLE = "@PersonResponsible";
        /// <summary>
        /// 采购公司
        /// </summary>
        private const string PARM_PURCHASINGFILIALEID = "@PurchasingFilialeId";
        /// <summary>采购分组ID
        /// </summary>
        private const string PARM_PURCHASEGROUPID = "@PurchaseGroupId";
        #endregion

        #region [参数实例]
        /// <summary>
        /// 插入参数
        /// </summary>
        /// <returns></returns>
        private static SqlParameter[] GetInsertParms()
        {
            var parms = new[] {
                new SqlParameter(PARM_PURCHASING_ID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_PURCHASING_NO,SqlDbType.VarChar),
	            new SqlParameter(PARM_COMPANY_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_COMPANY_NAME, SqlDbType.VarChar),
                new SqlParameter(PARM_FILIALEID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_WAREHOUSE_ID,SqlDbType.UniqueIdentifier),
	            new SqlParameter(PARM_PURCHASING_STATE,SqlDbType.Int),
                new SqlParameter(PARM_PURCHASING_TYPE,SqlDbType.Int),
                new SqlParameter(PARM_START_TIME, SqlDbType.DateTime),
                new SqlParameter(PARM_END_TIME,SqlDbType.DateTime),
	            new SqlParameter(PARM_DESCRIPTION,SqlDbType.VarChar),
                new SqlParameter(PARM_PM_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_ARRIVALTIME,SqlDbType.DateTime),
	            new SqlParameter(PARM_DIRECTOR,SqlDbType.VarChar),
	            new SqlParameter(PARM_PERSONRESPONSIBLE,SqlDbType.UniqueIdentifier),
	            new SqlParameter(PARM_PURCHASINGFILIALEID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_PURCHASEGROUPID,SqlDbType.UniqueIdentifier),
                new SqlParameter("@IsOut",SqlDbType.Bit),
                new SqlParameter("@PurchasingPersonName",SqlDbType.VarChar),
            };
            return parms;
        }
        /// <summary>
        /// 查询参数
        /// </summary>
        /// <returns></returns>
        private static SqlParameter[] GetSelectParms()
        {
            var parms = new[] {
                new SqlParameter(PARM_PURCHASING_NO,SqlDbType.VarChar),
	            new SqlParameter(PARM_COMPANY_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_WAREHOUSE_ID,SqlDbType.UniqueIdentifier),
	            new SqlParameter(PARM_PURCHASING_STATE,SqlDbType.Int),
                new SqlParameter(PARM_PURCHASING_TYPE,SqlDbType.Int),
                new SqlParameter(PARM_START_TIME, SqlDbType.DateTime),
                new SqlParameter(PARM_END_TIME,SqlDbType.DateTime),
                new SqlParameter(PARM_GOODS_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_PM_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_PERSON_ID,SqlDbType.UniqueIdentifier)
            };
            return parms;
        }
        #endregion

        #region [IPurchasing 成员]

        #region 插入一张采购单
        /// <summary>
        /// 插入一张采购单
        /// </summary>
        /// <param name="pInfo"></param>
        public void PurchasingInsert(PurchasingInfo pInfo)
        {
            SqlParameter[] parms = GetInsertParms();
            parms[0].Value = pInfo.PurchasingID;
            parms[1].Value = pInfo.PurchasingNo;
            parms[2].Value = pInfo.CompanyID;
            parms[3].Value = pInfo.CompanyName;
            parms[4].Value = pInfo.FilialeID;
            parms[5].Value = pInfo.WarehouseID;
            parms[6].Value = pInfo.PurchasingState;
            parms[7].Value = pInfo.PurchasingType;
            parms[8].Value = pInfo.StartTime;
            parms[9].Value = pInfo.EndTime == DateTime.MinValue ? (object)DBNull.Value : pInfo.EndTime;
            parms[10].Value = pInfo.Description;
            parms[11].Value = pInfo.PmId;
            parms[12].Value = pInfo.ArrivalTime == DateTime.MinValue ? DateTime.Parse("1753/01/01 00:00:00") : pInfo.ArrivalTime;
            parms[13].Value = pInfo.Director;
            parms[14].Value = pInfo.PersonResponsible;
            parms[15].Value = pInfo.PurchasingFilialeId;
            parms[16].Value = pInfo.PurchaseGroupId;
            parms[17].Value = pInfo.IsOut;
            parms[18].Value = pInfo.PurchasingPersonName;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT_PURCHASING, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public void PurchasingInsertWithDetails(PurchasingInfo pInfo, IList<PurchasingDetailInfo> purchasingDetailInfoList)
        {
            SqlParameter[] parms = GetInsertParms();
            parms[0].Value = pInfo.PurchasingID;
            parms[1].Value = pInfo.PurchasingNo;
            parms[2].Value = pInfo.CompanyID;
            parms[3].Value = pInfo.CompanyName;
            parms[4].Value = pInfo.FilialeID;
            parms[5].Value = pInfo.WarehouseID;
            parms[6].Value = pInfo.PurchasingState;
            parms[7].Value = pInfo.PurchasingType;
            parms[8].Value = pInfo.StartTime;
            parms[9].Value = pInfo.EndTime == DateTime.MinValue ? (object)DBNull.Value : pInfo.EndTime;
            parms[10].Value = pInfo.Description;
            parms[11].Value = pInfo.PmId;
            parms[12].Value = pInfo.ArrivalTime == DateTime.MinValue ? DateTime.Parse("1753/01/01 00:00:00") : pInfo.ArrivalTime;
            parms[13].Value = pInfo.Director;
            parms[14].Value = pInfo.PersonResponsible;
            parms[15].Value = pInfo.PurchasingFilialeId;
            parms[16].Value = pInfo.PurchaseGroupId;
            parms[17].Value = pInfo.IsOut;
            parms[18].Value = pInfo.PurchasingPersonName;

            using (var conn = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                conn.Open();
                using (var trans = conn.BeginTransaction())
                {
                    try
                    {
                        SqlHelper.ExecuteNonQuery(trans, SQL_INSERT_PURCHASING, parms);
                        SqlHelper.BatchInsert(trans, purchasingDetailInfoList, "lmShop_PurchasingDetail", _purchasingDetailMappings);
                        trans.Commit();
                    }
                    catch
                    {
                        trans.Rollback();
                        throw;
                    }
                }
            }
        }
        #endregion

        #region 更新一张采购单
        public void PurchasingUpdate(PurchasingInfo pInfo)
        {
            SqlParameter[] parms = GetInsertParms();
            parms[0].Value = pInfo.PurchasingID;
            parms[1].Value = pInfo.PurchasingNo;
            parms[2].Value = pInfo.CompanyID;
            parms[3].Value = pInfo.CompanyName;
            parms[4].Value = pInfo.FilialeID;
            parms[5].Value = pInfo.WarehouseID;
            parms[6].Value = pInfo.PurchasingState;
            parms[7].Value = pInfo.PurchasingType;
            parms[8].Value = pInfo.StartTime;
            parms[9].Value = pInfo.EndTime == DateTime.MinValue ? (object)DBNull.Value : pInfo.EndTime;
            parms[10].Value = pInfo.Description;
            parms[11].Value = pInfo.PmId;
            parms[12].Value = pInfo.ArrivalTime == DateTime.MinValue ? DateTime.Parse("1753/01/01 00:00:00") : pInfo.ArrivalTime;
            parms[13].Value = pInfo.Director;
            parms[14].Value = pInfo.PersonResponsible;
            parms[15].Value = pInfo.PurchasingFilialeId;
            parms[16].Value = pInfo.PurchaseGroupId;
            parms[17].Value = pInfo.IsOut;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_PURCHASING, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }
        #endregion

        #region 修改采购单的描述

        /// <summary>
        /// 修改采购单的描述
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="description"></param>
        public void PurchasingDescription(Guid purchasingId, string description)
        {
            string sqlUpdatePurchasing = @"Update  lmShop_Purchasing Set description=description+@description  ";
            var parms = new[]{new SqlParameter(PARM_PURCHASING_ID,SqlDbType.UniqueIdentifier),
                 new SqlParameter(PARM_DESCRIPTION,SqlDbType.VarChar)
            };
            parms[0].Value = purchasingId;
            parms[1].Value = description;

            sqlUpdatePurchasing += " Where PurchasingID=@PurchasingID;";
            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(trans, sqlUpdatePurchasing, parms);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }
        #endregion

        #region 修改采购单的状态

        /// <summary>
        /// 修改采购单的状态
        /// </summary>
        public void PurchasingUpdate(Guid purchasingId, PurchasingState purchasingState, Guid purchasingFilialeId)
        {
            string sqlUpdatePurchasing = @"Update  lmShop_Purchasing Set PurchasingState=@PurchasingState ";
            var parms = new[]{new SqlParameter(PARM_PURCHASING_ID,SqlDbType.UniqueIdentifier),
                 new SqlParameter(PARM_PURCHASING_STATE,SqlDbType.Int),
                      new SqlParameter("@State",SqlDbType.Int),new SqlParameter("@PurchasingFilialeId",purchasingFilialeId)
            };
            parms[0].Value = purchasingId;
            parms[1].Value = (int)purchasingState;
            parms[2].Value = (int)YesOrNo.Yes;//完成状态
            if (purchasingState == PurchasingState.Purchasing)
            {
                sqlUpdatePurchasing += " , StartTime=getdate(),PurchasingFilialeId=@PurchasingFilialeId ";
            }
            if (purchasingState == PurchasingState.AllComplete)
            {
                sqlUpdatePurchasing += " , endTime=getdate() ";
            }
            sqlUpdatePurchasing += @" Where PurchasingID=@PurchasingID;   ";
            if (purchasingState == PurchasingState.AllComplete)
            {
                sqlUpdatePurchasing += @"    UPDATE lmShop_PurchasingDetail SET  
                                        State=@State Where PurchasingID=@PurchasingID ;";
            }
            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(trans, sqlUpdatePurchasing, parms);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }
        #endregion

        #region [查询采购单]

        /// <summary>
        /// 查询操作人所在组的采购单
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="companyId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <param name="purchasingState"></param>
        /// <param name="purchasingType"></param>
        /// <param name="serchKey"></param>
        /// <param name="realGoodsIds"></param>
        /// <param name="pmId"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        public IList<PurchasingInfo> GetPurchasingList(DateTime startTime, DateTime endTime, Guid companyId, Guid warehouseId,Guid hostingFilialeId, PurchasingState purchasingState, PurchasingType purchasingType, string serchKey, List<Guid> realGoodsIds, Guid pmId, Guid personId)
        {
            StringBuilder sqlSelectPurchasing =new StringBuilder(@"
SELECT 
	a.[PurchasingID],[PurchasingNo],a.[CompanyID],[CompanyName],a.[filialeId],a.[wareHouseId],
	[PurchasingState],[PurchasingType],[StartTime],[EndTime],a.[Description],a.pmId,
	case when b.State=2 then b.pmName+ '(已删除)' else b.pmName end as pmname,
	SUM(isnull(d.PlanQuantity,0)*isnull(d.Price,0)) as sumprice,
	a.PurchasingToDate,a.IsException,a.PersonResponsible,a.PurchasingFilialeId
FROM [lmShop_Purchasing] a 
left join lmshop_PurchasingManagement b On a.PmId=b.pmId
left join lmShop_PurchasingDetail d on a.PurchasingID=d.PurchasingID 
where 1=1 
");
            IList<PurchasingInfo> pList = new List<PurchasingInfo>();
            SqlParameter[] parms = GetSelectParms();
            parms[0].Value = serchKey;
            parms[1].Value = companyId;
            parms[2].Value = warehouseId;
            parms[3].Value = (int)purchasingState;
            parms[4].Value = (int)purchasingType;
            parms[5].Value = startTime == DateTime.MinValue ? startTime.AddYears(1900) : startTime;
            parms[6].Value = endTime == DateTime.MinValue ? endTime.AddYears(1900) : endTime.AddMinutes(59);
            parms[7].Value = Guid.Empty;
            parms[8].Value = pmId;
            parms[9].Value = personId;
            if (!string.IsNullOrEmpty(serchKey))
            {
                sqlSelectPurchasing.AppendFormat(" And PurchasingNo like '%{0}%' ",serchKey);
            }
            if (startTime != DateTime.MinValue && endTime != DateTime.MinValue)
            {
                sqlSelectPurchasing.AppendFormat(" And StartTime between '{0}' and '{1}' ",startTime, endTime.AddMinutes(59));
            }
            else
            {
                if (startTime != DateTime.MinValue)
                {
                    sqlSelectPurchasing.AppendFormat(" And StartTime between '{0}' and getdate() ",startTime);
                }
                if (endTime != DateTime.MinValue)
                {
                    sqlSelectPurchasing.AppendFormat(" And StartTime between '1980-01-01' and '{0}' ", endTime.AddMinutes(59));
                }
            }

            if (companyId != Guid.Empty)
            {
                sqlSelectPurchasing.AppendFormat(" And a.CompanyID='{0}'",companyId);
            }
            if (pmId != Guid.Empty && pmId != new Guid("00000000-0000-0000-0000-000000000001"))
            {
                sqlSelectPurchasing.AppendFormat(" And (a.PmId='{0}' or a.PmId is null or a.PmId='00000000-0000-0000-0000-000000000000' or a.PmId in (select pmid from lmshop_PurchasingManagement where [State]=2) )",pmId);
            }
            if (warehouseId != Guid.Empty)
            {
                sqlSelectPurchasing.AppendFormat(" And a.WarehouseID='{0}'",warehouseId);
            }
            if (hostingFilialeId != Guid.Empty)
            {
                sqlSelectPurchasing.AppendFormat(" And a.PurchasingFilialeId='{0}' ", hostingFilialeId);
            }
            if ((int)purchasingState == -1)
            {

            }
            else if (6 == (int)purchasingState)
            {
                sqlSelectPurchasing.Append(" And PurchasingState in(0,1,2,3) ");
            }
            else
            {
                sqlSelectPurchasing.AppendFormat(" And PurchasingState={0} ", (int)purchasingState);
            }
            if (purchasingType != PurchasingType.All)
            {
                sqlSelectPurchasing.AppendFormat(" And PurchasingType={0} ", (int)purchasingType);
            }

            if (realGoodsIds != null && realGoodsIds.Count > 0)
            {
                var strb = new StringBuilder();
                foreach (var id in realGoodsIds)
                {
                    if (id != Guid.Empty)
                    {
                        if (strb.Length == 0)
                        {
                            strb.Append(id);
                        }
                        else
                        {
                            strb.Append(",").Append(id);
                        }
                    }
                }
                if (string.IsNullOrEmpty(strb.ToString()) == false)
                {
                    sqlSelectPurchasing.AppendFormat(@" and a.[PurchasingID] in (SELECT [PurchasingID] FROM lmShop_PurchasingDetail where GoodsID in (select id as RealGoodsId from splitToTable('" + strb + "',','))) ");
                }
            }
            sqlSelectPurchasing.Append(@" Group by a.[PurchasingID],[PurchasingNo],a.[CompanyID],[CompanyName],
        a.[filialeId],a.[wareHouseId],[PurchasingState],[PurchasingType],[StartTime],[EndTime],a.[Description] ,a.pmId,a.PurchasingToDate,
        b.pmName, b.State,a.IsException,a.PersonResponsible,a.PurchasingFilialeId ");
            sqlSelectPurchasing.Append(" Order By PurchasingState,StartTime desc ");

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sqlSelectPurchasing.ToString()))
            {
                while (rdr.Read())
                {
                    var pInfo = new PurchasingInfo(rdr.GetGuid(0), rdr.GetString(1), rdr.GetGuid(2), rdr.GetString(3),
                        rdr.GetGuid(4), rdr.GetGuid(5), rdr.GetInt32(6), rdr.GetInt32(7)
                        , rdr[8] == DBNull.Value ? DateTime.MinValue : rdr.GetDateTime(8),
                        rdr[9] == DBNull.Value ? DateTime.MinValue : rdr.GetDateTime(9),
                        rdr[10] == DBNull.Value ? "" : rdr.GetString(10),
                        rdr[11] == DBNull.Value ? Guid.Empty : rdr.GetGuid(11),
                        rdr[12] == DBNull.Value ? "" : rdr.GetString(12), Convert.ToDecimal(rdr[13]))
                    {
                        PurchasingToDate =
                            rdr.IsDBNull(14)
                                ? DateTime.MinValue.ToString(CultureInfo.InvariantCulture)
                                : rdr.GetDateTime(14).ToString(CultureInfo.InvariantCulture),
                        IsException = !rdr.IsDBNull(15) && rdr.GetBoolean(15),
                        PersonResponsible =
                            rdr["PersonResponsible"] == DBNull.Value
                                ? Guid.Empty
                                : new Guid(rdr["PersonResponsible"].ToString()),
                        PurchasingFilialeId =
                            rdr["PurchasingFilialeId"] == DBNull.Value
                                ? Guid.Empty
                                : new Guid(rdr["PurchasingFilialeId"].ToString()),
                        CompanyID = new Guid(rdr["CompanyID"].ToString())
                    };
                    pList.Add(pInfo);
                }
            }
            return pList;
        }


        /// <summary>
        /// 查询操作人所在组的采购单
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="companyId"></param>
        /// <param name="wareHouseId"></param>
        /// <param name="pStates"></param>
        /// <param name="pType"></param>
        /// <param name="serchKey"></param>
        /// <param name="goodsList"> </param>
        /// <param name="warehouseIdList"></param>
        /// <param name="personResponsible"></param>
        /// <param name="purchasingFilialeId">采购公司Id</param>
        /// <param name="startPage"></param>
        /// <param name="pageSize"></param>
        /// <param name="recordCount"></param>
        /// <returns></returns>
        public IList<PurchasingInfo> GetPurchasingListToPage(DateTime startTime, DateTime endTime, Guid companyId, Guid wareHouseId, List<int> pStates, PurchasingType pType, string serchKey, List<Guid> goodsList, List<Guid> warehouseIdList, Guid personResponsible, Guid purchasingFilialeId, int startPage, int pageSize, out long recordCount)
        {
            var sqlSelectPurchasing = new StringBuilder(@"
SELECT 
	a.[PurchasingID],[PurchasingNo],a.[CompanyID],[CompanyName],a.[filialeId],a.[wareHouseId],a.[ArrivalTime],
	[PurchasingState],
    case 
    when   PurchasingState=8 then 1
    when   PurchasingState=7 then 2
    when   PurchasingState=0 then 3
    when   PurchasingState=1 then 4
    when   PurchasingState=2 then 5
    when   PurchasingState=3 then 6
    when   PurchasingState=4 then 7
    when   PurchasingState=5 then 8
    when   PurchasingState=6 then 9
end	as OrderIndex,
[PurchasingType],[StartTime],[EndTime],a.[Description],
	SUM(isnull(d.PlanQuantity,0)*isnull(d.Price,0)) as sumprice,
	a.PurchasingToDate,a.IsException,a.PersonResponsible,a.PurchasingFilialeId,a.PurchaseGroupId,a.IsOut,a.PurchasingPersonName 
FROM [lmShop_Purchasing] a 
left join lmShop_PurchasingDetail d on a.PurchasingID=d.PurchasingID 
where 1=1 
");
            if (!string.IsNullOrEmpty(serchKey))
            {
                sqlSelectPurchasing.Append(" And PurchasingNo like '%'+@PurchasingNo + '%' ");
            }
            if (purchasingFilialeId != Guid.Empty)
            {
                sqlSelectPurchasing.AppendFormat(" And PurchasingFilialeId = '{0}'",purchasingFilialeId);
            }
            if (DateTime.MinValue != startTime && DateTime.MinValue != endTime)
            {
                sqlSelectPurchasing.Append(" And StartTime >= @StartTime and StartTime < @EndTime ");
            }
            else
            {
                if (DateTime.MinValue != startTime)
                {
                    sqlSelectPurchasing.Append(" And StartTime >= @StartTime and StartTime < getdate() ");
                }
                if (DateTime.MinValue != endTime)
                {
                    sqlSelectPurchasing.Append(" And StartTime >= '1980-01-01' and StartTime < @EndTime ");
                }
            }

            if (Guid.Empty != companyId)
            {
                sqlSelectPurchasing.AppendFormat(" And a.CompanyID='{0}'", companyId);
            }

            if (wareHouseId != Guid.Empty)
            {
                if (warehouseIdList == null || warehouseIdList.Count(w => w == wareHouseId) > 0)
                    sqlSelectPurchasing.Append(" And a.WarehouseID='" + wareHouseId + "'");
                else
                    sqlSelectPurchasing.Append(" And a.WarehouseID='" + Guid.Empty + "'");
            }
            if (wareHouseId == Guid.Empty && warehouseIdList != null && warehouseIdList.Count > 0)
            {
                string warehouseIdsSql = string.Empty;
                foreach (Guid id in warehouseIdList)
                {
                    if (id == Guid.Empty)
                        continue;
                    if (string.IsNullOrEmpty(warehouseIdsSql))
                        warehouseIdsSql += "'" + id + "'";
                    else
                        warehouseIdsSql += ",'" + id + "'";
                }
                if (!string.IsNullOrEmpty(warehouseIdsSql))
                    sqlSelectPurchasing.Append(" AND a.WarehouseID IN (" + warehouseIdsSql + ")");
            }

            if (pStates.Count == 1)
            {
                if ((pStates[0] == -1))
                {

                }
                else if (6 == pStates[0])
                {
                    sqlSelectPurchasing.Append(" And PurchasingState in(0,1,2,3) ");
                }
                else
                {
                    sqlSelectPurchasing.Append(" And PurchasingState=" + pStates[0] + " ");
                }
            }
            else
            {
                var stateStr = pStates.Aggregate("", (current, pState) => current + (pState + ","));
                stateStr = stateStr.Substring(0, stateStr.Length - 1);
                sqlSelectPurchasing.AppendFormat(" And PurchasingState in({0}) ", stateStr);
            }
            if (pType != PurchasingType.All)
            {
                sqlSelectPurchasing.Append(" And PurchasingType=" + (int)pType + " ");
            }
            if (personResponsible != Guid.Empty)
            {
                sqlSelectPurchasing.Append(" And a.PersonResponsible='" + personResponsible + "' ");
            }
            if (goodsList != null && goodsList.Count > 0)
            {
                var strb = new StringBuilder();
                foreach (var id in goodsList)
                {
                    if (strb.Length == 0)
                    {
                        strb.Append(id);
                    }
                    else
                    {
                        strb.Append(",").Append(id);
                    }
                }
                sqlSelectPurchasing.Append(@" and a.[PurchasingID] in (SELECT [PurchasingID] FROM lmShop_PurchasingDetail 
where GoodsID in (select id as RealGoodsId from splitToTable('" + strb + "',','))) ");
            }
            sqlSelectPurchasing.Append(@" Group by a.[PurchasingID],[PurchasingNo],a.[CompanyID],[CompanyName],
        a.[filialeId],a.[wareHouseId],a.[ArrivalTime],[PurchasingState],[PurchasingType],[StartTime],[EndTime],a.[Description] ,a.PurchasingToDate,
        a.IsException,a.PersonResponsible,a.PurchasingFilialeId,a.PurchaseGroupId,a.IsOut,a.PurchasingPersonName");

            var paramList = new List<Parameter>
                                {
                                    new Parameter(PARM_PURCHASING_NO, serchKey),
                                    new Parameter(PARM_START_TIME,startTime == DateTime.MinValue ? startTime.AddYears(1900) : startTime),
                                    new Parameter(PARM_END_TIME,endTime == DateTime.MinValue? endTime.AddYears(1900): endTime.AddMinutes(59))
                                };
            using (var db = DatabaseFactory.Create())
            {
                var pageQuery = new Keede.DAL.Helper.Sql.PageQuery(startPage, pageSize, sqlSelectPurchasing.ToString(), " OrderIndex asc,StartTime desc ");
                var pageItem = db.SelectByPage<PurchasingInfo>(true, pageQuery, paramList.ToArray());
                recordCount = pageItem.RecordCount;
                return pageItem.Items.ToList();
            }
        }

        /// <summary>
        /// 模型中只返回PurchasingID,PurchasingNo
        /// </summary>
        /// <param name="list"></param>
        /// <param name="searchKey"></param>
        /// <returns></returns>
        public IList<PurchasingInfo> GetPurchasingListByState(IList<PurchasingState> list, string searchKey)
        {
            IList<PurchasingInfo> pList = new List<PurchasingInfo>();

            string sqlSelectPurchasing = @"
SELECT 
	DISTINCT TOP 500 [PurchasingID],[PurchasingNo],PurchasingState,StartTime
FROM [lmShop_Purchasing] a 
LEFT JOIN lmshop_PurchasingManagement b On a.PmId=b.pmId 
WHERE 1=1 ";
            string strState = string.Empty;
            foreach (var state in list)
            {
                if (string.IsNullOrEmpty(strState))
                {
                    strState += (int)state;
                }
                else
                {
                    strState += "," + (int)state;
                }
            }
            if (!string.IsNullOrEmpty(strState))
            {
                sqlSelectPurchasing += " AND PurchasingState IN (" + strState + ") ";
            }
            if (!string.IsNullOrEmpty(searchKey))
            {
                sqlSelectPurchasing += " AND PurchasingNo LIKE '%'+@SearchKey+'%' ";
            }
            sqlSelectPurchasing += " ORDER BY PurchasingState,StartTime DESC ";

            var parameter = new SqlParameter("@SearchKey", searchKey);

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sqlSelectPurchasing, parameter))
            {
                while (rdr.Read())
                {
                    var pInfo = new PurchasingInfo
                    {
                        PurchasingID = rdr["PurchasingID"] == DBNull.Value ? Guid.Empty : new Guid(rdr["PurchasingID"].ToString()),
                        PurchasingNo = rdr["PurchasingNo"] == DBNull.Value ? string.Empty : rdr["PurchasingNo"].ToString()
                    };
                    pList.Add(pInfo);
                }
            }
            return pList;
        }

        /// <summary>
        /// 查询采购单
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="companyId"></param>
        /// <param name="wareHouseId"></param>
        /// <param name="pState"></param>
        /// <param name="pType"></param>
        /// <param name="serchKey"></param>
        /// <param name="goodsId"></param>
        /// <param name="personResponsible">责任人ID</param>
        /// <returns></returns>
        // ReSharper disable once FunctionComplexityOverflow
        public IList<PurchasingInfo> GetPurchasingList(DateTime startTime, DateTime endTime, Guid companyId, Guid wareHouseId,Guid hostingFilialeId, PurchasingState pState, PurchasingType pType, string serchKey, Guid goodsId, Guid personResponsible)
        {
            var strbSql = new StringBuilder(@"
SELECT 
	DISTINCT a.[PurchasingID],[PurchasingNo],a.[CompanyID],[CompanyName],a.[FilialeId],a.[WarehouseID],
	[PurchasingState],[PurchasingType],[StartTime],[EndTime],a.[Description],
	SUM(isnull(d.PlanQuantity,0)*isnull(d.Price,0)) as SumPrice,a.PersonResponsible,a.PurchasingFilialeId,a.IsOut,a.[PurchasingPersonName]   
FROM [lmShop_Purchasing] a 
LEFT JOIN lmShop_PurchasingDetail d on a.PurchasingID=d.PurchasingID 
WHERE 1=1 ");
            if (!string.IsNullOrEmpty(serchKey))
            {
                strbSql.Append(" And PurchasingNo like '%").Append(serchKey).Append("%'");
            }
            if (DateTime.MinValue != startTime && DateTime.MinValue != endTime)
            {
                strbSql.Append(" And StartTime between '").Append(startTime).Append("' and '").Append(endTime).Append("'");
            }
            else
            {
                if (DateTime.MinValue != startTime)
                {
                    strbSql.Append(" And StartTime between '").Append(startTime).Append("' and getdate()");
                }
                if (DateTime.MinValue != endTime)
                {
                    strbSql.Append(" And StartTime between '1980-01-01' and '").Append(endTime).Append("'");
                }
            }
            if (companyId != Guid.Empty)
            {
                strbSql.Append(" And a.CompanyID='").Append(companyId).Append("'");
            }
            if (personResponsible != Guid.Empty)
            {
                strbSql.Append(" And a.PersonResponsible='").Append(personResponsible).Append("'");
            }
            if (wareHouseId != Guid.Empty)
            {
                strbSql.Append(" And a.WarehouseID='").Append(wareHouseId).Append("'");
            }
            if (hostingFilialeId != Guid.Empty)
            {
                strbSql.Append(" And a.PurchasingFilialeId='").Append(hostingFilialeId).Append("'");
            }
            if (pState == PurchasingState.NoComplete)
            {
                strbSql.Append(" And PurchasingState in (0,1,2,3)");
            }
            else
            {
                strbSql.Append(" And PurchasingState=").Append((int)pState);
            }
            if (pType != PurchasingType.All)
            {
                strbSql.Append(" And PurchasingType=").Append((int)pType);
            }
            strbSql.Append(@" Group by a.[PurchasingID],[PurchasingNo],a.[CompanyID],[CompanyName],
        a.[FilialeId],a.[WarehouseID],[PurchasingState],[PurchasingType],[StartTime],[EndTime],a.[Description],
        a.PersonResponsible,a.PurchasingFilialeId,a.IsOut,a.[PurchasingPersonName] ");
            strbSql.Append(" Order By PurchasingState,StartTime desc ");

            IList<PurchasingInfo> pList = new List<PurchasingInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, strbSql.ToString()))
            {
                while (rdr.Read())
                {
                    var pInfo = new PurchasingInfo
                    {
                        PurchasingID = rdr["PurchasingID"] == DBNull.Value ? Guid.Empty : new Guid(rdr["PurchasingID"].ToString()),
                        PurchasingNo = rdr["PurchasingNo"] == DBNull.Value ? string.Empty : rdr["PurchasingNo"].ToString(),
                        CompanyID = rdr["CompanyID"] == DBNull.Value ? Guid.Empty : new Guid(rdr["CompanyID"].ToString()),
                        CompanyName = rdr["CompanyName"] == DBNull.Value ? string.Empty : rdr["CompanyName"].ToString(),
                        FilialeID = rdr["FilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["FilialeId"].ToString()),
                        WarehouseID = rdr["WarehouseID"] == DBNull.Value ? Guid.Empty : new Guid(rdr["WarehouseID"].ToString()),
                        PurchasingState = rdr["PurchasingState"] == DBNull.Value ? 0 : int.Parse(rdr["PurchasingState"].ToString()),
                        PurchasingType = rdr["PurchasingType"] == DBNull.Value ? 0 : int.Parse(rdr["PurchasingType"].ToString()),
                        StartTime = rdr["StartTime"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(rdr["StartTime"].ToString()),
                        EndTime = rdr["EndTime"] == DBNull.Value ? DateTime.MinValue : DateTime.Parse(rdr["EndTime"].ToString()),
                        Description = rdr["Description"] == DBNull.Value ? string.Empty : rdr["Description"].ToString(),
                        SumPrice = rdr["SumPrice"] == DBNull.Value ? 0 : decimal.Parse(rdr["SumPrice"].ToString()),
                        PersonResponsible = rdr["PersonResponsible"] == DBNull.Value ? Guid.Empty : new Guid(rdr["PersonResponsible"].ToString()),
                        PurchasingFilialeId = rdr["PurchasingFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["PurchasingFilialeId"].ToString()),
                    };
                    pList.Add(pInfo);
                }
            }
            return pList;
        }


        /// <summary>
        /// 查询该仓库下的采购中和部分完成的采购单
        /// </summary>
        public IList<PurchasingInfo> GetPurchasingList(Guid wareHouseId)
        {
            string sqlSelectPurchasing = "SELECT [PurchasingID],[PurchasingNo],[CompanyID],[CompanyName],[FilialeID],[WarehouseID],[PurchasingState],[PurchasingType],[StartTime],[EndTime],[Description],[PersonResponsible],[PurchasingFilialeId] FROM [lmShop_Purchasing] AS P Where WareHouseId=@wareHouseId And (PurchasingState=1 or PurchasingState=2) AND NOT EXISTS(SELECT TOP 1 1 FROM InnerPurchaseRelation WHERE PurchasingId=P.PurchasingID) Order By StartTime Desc";
            IList<PurchasingInfo> pList = new List<PurchasingInfo>();
            var parms = new SqlParameter(PARM_WAREHOUSE_ID, SqlDbType.UniqueIdentifier) { Value = wareHouseId };
            //sqlSelectPurchasing += " And wareHouseId=@wareHouseId And (PurchasingState=1 or PurchasingState=2) ";
            //sqlSelectPurchasing += " Order By StartTime Desc ";

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sqlSelectPurchasing, parms))
            {
                while (rdr.Read())
                {
                    var pInfo = new PurchasingInfo(rdr.GetGuid(0), rdr.GetString(1), rdr.GetGuid(2), rdr.GetString(3),
                        rdr.GetGuid(4), rdr.GetGuid(5), rdr.GetInt32(6), rdr.GetInt32(7)
                        , rdr[8] == DBNull.Value ? DateTime.MinValue : rdr.GetDateTime(8),
                        rdr[9] == DBNull.Value ? DateTime.MinValue : rdr.GetDateTime(9),
                        rdr[10] == DBNull.Value ? "" : rdr.GetString(10))
                    {
                        PersonResponsible =
                            rdr["PersonResponsible"] == DBNull.Value
                                ? Guid.Empty
                                : new Guid(rdr["PersonResponsible"].ToString()),
                        PurchasingFilialeId =
                            rdr["PurchasingFilialeId"] == DBNull.Value
                                ? Guid.Empty
                                : new Guid(rdr["PurchasingFilialeId"].ToString())
                    };
                    pList.Add(pInfo);
                }
            }
            return pList;
        }
        #endregion

        #region [根据id查询采购单]
        /// <summary>
        /// 根据id查询采购单
        /// </summary>
        /// <param name="purchasingId">采购id</param>
        /// <returns></returns>
        public PurchasingInfo GetPurchasingById(Guid purchasingId)
        {
            PurchasingInfo pInfo = null;
            const string SQL = "SELECT [PurchasingID],[PurchasingNo],[CompanyID],[CompanyName],[FilialeID],[WarehouseID],[PurchasingState],[PurchasingType],[StartTime],[EndTime],[Description],PmId,NextPurchasingDate,PurchasingToDate,ArrivalTime,Director,PersonResponsible,PurchasingFilialeId,IsOut,PurchasingPersonName FROM [lmShop_Purchasing] Where PurchasingID=@PurchasingID ";
            var parm = new SqlParameter(PARM_PURCHASING_ID, SqlDbType.UniqueIdentifier) { Value = purchasingId };
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                while (rdr.Read())
                {
                    pInfo = new PurchasingInfo(rdr.GetGuid(0), rdr.GetString(1), rdr.GetGuid(2), rdr.GetString(3),
                                               rdr.GetGuid(4), rdr.GetGuid(5), rdr.GetInt32(6), rdr.GetInt32(7)
                                               , rdr[8] == DBNull.Value ? DateTime.MinValue : rdr.GetDateTime(8),
                                               rdr[9] == DBNull.Value ? DateTime.MinValue : rdr.GetDateTime(9),
                                               rdr[10] == DBNull.Value ? "" : rdr.GetString(10))
                        {
                            PmId = rdr.IsDBNull(11) ? Guid.Empty : rdr.GetGuid(11),
                            PurchasingToDate = rdr.IsDBNull(13) ? "" : rdr.GetDateTime(13).ToString("yyyy-MM-dd"),
                            NextPurchasingDate = rdr.IsDBNull(12) ? "" : rdr.GetDateTime(12).ToString("yyyy-MM-dd"),
                            ArrivalTime = rdr.IsDBNull(14) ? DateTime.MinValue : rdr.GetDateTime(14) == DateTime.Parse("1753/01/01 00:00:00") ? DateTime.MinValue : rdr.GetDateTime(14),
                            Director = rdr.IsDBNull(15) ? "" : rdr.GetString(15),
                            PersonResponsible = rdr["PersonResponsible"] == DBNull.Value ? Guid.Empty : new Guid(rdr["PersonResponsible"].ToString()),
                            PurchasingFilialeId = rdr["PurchasingFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["PurchasingFilialeId"].ToString()),
                        };
                }
            }
            return pInfo;
        }
        #endregion

        /// <summary>
        /// 删除一条采购单记录(备注不允许删除部分完成和完成状态下的采购单)
        /// </summary>
        /// <param name="purchasingId"></param>
        public void DeleteById(Guid purchasingId)
        {
            var parm = new SqlParameter(PARM_PURCHASING_ID, SqlDbType.UniqueIdentifier) { Value = purchasingId };
            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(trans, SQL_DELETE_PURCHASING, parm);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }
        #endregion

        /// <summary>
        /// 根据供应商ID获取采购单列表
        /// </summary>
        /// <param name="companyId"></param>
        /// <param name="purchasingState"></param>
        /// <param name="purchasingType"></param>
        /// <param name="warehouseId"></param>
        /// <returns></returns>
        public IList<PurchasingInfo> GetPurchasingListByCompanyID(Guid companyId, int purchasingState, int purchasingType, Guid warehouseId)
        {
            var parms = new[]{
                new SqlParameter(PARM_COMPANY_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_PURCHASING_STATE,SqlDbType.Int),
                new SqlParameter(PARM_PURCHASING_TYPE,SqlDbType.Int),
                new SqlParameter(PARM_WAREHOUSE_ID,SqlDbType.UniqueIdentifier)
            };
            parms[0].Value = companyId;
            parms[1].Value = purchasingState;
            parms[2].Value = purchasingType;
            parms[3].Value = warehouseId;
            IList<PurchasingInfo> pList = new List<PurchasingInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_BY_COMPANYID, parms))
            {
                while (rdr.Read())
                {
                    var pInfo = new PurchasingInfo(rdr.GetGuid(0), rdr.GetString(1), rdr.GetGuid(2), rdr.GetString(3),
                        rdr.GetGuid(4), rdr.GetGuid(5), rdr.GetInt32(6), rdr.GetInt32(7)
                        , rdr[8] == DBNull.Value ? DateTime.MinValue : rdr.GetDateTime(8),
                        rdr[9] == DBNull.Value ? DateTime.MinValue : rdr.GetDateTime(9),
                        rdr[10] == DBNull.Value ? "" : rdr.GetString(10))
                    {
                        PersonResponsible =
                            rdr["PersonResponsible"] == DBNull.Value
                                ? Guid.Empty
                                : new Guid(rdr["PersonResponsible"].ToString()),
                        PurchasingFilialeId =
                            rdr["PurchasingFilialeId"] == DBNull.Value
                                ? Guid.Empty
                                : new Guid(rdr["PurchasingFilialeId"].ToString()),
                        //IsOut = rdr["IsOut"] != DBNull.Value && Convert.ToBoolean(rdr["IsOut"])
                    };
                    pList.Add(pInfo);
                }
            }
            return pList;
        }

        /// <summary>
        /// 根据采购单ID找出该采购单的负责人
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <returns></returns>
        public Guid GetRealNameByPurchasingID(Guid purchasingId)
        {
            const string sql = @"SELECT PersonResponsible from lmShop_Purchasing pu  where pu.PurchasingID=@PurchasingID";
            var parm = new SqlParameter(PARM_PURCHASING_ID, SqlDbType.UniqueIdentifier) { Value = purchasingId };
            var obj = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sql, parm);
            if (obj.Read())
            {
                return (obj.GetGuid(0));
            }
            return Guid.Empty;
        }

        /// <summary>
        /// 通过采购单获取采购单的所有缺货说明
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <returns></returns>
        public IList<GoodsStatementInfo> GetGoodsStockStatementByPurchacingID(Guid purchasingId)
        {
            const string SQL = @"
select 
	d.GoodsID,d.GoodsName,d.Specification,s.StockStatement,s.createtime 
from lmShop_PurchasingDetail d 
left join lmShop_Purchasing p on p.PurchasingID=d.PurchasingID and p.PurchasingState in (1,2) and d.State=0 
left join lmshop_GoodsStockStatement s on d.GoodsID=s.GoodsId 
where d.State=0 and d.PurchasingID=@PurchasingID
Order By d.GoodsName,d.Specification";
            var parm = new SqlParameter("@PurchasingID", SqlDbType.UniqueIdentifier) { Value = purchasingId };

            IList<GoodsStatementInfo> glist = new List<GoodsStatementInfo>();
            using (var obj = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                while (obj.Read())
                {
                    var gsinfo = new GoodsStatementInfo
                    {
                        GoodsId = obj.GetGuid(0),
                        GoodsName = obj.GetString(1),
                        Specification = obj.IsDBNull(2) ? "" : obj.GetString(2),
                        GoodsStatement = obj.IsDBNull(3) ? "" : obj.GetString(3),
                        StatementTime = obj.IsDBNull(4) ? DateTime.MinValue : obj.GetDateTime(4)
                    };
                    glist.Add(gsinfo);
                }
            }
            return glist;
        }

        /// <summary>
        /// 设置商品缺货说明
        /// </summary>
        /// <param name="goodsid"></param>
        /// <param name="statement"></param>
        public void SaveGoodsStatement(Guid goodsid, string statement)
        {
            const string SQL = @"
if not exists(select GoodsId from lmshop_GoodsStockStatement where GoodsId=@GoodsId)
    insert into lmshop_GoodsStockStatement(GoodsId,StockStatement,createtime) values(@GoodsId,@StockStatement,GETDATE());
else
    Update lmshop_GoodsStockStatement set StockStatement=StockStatement+@StockStatement,createtime=GETDATE() where GoodsId=@GoodsId
";
            var parm = new[]
                           {
                               new SqlParameter("@GoodsId", SqlDbType.UniqueIdentifier),
                               new SqlParameter("@StockStatement", SqlDbType.VarChar)
                           };
            parm[0].Value = goodsid;
            parm[1].Value = "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + "]" + statement;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parm);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        #region [根据采购单号查询采购单]
        /// <summary>
        /// 根据采购单号查询采购单
        /// Add by liucaijun at 2011-August-17th
        /// </summary>
        /// <param name="purchasingOrder">采购单号</param>
        /// <returns></returns>
        public PurchasingInfo GetPurchasingList(string purchasingOrder)
        {
            string sqlSelectPurchasing = @"SELECT DISTINCT [PurchasingID],[PurchasingNo],[CompanyID],[CompanyName],a.[filialeId],a.[wareHouseId],
        [PurchasingState],[PurchasingType],[StartTime],[EndTime],a.[Description] ,a.pmId,b.pmName,a.PersonResponsible,a.PurchasingFilialeId,a.IsOut 
        FROM [lmShop_Purchasing] a left join lmshop_PurchasingManagement b On a.PmId=b.pmId Where a.PurchasingState<>5 ";
            var pInfo = new PurchasingInfo();
            var parms = new SqlParameter(PARM_PURCHASING_NO, SqlDbType.VarChar, 32) { Value = purchasingOrder };
            sqlSelectPurchasing += " And a.PurchasingNo =@PurchasingNo ";
            sqlSelectPurchasing += " Order By a.PurchasingState,a.StartTime desc ";

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, sqlSelectPurchasing, parms))
            {
                if (rdr.Read())
                {
                    pInfo = new PurchasingInfo(rdr.GetGuid(0), rdr.GetString(1), rdr.GetGuid(2), rdr.GetString(3), rdr.GetGuid(4), rdr.GetGuid(5), rdr.GetInt32(6), rdr.GetInt32(7)
                        , rdr[8] == DBNull.Value ? DateTime.MinValue : rdr.GetDateTime(8), rdr[9] == DBNull.Value ? DateTime.MinValue : rdr.GetDateTime(9), rdr[10] == DBNull.Value ? "" : rdr.GetString(10), rdr[11] == DBNull.Value ? Guid.Empty : rdr.GetGuid(11), rdr[12] == DBNull.Value ? "" : rdr.GetString(12))
                                {
                                    PersonResponsible = rdr["PersonResponsible"] == DBNull.Value ? Guid.Empty : new Guid(rdr["PersonResponsible"].ToString()),
                                    PurchasingFilialeId = rdr["PurchasingFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["PurchasingFilialeId"].ToString()),
                                    //IsOut = Convert.ToBoolean(rdr["IsOut"])
                                };
                }
            }
            return pInfo;
        }
        #endregion

        #region [根据采购单号查询采购单的金额]
        /// <summary>
        /// 根据采购单号查询采购单的金额
        /// Add by liucaijun at 2011-November-04th
        /// </summary>
        /// <param name="purchasingOrder">采购单号</param>
        /// <returns></returns>
        public Decimal GetPurchasingAmount(string purchasingOrder)
        {
            const string SQL = @"SELECT SUM (CASE price WHEN -1 THEN 0 ELSE Price*PlanQuantity END) AS price 
            FROM lmShop_PurchasingDetail AS a inner join lmShop_Purchasing AS b 
            ON b.PurchasingID=a.PurchasingID WHERE b.PurchasingNo=@PurchasingNo ";

            var parms = new SqlParameter(PARM_PURCHASING_NO, SqlDbType.VarChar, 32) { Value = purchasingOrder };

            decimal amount = 0;
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                if (rdr.Read())
                {
                    amount = (Decimal)rdr.GetDouble(0);
                }
            }
            return amount;
        }
        #endregion
        /// <summary>
        /// 
        /// </summary>
        /// <param name="starttime"></param>
        /// <param name="endtime"></param>
        /// <returns></returns>
        public IList<PurchaseStatisticsInfo> GetPurchaseStatisticsList(DateTime starttime, DateTime endtime)
        {
            var parms = new[]{
                new SqlParameter(PARM_START_TIME,SqlDbType.DateTime),
                new SqlParameter(PARM_END_TIME,SqlDbType.DateTime)
            };
            parms[0].Value = starttime;
            parms[1].Value = endtime;

            IList<PurchaseStatisticsInfo> pList = new List<PurchaseStatisticsInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_PURCHASING_COUNT, parms))
            {
                while (rdr.Read())
                {
                    var psinfo = new PurchaseStatisticsInfo
                                                        {
                                                            GoodsName = rdr.GetString(0),
                                                            Count = rdr.GetInt32(1),
                                                            TotalCount = rdr.GetDouble(2)

                                                        };
                    pList.Add(psinfo);
                }
            }
            return pList;
        }

        /// <summary>
        /// 是否采购中
        /// </summary>
        /// <param name="pmId">采购组ID</param>
        /// <returns></returns>
        public bool IsPurchasing(Guid pmId)
        {
            var parms = new[]{
                                new SqlParameter(PARM_PM_ID, SqlDbType.UniqueIdentifier)
                            };
            parms[0].Value = pmId;
            bool isFlag = false;
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_PURCHASING_COUNT_BY_PMID, parms))
            {
                if (dr.Read())
                {
                    isFlag = Convert.ToInt32(dr["PURCHASINGCOUNT"].ToString()) > 0;
                }
            }
            return isFlag;
        }

        /// <summary>
        /// 修改采购单到货时间
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="arrivaltime"></param>
        /// <param name="director"> </param>
        public void PurchasingArrivalTime(Guid purchasingId, DateTime arrivaltime, string director)
        {
            string sqlUpdatePurchasing = @"Update lmShop_Purchasing Set ArrivalTime=@ArrivalTime,Director=@Director ";
            var parms = new[]{new SqlParameter(PARM_PURCHASING_ID,SqlDbType.UniqueIdentifier),
                 new SqlParameter(PARM_ARRIVALTIME,SqlDbType.DateTime),
                 new SqlParameter(PARM_DIRECTOR,SqlDbType.VarChar)
            };
            parms[0].Value = purchasingId;
            parms[1].Value = arrivaltime;
            parms[2].Value = director;
            sqlUpdatePurchasing += " Where PurchasingID=@PurchasingID;";
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sqlUpdatePurchasing, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        public IList<PurchasingGoodsAmountInfo> GetGoodsAmountList(Guid purchasingId)
        {
            const string SQL = @"
SELECT @PurchasingId AS PurchasingId,GoodsCode,SUM([Price]*[PlanQuantity]) AS AmountPrice
FROM [lmShop_PurchasingDetail]
WHERE PurchasingID=@PurchasingId
GROUP BY GoodsCode
";
            using (var db = DatabaseFactory.Create())
            {
                return db.Select<PurchasingGoodsAmountInfo>(true, SQL, new Parameter("purchasingId", purchasingId)).ToList();
            }
        }

        public void AddNewOrUpdate(Guid oldPurchasingId, IList<PurchasingInfo> newAddList, IDictionary<Guid, string> updateDetailPurchasingIdList)
        {
            const string INSERT = @"INSERT INTO [lmShop_Purchasing]([PurchasingID],[PurchasingNo],[CompanyID],[CompanyName],[FilialeID],[WarehouseID],[PurchasingState],[PurchasingType],[StartTime],[Description],PmId,ArrivalTime,Director,PersonResponsible,PurchasingFilialeId,PurchaseGroupId,IsOut) VALUES(@PurchasingID,@PurchasingNo,@CompanyID,@CompanyName,@FilialeID,@WarehouseID,@PurchasingState,@PurchasingType,@StartTime,@Description,@PmId,@ArrivalTime,@Director,@PersonResponsible,@PurchasingFilialeId,@PurchaseGroupId,@IsOut)";

            const string SQL = @"
UPDATE lmShop_PurchasingDetail SET PurchasingID=@PurchasingID
WHERE PurchasingID=@OldPurchasingID AND GoodsCode=@GoodsCode
";

            using (var db = DatabaseFactory.Create())
            {
                db.BeginTransaction();

                //添加新的采购单
                foreach (var pInfo in newAddList)
                {
                    var parms = new[]
                        {
                            new Parameter(PARM_PURCHASING_ID, pInfo.PurchasingID),
                            new Parameter(PARM_PURCHASING_NO, pInfo.PurchasingNo),
                            new Parameter(PARM_COMPANY_ID, pInfo.CompanyID),
                            new Parameter(PARM_COMPANY_NAME, pInfo.CompanyName),
                            new Parameter(PARM_FILIALEID, pInfo.FilialeID),
                            new Parameter(PARM_WAREHOUSE_ID, pInfo.WarehouseID),
                            new Parameter(PARM_PURCHASING_STATE, pInfo.PurchasingState),
                            new Parameter(PARM_PURCHASING_TYPE, pInfo.PurchasingType),
                            new Parameter(PARM_START_TIME, pInfo.StartTime),
                            new Parameter(PARM_END_TIME, pInfo.EndTime==DateTime.MinValue?(object) DBNull.Value:pInfo.EndTime),
                            new Parameter(PARM_DESCRIPTION, pInfo.Description),
                            new Parameter(PARM_PM_ID, pInfo.PmId),
                            new Parameter(PARM_ARRIVALTIME, pInfo.ArrivalTime == DateTime.MinValue
                                                                ? DateTime.Parse("1900/01/01 00:00:00")
                                                                : pInfo.ArrivalTime),
                            new Parameter(PARM_DIRECTOR, pInfo.Director),
                            new Parameter(PARM_PERSONRESPONSIBLE, pInfo.PersonResponsible),
                            new Parameter(PARM_PURCHASINGFILIALEID, pInfo.PurchasingFilialeId),
                            new Parameter("@PurchaseGroupId", pInfo.PurchaseGroupId),
                            new Parameter("@IsOut",pInfo.IsOut),
                        };
                    db.Execute(false, INSERT, parms);
                }

                //更新老采购详细单对应新采购单
                foreach (var item in updateDetailPurchasingIdList)
                {
                    if (item.Value.Contains(","))
                    {
                        string[] fen = item.Value.Split(',');
                        foreach (var s in fen)
                        {
                            if (!string.IsNullOrEmpty(s))
                            {
                                db.Execute(false, SQL, new Parameter("PurchasingID", item.Key), new Parameter("GoodsCode", s), new Parameter("OldPurchasingID", oldPurchasingId));
                            }
                        }
                    }
                    else
                    {
                        db.Execute(false, SQL, new Parameter("PurchasingID", item.Key), new Parameter("GoodsCode", item.Value), new Parameter("OldPurchasingID", oldPurchasingId));
                    }
                }

                db.CompleteTransaction();
            }
        }

        /// <summary>根据采购单号查询自采购单
        /// </summary>
        /// <param name="purchasingNo"> </param>
        /// <returns></returns>
        public IList<PurchasingInfo> GetPurchasingByMateNo(string purchasingNo)
        {
            const string SQL = @"select  [PurchasingID],[PurchasingNo],[CompanyID],[CompanyName],[FilialeID],[WarehouseID],[PurchasingState],[PurchasingType],[StartTime],[EndTime],[Description],[PersonResponsible],[PurchasingFilialeId],[IsOut] from  dbo.lmShop_Purchasing where PurchasingNo=@PurchasingNo
union
select  [PurchasingID],[PurchasingNo],[CompanyID],[CompanyName],[FilialeID],[WarehouseID],[PurchasingState],[PurchasingType],[StartTime],[EndTime],[Description],[PersonResponsible],[PurchasingFilialeId],[IsOut] from dbo.lmShop_Purchasing where PurchasingNo like @PurchasingNoV ";
            var parms = new[]{
                new SqlParameter(PARM_PURCHASING_NO,SqlDbType.VarChar),
                new SqlParameter("@PurchasingNoV",SqlDbType.VarChar)
            };
            parms[0].Value = purchasingNo;
            parms[1].Value = purchasingNo + "V_";
            IList<PurchasingInfo> pList = new List<PurchasingInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                while (rdr.Read())
                {
                    var pInfo = new PurchasingInfo(rdr.GetGuid(0), rdr.GetString(1), rdr.GetGuid(2), rdr.GetString(3), rdr.GetGuid(4), rdr.GetGuid(5), rdr.GetInt32(6), rdr.GetInt32(7), rdr[8] == DBNull.Value ? DateTime.MinValue : rdr.GetDateTime(8), rdr[9] == DBNull.Value ? DateTime.MinValue : rdr.GetDateTime(9), rdr[10] == DBNull.Value ? "" : rdr.GetString(10))
                                    {
                                        PersonResponsible = rdr["PersonResponsible"] == DBNull.Value ? Guid.Empty : new Guid(rdr["PersonResponsible"].ToString()),
                                        PurchasingFilialeId = rdr["PurchasingFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["PurchasingFilialeId"].ToString()),
                                        //IsOut = rdr["IsOut"] != DBNull.Value && Convert.ToBoolean(rdr["IsOut"])
                                    };
                    pList.Add(pInfo);
                }
            }
            return pList;
        }

        public DateTime GetGoodsPredictArrivalTime(Guid realGoodsId)
        {
            const string SQL = @"
SELECT 
    TOP 1 [ArrivalTime]
FROM [lmShop_Purchasing] AS P WITH(NOLOCK) 
INNER JOIN [lmShop_PurchasingDetail] AS PD WITH(NOLOCK) ON P.PurchasingID=PD.PurchasingID
WHERE P.PurchasingState=1 AND PD.GoodsID=@RealGoodsId";

            var parms = new SqlParameter("@RealGoodsId", realGoodsId) { SqlDbType = SqlDbType.UniqueIdentifier };

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                if (rdr != null && rdr.Read())
                {
                    return rdr.GetDateTime(0);
                }
            }
            return DateTime.MinValue;
        }
        
        /// <summary>
        /// 向lmShop_PurchasingGoods表中批量插入数据
        /// </summary>
        /// <param name="purchasingGoods"></param>
        /// <returns>lmShop_PurchasingGoods(此表用于解决进货申报任务超时的问题，每次用完后会清空此表中的数据)</returns>
        /// zal 2016-03-23
        public bool AddBulklmShopPurchasingGoods(List<Guid> purchasingGoods)
        {
            List<PurchasingGoodsModel> purchasingGoodsModel = purchasingGoods.Select(item => new PurchasingGoodsModel { GoodsId = item }).ToList();
            Dictionary<string, string> dics = new Dictionary<string, string> { { "GoodsID", "GoodsID" } };
            return SqlHelper.BatchInsert(GlobalConfig.ERP_DB_NAME, purchasingGoodsModel, "lmShop_PurchasingGoods", dics) > 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="realGoodsIds">null(取所有子商品)</param>
        /// <returns></returns>
        public bool SelectPurchasingNoCompleteByGoodsId(Guid goodsId, List<Guid> realGoodsIds)
        {
            try
            {
                if (realGoodsIds == null || realGoodsIds.Count == 0)
                {
                    return false;
                }
                var strbSql = new StringBuilder();
                strbSql.AppendLine("SELECT COUNT(1) AS Quantity FROM lmShop_Purchasing p");
                strbSql.AppendLine(" INNER JOIN lmShop_PurchasingDetail pd ON pd.PurchasingID=p.PurchasingID");
                strbSql.Append(" WHERE PurchasingState IN (0,1,2,3)");
                if (realGoodsIds.Count > 0)
                {
                    var strb = new StringBuilder();
                    foreach (var id in realGoodsIds)
                    {
                        if (strb.Length == 0)
                            strb.Append(id);
                        else
                            strb.Append(",").Append(id);
                    }
                    strbSql.Append(" AND GoodsID IN (SELECT id as RealGoodsId FROM splitToTable('" + strb + "',','))");
                }

                using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, strbSql.ToString()))
                {
                    if (dr.Read())
                    {
                        var qantity = dr["Quantity"] == DBNull.Value ? int.MaxValue : Convert.ToInt32(dr["Quantity"].ToString());
                        if (qantity == 0)
                        {
                            return false;
                        }
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 是否存在特定状态的采购单
        /// </summary>
        /// <param name="realGoodsIds">子商品列表</param>
        /// <param name="state">采购单状态</param>
        /// <returns></returns>
        public IList<Guid> GetPurchasingByRealGoodsIdList(IList<Guid> realGoodsIds, int state)
        {
            var builder = new StringBuilder(@"select P.PurchasingID from lmShop_PurchasingDetail PD
inner join lmShop_Purchasing P on PD.PurchasingID=P.PurchasingID and P.PurchasingState=@PurchasingState where GoodsID in(");
            for (int i = 0; i < realGoodsIds.Count; i++)
            {
                builder.AppendFormat(i == realGoodsIds.Count - 1 ? "'{0}'" : "'{0}',", realGoodsIds[i]);
            }
            builder.Append(") group by P.PurchasingID ");
            var parms = new[]
                              {
                                  new Parameter("@PurchasingState", state)
                              };

            using (var db = DatabaseFactory.Create())
            {
                return db.GetValues<Guid>(true, builder.ToString(), parms);
            }
        }

        /// <summary>更新采购单为采购中专用
        /// </summary>
        /// <param name="purchasingId">采购单ID</param>
        /// <param name="purchasingFilialeId">采购公司ID </param>
        /// <param name="isOut"></param>
        public void PurchasingUpdate(Guid purchasingId, Guid purchasingFilialeId, bool isOut)
        {
            const string sql = @"
UPDATE  lmShop_Purchasing SET 
        PurchasingState=@PurchasingState,
        StartTime=GETDATE(),
        PurchasingFilialeId=@PurchasingFilialeId,
        IsOut=@IsOut
WHERE PurchasingID=@PurchasingID; ";
            var parms = new[]{
                                new SqlParameter("@PurchasingID",SqlDbType.UniqueIdentifier){Value = purchasingId},
                                new SqlParameter("@PurchasingState",SqlDbType.Int){Value = (int)PurchasingState.Purchasing},
                                new SqlParameter("@PurchasingFilialeId",SqlDbType.UniqueIdentifier){Value = purchasingFilialeId},
                                new SqlParameter("@IsOut",SqlDbType.Bit){Value = isOut}
            };
            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(trans, sql, parms);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        /// <summary>赠品借记单生成的采购单IsOut为false(赠品借记单生成采购专用)
        /// </summary>
        /// <param name="purchasingId">采购单ID</param>
        public void PurchasingUpdateIsOut(Guid purchasingId)
        {
            const string sql = @"UPDATE  lmShop_Purchasing SET IsOut=0  WHERE PurchasingID=@PurchasingID; ";
            var parms = new[] { new SqlParameter("@PurchasingID", SqlDbType.UniqueIdentifier) { Value = purchasingId } };
            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(trans, sql, parms);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }

        /// <summary>绑定采购单采购公司  陈重文 ADD 2015-06-30
        /// </summary>
        /// <param name="purchasingId">采购单ID</param>
        /// <param name="purchasingFilialeId">采购公司</param>
        /// <returns></returns>
        public Boolean UpdatePurchasingFilialeId(Guid purchasingId, Guid purchasingFilialeId, Boolean isOut)
        {
            const string sql = @"UPDATE [lmShop_Purchasing] SET PurchasingFilialeId=@PurchasingFilialeId,IsOut=@IsOut WHERE PurchasingID=@PurchasingID";
            var parms = new[]{
                new SqlParameter("@PurchasingFilialeId",SqlDbType.UniqueIdentifier){Value = purchasingFilialeId},
                new SqlParameter("@PurchasingID",SqlDbType.UniqueIdentifier){Value = purchasingId},
                new SqlParameter("@IsOut",SqlDbType.Bit){Value = isOut}
            };
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sql, parms) > 0;
            }
            catch (Exception)
            {
                return false;
            }
        }


        /// <summary>
        /// 修改采购单的仓库 文雯 ADD 2016-05-11
        /// </summary>
        public void PurchasingWarehouseId(Guid purchasingId, Guid warehouseId)
        {
            const string SQL_UPDATE_PURCHASING = @"Update lmShop_Purchasing Set WarehouseID=@WarehouseID WHERE PurchasingID=@PurchasingID ";
            var parms = new[]{new SqlParameter("@WarehouseID",warehouseId),
                new SqlParameter("@PurchasingID",purchasingId),
            };
            using (var connection = Databases.GetSqlConnection(GlobalConfig.ERP_DB_NAME, false))
            {
                connection.Open();
                SqlTransaction trans = connection.BeginTransaction();
                try
                {
                    SqlHelper.ExecuteNonQuery(trans, SQL_UPDATE_PURCHASING, parms);
                    trans.Commit();
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    throw new ApplicationException(ex.Message);
                }
            }
        }


        /// <summary>
        /// 修改采购单的状态为部分完成(采购入库完成时使用)
        /// ADD ww
        /// 2016-08-24
        /// </summary>
        public bool PurchasingUpdateStateByPartComplete(Guid purchasingId, PurchasingState purchasingState)
        {
            const string SQL_UPDATE_PURCHASING = @"Update  lmShop_Purchasing Set PurchasingState=@PurchasingState Where PurchasingID=@PurchasingID;";
            var parms = new[]
            {
                new SqlParameter("@PurchasingID", SqlDbType.UniqueIdentifier),
                new SqlParameter("@PurchasingState", SqlDbType.Int)
            };
            parms[0].Value = purchasingId;
            parms[1].Value = (int)purchasingState;

            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_PURCHASING, parms) > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 修改采购单的状态为完成(采购入库完成时使用)
        /// ADD ww
        /// 2016-08-24
        /// </summary>
        public bool PurchasingUpdateStateByAllComplete(Guid purchasingId, PurchasingState purchasingState)
        {
            string sqlUpdatePurchasing = @"Update lmShop_Purchasing Set PurchasingState=@PurchasingState,endTime=getdate() Where PurchasingID=@PurchasingID; ";
            sqlUpdatePurchasing += @"    UPDATE lmShop_PurchasingDetail SET  
                                        State=@State Where PurchasingID=@PurchasingID ;";
            
            var parms = new[]
            {
                new SqlParameter("@PurchasingID", SqlDbType.UniqueIdentifier),
                new SqlParameter("@PurchasingState", SqlDbType.Int),
                new SqlParameter("@State", SqlDbType.Int)
            };
            parms[0].Value = purchasingId;
            parms[1].Value = (int)purchasingState;
            parms[2].Value = (int)YesOrNo.Yes; //完成状态

            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, sqlUpdatePurchasing, parms) > 0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

    }
}
