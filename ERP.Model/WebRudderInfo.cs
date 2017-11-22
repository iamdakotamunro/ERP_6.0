//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2006��12��22��
// �ļ�������:����
// ����޸�ʱ��:2007��1��25��
// ���һ���޸���:����
//================================================

using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// վ���������ģ����
    /// </summary>
    [Serializable]
    public class WebRudderInfo
    {
        //�����ֶ�
        /// <summary>
        /// Ĭ�Ϲ��캯��
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
        /// ���ι��캯��
        /// </summary>
        /// <param name="webName">webվ������</param>
        /// <param name="webHolder">webվ��������</param>
        /// <param name="webTitle">webվ�����</param>
        /// <param name="webUrl">webվ��url��ַ</param>
        /// <param name="defaultGoodsType">Ĭ����Ʒ����</param>
        /// <param name="defaultScoreId">Ĭ�ϻ���ID</param>
        /// <param name="defaultUnitsId">Ĭ�ϼ�����λ</param>
        /// <param name="defaultCurrencyId">Ĭ�ϻ��ҵ�λ</param>
        /// <param name="currencyDecimalType">����ȡ����ʽ</param>
        /// <param name="currencyDecimalDigits">���С��λ</param>
        /// <param name="ifValidate">�Ƿ�ʼ��Ա��¼��֤</param>
        /// <param name="defaultLanguage">Ĭ������</param>
        /// <param name="stockCountMode">������ģʽ</param>
        /// <param name="metaDescription">��վ����</param>
        /// <param name="metaKeywords">��վ�ؼ���</param>
        /// <param name="isActivity">�Ƿ����ô���</param>
        /// <param name="workingDay">������</param>
        /// <param name="workingHoursBegin">������ʼʱ��</param>
        /// <param name="workingHoursEnd">��������ʱ��</param>
        /// <param name="ifMarketPrice">�Ƿ���ʾ�г���</param>
        /// <param name="savePriceMode">��ʾ��Ʒ��ʡģʽ</param>
        /// <param name="ifInvoice">�Ƿ���ʾ����Ʊ</param>
        /// <param name="invoiceTaxRate">��Ʊ˰��</param>
        /// <param name="specialOfferGifts">ÿ���ɹ��ؼ���Ʒ��</param>
        /// <param name="scoreGifts">ÿ���ɹ�������Ʒ��</param>
        /// <param name="auditingTime">�����Ա�޸Ķ���ʱ��</param>
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
        /// <param name="webName">webվ������</param>
        /// <param name="webHolder">webվ��������</param>
        /// <param name="webTitle">webվ�����</param>
        /// <param name="webUrl">webվ��url��ַ</param>
        /// <param name="defaultGoodsType">Ĭ����Ʒ����</param>
        /// <param name="defaultUnitsId">Ĭ�ϼ�����λ</param>
        /// <param name="defaultCurrencyId">Ĭ�ϻ��ҵ�λ</param>
        /// <param name="currencyDecimalType">����ȡ����ʽ</param>
        /// <param name="currencyDecimalDigits">���С��λ</param>
        /// <param name="defaultLanguage">Ĭ������</param>
        /// <param name="metaDescription">��վ����</param>
        /// <param name="metaKeywords">��վ�ؼ���</param>
        /// <param name="workingDay">������</param>
        /// <param name="workingHoursBegin">������ʼʱ��</param>
        /// <param name="workingHoursEnd">��������ʱ��</param>
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

        //��������
        /// <summary>
        /// webվ������
        /// </summary>
        public string WebName { get; set; }

        /// <summary>
        /// webվ��������
        /// </summary>
        public string WebHolder { get; set; }

        /// <summary>
        /// webվ�����
        /// </summary>
        public string WebTitle { get; set; }

        /// <summary>
        /// webվ��url��ַ
        /// </summary>
        public string WebUrl { get; set; }

        /// <summary>
        /// Ĭ����Ʒ����
        /// </summary>
        public int DefaultGoodsType { get; set; }

        /// <summary>
        /// Ĭ�ϻ���ID
        /// </summary>
        public Guid DefaultScoreId { get; set; }

        /// <summary>
        /// Ĭ�ϼ�����λID
        /// </summary>
        public Guid DefaultUnitsId { get; set; }

        /// <summary>
        /// Ĭ�ϻ��ҵ�λID
        /// </summary>
        public Guid DefaultCurrencyId { get; set; }

        /// <summary>
        /// ���ȡ����ʽ
        /// </summary>
        public int CurrencyDecimalType { get; set; }

        /// <summary>
        /// ���С��λ
        /// </summary>
        public int CurrencyDecimalDigits { get; set; }

        /// <summary>
        /// �Ƿ���ʾ�г��۸�
        /// </summary>
        public int IfMarketPrice { get; set; }

        /// <summary>
        /// ��ʾ��Ʒ��ʡģʽ
        /// </summary>
        public int SavePriceMode { get; set; }

        /// <summary>
        /// ��ʾ����Ʊ
        /// </summary>
        public int IfInvoice { get; set; }

        /// <summary>
        /// ��Ʊ˰��
        /// </summary>
        public double InvoiceTaxRate { get; set; }

        /// <summary>
        /// ÿ���ɹ��ؼ���Ʒ��
        /// </summary>
        public double SpecialOfferGifts { get; set; }

        /// <summary>
        /// ÿ���ɹ�������Ʒ��
        /// </summary>
        public double ScoreGifts { get; set; }

        /// <summary>
        /// �Ƿ�����Ա��¼��֤��
        /// </summary>
        public int IfValidate { get; set; }

        /// <summary>
        /// Ա��Ĭ������
        /// </summary>
        public int DefaultLanguage { get; set; }

        /// <summary>
        /// �����Ա�޸Ķ���ʱ��
        /// </summary>
        public int AuditingTime { get; set; }

        /// <summary>
        /// ������ģʽ
        /// </summary>
        public int StockCountMode { get; set; }

        /// <summary>
        /// ��վ����
        /// </summary>
        public string METADescription { get; set; }

        /// <summary>
        /// ��վ�ؼ���
        /// </summary>
        public string METAKeywords { get; set; }

        /// <summary>
        /// �Ƿ����ô���
        /// </summary>
        public int IsActivity { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        public string WorkingDay { get; set; }

        /// <summary>
        /// ������ʼʱ��
        /// </summary>
        public DateTime WorkingHoursBegin { get; set; }

        /// <summary>
        /// ��������ʱ��
        /// </summary>
        public DateTime WorkingHoursEnd { get; set; }
    }
}
