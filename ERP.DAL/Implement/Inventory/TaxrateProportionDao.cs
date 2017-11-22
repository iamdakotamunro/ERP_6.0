using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    public class TaxrateProportionDao : ITaxrateProportion
    {
        readonly GlobalConfig.DB.FromType _fromType;
        public TaxrateProportionDao(GlobalConfig.DB.FromType fromType= GlobalConfig.DB.FromType.Write)
        {
            _fromType = fromType;
        }

        /// <summary>
        /// 新增税率记录
        /// </summary>
        /// <param name="info"></param>
        /// <returns></returns>
        public bool Insert(TaxrateProportionInfo info)
        {
            const string SQL = @"INSERT INTO TaxrateProportion(Id, GoodsType,GoodsTypeCode, Percentage, UpdateDate,Operator,OperateType, Remark) VALUES(@Id, @GoodsType,@GoodsTypeCode, @Percentage, @UpdateDate,@Operator,@OperateType, @Remark)";
            using (var db = DatabaseFactory.Create())
            {
                return db.Execute(false,SQL,new []
                {
                    new Parameter("@Id",info.Id),
                    new Parameter("@GoodsType",info.GoodsType),
                    new Parameter("@GoodsTypeCode",info.GoodsTypeCode),
                    new Parameter("@Percentage",info.Percentage),
                    new Parameter("@UpdateDate",info.UpdateDate),
                    new Parameter("@Operator",info.Operator),
                    new Parameter("@OperateType",info.OperateType),
                    new Parameter("@Remark",info.Remark)
                });
            }
        }

        /// <summary>
        /// 获取当前包含的税率比例
        /// </summary>
        /// <returns></returns>
        public List<decimal> AllPercentage()
        {
            const string SQL = @"SELECT Percentage FROM TaxrateProportion GROUP BY Percentage";
            using (var db = DatabaseFactory.Create())
            {
                var result = db.GetValues<decimal>(true,SQL);
                return result?.ToList() ?? new List<decimal>();
            }
        }

        /// <summary>
        /// 获取商品类型税率设置记录
        /// </summary>
        /// <param name="goodsType"></param>
        /// <returns></returns>
        public List<TaxrateProportionInfo> GetTaxrateProportionInfos(int goodsType)
        {
            const string SQL = @"select Id, GoodsType,GoodsTypeCode, Percentage, UpdateDate,Operator,OperateType, Remark from TaxrateProportion WITH(NOLOCK) where GoodsType=@GoodsType order by UpdateDate desc";
            using (var db = DatabaseFactory.Create())
            {
                var result= db.Select<TaxrateProportionInfo>(true, SQL, new Parameter("@GoodsType", goodsType));
                return result?.ToList() ?? new List<TaxrateProportionInfo>();
            }
        }

        public List<TaxrateProportionInfo> GetNewPercentages(IEnumerable<int> goodsTypes)
        {
            string typeStr = goodsTypes!=null && goodsTypes.Any() ? string.Format(" WHERE GoodsType IN({0})",string.Join(",", goodsTypes)) : "";
            string SQL = @"SELECT P.Id, P.GoodsType,P.GoodsTypeCode, P.Percentage, P.UpdateDate,P.Operator,P.OperateType, P.Remark FROM TaxrateProportion AS P WITH(NOLOCK)
INNER JOIN (
	select GoodsType,MAX(UpdateDate) AS UpdateDate from TaxrateProportion WITH(NOLOCK) {0} group by GoodsType
) AS T
ON P.GoodsType=T.GoodsType AND P.UpdateDate=T.UpdateDate";
            using (var db = DatabaseFactory.Create())
            {
                var result = db.Select<TaxrateProportionInfo>(true, string.Format(SQL, typeStr));
                return result?.ToList() ?? new List<TaxrateProportionInfo>();
            }
        } 

        /// <summary>
        /// 获取商品类型当前的税率
        /// </summary>
        /// <param name="goodsType"></param>
        /// <returns></returns>
        public decimal GetLastPercentage(int goodsType)
        {
            const string SQL = @"select top 1 Percentage from TaxrateProportion WITH(NOLOCK) where GoodsType=@GoodsType order by UpdateDate desc";
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<decimal?>(true, SQL, new Parameter("@GoodsType",goodsType))??-1;
            }
        }
    }
}
