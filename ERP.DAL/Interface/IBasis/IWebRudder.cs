using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IBasis
{
    /// <summary>
    /// 站点基本信息及控制类信息接口
    /// </summary>
    public interface IWebRudder
    {
        /// <summary>
        /// 获取网站基本控制信息
        /// </summary>
        /// <returns>返回站点基本信息类</returns>
        WebRudderInfo GetWebRudder();
    }
}
