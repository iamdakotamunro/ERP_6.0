using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using ERP.Enum;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class CompareDataInfo : FetchDataInfo,IComparable
    {
        /// <summary>
        /// 自己网站的名称
        /// </summary>
        public string MyGoodsName { set; get; }

        /// <summary>
        /// 自己网站的价格
        /// </summary>
        public decimal MyPrice { set; get; }

        private decimal _recentInPrice;

        /// <summary>
        /// 最后进货价
        /// </summary>
        public decimal RecentInPrice
        {
            set { _recentInPrice = value; }
            get { return Math.Round(_recentInPrice, 2); }
        }

        /// <summary>
        /// 利率
        /// </summary>
        public string InterestRate
        {
            get
            {
                if (RecentInPrice == 0) return "未进货";
                IList<FetchDataInfo> info = FetchList.Where(o => o.SiteId == (int)AloneFont.Keede).ToList();
                if (info.Count > 0)
                {
                    decimal price = (info[0].GoodsPrice - RecentInPrice) / RecentInPrice * 100;
                    return Math.Round(price, 2) + "%";
                }
                IList<FetchDataInfo> einfo = FetchList.Where(o => o.SiteId == (int)AloneFont.Eyesee).ToList();
                if (einfo.Count > 0)
                {
                    decimal price = (einfo[0].GoodsPrice - RecentInPrice) / RecentInPrice * 100;
                    return Math.Round(price, 2) + "%";
                }
                return "未销售";
            }
        }

        /// <summary>
        /// 匹配的抓取信息
        /// </summary>
        public IList<FetchDataInfo> FetchList { set; get; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
        {
            if (obj is CompareDataInfo)
            {
                if ((obj as CompareDataInfo).MyGoodsName == MyGoodsName && (obj as CompareDataInfo).GoodsId==GoodsId)
                    return true;
            }

            return base.Equals(obj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (MyGoodsName == string.Empty) return base.GetHashCode();
            string stringRepresentation = MethodBase.GetCurrentMethod().DeclaringType.FullName + "#" + GoodsName;
            return stringRepresentation.GetHashCode();
        }

        #region IComparable 成员
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public int CompareTo(object obj)
        {
            if (obj is CompareDataInfo)
            {
                CompareDataInfo compare = obj as CompareDataInfo;

                IList<FetchDataInfo> compList = compare.FetchList.Where(o => o.SiteId != (int)AloneFont.Keede && o.SiteId != (int)AloneFont.Eyesee).ToList();

                IList<FetchDataInfo> myCompList = this.FetchList.Where(o => o.SiteId != (int)AloneFont.Keede && o.SiteId != (int)AloneFont.Eyesee).ToList();

                if (compList.Count == 1 && compList[0].GoodsPrice == 0) return -1;
                if (myCompList.Min(o => o.GoodsPrice) == 0) return 1;

                decimal lowerprice = compList.Min(o => o.GoodsPrice);
                decimal mylowerprice = myCompList.Min(o => o.GoodsPrice);

                IList<FetchDataInfo> pri_comp = compare.FetchList.Where(o => o.SiteId == (int)AloneFont.Keede || o.SiteId == (int)AloneFont.Eyesee).ToList();

                if (pri_comp.Count == 0) return -1; //没有推该商品
                else
                {
                    IList<FetchDataInfo> pri_myComp = this.FetchList.Where(o => o.SiteId == (int)AloneFont.Keede || o.SiteId == (int)AloneFont.Eyesee).ToList();

                    if (pri_myComp.Count == 0) //没有推该商品
                        return 1;
                    else
                    {
                        decimal pri = pri_comp[0].GoodsPrice;
                        decimal myPri = pri_myComp[0].GoodsPrice;
                        if (pri - lowerprice > myPri - mylowerprice) return 1;
                        if (pri - lowerprice < myPri - mylowerprice) return -1;

                        return 0;
                    }
                }
            }

            return -1;
        }

        #endregion
    }
}
