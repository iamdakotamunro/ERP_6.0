using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Basis;
using ERP.BLL.Interface;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.SAL;
using ERP.SAL.WMS;
using Keede.Ecsoft.Model;
using MIS.Enum;
using OperationLog.Core.Attributes;
using Telerik.Web.UI;

namespace ERP.UI.Web.Common
{
    /// <summary> WebControl  优化注释显示，去除无用方法   2015-04-27   陈重文
    /// </summary>
    [Serializable]
    public static class WebControl
    {
        /// <summary>
        /// </summary>
        public static DateTime MinDateTime
        {
            get { return DateTime.Parse("1900.1.1"); }
        }

        private static readonly IOperationLogManager _operationLogManager = new OperationLogManager();
        private static readonly ICost _costDao = new Cost(GlobalConfig.DB.FromType.Read);

        /// <summary>获取系统当前时间
        /// </summary>
        /// <returns></returns>
        public static DateTime GetNowTime()
        {
            return DateTime.Now;
        }

        /// <summary>获取往来单位分类
        /// </summary>
        /// <param name="companyClassId"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static IList<CompanyClassInfo> RecursionCompanyClass(Guid companyClassId, int depth)
        {
            depth++;
            var companyClass = new CompanyClass(GlobalConfig.DB.FromType.Read);
            IList<CompanyClassInfo> companyClassTree = new List<CompanyClassInfo>();

            IList<CompanyClassInfo> companyClassList = companyClass.GetChildCompanyClassList(companyClassId);
            string tag = depth == 1 ? "+" : "|" + new String('-', depth * 2);
            foreach (CompanyClassInfo companyClassInfo in companyClassList)
            {
                companyClassInfo.CompanyClassName = tag + companyClassInfo.CompanyClassName;
                companyClassTree.Add(companyClassInfo);
                foreach (
                    CompanyClassInfo childCompanyClassInfo in
                        RecursionCompanyClass(companyClassInfo.CompanyClassId, depth))
                {
                    companyClassTree.Add(childCompanyClassInfo);
                }
            }
            return companyClassTree;
        }

        /// <summary>获取费用单位分类
        /// </summary>
        /// <param name="companyClassId"></param>
        /// <param name="depth"></param>
        /// <returns></returns>
        public static IList<CostCompanyClassInfo> RecursionCostClass(Guid companyClassId, int depth)
        {
            depth++;
            IList<CostCompanyClassInfo> companyClassTree = new List<CostCompanyClassInfo>();
            IList<CostCompanyClassInfo> companyClassList = _costDao.GetChildCompanyClassList(companyClassId);
            string tag = depth == 1 ? "+" : "|" + new String('-', depth * 2);
            foreach (CostCompanyClassInfo companyClassInfo in companyClassList)
            {
                companyClassInfo.CompanyClassName = tag + companyClassInfo.CompanyClassName;
                companyClassTree.Add(companyClassInfo);
                foreach (
                    CostCompanyClassInfo childCompanyClassInfo in
                        RecursionCostClass(companyClassInfo.CompanyClassId, depth))
                {
                    companyClassTree.Add(childCompanyClassInfo);
                }
            }
            return companyClassTree;
        }

        /// <summary>根据商品销售类型返回具体文字描述
        /// </summary>
        /// <param name="sellType"></param>
        /// <returns></returns>
        public static string GetUnitsBySellType(int sellType)
        {
            string units = string.Empty;
            switch (int.Parse(string.Format("{0}", sellType)))
            {
                case -1:
                    units = "元";
                    break;
                case 0:
                    units = "会员积分";
                    break;
                case 1:
                    units = "积分+货币";
                    break;
            }
            return units;
        }

        /// <summary> 获取指定类型币种的货币总额
        /// </summary>
        /// <param name="orderId">订单编号</param>
        /// <param name="currencyType">币种类型1为货币，2为积分</param>
        /// <returns></returns>
        public static decimal GetTotalOrderCurrency(Guid orderId, int currencyType)
        {
            decimal retValue = 0;
            var goodsOrderDetail = new GoodsOrderDetail(GlobalConfig.DB.FromType.Read);
            var goodsOrderDetailList = goodsOrderDetail.GetGoodsOrderDetailList(orderId);
            if (currencyType == 1)
            {
                retValue +=
                    goodsOrderDetailList.Where(w => w.SellType == (int)SellType.Currency)
                        .Sum(god => god.SellPrice * Convert.ToDecimal(god.Quantity));
            }
            else
            {
                retValue +=
                    goodsOrderDetailList.Where(w => w.SellType == (int)SellType.Score).Sum(god => god.TotalGoodsScore);
            }
            return retValue;
        }

        /// <summary>网站金额取整方式
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static decimal CurrencyValue(decimal value)
        {
            return WebRudder.ReadInstance.CurrencyValue(value);
        }

        /// <summary>检查请求URL某值是否为GUID并返回
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Guid GetGuidFromQueryString(string key)
        {
            var retValue = Guid.Empty;
            if (HttpContext.Current == null)
                return retValue;
            var qsValue = HttpContext.Current.Request.QueryString[key];
            if (!string.IsNullOrEmpty(qsValue))
                retValue = new Guid(qsValue);
            return retValue;
        }

        /// <summary>获取当前操作页面的名称
        /// </summary>
        public static string FileName
        {
            get
            {
                return HttpContext.Current != null ? Path.GetFileName(HttpContext.Current.Request.PhysicalPath) : null;
            }
        }

        /// <summary>AjajxRequest
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="e"></param>
        public static void RamAjajxRequest(RadGrid obj, AjaxRequestEventArgs e)
        {
            switch (e.Argument)
            {
                case "Rebind":
                    obj.MasterTableView.SortExpressions.Clear();
                    obj.MasterTableView.GroupByExpressions.Clear();
                    obj.Rebind();
                    break;
                case "RebindAndNavigate":
                    obj.MasterTableView.SortExpressions.Clear();
                    obj.MasterTableView.GroupByExpressions.Clear();
                    obj.MasterTableView.CurrentPageIndex = obj.MasterTableView.PageCount - 1;
                    obj.Rebind();
                    break;
            }
        }

        /// <summary>根据仓库类型返回财务编码类型
        /// </summary>
        /// <param name="storageRecordType"></param>
        /// <returns></returns>
        public static CodeType GetCodeTypeFromStockType(StorageRecordType storageRecordType)
        {
            if (storageRecordType == StorageRecordType.BuyStockIn)
                return CodeType.RK;
            if (storageRecordType == StorageRecordType.SellReturnIn)
                return CodeType.SI;
            if (storageRecordType == StorageRecordType.BorrowIn)
                return CodeType.BI;
            if (storageRecordType == StorageRecordType.LendIn)
                return CodeType.LI;
            if (storageRecordType == StorageRecordType.BuyStockOut)
                return CodeType.SO;
            if (storageRecordType == StorageRecordType.AfterSaleOut)
                return CodeType.SO;
            if (storageRecordType == StorageRecordType.SellStockOut)
                return CodeType.SL;
            if (storageRecordType == StorageRecordType.LendOut)
                return CodeType.LO;
            if (storageRecordType == StorageRecordType.BorrowOut)
                return CodeType.BO;
            return CodeType.RT;
        }

        /// <summary>获取当前用户和操作时间
        /// </summary>
        /// <param name="descrion">描述</param>
        /// < returns></returns>
        public static string RetrunUserAndTime(string descrion)
        {
            return char.ConvertFromUtf32(10) + descrion + " [" + CurrentSession.Personnel.Get().RealName + ": " +
                   DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "]";
        }

        /// <summary> 获取当前用户和操作时间
        /// </summary>
        /// <param name="descrion">描述</param>
        /// < returns></returns>
        public static string GetUserAndTime(string descrion)
        {
            return "[" + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + ":" + CurrentSession.Personnel.Get().RealName +
                   "]" + descrion;
        }

        /// <summary> 统一登录获取权限点 Add by liucaijun at 2012-11-26
        /// </summary>
        /// <param name="url"></param>
        /// <param name="pointCode"></param>
        /// <returns></returns>
        public static bool GetPowerOperationPoint(string url, string pointCode)
        {
            return PermissionSao.VerifyIsAllowPageOperation(CurrentSession.Personnel.Get().PersonnelId,
                CurrentSession.System.ID, url, pointCode);
        }

        /// <summary> 添加操作记录
        /// </summary>
        /// <param name="personnelId">操作人Id</param>
        /// <param name="realName">操作人</param>
        /// <param name="identiykey">关键字</param>
        /// <param name="identifycode"></param>
        /// <param name="state">操作点</param>
        /// <param name="newClew">备注</param>
        public static bool AddOperationLog(Guid personnelId, string realName, Guid identiykey, string identifycode,
            EnumPointAttribute state, string newClew)
        {
            return _operationLogManager.Add(personnelId, realName, identiykey, identifycode, state, 1, newClew);
        }

        /// <summary> 根据公司ID获取门店总公司
        /// </summary>
        /// <param name="filialeId"></param>
        /// <returns></returns>
        public static FilialeInfo GetShopHeadFilialeInfo(Guid filialeId)
        {
            var filialeInfo = CacheCollection.Filiale.Get(filialeId);
            FilialeInfo partialFilialeInfo;
            var headFilialeInfo = new FilialeInfo();

            if (filialeInfo.Rank == (int)FilialeRank.Child && filialeInfo.ParentId != Guid.Empty)
            {
                //分公司找上级旗舰店
                partialFilialeInfo = CacheCollection.Filiale.Get(filialeInfo.ParentId);
                if (partialFilialeInfo.Rank == (int)FilialeRank.Partial && partialFilialeInfo.ParentId != Guid.Empty)
                {
                    //旗舰店找上级门店公司
                    headFilialeInfo = CacheCollection.Filiale.Get(partialFilialeInfo.ParentId);
                }
            }
            else if (filialeInfo.Rank == (int)FilialeRank.Partial && filialeInfo.ParentId != Guid.Empty)
            {
                //旗舰店找上级门店公司
                partialFilialeInfo = filialeInfo;
                headFilialeInfo = CacheCollection.Filiale.Get(partialFilialeInfo.ParentId);
            }
            else if (filialeInfo.Rank == (int)FilialeRank.Head)
            {
                headFilialeInfo = filialeInfo;
            }
            return headFilialeInfo;
        }

        /// <summary>笛卡尔乘积
        /// </summary>
        public static List<List<T>> CartesianProduct<T>(List<List<T>> lstSplit)
        {
            int count = 1;
            lstSplit.ForEach(item => count *= item.Count);
            var lstResult = new List<List<T>>();
            for (int i = 0; i < count; ++i)
            {
                var lstTemp = new List<T>();
                int j = 1;
                lstSplit.ForEach(item =>
                {
                    j *= item.Count;
                    lstTemp.Add(item[(i / (count / j)) % item.Count]);
                });
                lstResult.Add(lstTemp);
            }
            return lstResult;
        }

        /// <summary>计算文本长度，区分中英文字符，中文算两个长度，英文算一个长度
        /// </summary>
        /// <param name="text">需计算长度的字符串</param>
        /// <returns>int</returns>
        public static int GetTextLength(string text)
        {
            int len = 0;
            for (int i = 0; i < text.Length; i++)
            {
                byte[] byteLen = Encoding.UTF8.GetBytes(text.Substring(i, 1));
                if (byteLen.Length > 1)
                    len += 2; //如果长度大于1，是中文，占两个字节，+2
                else
                    len += 1; //如果长度等于1，是英文，占一个字节，+1
            }
            return len;
        }

        #region  数字用逗号分隔  add by liangcanren at 2015-04-15

        /// <summary> 将数字用逗号分隔
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string NumberSeparator(object amount)
        {
            if (Convert.ToDecimal(amount) == 0) return "0.00";
            //设置在数值中对小数点左边数字进行分组的字符串为“”，默认为“,”
            NumberFormatInfo nfi = new CultureInfo("zh-CN", false).NumberFormat;
            nfi.NumberGroupSeparator = ",";
            return string.Format(nfi, "{0:n}", amount);
        }

        /// <summary>
        /// 将分隔数字还原
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public static string NumberRecovery(string amount)
        {
            var splits = amount.Split(',');
            var builder = new StringBuilder();
            foreach (var split in splits)
            {
                if (!string.IsNullOrEmpty(split))
                {
                    builder.Append(split);
                }
            }
            return string.Format("{0}", builder);
        }
        #endregion

        /// <summary>
        /// 移除小数末尾后多余的零,并格式化数字
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        /// zal 2015-07-31
        public static string RemoveDecimalEndZero(decimal num)
        {
            #region 判断num是否小于0
            string number; var flag = false;
            if (num < 0)
            {
                flag = true;
                number = num.ToString(CultureInfo.InvariantCulture).Substring(1);
            }
            else
            {
                number = num.ToString(CultureInfo.InvariantCulture);
            }
            #endregion

            #region 移除小数末尾后多余的零
            var index = number.IndexOf('.');
            if (index > -1)
            {
                string str = number.Substring(index, number.Length - index);
                for (var i = str.Length - 1; i >= 2; i--)
                {
                    if (string.Format("{0}", str[i]) != "0" || i == 2)
                    {
                        number = number.Substring(0, index + i + 1);
                        break;
                    }
                }
            }
            #endregion

            #region 格式化数字 如：2,500,000.0000
            string startNum, endNum = string.Empty;
            if (index > -1)
            {
                startNum = number.Substring(0, index);
                endNum = number.Substring(index, number.Length - index);
            }
            else
            {
                startNum = number;
            }

            if (startNum.Length <= 3) return flag ? "-" + number : number;
            var temp = string.Empty;
            var j = 0;
            for (var i = startNum.Length - 1; i >= 0; i--)
            {
                j++;
                temp = temp + startNum[i] + (j % 3 == 0 ? "," : "");
            }
            startNum = startNum.Length % 3 == 0 ? temp.Substring(0, temp.Length - 1) : temp;
            temp = string.Empty;
            for (var i = startNum.Length - 1; i >= 0; i--)
            {
                temp = temp + startNum[i];
            }
            number = temp + endNum;
            #endregion

            return flag ? "-" + number : number;
        }

        /// <summary>
        /// 将数字转换成中文
        /// </summary>
        /// <param name="num"></param>
        /// <returns></returns>
        /// zal 2016-12-05
        public static string ConvertToChnName(decimal num)
        {
            string stringChnNames = "零一二三四五六七八九点";
            string stringNumNames = "0123456789.";
            return num.ToString(CultureInfo.InvariantCulture).Aggregate(string.Empty, (current, item) => current + stringChnNames[stringNumNames.IndexOf(item)]);
        }

        /// <summary>
        /// 判断手续费金额是否正确
        /// </summary>
        /// <param name="sum"></param>
        /// <param name="poundage"></param>
        /// <returns></returns>
        public static bool CheckPoundage(decimal sum, decimal poundage)
        {
            if ((poundage <= 6) || (poundage / sum <= (decimal)0.02))
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 获取用户是否有销售出库价格审核的修改权限
        /// </summary>
        public static bool GetIsPermission()
        {
            return WebControl.GetPowerOperationPoint("StorageRecordApplyOut.aspx", "SettlementPrice");
        }

        #region 仓库相关

        public static Dictionary<Guid, HostingFilialeAuth> GetHostingFilialeAuth(WarehouseAuth warehouseAuth)
        {

            Dictionary<Guid,HostingFilialeAuth> hostiingFilialeAuthDics=new Dictionary<Guid, HostingFilialeAuth>();
            if (warehouseAuth.Storages!=null)
            {
                foreach (var item in warehouseAuth.Storages.Where(ent=>ent.Filiales!=null))
                {
                    foreach (var filialeAuth in item.Filiales)
                    {
                        if(!hostiingFilialeAuthDics.ContainsKey(filialeAuth.HostingFilialeId))
                            hostiingFilialeAuthDics.Add(filialeAuth.HostingFilialeId,filialeAuth);
                    }
                }                
            }
            return hostiingFilialeAuthDics;
        } 
        #endregion
    }
}

