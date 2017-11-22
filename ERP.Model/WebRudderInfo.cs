//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2006年12月22日
// 文件创建人:马力
// 最后修改时间:2007年1月25日
// 最后一次修改人:马力
//================================================

using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 站点基本配置模板类
    /// </summary>
    [Serializable]
    public class WebRudderInfo
    {
        //基本字段
        /// <summary>
        /// 默认构造函数
        /// </summary>
        public WebRudderInfo()
        {
            StockCountMode = 1;
            AuditingTime = 30;
            DefaultLanguage = 1;
            IfValidate = 1;
            ScoreGifts = 0;
            SpecialOfferGifts = 0;
            InvoiceTaxRate = 0;
            IfInvoice = 0;
            SavePriceMode = 1;
            IfMarketPrice = 1;
            CurrencyDecimalDigits = 0;
            CurrencyDecimalType = 1;
        }

        /// <summary>
        /// 带参构造函数
        /// </summary>
        /// <param name="webName">web站点名称</param>
        /// <param name="webHolder">web站点所有者</param>
        /// <param name="webTitle">web站点标题</param>
        /// <param name="webUrl">web站点url地址</param>
        /// <param name="defaultGoodsType">默认商品类型</param>
        /// <param name="defaultScoreId">默认积分ID</param>
        /// <param name="defaultUnitsId">默认计量单位</param>
        /// <param name="defaultCurrencyId">默认货币单位</param>
        /// <param name="currencyDecimalType">货币取整方式</param>
        /// <param name="currencyDecimalDigits">金额小数位</param>
        /// <param name="ifValidate">是否开始会员登录验证</param>
        /// <param name="defaultLanguage">默认语言</param>
        /// <param name="stockCountMode">库存计算模式</param>
        /// <param name="metaDescription">网站描述</param>
        /// <param name="metaKeywords">网站关键字</param>
        /// <param name="isActivity">是否启用促销</param>
        /// <param name="workingDay">工作日</param>
        /// <param name="workingHoursBegin">工作开始时间</param>
        /// <param name="workingHoursEnd">工作结束时间</param>
        /// <param name="ifMarketPrice">是否显示市场价</param>
        /// <param name="savePriceMode">显示商品节省模式</param>
        /// <param name="ifInvoice">是否显示开发票</param>
        /// <param name="invoiceTaxRate">发票税率</param>
        /// <param name="specialOfferGifts">每单可购特价赠品数</param>
        /// <param name="scoreGifts">每单可购积分赠品数</param>
        /// <param name="auditingTime">允许会员修改订单时间</param>
        public WebRudderInfo(string webName, string webHolder, string webTitle, string webUrl, int defaultGoodsType, Guid defaultScoreId, Guid defaultUnitsId, Guid defaultCurrencyId, int currencyDecimalType, int currencyDecimalDigits, int ifMarketPrice, int savePriceMode, int ifInvoice, double invoiceTaxRate, double specialOfferGifts, double scoreGifts, int ifValidate, int defaultLanguage, int auditingTime, int stockCountMode, string metaDescription, string metaKeywords, int isActivity, string workingDay, DateTime workingHoursBegin, DateTime workingHoursEnd)
        {
            WebName = webName;
            WebHolder = webHolder;
            WebTitle = webTitle;
            WebUrl = webUrl;
            DefaultGoodsType = defaultGoodsType;
            DefaultScoreId = defaultScoreId;
            DefaultUnitsId = defaultUnitsId;
            DefaultCurrencyId = defaultCurrencyId;
            CurrencyDecimalType = currencyDecimalType;
            CurrencyDecimalDigits = currencyDecimalDigits;
            IfMarketPrice = ifMarketPrice;
            SavePriceMode = savePriceMode;
            IfInvoice = ifInvoice;
            InvoiceTaxRate = invoiceTaxRate;
            SpecialOfferGifts = specialOfferGifts;
            ScoreGifts = scoreGifts;
            IfValidate = ifValidate;
            DefaultLanguage = defaultLanguage;
            AuditingTime = auditingTime;
            StockCountMode = stockCountMode;
            METADescription = metaDescription;
            METAKeywords = metaKeywords;
            IsActivity = isActivity;
            WorkingDay = workingDay;
            WorkingHoursBegin = workingHoursBegin;
            WorkingHoursEnd = workingHoursEnd;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="webName">web站点名称</param>
        /// <param name="webHolder">web站点所有者</param>
        /// <param name="webTitle">web站点标题</param>
        /// <param name="webUrl">web站点url地址</param>
        /// <param name="defaultGoodsType">默认商品类型</param>
        /// <param name="defaultUnitsId">默认计量单位</param>
        /// <param name="defaultCurrencyId">默认货币单位</param>
        /// <param name="currencyDecimalType">货币取整方式</param>
        /// <param name="currencyDecimalDigits">金额小数位</param>
        /// <param name="defaultLanguage">默认语言</param>
        /// <param name="metaDescription">网站描述</param>
        /// <param name="metaKeywords">网站关键字</param>
        /// <param name="workingDay">工作日</param>
        /// <param name="workingHoursBegin">工作开始时间</param>
        /// <param name="workingHoursEnd">工作结束时间</param>
        public WebRudderInfo(string webName, string webHolder, string webTitle, string webUrl, int defaultGoodsType, Guid defaultUnitsId, Guid defaultCurrencyId, int currencyDecimalType, int currencyDecimalDigits, int defaultLanguage, string metaDescription, string metaKeywords, string workingDay, DateTime workingHoursBegin, DateTime workingHoursEnd)
        {
            WebName = webName;
            WebHolder = webHolder;
            WebTitle = webTitle;
            WebUrl = webUrl;
            DefaultGoodsType = defaultGoodsType;
            DefaultScoreId = DefaultScoreId;
            DefaultUnitsId = defaultUnitsId;
            DefaultCurrencyId = defaultCurrencyId;
            CurrencyDecimalType = currencyDecimalType;
            CurrencyDecimalDigits = currencyDecimalDigits;
            IfMarketPrice = IfMarketPrice;
            SavePriceMode = SavePriceMode;
            IfInvoice = IfInvoice;
            InvoiceTaxRate = InvoiceTaxRate;
            SpecialOfferGifts = SpecialOfferGifts;
            ScoreGifts = ScoreGifts;
            IfValidate = IfValidate;
            DefaultLanguage = defaultLanguage;
            AuditingTime = AuditingTime;
            StockCountMode = StockCountMode;
            METADescription = metaDescription;
            METAKeywords = metaKeywords;
            IsActivity = IsActivity;
            WorkingDay = workingDay;
            WorkingHoursBegin = workingHoursBegin;
            WorkingHoursEnd = workingHoursEnd;
        }

        //基本属性
        /// <summary>
        /// web站点名称
        /// </summary>
        public string WebName { get; set; }

        /// <summary>
        /// web站点所有者
        /// </summary>
        public string WebHolder { get; set; }

        /// <summary>
        /// web站点标题
        /// </summary>
        public string WebTitle { get; set; }

        /// <summary>
        /// web站点url地址
        /// </summary>
        public string WebUrl { get; set; }

        /// <summary>
        /// 默认商品类型
        /// </summary>
        public int DefaultGoodsType { get; set; }

        /// <summary>
        /// 默认积分ID
        /// </summary>
        public Guid DefaultScoreId { get; set; }

        /// <summary>
        /// 默认计量单位ID
        /// </summary>
        public Guid DefaultUnitsId { get; set; }

        /// <summary>
        /// 默认货币单位ID
        /// </summary>
        public Guid DefaultCurrencyId { get; set; }

        /// <summary>
        /// 金额取整方式
        /// </summary>
        public int CurrencyDecimalType { get; set; }

        /// <summary>
        /// 金额小数位
        /// </summary>
        public int CurrencyDecimalDigits { get; set; }

        /// <summary>
        /// 是否显示市场价格
        /// </summary>
        public int IfMarketPrice { get; set; }

        /// <summary>
        /// 显示商品节省模式
        /// </summary>
        public int SavePriceMode { get; set; }

        /// <summary>
        /// 显示开发票
        /// </summary>
        public int IfInvoice { get; set; }

        /// <summary>
        /// 发票税率
        /// </summary>
        public double InvoiceTaxRate { get; set; }

        /// <summary>
        /// 每单可购特价赠品数
        /// </summary>
        public double SpecialOfferGifts { get; set; }

        /// <summary>
        /// 每单可购积分赠品数
        /// </summary>
        public double ScoreGifts { get; set; }

        /// <summary>
        /// 是否开启会员登录验证码
        /// </summary>
        public int IfValidate { get; set; }

        /// <summary>
        /// 员工默认语种
        /// </summary>
        public int DefaultLanguage { get; set; }

        /// <summary>
        /// 允许会员修改订单时间
        /// </summary>
        public int AuditingTime { get; set; }

        /// <summary>
        /// 库存计算模式
        /// </summary>
        public int StockCountMode { get; set; }

        /// <summary>
        /// 网站描述
        /// </summary>
        public string METADescription { get; set; }

        /// <summary>
        /// 网站关键字
        /// </summary>
        public string METAKeywords { get; set; }

        /// <summary>
        /// 是否启用促销
        /// </summary>
        public int IsActivity { get; set; }

        /// <summary>
        /// 工作日
        /// </summary>
        public string WorkingDay { get; set; }

        /// <summary>
        /// 工作开始时间
        /// </summary>
        public DateTime WorkingHoursBegin { get; set; }

        /// <summary>
        /// 工作结束时间
        /// </summary>
        public DateTime WorkingHoursEnd { get; set; }
    }
}
