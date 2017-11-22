using System;
using System.Collections.Generic;
using Keede.DAL.Helper;

namespace AutoPurchasing.Core
{
    public class PrepareParameter
    {
        private readonly IList<Parameter> parameterList;

        public PrepareParameter()
        {
            parameterList = new List<Parameter>();
        }

        public static PrepareParameter Create()
        {
            return new PrepareParameter();
        }

        public void Append(string parameterField, object parameterValue)
        {
            parameterList.Add(new Parameter(parameterField, parameterValue));
        }

        public Parameter[] Result()
        {
            var paramList = new Parameter[parameterList.Count];
            for (var i = 0; i < parameterList.Count; i++)
            {
                paramList[i] = parameterList[i];
            }
            return paramList;
        }

        /// <summary>
        /// 插入采购单参数
        /// </summary>
        /// <param name="purchasingId"></param>
        /// <param name="purchasingNo"></param>
        /// <param name="companyId"></param>
        /// <param name="companyName"></param>
        /// <param name="filialeId"></param>
        /// <param name="warehouseId"></param>
        /// <param name="purchasingFilialeId"></param>
        /// <param name="purchasingState"></param>
        /// <param name="purchasingType"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="description"></param>
        /// <param name="pmId"></param>
        /// <param name="purchasingToDate"></param>
        /// <param name="nextPurchasingDate"></param>
        /// <param name="isException"> </param>
        /// <param name="personResponsible"> </param>
        /// <returns></returns>
        public static Parameter[] GetInsertPurchasingParms(Guid purchasingId, string purchasingNo, Guid companyId, string companyName, Guid filialeId, Guid warehouseId, Guid purchasingFilialeId, int purchasingState, int purchasingType, DateTime startTime, DateTime endTime, string description, Guid pmId, DateTime purchasingToDate, DateTime nextPurchasingDate, bool isException, Guid personResponsible)
        {
            var parms = new[]
                {
                    new Parameter(QueryString.PARM_PURCHASING_ID, purchasingId),
                    new Parameter(QueryString.PARM_PURCHASING_NO, purchasingNo),
                    new Parameter(QueryString.PARM_COMPANYID, companyId),
                    new Parameter(QueryString.PARM_COMPANY_NAME, companyName),
                    new Parameter(QueryString.PARM_FILIALEID, filialeId),
                    new Parameter(QueryString.PARM_WAREHOUSEID, warehouseId),
                    new Parameter("@PurchasingFilialeId", purchasingFilialeId),
                    new Parameter(QueryString.PARM_PURCHASING_STATE, purchasingState),
                    new Parameter(QueryString.PARM_PURCHASING_TYPE, purchasingType),
                    new Parameter(QueryString.PARM_START_TIME, startTime),
                    new Parameter(QueryString.PARM_END_TIME, endTime),
                    new Parameter(QueryString.PARM_DESCRIPTION, description),
                    new Parameter(QueryString.PARM_PM_ID, pmId),
                    new Parameter(QueryString.PARM_ARRIVAL_DATE, purchasingToDate),
                    new Parameter(QueryString.PARM_NEXT_PURCHASING_DATE, nextPurchasingDate),
                    new Parameter(QueryString.PARM_ISEXCEPTION, isException),
                    new Parameter(QueryString.PARM_PERSONRESPONSIBLE, personResponsible)
                };
            return parms;
        }

        /// <summary>
        /// 插入采购详细参数
        /// </summary>
        /// <param name="purchasingGoodsId"> </param>
        /// <param name="purchasingId"></param>
        /// <param name="goodsId"></param>
        /// <param name="goodsName"></param>
        /// <param name="units"></param>
        /// <param name="goodsCode"></param>
        /// <param name="specification"></param>
        /// <param name="companyId"></param>
        /// <param name="price"></param>
        /// <param name="planQuantity"></param>
        /// <param name="realityQuantity"></param>
        /// <param name="state"></param>
        /// <param name="description"></param>
        /// <param name="dayAvgStocking"></param>
        /// <param name="planStocking"></param>
        /// <param name="sixtyDays"></param>
        /// <param name="isException"> </param>
        /// <param name="sixtyDaySales"> </param>
        /// <param name="thirtyDaySales"> </param>
        /// <param name="elevenDaySales"> </param>
        /// <param name="purchasingGoodsType"> </param>
        /// <returns></returns>
        public static Parameter[] GetInsertPurchasingDetailParms(Guid purchasingGoodsId, Guid purchasingId, Guid goodsId, string goodsName, string units, string goodsCode, string specification, Guid companyId, decimal price, double planQuantity, double realityQuantity, int state, string description, double dayAvgStocking, double planStocking, int sixtyDays, bool isException, int sixtyDaySales, int thirtyDaySales, float elevenDaySales, int purchasingGoodsType)
        {
            return new[]
                {
                    new Parameter(QueryString.PARM_PURCHASING_ID, purchasingId),
                    new Parameter(QueryString.PARM_GOODSID, goodsId),
                    new Parameter(QueryString.PARM_GOODS_NAME, goodsName),
                    new Parameter(QueryString.PARM_UNITS, units),
                    new Parameter(QueryString.PARM_GOODS_CODE, goodsCode),
                    new Parameter(QueryString.PARM_SPECIFICATION, specification),
                    new Parameter(QueryString.PARM_COMPANYID, companyId),
                    new Parameter(QueryString.PARM_PRICE, price),
                    new Parameter(QueryString.PARM_PLAN_QUANTITY, planQuantity),
                    new Parameter(QueryString.PARM_REALITY_QUANTITY, realityQuantity),
                    new Parameter(QueryString.PARM_STATE, state),
                    new Parameter(QueryString.PARM_DESCRIPTION, description),
                    new Parameter(QueryString.PARM_DAY_AVG_STOCKING, dayAvgStocking),
                    new Parameter(QueryString.PARM_PLAN_STOCKING, planStocking),
                    new Parameter(QueryString.PARM_SIXTY_DAYS, sixtyDays),
                    new Parameter(QueryString.PARM_ISEXCEPTION, isException),
                    new Parameter(QueryString.PARM_SIXTYDAYSALES, sixtyDaySales),
                    new Parameter(QueryString.PARM_THIRTYDAYSALES, thirtyDaySales),
                    new Parameter(QueryString.PARM_ELEVENDAYSALES, elevenDaySales),
                    new Parameter("@PurchasingGoodsId", purchasingGoodsId),
                    new Parameter("@PurchasingGoodsType", purchasingGoodsType)
                };
        }
    }
}
