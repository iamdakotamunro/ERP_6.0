using Keede.Ecsoft.Model;

namespace ERP.DAL.Interface.IBasis
{
    /// <summary>
    /// վ�������Ϣ����������Ϣ�ӿ�
    /// </summary>
    public interface IWebRudder
    {
        /// <summary>
        /// ��ȡ��վ����������Ϣ
        /// </summary>
        /// <returns>����վ�������Ϣ��</returns>
        WebRudderInfo GetWebRudder();
    }
}
