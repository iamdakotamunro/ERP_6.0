//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2007年9月11日
// 文件创建人:马力
// 最后修改时间:2007年9月11日
// 最后一次修改人:马力
//================================================
using System;
using System.Runtime.Serialization;
namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 会员联系地址
    /// </summary>
    [Serializable]
    [DataContract]
    public class ConsigneeInfo
    {
        /// <summary>
        /// 收货人编号
        /// </summary>
        [DataMember]
        public Guid ConsigneeId { get; set; }

        /// <summary>
        /// 客户编号
        /// </summary>
        [DataMember]
        public Guid MemberId { get; set; }

        /// <summary>
        /// 收货人
        /// </summary>
        [DataMember]
        public string Consignee { get; set; }

        /// <summary>
        /// 国家编号
        /// </summary>
        [DataMember]
        public Guid CountryId { get; set; }

        /// <summary>
        /// 省/市 编号
        /// </summary>
        [DataMember]
        public Guid ProvinceId { get; set; }

        /// <summary>
        /// 市区县 编号
        /// </summary>
        [DataMember]
        public Guid CityId { get; set; }

        /// <summary>
        /// 区县编号
        /// </summary>
        [DataMember]
        public Guid DistrictID { get; set; }

        /// <summary>
        /// 收货地址
        /// </summary>
        [DataMember]
        public string Address { get; set; }

        /// <summary>
        /// 邮政编码
        /// </summary>
        [DataMember]
        public string PostalCode { get; set; }

        /// <summary>
        /// 手机号码
        /// </summary>
        [DataMember]
        public string Mobile { get; set; }

        /// <summary>
        /// 电话号码
        /// </summary>
        [DataMember]
        public string Phone { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        [DataMember]
        public int PayMode { get; set; }

        /// <summary>
        /// 支付类型
        /// </summary>
        [DataMember]
        public int PaymentType { get; set; }

        /// <summary>
        /// 银行帐号编号
        /// </summary>
        [DataMember]
        public Guid BankAccountsId { get; set; }

        /// <summary>
        /// 退款类型0.直接退到keede账户1.退款到银行（原银行帐号）
        /// </summary>
        [DataMember]
        public int RefundmentMode { get; set; }

        /// <summary>
        /// 注册时间
        /// </summary>
        [DataMember]
        public DateTime RegTime { get; set; }

        /// <summary>
        /// 是否默认。每次最后一次收货信息为默认。
        /// </summary>
        [DataMember]
        public int IsDefault { get; set; }

        /// <summary>
        /// 快递ID
        /// </summary>
        [DataMember]
        public Guid ExpressID { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public ConsigneeInfo()
        {
            IsDefault = 0;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="consigneeId">收货人编号</param>
        /// <param name="memberId">客户编号</param>
        /// <param name="consignee">收货人</param>
        /// <param name="countryId">国家编号</param>
        /// <param name="provinceId">省/市 编号</param>
        /// <param name="cityId">市区县 编号</param>
        /// <param name="address">收货地址</param>
        /// <param name="postalCode">邮政编码</param>
        /// <param name="mobile">手机号码</param>
        /// <param name="phone">电话号码</param>
        /// <param name="payMode">支付方式</param>
        /// <param name="paymentType">支付类型</param>
        /// <param name="bankAccountsId">银行帐号编号</param>
        /// <param name="refundmentMode">退款类型0.直接退到keede账户1.退款到银行（原银行帐号）</param>
        /// <param name="regTime">注册时间</param>
        /// <param name="isDefault">是否默认。每次最后一次收货信息为默认</param>
        /// <param name="expressId">快递ID</param>
        public ConsigneeInfo(Guid consigneeId, Guid memberId, string consignee, Guid countryId, Guid provinceId, Guid cityId, string address, string postalCode, string mobile, string phone, int payMode, int paymentType, Guid bankAccountsId, int refundmentMode, DateTime regTime, int isDefault, Guid expressId)
        {
            ConsigneeId = consigneeId;
            MemberId = memberId;
            Consignee = consignee;
            CountryId = countryId;
            ProvinceId = provinceId;
            CityId = cityId;
            Address = address;
            PostalCode = postalCode;
            Mobile = mobile;
            Phone = phone;
            PayMode = payMode;
            PaymentType = paymentType;
            BankAccountsId = bankAccountsId;
            RefundmentMode = refundmentMode;
            RegTime = regTime;
            IsDefault = isDefault;
            ExpressID = expressId;
        }
    }
}
