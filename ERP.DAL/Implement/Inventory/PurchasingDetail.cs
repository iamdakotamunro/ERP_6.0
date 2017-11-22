using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Text;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    public class PurchasingDetail : IPurchasingDetail
    {
        public PurchasingDetail(GlobalConfig.DB.FromType fromType)
        {

        }

        private const string SQL_INSERT = @"INSERT INTO lmShop_PurchasingDetail([PurchasingID],[GoodsID],[GoodsName],[Units],[GoodsCode],[Specification],[CompanyID],[Price],[PlanQuantity],[RealityQuantity],
            [State],[Description],PurchasingGoodsID,PurchasingGoodsType,PlanStocking,[SixtyDaySales],[ThirtyDaySales],[ElevenDaySales],[CPrice]) VALUES(@PurchasingID,@GoodsID,@GoodsName,@Units,@GoodsCode,@Specification,@CompanyID,@Price,@PlanQuantity,@RealityQuantity,@State,@Description,NEWID(),@PurchasingGoodsType,@PlanQuantity,@SixtyDaySales,@ThirtyDaySales,@ElevenDaySales,@CPrice)";
        private const string SQL_DELETE = "DELETE FROM lmShop_PurchasingDetail WHERE PurchasingID=@PurchasingID ";

        private const string SQL_DELETE_GOODSID = "DELETE FROM lmShop_PurchasingDetail WHERE PurchasingID=@PurchasingID And GoodsId=@GoodsID  And PurchasingGoodsID=@PurchasingGoodsID ";
        private const string SQL_UPDATE = @"UPDATE lmShop_PurchasingDetail SET Price=@Price,PlanQuantity=@PlanQuantity,CPrice=@CPrice 
WHERE PurchasingID=@PurchasingID And GoodsID=@GoodsID  And PurchasingGoodsID=@PurchasingGoodsID  ";
        private const string SQL_UPDATE_GOOD_STATE = @"UPDATE lmShop_PurchasingDetail SET  State=@State
WHERE     PurchasingGoodsID=@PurchasingGoodsID ";

        private const string SQL_UPDATE_REALITY_QUANTITY = @"UPDATE lmShop_PurchasingDetail SET RealityQuantity=@RealityQuantity 
WHERE PurchasingID=@PurchasingID And GoodsID=@GoodsID And PurchasingGoodsID=@PurchasingGoodsID ";

        /// <summary>
        /// 增加60天日均 30天日均 11天日均 仅限于采购单修改时查询
        /// </summary>
        private const string SQL_SELECT_BY_60_DAY_30_DAY_11_DAY_AVG = @"
Declare @EndTime datetime;
declare @WareHouseId uniqueidentifier;
Declare @PurchasingToDate datetime;
select @EndTime=StartTime,@WarehouseID=WarehouseID,@PurchasingToDate=PurchasingToDate from lmShop_Purchasing where PurchasingID=@PurchasingID
select distinct dt.[PurchasingID]
,dt.[GoodsId],[GoodsName],[Units],[GoodsCode],[Specification],[CompanyID],[Price],[PlanQuantity],[RealityQuantity],[State],[Description],[PurchasingGoodsID],PurchasingGoodsType,PlanStocking,DayAvgStocking,SixtyDays,
0 as Total,isnull(SixtyDaySales,0) as SixtyDaySales,isnull(ThirtyDaySales,0) as ThirtyDaySales,isnull(ElevenDaySales,0) as ElevenDaySales,@PurchasingToDate as PurchasingToDate,IsException,dt.CPrice  from lmShop_PurchasingDetail dt
where dt.PurchasingID=@PurchasingID 
Order By Specification  ";

        private const string SQL_SELECT_BY_COMPANYID_WAREHOUSEID_GOODSID = @"SELECT PD.PurchasingID,PD.GoodsID,PD.GoodsName,PD.Units,PD.GoodsCode,PD.Specification,PD.CompanyID,PD.Price,PD.PlanQuantity,PD.RealityQuantity,PD.State,PD.Description,
            PD.PurchasingGoodsID,PD.PurchasingGoodsType,pd.PlanStocking,pd.DayAvgStocking,SixtyDays,PD.CPrice FROM lmShop_PurchasingDetail PD INNER JOIN lmShop_Purchasing P ON PD.PurchasingID=P.PurchasingID AND P.CompanyID=@CompanyID AND PD.GoodsID=@GoodsID AND P.WarehouseID=@WarehouseID And P.PurchasingState=@PurchasingState And P.PurchasingType=@PurchasingType AND PD.Price <> 0";


        private const string SQL_SELECT_PURCHASINGNUMBER = "select SUM(isnull(PlanQuantity,0))-SUM(isnull(RealityQuantity,0)) as PurchasingQuantity from lmShop_PurchasingDetail pd  left join lmShop_Purchasing p  on p.PurchasingID=pd.PurchasingID WHERE pd.GoodsID=@GoodsID AND P.WarehouseID=@WarehouseID AND p.PurchasingState <> 4 group by GoodsID";

        //        /// <summary>
        //        /// 查询采购单明细包含货位号和批文号
        //        /// </summary>
        //        private const string SQL_SELECT_DETAIL =
        //            @"select  pd.[PurchasingID],pd.[GoodsId],pd.[GoodsName],pd.[Units],pd.[GoodsCode],[Specification],pd.[CompanyID],[Price],[PlanQuantity],[RealityQuantity],
        //pd.[State],pd.[Description],[PurchasingGoodsID],PurchasingGoodsType,PlanStocking,DayAvgStocking,SixtyDays,0,ISNULL(g.ApprovalNO,'') as ApprovalNO,
        //ISNULL(s.ShelfNo,'') as ShelfNo 
        //from lmShop_PurchasingDetail as pd
        //left join lmShop_Purchasing as p on p.PurchasingID=pd.PurchasingID
        //left join lmShop_RealGoods as rg on rg.RealGoodsId=pd.GoodsID
        //left join lmshop_goods as g on g.GoodsId=rg.GoodsId
        //left join lmShop_Shelf as s on s.GoodsID=g.GoodsId and s.WarehouseID=p.WarehouseID
        //where pd.PurchasingID=@PurchasingID Order By pd.[GoodsCode],Specification ";

        private const string PARM_PURCHASING_ID = "@PurchasingID";
        private const string PARM_GOODS_ID = "@GoodsID";
        private const string PARM_GOODS_NAME = "@GoodsName";
        private const string PARM_UNITS = "@Units";
        private const string PARM_GOODS_CODE = "@GoodsCode";
        private const string PARM_SPECIFICATION = "@Specification";
        private const string PARM_COMPANY_ID = "@CompanyID";
        private const string PARM_PRICE = "@Price";
        private const string PARM_PLAN_QUANTITY = "@PlanQuantity";
        private const string PARM_REALITY_QUANTITY = "@RealityQuantity";
        private const string PARM_STATE = "@State";
        private const string PARM_DESCRIPTION = "@Description";
        private const string PARM_WAREHOUSEID = "@WarehouseID";
        private const string PARM_PURCHASING_GOODS_ID = "@PurchasingGoodsID";
        private const string PARM_PURCHASING_GOODS_TYPE = @"PurchasingGoodsType";
        private const string PARM_PURCHASING_STATE = "@PurchasingState";
        private const string PARM_PURCHASING_TYPE = "@PurchasingType";
        private const string PARM_SIXTY_DAY_SALES = "@SixtyDaySales";
        private const string PARM_THIRTY_DAY_SALES = "@ThirtyDaySales";
        private const string PARM_ELEVEN_DAY_SALES = "@ElevenDaySales";
        private const string PARM_CPRICE = "@CPrice";
        /// <summary>
        /// 插入
        /// </summary>
        /// <param name="info"></param>
        public void Insert(PurchasingDetailInfo info)
        {
            SqlParameter[] parms = GetParameters();
            parms[0].Value = info.PurchasingID;
            parms[1].Value = info.GoodsID;
            parms[2].Value = info.GoodsName;
            parms[3].Value = string.IsNullOrEmpty(info.Units) ? "" : info.Units;
            parms[4].Value = info.GoodsCode;
            parms[5].Value = info.Specification;
            parms[6].Value = info.CompanyID;
            parms[7].Value = info.Price;
            parms[8].Value = info.PlanQuantity;
            parms[9].Value = info.RealityQuantity;
            parms[10].Value = info.State;
            parms[11].Value = info.Description;
            parms[12].Value = info.PurchasingGoodsType;
            parms[13].Value = info.SixtyDaySales;
            parms[14].Value = info.ThirtyDaySales;
            parms[15].Value = info.ElevenDaySales;
            parms[16].Value = info.CPrice;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_INSERT, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }

        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="purchasingID"></param>
        public void Delete(Guid purchasingID)
        {
            var parm = new SqlParameter(PARM_PURCHASING_ID, SqlDbType.UniqueIdentifier) { Value = purchasingID };

            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE, parm);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 删除采购单下的商品
        /// </summary>
        /// <param name="purchasingID"></param>
        /// <param name="goodsId"></param>
        /// <param name="purchasingGoodsID"></param>
        public void DeleteByGoodsId(Guid purchasingID, Guid goodsId, Guid purchasingGoodsID)
        {
            var parms = new[]
            {
                new SqlParameter(PARM_PURCHASING_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_GOODS_ID,SqlDbType.UniqueIdentifier),
                 new SqlParameter(PARM_PURCHASING_GOODS_ID,SqlDbType.UniqueIdentifier)
            };
            parms[0].Value = purchasingID;
            parms[1].Value = goodsId;
            parms[2].Value = purchasingGoodsID;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_DELETE_GOODSID, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// 修改指定商品价格和计划数量
        /// </summary>
        /// <param name="info"></param>
        /// <param name="purchasingGoodsID"></param>
        public void Update(PurchasingDetailInfo info, Guid purchasingGoodsID)
        {
            var parms = new[]
                            {
                                new SqlParameter(PARM_PURCHASING_ID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_GOODS_ID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_PRICE, SqlDbType.Decimal),
                                new SqlParameter(PARM_PLAN_QUANTITY, SqlDbType.Float),
                                new SqlParameter(PARM_PURCHASING_GOODS_ID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_CPRICE,SqlDbType.Decimal)
                            };
            parms[0].Value = info.PurchasingID;
            parms[1].Value = info.GoodsID;
            parms[2].Value = info.Price;
            parms[3].Value = info.PlanQuantity;
            parms[4].Value = purchasingGoodsID;
            parms[5].Value = info.CPrice;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }

        /// <summary>
        /// 修改指定商品采购状态
        /// </summary>
        /// <param name="yesno"></param>
        /// <param name="purchasingGoodsID"></param>
        public void UpdateGoodState(YesOrNo yesno, Guid purchasingGoodsID)
        {
            var parms = new[]
                            {
                                new SqlParameter(PARM_STATE, SqlDbType.Int),
                                new SqlParameter(PARM_PURCHASING_GOODS_ID, SqlDbType.UniqueIdentifier)
                            };

            parms[0].Value = (int)yesno;
            parms[1].Value = purchasingGoodsID;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_GOOD_STATE, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }

        /// <summary>
        /// 修改实际来货数量
        /// </summary>
        /// <param name="info"></param>
        /// <param name="purchasingGoodsID"></param>
        public void UpdateRealQuantity(PurchasingDetailInfo info, Guid purchasingGoodsID)
        {
            var parms = new[]
                            {
                                new SqlParameter(PARM_PURCHASING_ID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_GOODS_ID, SqlDbType.UniqueIdentifier),
                                new SqlParameter(PARM_REALITY_QUANTITY, SqlDbType.Int),
                                new SqlParameter(PARM_PURCHASING_GOODS_ID, SqlDbType.UniqueIdentifier)
                            };
            parms[0].Value = info.PurchasingID;
            parms[1].Value = info.GoodsID;
            parms[2].Value = info.RealityQuantity;
            parms[3].Value = purchasingGoodsID;
            try
            {
                SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL_UPDATE_REALITY_QUANTITY, parms);
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }

        }

        /// <summary>
        /// 修改实际来货数量、状态、价格
        /// </summary>
        /// <param name="info"></param>
        /// <param name="purchasingGoodsId"></param>
        public bool UpdatePurchasingDetail(PurchasingDetailInfo info, Guid purchasingGoodsId)
        {
            const string SQL=@"UPDATE lmShop_PurchasingDetail SET RealityQuantity=@RealityQuantity,Price=@Price,PlanQuantity=@PlanQuantity,CPrice=@CPrice,State=@State 
WHERE PurchasingGoodsID=@PurchasingGoodsID";
            var parms = new[]
                            {
                                new SqlParameter(PARM_REALITY_QUANTITY, SqlDbType.Int) {Value = info.RealityQuantity},
                                new SqlParameter(PARM_PURCHASING_GOODS_ID, SqlDbType.UniqueIdentifier) {Value = info.PurchasingGoodsID},
                                new SqlParameter(PARM_PRICE, SqlDbType.Decimal) {Value = info.Price},
                                new SqlParameter(PARM_PLAN_QUANTITY, SqlDbType.Int) {Value = info.PlanQuantity},
                                new SqlParameter(PARM_CPRICE, SqlDbType.Decimal) {Value = info.CPrice},
                                new SqlParameter(PARM_STATE, SqlDbType.Int) {Value = info.State}
                            };
            try
            {
                return SqlHelper.ExecuteNonQuery(GlobalConfig.ERP_DB_NAME, false, SQL, parms)>0;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }


        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="purchasingID"></param>
        /// <returns></returns>
        public List<PurchasingDetailInfo> Select(Guid purchasingID)
        {
            var strSql = new StringBuilder(@"
SELECT 
	[PurchasingID]
	,[GoodsID]
	,[GoodsName]
	,[GoodsCode]
	,[Specification]
	,[CompanyID]
	,[Price]
	,[PlanQuantity]
	,[RealityQuantity]
	,[State]
	,[Description]
	,[Units]
	,[PurchasingGoodsID]
	,[PurchasingGoodsType]
	,[DayAvgStocking]
	,[PlanStocking]
	,[SixtyDays]
	,[IsException]
	,[SixtyDaySales]
	,[ThirtyDaySales]
	,[ElevenDaySales]
FROM [lmShop_PurchasingDetail]");
            strSql.Append(" WHERE [PurchasingID]='").Append(purchasingID).Append("'");

            var pList = new List<PurchasingDetailInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, strSql.ToString()))
            {
                if (rdr != null)
                {
                    while (rdr.Read())
                    {
                        var pinfo = new PurchasingDetailInfo
                        {
                            PurchasingID = rdr["PurchasingID"] == DBNull.Value ? Guid.Empty : new Guid(rdr["PurchasingID"].ToString()),
                            GoodsID = rdr["GoodsID"] == DBNull.Value ? Guid.Empty : new Guid(rdr["GoodsID"].ToString()),
                            GoodsName = rdr["GoodsName"] == DBNull.Value ? string.Empty : rdr["GoodsName"].ToString(),
                            GoodsCode = rdr["GoodsCode"] == DBNull.Value ? string.Empty : rdr["GoodsCode"].ToString(),
                            Specification = rdr["Specification"] == DBNull.Value ? string.Empty : rdr["Specification"].ToString(),
                            CompanyID = rdr["CompanyID"] == DBNull.Value ? Guid.Empty : new Guid(rdr["CompanyID"].ToString()),
                            Price = rdr["Price"] == DBNull.Value ? -1 : decimal.Parse(rdr["Price"].ToString()),
                            PlanQuantity = rdr["PlanQuantity"] == DBNull.Value ? 0 : double.Parse(rdr["PlanQuantity"].ToString()),
                            RealityQuantity = rdr["RealityQuantity"] == DBNull.Value ? 0 : double.Parse(rdr["RealityQuantity"].ToString()),
                            State = rdr["State"] == DBNull.Value ? 0 : int.Parse(rdr["State"].ToString()),
                            Description = rdr["Description"] == DBNull.Value ? string.Empty : rdr["Description"].ToString(),
                            Units = rdr["Units"] == DBNull.Value ? string.Empty : rdr["Units"].ToString(),
                            PurchasingGoodsID = rdr["PurchasingGoodsID"] == DBNull.Value ? Guid.Empty : new Guid(rdr["PurchasingGoodsID"].ToString()),
                            PurchasingGoodsType = rdr["PurchasingGoodsType"] == DBNull.Value ? 0 : int.Parse(rdr["PurchasingGoodsType"].ToString()),
                            DayAvgStocking = rdr["DayAvgStocking"] == DBNull.Value ? 0 : double.Parse(rdr["DayAvgStocking"].ToString()),
                            PlanStocking = rdr["PlanStocking"] == DBNull.Value ? 0 : double.Parse(rdr["PlanStocking"].ToString()),
                            SixtyDays = rdr["SixtyDays"] == DBNull.Value ? 0 : int.Parse(rdr["SixtyDays"].ToString()),
                            IsException = rdr["IsException"] != DBNull.Value && bool.Parse(rdr["IsException"].ToString()),
                            SixtyDaySales = rdr["SixtyDaySales"] == DBNull.Value ? 0 : int.Parse(rdr["SixtyDaySales"].ToString()),
                            ThirtyDaySales = rdr["ThirtyDaySales"] == DBNull.Value ? 0 : int.Parse(rdr["ThirtyDaySales"].ToString()),
                            ElevenDaySales = rdr["ElevenDaySales"] == DBNull.Value ? 0 : int.Parse(rdr["ElevenDaySales"].ToString()),
                        };
                        pList.Add(pinfo);
                    }
                }
            }
            return pList;
        }
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="purchasingID"></param>
        /// <returns></returns>
        public List<PurchasingDetailInfo> SelectByGoodsDayAvgSales(Guid purchasingID)
        {
            var parm = new SqlParameter(PARM_PURCHASING_ID, SqlDbType.UniqueIdentifier) { Value = purchasingID };

            var pList = new List<PurchasingDetailInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_BY_60_DAY_30_DAY_11_DAY_AVG, parm))
            {
                while (rdr.Read())
                {
                    var pinfo = new PurchasingDetailInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr[3] == DBNull.Value ? "" :
                        rdr.GetString(3), rdr.GetString(4), rdr.GetString(5), rdr.GetGuid(6), rdr[7] == DBNull.Value ? -1 : rdr.GetDecimal(7),
                        rdr.GetDouble(8), rdr[9] == DBNull.Value ? 0 : rdr.GetDouble(9), rdr.GetInt32(10), rdr[11] == DBNull.Value ? null :
                        rdr.GetString(11), rdr.GetGuid(12), rdr.IsDBNull(13) ? 0 : rdr.GetInt32(13), rdr.IsDBNull(14) ? 0 : Convert.ToDouble(rdr[14]), rdr.IsDBNull(15) ? 0 : Convert.ToDouble(rdr[15]), rdr.IsDBNull(16) ? 0 : Convert.ToInt32(rdr[16]), rdr.IsDBNull(17) ? 0 : Convert.ToInt32(rdr[17]))
                    {
                        SixtyDaySales = Convert.ToInt32(rdr[18]),
                        ThirtyDaySales = Convert.ToInt32(rdr[19]),
                        ElevenDaySales = Convert.ToInt32(rdr[20]),
                        PurchasingToDate = rdr.IsDBNull(21) ? "" : rdr.GetDateTime(21).ToString("yyyy-MM-dd"),
                        IsException = !rdr.IsDBNull(22) && rdr.GetBoolean(22),
                        CPrice = rdr[23] == DBNull.Value ? 0 : rdr.GetDecimal(23)
                    };
                    pList.Add(pinfo);
                }
            }
            return pList;
        }

        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="purchasingID"></param>
        /// <param name="purchGoodsId"></param>
        /// <returns></returns>
        public PurchasingDetailInfo GetPurchGoodsId(Guid purchasingID, Guid purchGoodsId)
        {
            var parms = new[] { new SqlParameter(PARM_PURCHASING_ID, SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_PURCHASING_GOODS_ID, SqlDbType.UniqueIdentifier) };
            parms[0].Value = purchasingID;
            parms[1].Value = purchGoodsId;
            const string SQL = @"SELECT [PurchasingID]
                                          ,[GoodsID]
                                          ,[GoodsName]
                                          ,[Units]
                                          ,[GoodsCode]
                                          ,[Specification]
                                          ,[CompanyID]
                                          ,[Price]
                                          ,[PlanQuantity]
                                          ,[RealityQuantity]
                                          ,[State]
                                          ,[Description],[PurchasingGoodsID],PurchasingGoodsType,PlanStocking,DayAvgStocking,SixtyDays,CPrice
                                      FROM [lmShop_PurchasingDetail] Where  PurchasingID=@PurchasingID And PurchasingGoodsID=@PurchasingGoodsID Order By GoodsName,Specification ";
            var pinfo = new PurchasingDetailInfo();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                while (rdr.Read())
                {
                    pinfo = new PurchasingDetailInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr[3] == DBNull.Value ? "" : rdr.GetString(3), rdr.GetString(4), rdr.GetString(5), rdr.GetGuid(6),
                        rdr[7] == DBNull.Value ? -1 : rdr.GetDecimal(7), rdr.GetDouble(8), rdr[9] == DBNull.Value ? 0 : rdr.GetDouble(9),
                        rdr.GetInt32(10), rdr[11] == DBNull.Value ? null : rdr.GetString(11), rdr.GetGuid(12),
rdr.IsDBNull(13) ? 0 : rdr.GetInt32(13), rdr.IsDBNull(14) ? 0 : Convert.ToDouble(rdr[14]), rdr.IsDBNull(15) ? 0 : Convert.ToDouble(rdr[15]), rdr.IsDBNull(16) ? 0 : Convert.ToInt32(rdr[16]))
                    {
                        CPrice = rdr[17] == DBNull.Value ? 0 : rdr.GetDecimal(17)
                    };
                    break;
                }
            }
            return pinfo;
        }

        /// <summary>
        /// 根据商品ID，供应商ID，仓库ID获取是否有该商品的采购记录 价格为-1是未定价，0为赠品
        /// </summary>
        /// <param name="goodsID"></param>
        /// <param name="companyID"></param>
        /// <param name="warehouseID"></param>
        /// <param name="ptype"></param>
        /// <param name="pstate"></param>
        /// <returns></returns>
        public IList<PurchasingDetailInfo> GetPurchasingDetail(Guid goodsID, Guid companyID, Guid warehouseID, PurchasingType ptype, PurchasingState pstate)
        {
            var parms = new[] {
            new SqlParameter(PARM_GOODS_ID,SqlDbType.UniqueIdentifier),
            new SqlParameter(PARM_COMPANY_ID,SqlDbType.UniqueIdentifier),
            new SqlParameter(PARM_WAREHOUSEID,SqlDbType.UniqueIdentifier),
            new SqlParameter(PARM_PURCHASING_STATE,SqlDbType.Int),
            new SqlParameter(PARM_PURCHASING_TYPE,SqlDbType.Int)
            };
            parms[0].Value = goodsID;
            parms[1].Value = companyID;
            parms[2].Value = warehouseID;
            parms[3].Value = (int)pstate;
            parms[4].Value = (int)ptype;
            var pList = new List<PurchasingDetailInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_BY_COMPANYID_WAREHOUSEID_GOODSID, parms))
            {
                while (rdr.Read())
                {
                    var pinfo = new PurchasingDetailInfo(rdr.GetGuid(0), rdr.GetGuid(1), rdr.GetString(2), rdr[3] == DBNull.Value ? "" :
                        rdr.GetString(3), rdr.GetString(4), rdr.GetString(5), rdr.GetGuid(6), rdr[7] == DBNull.Value ? -1 : rdr.GetDecimal(7),
                        rdr.GetDouble(8), rdr[9] == DBNull.Value ? 0 : rdr.GetDouble(9), rdr.GetInt32(10), rdr[11] == DBNull.Value ? null :
                        rdr.GetString(11), rdr.GetGuid(12), rdr.IsDBNull(13) ? 0 : rdr.GetInt32(13), rdr.IsDBNull(14) ? 0 :
                        Convert.ToDouble(rdr[14]), rdr.IsDBNull(15) ? 0 : Convert.ToInt32(rdr[15]), rdr.IsDBNull(16) ? 0 : Convert.ToInt32(rdr[16]))
                    {
                        CPrice = rdr[17] == DBNull.Value ? 0 : rdr.GetDecimal(17)
                    };

                    pList.Add(pinfo);
                }
            }
            return pList;
        }

        /// <summary>
        /// 根据商品ID和仓库ID求出该商品在采购中的数量 add by lxm 20110428
        /// </summary>
        /// <param name="goodsID"></param>
        /// <param name="warehouseID"></param>
        /// <returns></returns>
        public double GetPurchasingNumber(Guid goodsID, Guid warehouseID)
        {
            var parms = new[]{
                new SqlParameter(PARM_GOODS_ID,SqlDbType.UniqueIdentifier),
                new SqlParameter(PARM_WAREHOUSEID,SqlDbType.UniqueIdentifier)
            };
            parms[0].Value = goodsID;
            parms[1].Value = warehouseID;
            double num = 0;
            object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_PURCHASINGNUMBER, parms);
            if (obj != DBNull.Value)
            {
                num = Convert.ToDouble(obj);
            }
            return num;
        }

        #region[查询采购单明细，包含货位号批文号信息]
        /// <summary>
        /// 查询
        /// </summary>
        /// <param name="purchasingID"></param>
        /// <returns></returns>
        public List<PurchasingDetailInfo> SelectDetail(Guid purchasingID)
        {
            //SQL_SELECT_DETAIL
            const string SQL = @"
select 
	pd.[PurchasingID],pd.[GoodsId],pd.[GoodsName],pd.[Units],pd.[GoodsCode],[Specification],pd.[CompanyID],[Price],[PlanQuantity],
	[RealityQuantity],pd.[State],pd.[Description],[PurchasingGoodsID],PurchasingGoodsType,PlanStocking,DayAvgStocking,SixtyDays,p.WarehouseID,pd.CPrice
from lmShop_PurchasingDetail as pd
left join lmShop_Purchasing as p on p.PurchasingID=pd.PurchasingID
where pd.PurchasingID=@PurchasingID 
Order By pd.[GoodsCode],Specification";

            var parm = new SqlParameter(PARM_PURCHASING_ID, SqlDbType.UniqueIdentifier) { Value = purchasingID };

            var pList = new List<PurchasingDetailInfo>();
            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parm))
            {
                while (rdr.Read())
                {
                    var pinfo = new PurchasingDetailInfo
                    {
                        PurchasingID = rdr["PurchasingID"] == DBNull.Value ? Guid.Empty : new Guid(rdr["PurchasingID"].ToString()),
                        GoodsID = rdr["GoodsId"] == DBNull.Value ? Guid.Empty : new Guid(rdr["GoodsId"].ToString()),
                        GoodsName = rdr["GoodsName"] == DBNull.Value ? string.Empty : rdr["GoodsName"].ToString(),
                        Units = rdr["Units"] == DBNull.Value ? string.Empty : rdr["Units"].ToString(),
                        GoodsCode = rdr["GoodsCode"] == DBNull.Value ? string.Empty : rdr["GoodsCode"].ToString(),
                        Specification = rdr["Specification"] == DBNull.Value ? string.Empty : rdr["Specification"].ToString(),
                        CompanyID = rdr["CompanyID"] == DBNull.Value ? Guid.Empty : new Guid(rdr["CompanyID"].ToString()),
                        Price = rdr["Price"] == DBNull.Value ? 0 : decimal.Parse(rdr["Price"].ToString()),
                        PlanQuantity = rdr["PlanQuantity"] == DBNull.Value ? 0 : double.Parse(rdr["PlanQuantity"].ToString()),
                        RealityQuantity = rdr["RealityQuantity"] == DBNull.Value ? 0 : double.Parse(rdr["RealityQuantity"].ToString()),
                        State = rdr["State"] == DBNull.Value ? 0 : int.Parse(rdr["State"].ToString()),
                        Description = rdr["Description"] == DBNull.Value ? string.Empty : rdr["Description"].ToString(),
                        PurchasingGoodsID = rdr["PurchasingGoodsID"] == DBNull.Value ? Guid.Empty : new Guid(rdr["PurchasingGoodsID"].ToString()),
                        PurchasingGoodsType = rdr["PurchasingGoodsType"] == DBNull.Value ? 0 : int.Parse(rdr["PurchasingGoodsType"].ToString()),
                        PlanStocking = rdr["PlanStocking"] == DBNull.Value ? 0 : double.Parse(rdr["PlanStocking"].ToString()),
                        DayAvgStocking = rdr["DayAvgStocking"] == DBNull.Value ? 0 : double.Parse(rdr["DayAvgStocking"].ToString()),
                        SixtyDays = rdr["SixtyDays"] == DBNull.Value ? 0 : int.Parse(rdr["SixtyDays"].ToString()),
                        WarehouseID = rdr["WarehouseID"] == DBNull.Value ? Guid.Empty : new Guid(rdr["WarehouseID"].ToString()),
                        CPrice = rdr["CPrice"] == DBNull.Value ? 0 : decimal.Parse(rdr["CPrice"].ToString())
                    };

                    pList.Add(pinfo);
                }
            }
            return pList;
        }
        #endregion

        public static SqlParameter[] GetParameters()
        {
            //,[SixtyDaySales],[ThirtyDaySales],[ElevenDaySales]
            var parms = new[]{
            new SqlParameter(PARM_PURCHASING_ID,SqlDbType.UniqueIdentifier),
            new SqlParameter(PARM_GOODS_ID,SqlDbType.UniqueIdentifier),
            new SqlParameter(PARM_GOODS_NAME,SqlDbType.VarChar,128),
            new SqlParameter(PARM_UNITS,SqlDbType.VarChar,32),
            new SqlParameter(PARM_GOODS_CODE,SqlDbType.VarChar,32),
            new SqlParameter(PARM_SPECIFICATION,SqlDbType.VarChar,128),
            new SqlParameter(PARM_COMPANY_ID,SqlDbType.UniqueIdentifier),
            new SqlParameter(PARM_PRICE,SqlDbType.Decimal),
            new SqlParameter(PARM_PLAN_QUANTITY,SqlDbType.Float),
            new SqlParameter(PARM_REALITY_QUANTITY,SqlDbType.Float),
            new SqlParameter(PARM_STATE,SqlDbType.Int),
            new SqlParameter(PARM_DESCRIPTION,SqlDbType.VarChar,128),
            new SqlParameter(PARM_PURCHASING_GOODS_TYPE,SqlDbType.Int),
            new SqlParameter(PARM_SIXTY_DAY_SALES,SqlDbType.Int),
            new SqlParameter(PARM_THIRTY_DAY_SALES,SqlDbType.Int),
            new SqlParameter(PARM_ELEVEN_DAY_SALES,SqlDbType.Int),
            new SqlParameter(PARM_CPRICE,SqlDbType.Decimal)
            };

            return parms;
        }

        /// <summary>
        /// 获取子商品的60、30、11天销量
        /// </summary>
        /// <param name="realGoodsId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="endTime"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        public PurchasingDetailInfo GetChildGoodsSale(Guid realGoodsId, Guid warehouseId, DateTime endTime, Guid hostingFilialeId)
        {
            const string SQL = @"
SELECT 
    RealGoodsId,
    DeliverWarehouseId, 
    isnull([60],0) AS SixtyDaySales, 
    isnull([30],0) AS ThirtyDaySales, 
    isnull([11],0) AS ElevenDaySales
FROM 
(
	select RealGoodsId,DeliverWarehouseId,goodsSales,60 as t from lmshop_GoodsDaySalesStatistics as t1
	where DayTime between dateadd(day,-60,convert(varchar(10),@EndTime,120)) and dateadd(day,-30,convert(varchar(10),@EndTime,120)) 
	and DeliverWarehouseId=@WarehouseID and RealGoodsId=@GoodsId and HostingFilialeId=@HostingFilialeId
	union all
	select RealGoodsId,DeliverWarehouseId,goodsSales, 30 as t from lmshop_GoodsDaySalesStatistics as t2
	where DayTime between dateadd(day,-30,convert(varchar(10),@EndTime,120)) and @EndTime 
	and  DeliverWarehouseId=@WarehouseID and RealGoodsId=@GoodsId and HostingFilialeId=@HostingFilialeId
	union all
	select RealGoodsId,DeliverWarehouseId,goodsSales ,11 as t from lmshop_GoodsDaySalesStatistics as t3
	where DayTime between dateadd(day,-11,convert(varchar(10),@EndTime,120)) and @EndTime 
	and  DeliverWarehouseId=@WarehouseID and RealGoodsId=@GoodsId and HostingFilialeId=@HostingFilialeId
)kkk
PIVOT
(
	sum (goodssales)
	FOR t IN( [11], [30], [60] )
) AS pvt
";
            var parms = new[] {
            new SqlParameter(PARM_GOODS_ID,SqlDbType.UniqueIdentifier),
            new SqlParameter(PARM_WAREHOUSEID,SqlDbType.UniqueIdentifier),
            new SqlParameter("@EndTime",SqlDbType.DateTime),
            new SqlParameter("@HostingFilialeId",SqlDbType.UniqueIdentifier),
            };
            parms[0].Value = realGoodsId;
            parms[1].Value = warehouseId;
            parms[2].Value = endTime;
            parms[3].Value = hostingFilialeId;

            PurchasingDetailInfo info = null;
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL, parms))
            {
                if (dr.Read())
                {
                    info = new PurchasingDetailInfo();
                    info.GoodsID = dr["RealGoodsId"] == DBNull.Value ? Guid.Empty : new Guid(dr["RealGoodsId"].ToString());
                    info.SixtyDaySales = dr["SixtyDaySales"] == DBNull.Value ? 0 : int.Parse(dr["SixtyDaySales"].ToString());
                    info.ThirtyDaySales = dr["ThirtyDaySales"] == DBNull.Value ? 0 : int.Parse(dr["ThirtyDaySales"].ToString());
                    info.ElevenDaySales = dr["ElevenDaySales"] == DBNull.Value ? 0 : int.Parse(dr["ElevenDaySales"].ToString());
                }
            }
            return info;
        }

        /// <summary>
        /// 多条采购单存在同主商品且有一条待调价的记录
        /// 调价审核通过时同步更新未提交的采购单中的商品采购价
        /// </summary>
        /// <param name="realGoodsIds">子商品列表</param>
        /// <param name="companyId">主商品绑定公司</param>
        /// <param name="price"> </param>
        /// <param name="warehouseId"></param>
        /// <param name="hostingFilialeId"></param>
        /// <returns></returns>
        public bool UpdatePurchasingDetailPrice(IEnumerable<Guid> realGoodsIds, Guid companyId, decimal price, Guid warehouseId,Guid hostingFilialeId)
        {
            var builder = new StringBuilder(@"update lmShop_PurchasingDetail set Price=@Price,CPrice=@Price
where PurchasingID in(select PurchasingID from lmShop_Purchasing where PurchasingState=0 and CompanyID=@CompanyID and WarehouseID=@WarehouseId and PurchasingFilialeId=@PurchasingFilialeId) and GoodsID in( ");
            var realGoodsIdStr = new StringBuilder("");
            var parms = new[]
                              {
                                  new Parameter("@Price", price),
                                  new Parameter("@CompanyID", companyId),
                                  new Parameter("@WarehouseId",warehouseId),
                                  new Parameter("@PurchasingFilialeId",hostingFilialeId), 
                              };
            foreach (var id in realGoodsIds)
            {
                if (id != Guid.Empty)
                {
                    if (realGoodsIdStr.Length == 0)
                    {
                        realGoodsIdStr.Append(id);
                    }
                    else
                    {
                        realGoodsIdStr.Append(",").Append(id);
                    }
                }
            }
            builder.Append("select id as RealGoodsId from splitToTable('" + realGoodsIdStr + "',','))");
            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false, builder.ToString(), parms);
            }
        }

        /// <summary>
        /// 修改采购单商品价格，排除赠品
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="realGoodsIds"></param>
        /// <param name="price"></param>
        /// <param name="cprice"></param>
        /// <returns></returns>
        public bool UpdateDetailsPrice(Guid purchasingId, List<Guid> realGoodsIds, decimal price, decimal cprice)
        {
            if (realGoodsIds.Count > 0)
            {
                var strbSql = new StringBuilder();
                strbSql.Append("UPDATE lmShop_PurchasingDetail SET Price=").Append(price).Append(",CPrice=").Append(cprice);
                strbSql.Append(" WHERE Price!=0 AND PurchasingID='").Append(purchasingId).Append("'");

                var strbRealGoodsIds = new StringBuilder();
                foreach (var id in realGoodsIds)
                {
                    if (id != Guid.Empty)
                    {
                        if (strbRealGoodsIds.Length == 0)
                            strbRealGoodsIds.Append(id);
                        else
                            strbRealGoodsIds.Append(",").Append(id);
                    }
                }
                strbSql.Append(" AND GoodsId IN (select id as RealGoodsId from splitToTable('" + strbRealGoodsIds + "',','))");
                using (var db = DatabaseFactory.Create())
                {
                    return db.Execute(false, strbSql.ToString());
                }
            }
            return false;
        }


        /// <summary>
        /// 获取进货申报生成的采购明细 
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="purchasingStates"></param>
        /// <param name="realGoodsIds"></param>
        /// <param name="hostingFilialeIds"></param>
        /// <returns>Key  realGoodsId,value:Quantity</returns>
        public Dictionary<Guid, Dictionary<Guid, int>> GetStockDeclarePursingGoodsDicsWithFiliale(Guid warehouseId,IEnumerable<Guid> hostingFilialeIds ,PurchasingState[] purchasingStates, IEnumerable<Guid> realGoodsIds)
        {
            var purchasingGoodsDics = new Dictionary<Guid, Dictionary<Guid, int>>();
            var builder = new StringBuilder(@"
SELECT 
GoodsID,P.PurchasingFilialeId,
case when SUM(isnull(PlanQuantity,0))-SUM(isnull(RealityQuantity,0))<0 
then 0 
else SUM(isnull(PlanQuantity,0))-SUM(isnull(RealityQuantity,0)) 
END AS Quantity  
FROM dbo.lmShop_PurchasingDetail PD
INNER JOIN lmshop_Purchasing P ON PD.PurchasingID=P.PurchasingID AND P.WarehouseId=@WarehouseId 
where P.PurchasingState IN({0})  AND PD.[State]=0 ");
            string filialeStr = hostingFilialeIds != null && hostingFilialeIds.Any()
                ? string.Format(" AND P.PurchasingFilialeId IN('{0}') ", string.Join("','", hostingFilialeIds)):"";

            string statesWhere = purchasingStates.Aggregate("", (current, state) => current + string.Format("{0}{1}", current.Length == 0 ? "" : ",", (int)state));
            string goodsWhere = realGoodsIds != null && realGoodsIds.Any() ? string.Format(" AND PD.GoodsID IN ('{0}')",string.Join("','",realGoodsIds)) : "";
            builder.Append(goodsWhere);
            builder.Append(" group by GoodsID,P.PurchasingFilialeId ");
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, string.Format(builder.ToString(), statesWhere),
                new[] { new SqlParameter("@WarehouseId", SqlDbType.UniqueIdentifier) { Value = warehouseId } }))
            {
                while (dr.Read())
                {
                    var goodsId = dr["GoodsID"] == DBNull.Value ? Guid.Empty : new Guid(dr["GoodsID"].ToString());
                    var quantity =Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(dr["Quantity"].ToString())));
                    var filialeId = dr["PurchasingFilialeId"] == DBNull.Value ? Guid.Empty : new Guid(dr["PurchasingFilialeId"].ToString());
                    if (purchasingGoodsDics.ContainsKey(filialeId))
                    {
                        purchasingGoodsDics[filialeId].Add(goodsId, quantity);
                    }
                    else
                    {
                        purchasingGoodsDics.Add(filialeId, new Dictionary<Guid, int> { {goodsId,quantity} });
                    }
                }
            }
            return purchasingGoodsDics;
        }


        /// <summary>
        /// 获取进货申报生成的采购明细 
        /// </summary>
        /// <param name="warehouseId"></param>
        /// <param name="purchasingStates"></param>
        /// <param name="realGoodsIds"></param>
        /// <returns>Key  realGoodsId,value:Quantity</returns>
        public Dictionary<Guid, int> GetStockDeclarePursingGoodsDics(Guid warehouseId, PurchasingState[] purchasingStates, IEnumerable<Guid> realGoodsIds)
        {
            var purchasingGoodsDics = new Dictionary<Guid, int>();
            var builder = new StringBuilder(@"
SELECT 
GoodsID,
case when SUM(isnull(PlanQuantity,0))-SUM(isnull(RealityQuantity,0))<0 
then 0 
else SUM(isnull(PlanQuantity,0))-SUM(isnull(RealityQuantity,0)) 
END AS Quantity  
FROM dbo.lmShop_PurchasingDetail PD
INNER JOIN lmshop_Purchasing P ON PD.PurchasingID=P.PurchasingID AND P.WarehouseId=@WarehouseId 
where P.PurchasingState IN({0})  AND PD.[State]=0 ");

            string statesWhere = purchasingStates.Aggregate("", (current, state) => current + string.Format("{0}{1}", current.Length == 0 ? "" : ",", (int)state));
            string goodsStr = realGoodsIds.Aggregate("", (current, goodsId) => current + string.Format("{0}{1}", current.Length == 0 ? "" : ",", goodsId));
            string goodsWhere = realGoodsIds != null && realGoodsIds.Any() ? (" AND PD.GoodsID IN (select id as RealGoodsId from splitToTable('" + goodsStr + "',','))") : "";
            builder.Append(goodsWhere);
            builder.Append(" group by GoodsID ");
            using (var dr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, string.Format(builder.ToString(), statesWhere),
                new[] { new SqlParameter("@WarehouseId", SqlDbType.UniqueIdentifier) { Value = warehouseId } }))
            {
                while (dr.Read())
                {
                    var goodsId = dr["GoodsID"] == DBNull.Value ? Guid.Empty : new Guid(dr["GoodsID"].ToString());
                    var quantity = Convert.ToInt32(Math.Ceiling(Convert.ToDecimal(dr["Quantity"].ToString())));
                    purchasingGoodsDics.Add(goodsId, quantity);
                }
            }
            return purchasingGoodsDics;
        }
    }
}
