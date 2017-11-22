using System;

namespace ERP.Model
{
    /// <summary>
    /// 登录结果信息
    /// </summary>
    [Serializable]
    public class LoginResultInfo
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="resultInfo"></param>
        public LoginResultInfo(MIS.Model.View.LoginResultInfo resultInfo)
        {
            IsSuccess = resultInfo.IsSucess;
            FailMessage = resultInfo.FailMessage;
            Token = resultInfo.Token;
            PersonnelInfo = new PersonnelInfo(resultInfo.LoginAccountInfo);
        }

        /// <summary>
        /// 是否登录成功
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// 登录失败的消息
        /// </summary>
        public string FailMessage { get; set; }

        /// <summary>
        /// 登录成功后，生成的TOKEN
        /// </summary>
        public string Token { get; set; }

        /// <summary>
        /// 用户详细信息
        /// </summary>
        public PersonnelInfo PersonnelInfo { get; set; }
    }
}
