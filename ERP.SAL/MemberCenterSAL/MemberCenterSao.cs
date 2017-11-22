using System;
using System.Collections.Generic;
using System.Linq;
using ERP.Model;
using Keede.Ecsoft.Model;
using MemberCenter.Contract.Client;
using MemberCenter.Contract.DTO;
using MemberCenter.Contract.Query;
using MemberCenter.Enum;
using ERP.Model.SubsidyPayment;

namespace ERP.SAL.MemberCenterSAL
{
    /// <summary>会员中心服务
    /// </summary>
    public class MemberCenterSao
    {
        private const string ENDPOINTNAME = "MemberCenterService";
        private const string CLIENTNAME = "ERP网站";

        #region [获取会员信息]

        /// <summary>根据会员Id获取用户信息
        /// </summary>
        /// <param name="salePlatformId">销售平台</param>
        /// <param name="userID">用户Id</param>
        /// <returns>用户基本信息</returns>
        public static MemberBaseInfo GetUserBase(Guid salePlatformId, Guid userID)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(salePlatformId, CLIENTNAME, ENDPOINTNAME);
                var result = erpMemberService.GetUserBase(salePlatformId, userID);
                if (result.IsSuccess && result.Data != null)
                {
                    var memberBaseInfo = new MemberBaseInfo
                    {
                        MemberId = result.Data.ID,
                        UserName = result.Data.UserName,
                        Password = result.Data.Password,
                        Nick = result.Data.Nick,
                        Mobile = result.Data.Mobile,
                        Email = result.Data.Email
                    };
                    return memberBaseInfo;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogCenter.LogService.LogError(string.Format("会员中心根据会员Id获取用户信息异常,salePlatformId={0},userID={1}", salePlatformId, userID), "会员中心", ex);
                return null;
            }
        }

        /// <summary>根据会员名获取用户信息（精确查找）
        /// </summary>
        /// <param name="salePlatformId">销售平台</param>
        /// <param name="userName">用户名</param>
        /// <returns>用户基本信息</returns>
        public static MemberBaseInfo GetUserBase(Guid salePlatformId, string userName)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(salePlatformId, CLIENTNAME, ENDPOINTNAME);
                var result = erpMemberService.GetUserBase(salePlatformId, userName);
                if (result != null && result.Data != null)
                {
                    var memberBaseInfo = new MemberBaseInfo
                    {
                        MemberId = result.Data.ID,
                        UserName = result.Data.UserName,
                        Password = result.Data.Password,
                        Nick = result.Data.Nick,
                        Mobile = result.Data.Mobile,
                        Email = result.Data.Email
                    };
                    return memberBaseInfo;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogCenter.LogService.LogError(string.Format("会员中心根据会员名获取用户信息（精确查找）异常,salePlatformId={0},userName={1}", salePlatformId, userName), "会员中心", ex);
                return null;
            }
        }

        /// <summary>会员下拉搜索
        /// </summary>
        /// <param name="salePlatformId">销售公司</param>
        /// <param name="userName">用户名称</param>
        /// <returns>key:会员ID,value:会员名称</returns>
        public static IDictionary<Guid, string> GetUserToDic(Guid salePlatformId, string userName)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(salePlatformId, CLIENTNAME, ENDPOINTNAME);
                var result = erpMemberService.FuzzySearchUserReturnDropDict(salePlatformId, userName);
                if (result != null && result.Data != null)
                {
                    return result.Data;
                }
                return new Dictionary<Guid, string>();
            }
            catch (Exception ex)
            {
                LogCenter.LogService.LogError(string.Format("会员中心会员下拉搜索异常,salePlatformId={0},userName={1}", salePlatformId, userName), "会员中心", ex);
                return new Dictionary<Guid, string>();
            }
        }

        #endregion

        #region [订单配货会员中心交互]

        /// <summary>会员订单配货
        /// </summary>
        /// <param name="salePlatformId">销售平台ID</param>
        /// <param name="orderId">订单ID</param>
        public static void OrderAllocateGoods(Guid salePlatformId, Guid orderId)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(salePlatformId, CLIENTNAME, ENDPOINTNAME);
                erpMemberService.OrderAllocateGoods(orderId);
            }
            catch (Exception ex)
            {
                LogCenter.LogService.LogError(string.Format("会员中心会员订单配货异常,salePlatformId={0},orderId={1}", salePlatformId, orderId), "会员中心", ex);
            }
        }

        #endregion

        #region [会员提现记录相关操作]

        /// <summary>获取会员提现记录
        /// </summary>
        /// <param name="userName">会员名</param>
        /// <param name="applyNo">体现申请号</param>
        /// <param name="bankName">银行名称 </param>
        /// <param name="startDate">起始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="state">状态</param>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformId">销售平台 </param>
        /// <param name="bankAccounts">银行账号</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示记录数 </param>
        /// <param name="totalCount">总记录数 </param>
        /// <returns>集合</returns>
        public static IList<MemberMentionApplyInfo> GetMemberMentionApplyByPage(string userName, string applyNo, string bankName, DateTime startDate,
            DateTime endDate, int state, Guid saleFilialeId, Guid salePlatformId, string bankAccounts, int pageIndex, int pageSize, out int totalCount)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(salePlatformId, CLIENTNAME, ENDPOINTNAME);
                var balanceMentionFinishPage = new ERPBalanceWithdrawQuery
                {
                    UserName = userName,
                    ApplyNo = applyNo,
                    BankName = bankName,
                    StartTime = startDate,
                    EndTime = endDate,
                    UserMentionState = (BalanceWithdrawState)state,
                    SaleFilialeID = saleFilialeId,
                    SalePlatformID = salePlatformId,
                    PayeeBankAccount = bankAccounts,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                var result = erpMemberService.GetBalanceWithdrawByPage(balanceMentionFinishPage);
                totalCount = 0;
                IList<MemberMentionApplyInfo> list = new List<MemberMentionApplyInfo>();
                if (result.IsSuccess && result.Data.Count > 0)
                {
                    foreach (MemberMentionApplyInfo info in result.Data.Select(item => new MemberMentionApplyInfo
                    {
                        Id = item.ID,
                        ApplyNo = item.ApplyNo,
                        ApplyTime = item.ApplyTime,
                        UserName = item.ApplyUserName,
                        Amount = item.Amount,
                        Balance = item.CurrentBalance,
                        State = item.BalanceWithdrawState,
                        BankName = item.PayeeRealName,
                        BankAccountName = item.PayeeBankName,
                        BankAccounts = item.PayeeBankAccount,
                        MemberId = item.ApplyUserID,
                        Description = item.Description ?? string.Empty,
                        Memo = item.Memo ?? string.Empty,
                        SaleFilialeId = item.SaleFilialeID,
                        SalePlatformId = item.SalePlatformID
                    }))
                    {
                        list.Add(info);
                    }
                    totalCount = (int)result.Total;
                    return list;
                }
                return list;
            }
            catch (Exception ex)
            {
                LogCenter.LogService.LogError(string.Format("会员中心获取会员提现记录异常，userName={0}, applyNo={1}, bankName={2}, startDate={3}, endDate={4}, state={5}, saleFilialeId={6}, salePlatformId={7}", userName, applyNo, bankName, startDate, endDate, state, saleFilialeId, salePlatformId), "会员中心", ex);
                totalCount = 0;
                return new List<MemberMentionApplyInfo>();
            }
        }

        /// <summary>获取会员提现记录
        /// </summary>
        /// <param name="salePlatformId">销售平台Id </param>
        /// <param name="applyID">会员提现记录ID</param>
        /// <returns>对象</returns>
        public static MemberMentionApplyInfo GetMemberMentionApply(Guid salePlatformId, Guid applyID)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(salePlatformId, CLIENTNAME, ENDPOINTNAME);
                var result = erpMemberService.GetBalanceWithdrawByApplyID(applyID);
                if (result.IsSuccess && result.Data != null)
                {
                    var info = new MemberMentionApplyInfo
                    {
                        Id = result.Data.ID,
                        ApplyNo = result.Data.ApplyNo,
                        ApplyTime = result.Data.ApplyTime,
                        UserName = result.Data.ApplyUserName,
                        Amount = result.Data.Amount,
                        Balance = result.Data.CurrentBalance,
                        State = result.Data.BalanceWithdrawState,
                        BankName = result.Data.PayeeBankName,
                        BankAccountName = result.Data.PayeeRealName,
                        BankAccounts = result.Data.PayeeBankAccount,
                        MemberId = result.Data.ApplyUserID,
                        Description = result.Data.Description,
                        Memo = result.Data.Memo,
                        SaleFilialeId = result.Data.SaleFilialeID,
                        SalePlatformId = result.Data.SalePlatformID,
                        OrderNo = result.Data.OrderNo,
                        ThirdPartyOrderNo = result.Data.ThirdPartyOrderNo
                    };
                    return info;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogCenter.LogService.LogError(string.Format("会员中心获取会员提现记录异常，salePlatformId={0}, applyID={1}", salePlatformId, applyID), "会员中心", ex);
                return null;
            }
        }

        /// <summary>会员提现审核通过执行到待打款
        /// </summary>
        /// <param name="salePlatformId">销售平台</param>
        /// <param name="applyID">提现记录ID</param>
        /// <param name="memo">对外说明</param>
        /// <param name="description">对内说明</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns>true/false</returns>
        public static bool ConfirmWithdrawWaitRemittance(Guid salePlatformId, Guid applyID, string memo, string description, out string errorMessage)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(salePlatformId, CLIENTNAME, ENDPOINTNAME);
                var result = erpMemberService.ConfirmWithdrawWaitRemittance(applyID, memo, description);
                errorMessage = string.Empty;
                if (!result.IsSuccess)
                {
                    errorMessage = result.ShowErrorMsg;
                    LogCenter.LogService.LogError(string.Format("会员中心会员提现审核通过执行到待打款错误:{0}, salePlatformId={1}, applyID={2}, memo={3}, description={4}", errorMessage, salePlatformId, applyID, memo, description), "会员中心");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "会员中心异常！";
                LogCenter.LogService.LogError(string.Format("会员中心会员提现审核通过执行到待打款异常:{0}, salePlatformId={1}, applyID={2}, memo={3}, description={4}", ex.Message, salePlatformId, applyID, memo, description), "会员中心", ex);
                return false;
            }
        }

        /// <summary>完成提现打款
        /// </summary>
        /// <param name="salePlatformId">销售平台</param>
        /// <param name="applyID">提现记录ID</param>
        /// <param name="memo">对外说明</param>
        /// <param name="description">对内说明</param>
        /// <param name="errorMessage">错误信息 </param>
        /// <returns>true/false</returns>
        /// <remarks>更新提现信息的状态为完成状态 添加一笔余额流水记录 记录操作日志</remarks>
        public static bool CompleteWithdrawApply(Guid salePlatformId, Guid applyID, string memo, string description, out string errorMessage)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(salePlatformId, CLIENTNAME, ENDPOINTNAME);
                var result = erpMemberService.CompleteWithdrawApply(applyID, memo, description);
                errorMessage = string.Empty;
                if (!result.IsSuccess)
                {
                    errorMessage = result.ShowErrorMsg;
                    LogCenter.LogService.LogError(string.Format("会员中心完成提现打款错误:{0},salePlatformId={1}, applyID={2}, memo={3}, description={4}", errorMessage, salePlatformId, applyID, memo, description), "会员中心", null);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "会员中心异常！";
                LogCenter.LogService.LogError(string.Format("会员中心完成提现打款异常:{0},salePlatformId={1}, applyID={2}, memo={3}, description={4}", ex.Message, salePlatformId, applyID, memo, description), "会员中心", ex);
                return false;
            }
        }

        /// <summary>退回提现申请 
        /// </summary>
        /// <param name="salePlatformId">销售平台</param>
        /// <param name="applyID">提现记录ID</param>
        /// <param name="memo">对外说明</param>
        /// <param name="description">对内说明</param>
        /// <param name="errorMessage">错误信息 </param>
        /// <returns>true/false</returns>
        /// <remarks>更新提现信息状态为退回（此状态可编辑提现信息或者取消） 记录操作日志</remarks>
        public static bool ReturnBackWithdrawApply(Guid salePlatformId, Guid applyID, string memo, string description, out string errorMessage)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(salePlatformId, CLIENTNAME, ENDPOINTNAME);
                var result = erpMemberService.ReturnBackWithdrawApply(applyID, memo, description);
                errorMessage = string.Empty;
                if (!result.IsSuccess)
                {
                    errorMessage = result.ShowErrorMsg;
                    LogCenter.LogService.LogError(string.Format("会员中心退回提现申请错误:{0},salePlatformId={1}, applyID={2}, memo={3}, description={4}", errorMessage, salePlatformId, applyID, memo, description), "会员中心");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "会员中心异常！";
                LogCenter.LogService.LogError(string.Format("会员中心完成提现打款异常:{0},salePlatformId={1}, applyID={2}, memo={3}, description={4}", ex.Message, salePlatformId, applyID, memo, description), "会员中心", ex);
                return false;
            }
        }

        /// <summary>设置会员提现记录备注信息
        /// </summary>
        /// <param name="salePlatformId">销售平台</param>
        /// <param name="applyID">记录ID</param>
        /// <param name="remark">备注信息</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns></returns>
        public static bool SetWithdrawApplyDescription(Guid salePlatformId, Guid applyID, string remark, out string errorMessage)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(salePlatformId, CLIENTNAME, ENDPOINTNAME);
                var result = erpMemberService.SupplementWithdrawApplyDescription(applyID, remark);
                errorMessage = string.Empty;
                if (!result.IsSuccess)
                {
                    errorMessage = result.ShowErrorMsg;
                    LogCenter.LogService.LogError(string.Format("会员中心设置会员提现记录备注信息错误:{0},salePlatformId={1}, applyID={2}, remark={3}", errorMessage, salePlatformId, applyID, remark), "会员中心", null);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "会员中心异常！";
                LogCenter.LogService.LogError(string.Format("会员中心设置会员提现记录备注信息异常:{0},salePlatformId={1}, applyID={2}, remark={3}", ex.Message, salePlatformId, applyID, remark), "会员中心", ex);
                return false;
            }
        }

        /// <summary>
        /// 添加会员体现申请
        /// </summary>
        /// <param name="salePlatformId"></param>
        /// <param name="amount"></param>
        /// <param name="personnelName"></param>
        /// <param name="errorMessage"></param>
        /// <param name="applyUserId"></param>
        /// <param name="userName"></param>
        /// <param name="description"></param>
        /// <param name="memo"></param>
        /// <param name="payeeBankAccount"></param>
        /// <param name="payeeBankName"></param>
        /// <param name="payeeRealName"></param>
        /// <param name="saleFilialeID"></param>
        /// <returns></returns>
        public static bool InsertMemberMentionApply(Guid salePlatformId, decimal amount, Guid applyUserId,
            string userName, string description,
            string memo, string payeeBankAccount, string payeeBankName,
            string payeeRealName, Guid saleFilialeID, string personnelName, out string errorMessage)
        {
            try
            {
                var memberMentionApply = new CustomServiceApplyBalanceWithdrawDTO
                {
                    SaleFilialeID = saleFilialeID,
                    Amount = amount,
                    ApplyUserID = applyUserId,
                    Description = description,
                    Memo = memo,
                    PayeeBankAccount = payeeBankAccount,
                    PayeeBankName = payeeBankName,
                    PayeeRealName = payeeRealName,
                    SalePlatformID = salePlatformId,
                    IsOpenMobileVerify = false,
                    VerifyCode = string.Empty,
                    PersonnelName = personnelName,
                    ApplyUserName = userName
                };
                var erpMemberService = new ERPMemberClient(salePlatformId, CLIENTNAME, ENDPOINTNAME);
                var result = erpMemberService.CustomServiceApplyBalanceWithdraw(memberMentionApply);
                errorMessage = string.Empty;
                if (!result.IsSuccess)
                {
                    errorMessage = result.ShowErrorMsg;
                    LogCenter.LogService.LogError(string.Format("会员中心添加提现记录错误:{0} salePlatformId={1}, amount={2}, applyUserId={3}, userName={4}, description={5}, memo={6}, payeeBankAccount={7}, payeeBankName={8}, payeeRealName={9}, saleFilialeID={10}, personnelName={11}", errorMessage, salePlatformId, amount, applyUserId, userName, description, memo, payeeBankAccount, payeeBankName, payeeRealName, saleFilialeID, personnelName), "会员中心");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "会员中心异常！";
                LogCenter.LogService.LogError(string.Format("会员中心添加信息异常:{0} salePlatformId={1}, amount={2}, applyUserId={3}, userName={4}, description={5}, memo={6}, payeeBankAccount={7}, payeeBankName={8}, payeeRealName={9}, saleFilialeID={10}, personnelName={11}", ex.Message, salePlatformId, amount, applyUserId, userName, description, memo, payeeBankAccount, payeeBankName, payeeRealName, saleFilialeID, personnelName), "会员中心", ex);
                return false;
            }
        }
        #endregion

        #region [待付款确认订单多支付金额退还余额]

        /// <summary>待付款确认多支付退还余额
        /// </summary>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformId">销售平台</param>
        /// <param name="userId">用户ID</param>
        /// <param name="orderId">支付的订单Id</param>
        /// <param name="orderNo">订单号</param>
        /// <param name="amount">退还金额</param>
        /// <param name="remark">备注信息</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns>true/false</returns>
        public static bool ReturnUserBalanceByPayOrder(Guid saleFilialeId, Guid salePlatformId, Guid userId, Guid orderId, String orderNo, decimal amount, string remark, out string errorMessage)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(salePlatformId, CLIENTNAME, ENDPOINTNAME);
                var result = erpMemberService.ReturnUserBalanceByPayOrder(saleFilialeId, userId, orderId, orderNo, amount, remark);
                errorMessage = string.Empty;
                if (!result.IsSuccess)
                {
                    errorMessage = result.ShowErrorMsg;
                    LogCenter.LogService.LogError(string.Format("会员中心待付款确认多支付退还余额错误:{0}, saleFilialeId={1}, salePlatformId={2}, userId={3}, orderId={4}, orderNo={5}, amount={6}, remark={7}", errorMessage, saleFilialeId, salePlatformId, userId, orderId, orderNo, amount, remark), "会员中心");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "会员中心异常！";
                LogCenter.LogService.LogError(string.Format("会员中心完成提现打款异常:{0}, saleFilialeId={1}, salePlatformId={2}, userId={3}, orderId={4}, orderNo={5}, amount={6}, remark={7}", ex.Message, saleFilialeId, salePlatformId, userId, orderId, orderNo, amount, remark), "会员中心", ex);
                return false;
            }
        }

        #endregion

        #region [会员余额管理]

        /// <summary>会员余额管理查询
        /// </summary>
        /// <param name="saleFilialeId">销售公司</param>
        /// <param name="salePlatformId">销售平台 </param>
        /// <param name="memberId">会员ID</param>
        /// <param name="changeState">操作状态</param>
        /// <param name="changeType">操作类型</param>
        /// <param name="tradeCode">单据编号</param>
        /// <param name="startDate">起始时间</param>
        /// <param name="endDate">截至时间</param>
        /// <param name="typeId">问题类型</param>
        /// <param name="isOfficial">是否官方(1:是;2:否;)</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns>集合</returns>
        public static IList<MemeberBalanceChangeInfo> SearchUserBalanceChangeListByPage(Guid saleFilialeId, Guid salePlatformId, Guid memberId, int changeState, int changeType, string tradeCode,
            DateTime startDate, DateTime endDate, Guid typeId, bool isOfficial, int pageIndex, int pageSize, out int totalCount)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(salePlatformId, CLIENTNAME, ENDPOINTNAME);
                var searchInfo = new SearchUserBalanceChangeQuery
                {
                    TradeCode = tradeCode,
                    StartTime = startDate == DateTime.MinValue ? (DateTime?)null : startDate,
                    EndTime = endDate == DateTime.MinValue ? (DateTime?)null : endDate,
                    SaleFilialeId = saleFilialeId,
                    SalePlatformID = salePlatformId,
                    UserID = memberId,
                    UserBalanceChangeState = changeState == 0 ? (int?)null : changeState,
                    UserBalanceChangeType = changeType == 0 ? (int?)null : changeType,
                    TypeOfProblemId = typeId,
                    PageIndex = pageIndex,
                    PageSize = pageSize
                };
                totalCount = 0;
                IList<MemeberBalanceChangeInfo> list = new List<MemeberBalanceChangeInfo>();
                var result = erpMemberService.SearchUserBalanceChangeListByPage(isOfficial, searchInfo);
                if (result.IsSuccess && result.Data.Count > 0)
                {
                    foreach (var item in result.Data)
                    {
                        var info = new MemeberBalanceChangeInfo
                        {
                            ApplicantID = item.ApplicantID,
                            ApplicantName = item.ApplicantName,
                            ApplyID = item.ApplyID,
                            ApplyTime = item.ApplyTime,
                            BankAccountId = item.BankAccountID,
                            TradeCode = item.TradeCode,
                            CurrentBalance = item.CurrentBalance,
                            Increase = item.Increase,
                            Subtract = item.Subtract,
                            Remark = item.ApplyReason,
                            SaleFilialeId = item.SaleFilialeID,
                            SalePlatformId = item.SalePlatformID,
                            UserBalanceChangeType = item.ChangeType,
                            UserID = item.UserID,
                            UserName = item.UserName,
                            State = item.State,
                            PayTreasureAccount = item.PayTreasureAccount,
                            TypeOfProblemName = item.TypeOfProblemName,
                            PayBankName = item.PayeeBankName

                        };
                        totalCount = (int)result.Total;
                        list.Add(info);
                    }
                }
                return list;
            }
            catch (Exception ex)
            {
                LogCenter.LogService.LogError(string.Format("会员中心获取会员提现记录异常:{0}, saleFilialeId={1}, salePlatformId={2}, memberId={3}, changeState={4}, changeType={5}, tradeCode={6}, startDate={7}, endDate={8}, typeId={9}", ex.Message, saleFilialeId, salePlatformId, memberId, changeState, changeType, tradeCode, startDate, endDate, typeId), "会员中心", ex);
                totalCount = 0;
                return new List<MemeberBalanceChangeInfo>();
            }
        }

        /// <summary>
        /// 获取会员余额问题类型列表
        /// </summary>
        /// <param name="salePlateformId"></param>
        /// <returns></returns>
        public static Dictionary<Guid, string> GetMemberTypeProblemTypeList(Guid salePlateformId)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(Guid.Empty, CLIENTNAME, ENDPOINTNAME);
                var result = erpMemberService.GetTypeOfProblemList(salePlateformId);
                if (result.IsSuccess && result.Data != null)
                {
                    return result.Data.ToDictionary(k => k.TypeOfProblemId, v => v.TypeOfProblemName);
                }
                return new Dictionary<Guid, string>();
            }
            catch (Exception ex)
            {
                LogCenter.LogService.LogError(string.Format("会员中心获取会员余额问题类型异常, salePlateformId={0}", salePlateformId), "会员中心", ex);
                return null;
            }
        }

        /// <summary>获取会员余额记录
        /// </summary>
        /// <param name="applyId">申请记录ID</param>
        /// <returns></returns>
        public static MemeberBalanceChangeInfo GetMemeberBalanceChangeInfo(Guid applyId)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(Guid.Empty, CLIENTNAME, ENDPOINTNAME);
                var result = erpMemberService.GetUserBalanceChange(applyId);
                if (result.IsSuccess && result.Data != null)
                {
                    var info = new MemeberBalanceChangeInfo
                    {
                        ApplicantID = result.Data.ApplicantID,
                        ApplicantName = result.Data.ApplicantName,
                        ApplyID = result.Data.ID,
                        BankAccountId = result.Data.BankAccountID,
                        TradeCode = result.Data.TradeCode,
                        Increase = result.Data.Amount,
                        Remark = result.Data.ApplyReason,
                        SaleFilialeId = result.Data.SaleFilialeID,
                        SalePlatformId = result.Data.SalePlatformID,
                        UserID = result.Data.UserID,
                        UserBalanceChangeType = result.Data.ChangeType,
                        PayTreasureAccount = result.Data.PayTreasureAccount,
                        State = result.Data.State,
                        TypeOfProblemName = result.Data.TypeOfProblemName,
                        PayeeRealName = result.Data.PayeeRealName,
                        PayBankName = result.Data.PayeeBankName
                    };
                    return info;
                }
                return null;
            }
            catch (Exception ex)
            {
                LogCenter.LogService.LogError(string.Format("会员中心获取会员余额记录异常,applyId={0}", applyId), "会员中心", ex);
                return null;
            }
        }


        /// <summary>会员余额管理确认不通过
        /// </summary>
        /// <param name="salePlatformId">销售平台ID</param>
        /// <param name="applyId">申请记录ID</param>
        /// <param name="affirmId">审核人ID</param>
        /// <param name="affirmName">审核人姓名</param>
        /// <param name="refuseReason">不通过理由</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns>true/false</returns>
        public static bool AffirmNoPassUserBalanceChange(Guid salePlatformId, Guid applyId, Guid affirmId, string affirmName, string refuseReason, out string errorMessage)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(salePlatformId, CLIENTNAME, ENDPOINTNAME);
                errorMessage = string.Empty;
                var result = erpMemberService.AffirmNoPassUserBalanceChange(applyId, affirmId, affirmName, refuseReason);
                if (!result.IsSuccess)
                {
                    errorMessage = result.ShowErrorMsg + "------" + result.SystemErrorMsg;
                    LogCenter.LogService.LogError(string.Format("会员中心会员余额管理确认不通过:{0}, salePlatformId={1}, applyId={2}, affirmId={3}, affirmName={4}, refuseReason={5}", errorMessage, salePlatformId, applyId, affirmId, affirmName, refuseReason), "会员中心");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "会员中心异常！";
                LogCenter.LogService.LogError(string.Format("会员中心会员余额管理确认不通过异常:{0}, salePlatformId={1}, applyId={2}, affirmId={3}, affirmName={4}, refuseReason={5}", ex.Message, salePlatformId, applyId, affirmId, affirmName, refuseReason), "会员中心", ex);
                return false;
            }
        }

        /// <summary>会员余额管理操作完成
        /// </summary>
        /// <param name="salePlatformId">销售平台ID</param>
        /// <param name="applyId">申请记录ID</param>
        /// <param name="affirmId">审核人ID</param>
        /// <param name="affirmName">审核人姓名</param>
        /// <param name="errorMessage">错误信息</param>
        /// <returns>true/false</returns>
        public static bool CompleteUserBalanceChange(Guid salePlatformId, Guid applyId, Guid affirmId, string affirmName, out string errorMessage)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(salePlatformId, CLIENTNAME, ENDPOINTNAME);
                errorMessage = string.Empty;
                var result = erpMemberService.CompleteUserBalanceChange(applyId, affirmId, affirmName);
                if (!result.IsSuccess)
                {
                    errorMessage = result.ShowErrorMsg + "------" + result.SystemErrorMsg;
                    LogCenter.LogService.LogError(string.Format("会员中心会员余额管理操作完成错误:{0}, salePlatformId={1}, applyId={2}, affirmId={3}, affirmName={4}", errorMessage, salePlatformId, applyId, affirmId, affirmName), "会员中心");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                errorMessage = "会员中心异常！";
                LogCenter.LogService.LogError(string.Format("会员中心会员余额管理操作完成异常:{0}, salePlatformId={1}, applyId={2}, affirmId={3}, affirmName={4}", ex.Message, salePlatformId, applyId, affirmId, affirmName), "会员中心", ex);
                return false;
            }
        }

        #endregion

        #region [获取会员总账]

        /// <summary>获取会员总账
        /// </summary>
        /// <param name="saleFilialeId">销售公司ID</param>
        /// <param name="startDate">起始时间 </param>
        /// <param name="endDate">截至时间</param>
        /// <param name="type">资金流动情况（增加或减少，零为所有，大于零收入，小于零支出）</param>
        /// <param name="tradeCode">单据号</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示记录数</param>
        /// <param name="totalCount">总记录数</param>
        /// <returns>集合</returns>
        public static IList<MemberReckoningInfo> SearchReckoningByPage(Guid saleFilialeId, DateTime startDate, DateTime endDate, int type, string tradeCode, int pageIndex, int pageSize, out int totalCount)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(Guid.Empty, CLIENTNAME, ENDPOINTNAME);
                var query = new SearchReckoningQuery
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    SaleFilialeID = saleFilialeId,
                    TradeCode = tradeCode,
                    StartTime = startDate == DateTime.MinValue ? (DateTime?)null : startDate,
                    EndTime = endDate == DateTime.MinValue ? (DateTime?)null : endDate,
                    Type = type
                };
                var result = erpMemberService.SearchReckoningByPage(query);
                totalCount = 0;
                var list = new List<MemberReckoningInfo>();
                if (result.IsSuccess && result.Data.Count > 0)
                {
                    totalCount = (int)result.Total;
                    list.AddRange(result.Data.Select(item => new MemberReckoningInfo
                    {
                        ID = item.ID,
                        TradeCode = item.TradeCode,
                        OriginalTradeCode = item.OriginalTradeCode,
                        BalanceFlowNo = item.BalanceFlowNo,
                        UserID = item.UserID,
                        CreateTime = item.CreateTime,
                        CurrentBalance = item.CurrentBalance,
                        Amount = item.Amount,
                        Description = item.Description,
                        SaleFilialeID = item.SaleFilialeID
                    }));
                }
                return list;
            }
            catch (Exception ex)
            {
                LogCenter.LogService.LogError(string.Format("会员中心获取会员总账异常:{0}, saleFilialeId={1}, startDate={2}, endDate={3}, type={4}, tradeCode={5}", ex.Message, saleFilialeId, startDate, endDate, type, tradeCode), "会员中心", ex);
                totalCount = 0;
                return new List<MemberReckoningInfo>();
            }
        }

        #endregion

        #region [获取会员余额流水]

        /// <summary>获取会员余额流水
        /// </summary>
        /// <param name="memberId">会员ID</param>
        /// <param name="flowType">资金流向</param>
        /// <param name="startDate">起始时间</param>
        /// <param name="endDate">截止时间</param>
        /// <param name="pageIndex">当前页</param>
        /// <param name="pageSize">每页显示数量</param>
        /// <param name="totalCount">总数量</param>
        /// <returns>集合</returns>
        public static IList<MemberBalanceFlowInfo> GetMemberBalanceFlowByPage(Guid memberId, int flowType, DateTime startDate, DateTime endDate, int pageIndex, int pageSize, out int totalCount)
        {
            try
            {
                var erpMemberService = new ERPMemberClient(Guid.Empty, CLIENTNAME, ENDPOINTNAME);
                var query = new SearchBalanceFlowQuery
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    MoreOrLessType = flowType == 0 ? (int?)null : flowType,
                    StartTime = startDate == DateTime.MinValue ? (DateTime?)null : startDate,
                    EndTime = endDate == DateTime.MinValue ? (DateTime?)null : endDate,
                    UserID = memberId
                };
                totalCount = 0;
                var list = new List<MemberBalanceFlowInfo>();
                //isOffical 是否官网，会员中心的处理如果有memberId，忽略isOffical,所以这里的传值true\false都行
                var result = erpMemberService.GetBalanceFlowByPage(true, query);
                if (result.IsSuccess && result.Data != null)
                {
                    totalCount = (int)result.Total;
                    list.AddRange(result.Data.Select(item => new MemberBalanceFlowInfo
                    {
                        MemberId = item.UserID,
                        TradeCode = item.TradeCode,
                        MemberName = item.UserName,
                        CreateTime = item.CreateTime,
                        IncreaseAmount = item.IncreaseAmount,
                        SubtractAmount = item.SubtractAmount,
                        CurrentBalance = item.CurrentBalance,
                        ManageDescription = item.ManageDescription,
                        Memo = item.Memo
                    }));
                }

                return list;
            }
            catch (Exception ex)
            {
                LogCenter.LogService.LogError(string.Format("会员中心获取会员余额流水异常:{0}, memberId={1}, flowType={2}, startDate={3}, endDate={4}", ex.Message, memberId, flowType, startDate, endDate), "会员中心", ex);
                totalCount = 0;
                return new List<MemberBalanceFlowInfo>();
            }
        }

        #endregion
        
    }
}
