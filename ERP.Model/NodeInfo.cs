//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2006��12��31��
// �ļ�������:����
// ����޸�ʱ��:2006��12��31��
// ���һ���޸���:����
//================================================
using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// �ڵ���
    /// </summary>
    [Serializable]
    public class NodeInfo
    {
        /// <summary>
        /// �ڵ��
        /// </summary>
        public Guid NodeKey { get; set; }

        /// <summary>
        /// �ڵ�ֵ
        /// </summary>
        public string NodeValue { get; set; }

        /// <summary>
        /// ��ʼ�����캯��
        /// </summary>
        public NodeInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeKey">�ڵ��</param>
        /// <param name="nodeValue">�ڵ�ֵ</param>
        public NodeInfo(Guid nodeKey, string nodeValue)
        {
            NodeKey = nodeKey;
            NodeValue = nodeValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public override bool Equals(object compareObj)
        {
            if (compareObj is NodeInfo)
                return (compareObj as NodeInfo).NodeKey == NodeKey;
            return base.Equals(compareObj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (NodeKey == Guid.Empty) return base.GetHashCode();
            string stringRepresentation = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "#" + NodeKey.ToString();
            return stringRepresentation.GetHashCode();
        }
    }
}
