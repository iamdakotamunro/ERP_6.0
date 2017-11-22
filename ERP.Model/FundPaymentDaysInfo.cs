using System;

namespace ERP.Model
{
    /// <summary>
    /// 资金流展示模型  add 20150615 CAA
    /// </summary>
    [Serializable]
    public class FundPaymentDaysInfo
    {
        /// <summary>
        /// 公司ID
        /// </summary>
        public Guid SaleFilialeId { get; set; }
        /// <summary>
        /// 公司Name
        /// </summary>
        public string SaleFilialeName { get; set; }

        /// <summary>
        /// 银行ID
        /// </summary>
        public Guid BankAccountsId { get; set; }

        /// <summary>
        /// 银行Name
        /// </summary>
        public string BankName { get; set; }


        /// <summary>
        /// 搜索年份
        /// </summary>
        public int Year { get; set; }
        #region 月份
        /// <summary>一月
        /// </summary>
        public Decimal Jan { get; set; }

        /// <summary>二月
        /// </summary>
        public Decimal Feb { get; set; }

        /// <summary>三月
        /// </summary>
        public Decimal Mar { get; set; }

        /// <summary>四月
        /// </summary>
        public Decimal Apr { get; set; }

        /// <summary>五月
        /// </summary>
        public Decimal May { get; set; }

        /// <summary>六月
        /// </summary>
        public Decimal Jun { get; set; }

        /// <summary>七月
        /// </summary>
        public Decimal July { get; set; }

        /// <summary>八月
        /// </summary>
        public Decimal Aug { get; set; }

        /// <summary>九月
        /// </summary>
        public Decimal Sept { get; set; }

        /// <summary>十月
        /// </summary>
        public Decimal Oct { get; set; }

        /// <summary>十一月
        /// </summary>
        public Decimal Nov { get; set; }

        /// <summary>十二月
        /// </summary>
        public Decimal December { get; set; }

        #endregion

        #region 末期余额
        /// <summary>一月
        /// </summary>
        public Decimal MaxJan { get; set; }

        /// <summary>二月
        /// </summary>
        public Decimal MaxFeb { get; set; }

        /// <summary>三月
        /// </summary>
        public Decimal MaxMar { get; set; }

        /// <summary>四月
        /// </summary>
        public Decimal MaxApr { get; set; }

        /// <summary>五月
        /// </summary>
        public Decimal MaxMay { get; set; }

        /// <summary>六月
        /// </summary>
        public Decimal MaxJun { get; set; }

        /// <summary>七月
        /// </summary>
        public Decimal MaxJuly { get; set; }

        /// <summary>八月
        /// </summary>
        public Decimal MaxAug { get; set; }

        /// <summary>九月
        /// </summary>
        public Decimal MaxSept { get; set; }

        /// <summary>十月
        /// </summary>
        public Decimal MaxOct { get; set; }

        /// <summary>十一月
        /// </summary>
        public Decimal MaxNov { get; set; }

        /// <summary>十二月
        /// </summary>
        public Decimal MaxDecember { get; set; }
        #endregion
        #region 展示Str
        /// <summary>一月Str
        /// </summary>
        public string MaxJanStr
        {
            get
            {
                var janStr = Jan == 0 ? string.Empty : Jan.ToString("N") + "<br/>";
                return janStr + (MaxJan == 0 ? string.Empty : "[" + MaxJan.ToString("N") + "]");
            }
        }

        /// <summary>二月
        /// </summary>
        public string MaxFebStr
        {
            get
            {
                var febStr = Feb == 0 ? string.Empty : Feb.ToString("N") + "<br/>";
                return febStr + (MaxFeb == 0 ? string.Empty : "[" + MaxFeb.ToString("N") + "]");
            }
        }

        /// <summary>三月
        /// </summary>
        public string MaxMarStr {
            get
            {
                var marStr = Mar == 0 ? string.Empty : Mar.ToString("N") + "<br/>";
                return marStr + (MaxMar == 0 ? string.Empty : "[" + MaxMar.ToString("N") + "]");
            }
        }

        /// <summary>四月
        /// </summary>
        public string MaxAprStr {
            get
            {
                var aprStr = Apr == 0 ? string.Empty : Apr.ToString("N") + "<br/>";
                return aprStr + (MaxApr == 0 ? string.Empty : "[" + MaxApr.ToString("N") + "]");
            }
        }

        /// <summary>五月
        /// </summary>
        public string MaxMayStr {
            get
            {
                var mayStr = May == 0 ? string.Empty : May.ToString("N") + "<br/>";
                return mayStr + (MaxMay == 0 ? string.Empty : "[" + MaxMay.ToString("N") + "]");
            }
        }

        /// <summary>六月
        /// </summary>
        public string MaxJunStr {
            get
            {
                var junStr = Jun == 0 ? string.Empty : Jun.ToString("N") + "<br/>";
                return junStr + (MaxJun == 0 ? string.Empty : "[" + MaxJun.ToString("N") + "]");
            }
        }

        /// <summary>七月
        /// </summary>
        public string MaxJulyStr {
            get
            {
                var julyStr = July == 0 ? string.Empty : July.ToString("N") + "<br/>";
                return julyStr + (MaxJuly == 0 ? string.Empty : "[" + MaxJuly.ToString("N") + "]");
            }
        }

        /// <summary>八月
        /// </summary>
        public string MaxAugStr {
            get
            {
                var augStr = Aug == 0 ? string.Empty : Aug.ToString("N") + "<br/>";
                return augStr + (MaxAug == 0 ? string.Empty : "[" + MaxAug.ToString("N") + "]");
            }
        }

        /// <summary>九月
        /// </summary>
        public string MaxSeptStr {
            get
            {
                var septStr = Sept == 0 ? string.Empty : Sept.ToString("N") + "<br/>";
                return septStr + (MaxSept == 0 ? string.Empty : "[" + MaxSept.ToString("N") + "]");
            }
        }

        /// <summary>十月
        /// </summary>
        public string MaxOctStr {
            get
            {
                var octStr = Oct == 0 ? string.Empty : Oct.ToString("N") + "<br/>";
                return octStr + (MaxOct == 0 ? string.Empty : "[" + MaxOct.ToString("N") + "]");
            }
        }

        /// <summary>十一月
        /// </summary>
        public string MaxNovStr {
            get
            {
                var novStr = Nov == 0 ? string.Empty : Nov.ToString("N") + "<br/>";
                return novStr + (MaxNov == 0 ? string.Empty : "[" + MaxNov.ToString("N") + "]");
            }
        }

        /// <summary>十二月
        /// </summary>
        public string MaxDecemberStr {
            get
            {
                var decemberStr = December == 0 ? string.Empty : December.ToString("N") + "<br/>";
                return decemberStr + (MaxDecember == 0 ? string.Empty : "[" + MaxDecember.ToString("N") + "]");
            }
        }
        #endregion
    }
}
