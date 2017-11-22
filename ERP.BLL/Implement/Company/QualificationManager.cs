using System.Collections.Generic;
using System.Linq;
using ERP.Enum;
using System;
using ERP.Model;

namespace ERP.BLL.Implement.Company
{
    public class QualificationManager
    {

        /// <summary>
        /// 判断供应商资质是否完整
        /// </summary>
        /// <param name="dataList"></param>
        /// <returns></returns>
        public int IsComplete(IList<SupplierInformationInfo> dataList)
        {
            if (dataList == null || dataList.Count == 0) return (int)SupplierCompleteType.NoComplete;
            var datetime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            var info =dataList.Any(act => act.QualificationType == (int) SupplierQualificationType.BusinessLicense && act.OverdueDate >= datetime);
            if (!info) return (int)SupplierCompleteType.NoComplete;
            var info1 = dataList.Any(
                    act => act.QualificationType == (int)SupplierQualificationType.OrganizationCodeCertificate && act.OverdueDate >= datetime);
            if (!info1 ) return (int)SupplierCompleteType.NoComplete;
            var info2 =dataList.Any(
                    act => act.QualificationType == (int)SupplierQualificationType.TaxRegistrationCertificate && act.OverdueDate >= datetime);
            return !info2  ? (int)SupplierCompleteType.NoComplete : (int)SupplierCompleteType.Complete;
        }

        /// <summary>
        /// 判断资质列表是否过期
        /// </summary>
        /// <param name="dataList"></param>
        /// <param name="days">过期时间 取配置</param>
        /// <returns></returns>
        public string IsExpire(IList<SupplierInformationInfo> dataList,int days)
        {
            if (dataList == null || dataList.Count == 0) return "0";
            string str = string.Empty;
            var now = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
            foreach (var info in dataList)
            {
                string type;
                if (info.OverdueDate < now)
                    type = string.Format("{0}", (int) SupplierTimeLimitType.Expired);
                else
                {
                    type = now.AddDays(days) >= Convert.ToDateTime(info.OverdueDate) 
                        ? string.Format("{0}", (int)SupplierTimeLimitType.Expire) 
                        : string.Format("{0}", (int)SupplierTimeLimitType.Normal);
                }
                if (str.Length>0 && str.Contains(type))
                {
                    continue;
                }
                str += str.Length == 0 ? type : string.Format(",{0}", type);
            }
            return str;
        }
    }
}
