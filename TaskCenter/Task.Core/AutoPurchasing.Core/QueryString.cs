using System.Text;

namespace AutoPurchasing.Core
{
    public class QueryString
    {
        #region -- SQL参数
        /// <summary>
        /// 参数：商品名称
        /// </summary>
        public const string PARM_GOODS_NAME = "@GoodsName";

        /// <summary>
        /// 参数：商品计量单位
        /// </summary>
        public const string PARM_UNITS = "@Units";

        /// <summary>
        /// 参数：商品条码
        /// </summary>
        public const string PARM_GOODS_CODE = "@GoodsCode";

        /// <summary>
        /// 参数：商品规格
        /// </summary>
        public const string PARM_SPECIFICATION = "@Specification";

        /// <summary>
        /// 参数：商品价格
        /// </summary>
        public const string PARM_PRICE = "@Price";

        /// <summary>
        /// 参数：预计采购数量
        /// </summary>
        public const string PARM_PLAN_QUANTITY = "@PlanQuantity";

        /// <summary>
        /// 参数：实际采购数量
        /// </summary>
        public const string PARM_REALITY_QUANTITY = "@RealityQuantity";

        /// <summary>
        /// 参数：状态
        /// </summary>
        public const string PARM_STATE = "@State";

        /// <summary>
        /// 参数：采购信息ID
        /// </summary>
        public const string PARM_PURCHASING_ID = "@PurchasingID";

        /// <summary>
        /// 参数：采购单号
        /// </summary>
        public const string PARM_PURCHASING_NO = "@PurchasingNo";

        /// <summary>
        /// 参数：采购单位名称
        /// </summary>
        public const string PARM_COMPANY_NAME = "@CompanyName";

        /// <summary>
        /// 参数：分公司ID
        /// </summary>
        public const string PARM_FILIALEID = "@FilialeID";

        /// <summary>
        /// 参数：采购状态
        /// </summary>
        public const string PARM_PURCHASING_STATE = "@PurchasingState";

        /// <summary>
        /// 参数：采购类型
        /// </summary>
        public const string PARM_PURCHASING_TYPE = "@PurchasingType";

        /// <summary>
        /// 参数：采购开始时间
        /// </summary>
        public const string PARM_START_TIME = "@StartTime";

        /// <summary>
        /// 参数：采购结束时间
        /// </summary>
        public const string PARM_END_TIME = "@EndTime";

        /// <summary>
        /// 参数：描述
        /// </summary>
        public const string PARM_DESCRIPTION = "@Description";

        /// <summary>
        /// 参数：商品ID
        /// </summary>
        public const string PARM_GOODSID = "@GoodsId";

        /// <summary>
        /// 参数：仓库ID
        /// </summary>
        public const string PARM_WAREHOUSEID = "@WarehouseId";

        /// <summary>
        /// 参数：任务ID
        /// </summary>
        public const string PARM_TASKID = "@TaskId";

        /// <summary>
        /// 参数：公司ID
        /// </summary>
        public const string PARM_COMPANYID = "@CompanyId";

        /// <summary>
        /// 参数：
        /// </summary>
        public const string PARM_CODETYPE = "@CodeType";

        /// <summary>
        /// 参数：每日平均销量（字段名字建议调整）
        /// </summary>
        public const string PARM_DAY_AVG_STOCKING = "@DayAvgStocking";

        /// <summary>
        /// 参数：采购备货小组ID
        /// </summary>
        public const string PARM_PM_ID = "@PmId";

        /// <summary>
        /// 参数：计划备货
        /// </summary>
        public const string PARM_PLAN_STOCKING = "@PlanStockin";

        /// <summary>
        /// 参数：备货天数
        /// </summary>
        public const string PARM_STOCKDAYS = "@StockDays";

        /// <summary>
        /// 参数：备货+到货=总备货流程天数
        /// </summary>
        public const string PARM_SIXTY_DAYS = "@SixtyDays";

        /// <summary>
        /// 下一次采购时间
        /// </summary>
        public const string PARM_NEXT_PURCHASING_DATE = "@NextPurchasingDate";

        /// <summary>
        /// 是否异常
        /// </summary>
        public const string PARM_ISEXCEPTION = "@IsException";

        /// <summary>
        /// 60天销量
        /// </summary>
        public const string PARM_SIXTYDAYSALES = "@SixtyDaySales";

        /// <summary>
        /// 30天销量
        /// </summary>
        public const string PARM_THIRTYDAYSALES = "@ThirtyDaySales";

        /// <summary>
        /// 11天日均销量
        /// </summary>
        public const string PARM_ELEVENDAYSALES = "@ElevenDaySales";

        /// <summary>
        /// 采购到货时间
        /// </summary>
        public const string PARM_ARRIVAL_DATE = "@PurchasingToDate";
        
        /// <summary>
        /// 责任人
        /// </summary>
        public const string PARM_PERSONRESPONSIBLE = "@PersonResponsible";
        #endregion

        /// <summary>
        /// 列出所有的采购商品
        /// </summary>
        public const string SELECT_PURCHASINGGOODS_ALL = @"
SELECT
	ps.FilingDays AS FilingDay,
	ps.StockingDays AS StockingDays,
	ps.ArrivalDays AS ArrivalDays,
    ps.WarehouseId ,
	ps.GoodsId,
	ps.LastPurchasingDate AS LastPurchasingDate,
	ISNULL(ps.CompanyId,'00000000-0000-0000-0000-000000000000') AS CompanyId,
	ISNULL(c.CompanyName,'') AS CompanyName,
	ISNULL(ps.PurchasePrice,0.00) AS Price,
	ps.PersonResponsible,
    PurchaseGroupId,FilingForm,StockUpDay,FirstWeek,SecondWeek,ThirdWeek,FourthWeek,FilingTrigger,Insufficient,PromotionId,HostingFilialeId   
FROM lmshop_PurchaseSet AS ps
LEFT JOIN lmShop_CompanyCussent AS c ON c.CompanyId = ps.CompanyId
";

        /// <summary>
        /// 统计采购数量
        /// </summary>
        public const string SUM_PURCHASING_QUANTITY = @"
SELECT ISNULL(SUM(PD.PlanQuantity-PD.RealityQuantity),0) AS PurchasingQuantity FROM lmShop_PurchasingDetail AS PD
INNER JOIN lmShop_Purchasing AS P ON P.PurchasingID = PD.PurchasingID 
WHERE PD.PlanQuantity>PD.RealityQuantity 
AND ( PD.PurchasingGoodsType <> 1 OR PD.PurchasingGoodsType IS NULL ) 
AND PD.GoodsID=@GoodsID
AND P.PurchasingState IN (0,1,2,3,7,8) 
AND PD.[State] = 0
AND P.WarehouseID=@WarehouseID
AND P.FilialeId=@HostingFilialeId
AND P.PurchasingType IN (1,2) 
";

        public const string SUM_PURCHASING_ALL_QUANTITY = @"SELECT PD.GoodsID,ISNULL(SUM(PD.PlanQuantity-PD.RealityQuantity),0) AS PurchasingQuantity FROM lmShop_PurchasingDetail AS PD
INNER JOIN lmShop_Purchasing AS P ON P.PurchasingID = PD.PurchasingID 
WHERE PD.PlanQuantity>PD.RealityQuantity 
AND P.PurchasingState IN (0,1,2,3,7,8) 
AND PD.[State] = 0
AND P.WarehouseID=@WarehouseID AND P.FilialeId=@HostingFilialeId
GROUP BY PD.GoodsID
";

        /// <summary>
        /// 插入采购计划
        /// </summary>
        public const string INSERT_PURCHASING = @"INSERT INTO [lmShop_Purchasing]([PurchasingID],[PurchasingNo],[CompanyID],[CompanyName],[FilialeID],[WarehouseID],[PurchasingFilialeId],[PurchasingState],[PurchasingType],[StartTime],[Description],[PmId],[PurchasingToDate],[NextPurchasingDate],[IsException],[PersonResponsible]) VALUES(@PurchasingID,@PurchasingNo,@CompanyID,@CompanyName,@FilialeID,@WarehouseID,@PurchasingFilialeId,@PurchasingState,@PurchasingType,@StartTime,@Description,@PmId,@PurchasingToDate,@NextPurchasingDate,@IsException,@PersonResponsible)";

        /// <summary>
        /// 插入采购计划的详细信息
        /// </summary>
        public const string INSERT_PURCHASING_DETAILS =
            "INSERT INTO lmShop_PurchasingDetail([PurchasingGoodsId],[PurchasingID],[GoodsID],[GoodsName],[Units],[GoodsCode],[Specification],[CompanyID],[Price],[PlanQuantity],[RealityQuantity],[State],[Description],DayAvgStocking,PlanStocking,SixtyDays,IsException,SixtyDaySales,ThirtyDaySales,ElevenDaySales,PurchasingGoodsType) " +
            "VALUES(@PurchasingGoodsId,@PurchasingID,@GoodsID,@GoodsName,@Units,@GoodsCode,@Specification,@CompanyID,@Price,@PlanQuantity,@RealityQuantity,@State,@Description,@DayAvgStocking,@PlanStockin,@SixtyDays,@IsException,@SixtyDaySales,@ThirtyDaySales,@ElevenDaySales,@PurchasingGoodsType)";

        /// <summary>
        /// 查询并获取单据编号信息
        /// </summary>
        public const string SELECT_GET_CODEVALUE = @"declare @CodeValue int;UPDATE lmShop_Code SET @CodeValue=CodeValue=(case when(DATEDIFF(hh,LastDateTime,GETDATE())>0) then 0 else CodeValue end)+1,LastDateTime=GETDATE() WHERE CodeType=@CodeType;SELECT @CodeValue";

        /// <summary>
        /// 查询是否存在类似的采购单
        /// </summary>
        public const string SELECT_SAME_PURCHASINGINFO = @"SELECT P.PurchasingID FROM lmShop_Purchasing AS P 
WHERE P.CompanyID=@CompanyID 
AND P.PersonResponsible=@PersonResponsible 
AND [P].[Description]='[自动报备]' 
AND P.PurchasingState = 0 
AND P.WarehouseID=@WarehouseID AND P.PurchasingFilialeId=@HostingFilialeId ";

        /// <summary>
        /// 查询是否存在类似的详细采购单，包括规格
        /// </summary>
        public const string SELECT_SAME_PURCHASINGDETAIL_SPECIFICATION = @"SELECT TOP 1  PD.PurchasingGoodsID FROM lmShop_PurchasingDetail AS PD 
INNER JOIN lmShop_Purchasing as P ON P.PurchasingID = PD.PurchasingID 
WHERE P.PurchasingFilialeId=@FilialeId AND PD.PurchasingID=@PurchasingID AND PD.GoodsId=@GoodsId AND (PD.PurchasingGoodsType<>1 OR PD.PurchasingGoodsType IS NULL) AND PD.Specification=@Specification ";


        public const string UPDATE_PURCHASINGDETAIL_PLAN_QUANTITY = @"UPDATE lmShop_PurchasingDetail SET lmShop_PurchasingDetail.PlanQuantity=(lmShop_PurchasingDetail.PlanQuantity+@PlanQuantity) WHERE lmShop_PurchasingDetail.PurchasingGoodsID=@PurchasingGoodsID";

        /// <summary>
        /// 更新最后的采购时间
        /// </summary>
        public const string UPDATE_LASTPURCHASINGDATE = @"UPDATE lmshop_PurchaseSet SET LastPurchasingDate=@LastPurchasingDate WHERE WarehouseId=@WarehouseId AND HostingFilialeId=@HostingFilialeId AND GoodsId=@GoodsId";
        
        /// <summary>
        /// 加入WHERE条件的查询
        /// </summary>
        /// <param name="selectQueryString"> </param>
        /// <param name="whereFilter"></param>
        /// <returns></returns>
        public static string Where(string selectQueryString, string whereFilter)
        {
            var sb = new StringBuilder();
            sb.Append(selectQueryString);
            if (whereFilter.Trim() != string.Empty)
            {
                sb.Append(" WHERE ");
                sb.Append(whereFilter);
            }
            return sb.ToString();
        }
    }
}
