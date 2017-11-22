using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ERP.Model
{
    /// <summary> 会员基本信息    ADD  2014-08-29   陈重文
    /// </summary>
    public class MemberBaseInfo
    {
        /// <summary>会员ID
        /// </summary>
        public Guid MemberId { get; set; }

        /// <summary>用户(登录)名称  
        /// </summary>
        public string UserName { get; set; }

        /// <summary>登录密码 
        /// </summary>
        public string Password { get; set; }

        /// <summary>昵称
        /// </summary>
        public string Nick { get; set; }

        /// <summary>手机
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>邮件
        /// </summary>
        public string Email { get; set; }
    }
}
