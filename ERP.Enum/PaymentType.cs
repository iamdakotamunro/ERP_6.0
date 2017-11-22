using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// ֧������ö��
    /// modify by liangcanren at 2015-05-04
    /// </summary>
    [Serializable]
    public enum PaymentType
    {
        /// <summary>
		/// δ����
		/// </summary>
        [Enum("δ����")]
		NoSet = -1,

        /// <summary>
        /// ����֧��
        /// </summary>
        [Enum("����֧��")]
        OnLine = 0,

        /// <summary>
        /// ��ͳ�ʺ�
        /// </summary>
        [Enum("��ͳ�ʺ�")]
        Tradition = 1,

        /// <summary>
        /// ˢ���ʺ�
        /// </summary>
        [Enum("ˢ���ʺ�")]
        SwipeCard = 2,

        ///// <summary>
        ///// �ʾֻ��
        ///// </summary>
        //[Enum("�ʾֻ��")]
        //Post = 0,

        ///// <summary>
        ///// �ֽ�
        ///// </summary>
        //[Enum("�ֽ�")]
        //Cash = 1,

        ///// <summary>
        ///// ����֧��
        ///// </summary>
        //[Enum("����֧��")]
        //Internet = 2,

        ///// <summary>
        ///// ���л��
        ///// </summary>
        //[Enum("���л��")]
        //Remittance = 3,

        ///// <summary>
        ///// ���
        ///// </summary>
        //[Enum("���")]
        //TMO = 4,

        ///// <summary>
        ///// ����֧������
        ///// </summary>
        //[Enum("����֧������")]
        //Other = 5,

        ///// <summary>
        ///// ���ÿ�
        ///// </summary>
        //[Enum("���ÿ�")]
        //CreditCard=6
    }
}
