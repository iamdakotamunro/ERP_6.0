using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// ����״̬��ö��
    /// </summary>
    [Serializable]
    public enum OrderOtherState
    {
        ///// <summary>
        ///// ������Ч����,����״̬С�ڵ���Consignmented�Ķ���
        ///// </summary>
        //[EnumArrtibute("������Ч����,����״̬С�ڵ���Consignmented�Ķ���")]
        //All = -1,
        ///// <summary>
        ///// δ������,����״̬ΪUnVerify
        ///// </summary>
        //[EnumArrtibute("δ������,����״̬ΪUnVerify")]
        //UnApproved = 0,
        ///// <summary>
        ///// ������,����״̬����UnVerify��С��Consignmented
        ///// </summary>
        //[EnumArrtibute("������,����״̬����UnVerify��С��Consignmented")]
        //Approved = 1,
        ///// <summary>
        ///// δ���,����״̬С��Consignmented
        ///// </summary>
        //[EnumArrtibute("δ���,����״̬С��Consignmented")]
        //UnConsignmented = 2,
        ///// <summary>
        ///// �����,����״̬����Consignmented
        ///// </summary>
        //[EnumArrtibute("�����,����״̬����Consignmented")]
        //Consignmented = 3,
        ///// <summary>
        ///// ��Ч���� ����ӡ�Ķ���
        ///// </summary>
        //[EnumArrtibute("��Ч���� ����ӡ�Ķ���")]
        //Effective = 4
        
        /// <summary>
        /// 
        /// </summary>
        [Enum("�����ּ�")]
        Sorting=100,

        /// <summary>
        /// 
        /// </summary>
        [Enum("�������")]
        Pack=200,

        /// <summary>
        /// 
        /// </summary>
        [Enum("����ƥ��")]
        Matching=300
    }
}
