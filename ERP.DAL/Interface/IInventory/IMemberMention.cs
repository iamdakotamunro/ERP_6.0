using System;
using System.Collections.Generic;
using ERP.Enum;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IInventory
{
    /// <summary>
    /// 会员提现类接口
    /// Add by liucaijun at 2011-October-20th
    /// Modify by zhangfan at 2012-April-28th
    /// Modify content:修改备注信息
    /// </summary>
    public interface IMemberMention
    {
        /// <summary>
        /// 添加会员提现申请
        /// </summary>
        /// <param name="mentionapplyInfo">提现申请模型</param>
        void AddMembermentionapply(MemberMentionApplyInfo mentionapplyInfo);

        /// <summary>
        /// 获取会员提现申请
        /// </summary>
        /// <param name="userName">会员用户名</param>
        /// <param name="startDate">开始时间</param>
        /// <param name="endDate">截至时间</param>
        /// <param name="state">申请状态</param>
        /// <param name="filialeId">网站类型</param>
        /// <returns></returns>
        List<MemberMentionApplyInfo> GetMemberMentionApply(string userName, DateTime startDate, DateTime endDate, int state, Guid filialeId);

        /// <summary>
        /// 会员提现申请审核
        /// </summary>
        /// <param name="id">申请ID</param>
        /// <param name="state">状态</param>
        /// <param name="memo">备注</param>
        /// <param name="description">操作流水</param>
        void MembermentionapplyAuditing(Guid id, int state, string memo, string description);

        /// <summary>
        /// 删除提现申请审核
        /// </summary>
        /// <param name="id">申请ID</param>
        void DeleteMembermentionApply(Guid id);

        /// <summary>
        /// 获取会员提现申请
        /// </summary>
        /// <param name="applyId">申请ID</param>
        /// <returns></returns>
        MemberMentionApplyInfo GetMemberMentionApply(Guid applyId);

        /// <summary>
        /// 修改备注信息
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        void MembermentionAddMemo(Guid id, string description);

        #region INSERT

        /// <summary>
        /// 添加会员提现申请
        /// </summary>
        /// <param name="mentionapplyInfo">提现申请模型</param>
        void AddMembermentionapply_Server(MemberMentionApplyInfo mentionapplyInfo);
        #endregion


        #region UPDATE

        /// <summary>
        /// 修改状态
        /// </summary>
        /// <param name="id"></param>
        /// <param name="memberMentionState"></param>
        void UpdateState(Guid id, MemberMentionState memberMentionState);


        /// <summary>
        /// 修改备注
        /// </summary>
        /// <param name="id">ID</param>
        /// <param name="memo">备注信息</param>
        /// <param name="description">操作流水</param>
        void SetMemberMentionApplyClew(Guid id, string memo, string description);

        /// <summary>
        /// 前台修改会员提现信息
        /// </summary>
        /// <param name="mentionapplyInfo"></param>
        void UpdateFront(MemberMentionApplyInfo mentionapplyInfo);

        #endregion
    }
}
