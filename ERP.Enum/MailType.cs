using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// ϵͳ�ʼ���������ö��
    /// </summary>
    [Serializable]
    public enum MailType
    {
        /// <summary>
        /// ע��
        /// </summary>
        [Enum("ע��")]
        Register = 1,
        /// <summary>
        /// ��¼
        /// </summary>
        [Enum("��¼")]
        Login = 2,
        /// <summary>
        /// ��ȡ����
        /// </summary>
        [Enum("��ȡ����")]
        GetPassword = 3,
        /// <summary>
        /// ��д�û�����
        /// </summary>
        [Enum("��д�û�����")]
        RewriteMember = 4,
        /// <summary>
        /// �޸�����
        /// </summary>
        [Enum("�޸�����")]
        ChangePassword = 5,
        /// <summary>
        /// �Ƽ�������
        /// </summary>
        [Enum("�Ƽ�������")]
        Recommend = 6,
        /// <summary>
        /// ѯ��
        /// </summary>
        [Enum("ѯ��")]
        Quaere = 7,
        /// <summary>
        /// ����
        /// </summary>
        [Enum("����")]
        Shopping = 10,
        /// <summary>
        /// ����
        /// </summary>
        [Enum("����")]
        Approved = 11,
        /// <summary>
        /// �߿�
        /// </summary>
        [Enum("�߿�")]
        Reminder = 12,
        /// <summary>
        /// ����ȷ��
        /// </summary>
        [Enum("����ȷ��")]
        Pay = 13,
        /// <summary>
        /// �ȴ�����
        /// </summary>
        [Enum("�ȴ�����")]
        WaitForConsignment = 14,
        /// <summary>
        /// ������ɶ���
        /// </summary>
        [Enum("������ɶ���")]
        Consignment = 15,
        /// <summary>
        /// �˿�
        /// </summary>
        [Enum("�˿�")]
        Refundment = 16,
        /// <summary>
        /// ȱ��֪ͨ
        /// </summary>
        [Enum("ȱ��֪ͨ")]
        Shortage = 17,
        /// <summary>
        /// �߿֪ͨ
        /// </summary>
        [Enum("�߿֪ͨ")]
        NotifyAndPayment = 18,
        /// <summary>
        /// �����Ƽ�
        /// </summary>
        [Enum("�����Ƽ�")]
        RemarkRecommend = 19,

        // Begin add by tianys at 2009-06-15
        /// <summary>
        /// �Ź���ʼ֪ͨ
        /// </summary>
        [Enum("�Ź���ʼ֪ͨ")]
        GroupPurchaseNotify = 20,
        // End add

        /// <summary>
        /// �һ���ȯ
        /// </summary>
        [Enum("�һ���ȯ")]
        ExchangeCoupon = 21,

        /// <summary>
        /// ���Ѵ���
        /// </summary>
        [Enum("���Ѵ���")]
        FriendPaid = 22,

        /// <summary>
        /// ���Իظ�
        /// </summary>
        [Enum("������֤")]
        ValidEmail = 23,

        /// <summary>
        /// �ʼ���֤��
        /// </summary>
        [Enum("ȡ�������ʼ�")]
        CancelPromotionEmail = 24,

        /// <summary>
        /// ��Ʒ�����ʼ�
        /// </summary>
        [Enum("��Ʒ�����ʼ�")]
        GoodsMail = 25,

        /// <summary>
        /// �޸��ʼ�
        /// </summary>
        [Enum("�޸��ʼ�")]
        ChangeMail = 26,

        /// <summary>
        /// AppȺ��
        /// </summary>
        [Enum("AppȺ��")]
        AppGroup = 27,

        /// <summary>
        /// ����ҩ����֪ͨ
        /// </summary>
        [Enum("����ҩ����֪ͨ")]
        PrescriptionMedicine = 28,

        /// <summary>
        /// ���ԤԼ
        /// </summary>
        [Enum("���ԤԼ")]
        Optometry = 29
    }
}
