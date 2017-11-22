//================================================
// 源码所属公司:上海龙媒信息技术有限公司
// 文件产生时间:2006年12月31日
// 文件创建人:马力
// 最后修改时间:2006年12月31日
// 最后一次修改人:马力
//================================================
using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 节点类
    /// </summary>
    [Serializable]
    public class NodeInfo
    {
        /// <summary>
        /// 节点键
        /// </summary>
        public Guid NodeKey { get; set; }

        /// <summary>
        /// 节点值
        /// </summary>
        public string NodeValue { get; set; }

        /// <summary>
        /// 初始化构造函数
        /// </summary>
        public NodeInfo() { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nodeKey">节点键</param>
        /// <param name="nodeValue">节点值</param>
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
