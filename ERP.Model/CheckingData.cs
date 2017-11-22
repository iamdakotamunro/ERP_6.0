using System;

namespace Keede.Ecsoft.Model
{
    /// <summary> 对账历史记录 类型
    /// </summary>
    public class CheckHistoryInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public CheckHistoryInfo()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checker">对账人</param>
        /// <param name="checkDate">对账日期</param>
        /// <param name="description">描述</param>
        /// <param name="bankID">对账ID</param>
        /// <param name="uploadPath">上传文件</param>
        /// <param name="checkPath">记录文件</param>
        /// <param name="checkHistory">对账单位</param>
        public CheckHistoryInfo(String checker, DateTime checkDate, String description, String bankID, String uploadPath, String checkPath, String checkHistory)
        {
            Checker = checker;
            CheckDate = checkDate;
            Description = description;
            BankID = bankID;
            UploadPath = uploadPath;
            CheckPath = checkPath;
            CheckCompany = checkHistory;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="checker"></param>
        /// <param name="checkDate">对账日期</param>
        /// <param name="description">描述</param>
        /// <param name="bankID">对账ID</param>
        /// <param name="uploadPath">上传文件</param>
        /// <param name="checkPath">记录文件</param>
        /// <param name="checkHistory">对账单位</param>
        /// <param name="successPath">成功</param>
        /// <param name="failurePath">失败</param>
        public CheckHistoryInfo(String checker, DateTime checkDate, String description, String bankID, String uploadPath, String checkPath, String checkHistory, String successPath, String failurePath)
        {
            Checker = checker;
            CheckDate = checkDate;
            Description = description;
            BankID = bankID;
            UploadPath = uploadPath;
            CheckPath = checkPath;
            CheckCompany = checkHistory;
            SucessPath = successPath;
            FailurePath = failurePath;
        }

        /// <summary>
        ///  对账人
        /// </summary>
        public string Checker { get; set; }

        /// <summary>
        /// 往来单位ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        ///  对账日期
        /// </summary>
        public DateTime CheckDate { get; set; }

        /// <summary>
        ///  描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 对账ID
        /// </summary>
        public string BankID { get; set; }

        /// <summary>
        ///  上传文件
        /// </summary>
        public string UploadPath { get; set; }

        /// <summary>
        ///  记录文件
        /// </summary>
        public string CheckPath { get; set; }

        /// <summary>
        /// 对账单位
        /// </summary>
        public string CheckCompany { get; set; }

        /// <summary>
        ///  成功
        /// </summary>
        public string SucessPath { get; set; }

        /// <summary>
        ///  失败
        /// </summary>
        public string FailurePath { get; set; }
    }

    /// <summary> 对账错误类
    /// </summary>
    public class ErrorInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public ErrorInfo()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sExpressNo">快递编号</param>
        /// <param name="sErrorMsg">错误内容</param>
        public ErrorInfo(String sExpressNo, String sErrorMsg)
        {
            ExpressNO = sExpressNo;
            ErrorMsg = sErrorMsg;
        }

        /// <summary>
        /// 快递编号
        /// </summary>
        public string ExpressNO { get; set; }

        /// <summary>
        /// 错误内容
        /// </summary>
        public string ErrorMsg { get; set; }
    }

    /// <summary> 对账结果类
    /// </summary>
    public class CheckingDiffInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public CheckingDiffInfo()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reckoningNo"></param>
        /// <param name="expressNo"></param>
        /// <param name="postMoney"></param>
        /// <param name="serverMoney"></param>
        /// <param name="diffNum"></param>
        /// <param name="desc"></param>
        public CheckingDiffInfo(Guid reckoningNo, String expressNo, decimal postMoney, decimal serverMoney, decimal diffNum, String desc)
        {
            ReckoningNO = reckoningNo;
            ExpressNO = expressNo;
            PostMoney = postMoney;
            ServerMoney = serverMoney;
            DiffNum = diffNum;
            //BankAccontID = _bankAccountID;
            Description = desc;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reckoningNo">账目编号</param>
        /// <param name="expressNo">快递号</param>
        /// <param name="postMoney"></param>
        /// <param name="serverMoney">服务费</param>
        /// <param name="diffNum"></param>
        /// <param name="desc">描述</param>
        /// <param name="isQuestion"></param>
        public CheckingDiffInfo(Guid reckoningNo, String expressNo, decimal postMoney, decimal serverMoney, decimal diffNum, String desc, bool isQuestion)
        {
            ReckoningNO = reckoningNo;
            ExpressNO = expressNo;
            PostMoney = postMoney;
            ServerMoney = serverMoney;
            DiffNum = diffNum;
            //BankAccontID = _bankAccountID;
            Description = desc;
            IsQuestionReck = IsQuestionReck;
        }

        //Guid _bankAccountID;

        /// <summary>
        /// 
        /// </summary>
        public bool IsQuestionReck { get; set; }

        //public Guid BankAccontID
        //{
        //    get { return _bankAccountID; }
        //    set { _bankAccountID = value; }
        //}

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 账目编号
        /// </summary>
        public Guid ReckoningNO { get; set; }

        /// <summary>
        /// 快递编号
        /// </summary>
        public string ExpressNO { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal PostMoney { get; set; }

        /// <summary>
        /// 服务费
        /// </summary>
        public decimal ServerMoney { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public decimal DiffNum { get; set; }
    }

    /// <summary> 上传对账单的数据类
    /// </summary>
    public class CheckingDataInfo
    {
        /// <summary>
        /// 
        /// </summary>
        public CheckingDataInfo()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sExpressNo">快递编号</param>
        /// <param name="sMoney"></param>
        public CheckingDataInfo(String sExpressNo, String sMoney)
        {
            ExpressNo = sExpressNo;
            Money = sMoney;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sExpressNo">快递编号</param>
        /// <param name="sMoney"></param>
        /// <param name="sExpressCost">快递花费</param>
        public CheckingDataInfo(String sExpressNo, String sMoney, String sExpressCost)
        {
            ExpressNo = sExpressNo;
            Money = sMoney;
            ExpressCost = sExpressCost;
        }

        /// <summary>
        /// 快递花费
        /// </summary>
        public string ExpressCost { get; set; }

        /// <summary>
        /// 快递编号
        /// </summary>
        public string ExpressNo { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Money { get; set; }

        /// <summary>
        /// 订单重量
        /// </summary>
        public string Weight { get; set; }
    }

    /// <summary> 对账记录模型
    /// </summary>
    public class CheckInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="companyId">快递公司ID</param>
        /// <param name="companyName">快递公司名称</param>
        /// <param name="checkUser">对账人</param>
        /// <param name="checkDate">对账日期</param>
        /// <param name="bankName">银行名字</param>
        /// <param name="srcFileName">原始文件名字</param>
        /// <param name="checkFileName">对账结果文件名字</param>
        /// <param name="reckoningFileName">新增单位往来文件名字</param>
        /// <param name="description">描述</param>
        public CheckInfo(Guid companyId, String companyName, String checkUser, DateTime checkDate, String bankName, String srcFileName, String checkFileName, String reckoningFileName, String description)
        {
            CompanyId = companyId;
            CompanyName = companyName;
            CheckUser = checkUser;
            CheckDate = checkDate;
            BankName = bankName;
            SrcFileName = srcFileName;
            CheckFileName = checkFileName;
            ReckoningFileName = reckoningFileName;
            Description = description;
        }

        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 银行名字
        /// </summary>
        public string BankName { get; set; }

        /// <summary>
        /// 快递公司ID
        /// </summary>
        public Guid CompanyId { get; set; }

        /// <summary>
        /// 快递公司名称
        /// </summary>
        public string CompanyName { get; set; }

        /// <summary>
        /// 对账人
        /// </summary>
        public string CheckUser { get; set; }

        /// <summary>
        /// 对账日期
        /// </summary>
        public DateTime CheckDate { get; set; }

        /// <summary>
        /// 原始文件名字
        /// </summary>
        public string SrcFileName { get; set; }

        /// <summary>
        /// 对账结果文件名字
        /// </summary>
        public string CheckFileName { get; set; }

        /// <summary>
        /// 新增单位往来文件名字
        /// </summary>
        public string ReckoningFileName { get; set; }
    }
}
