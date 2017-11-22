using System;
using System.Runtime.Serialization;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// ���񵥱�������ö��
    /// </summary>
    [Serializable]
    [DataContract]
    public enum CodeType
    {
        /// <summary>
        /// ���Ϲ��򣬶���
        /// </summary>
        [Enum("���Ϲ��򣬶���")]
        [EnumMember]
        RT = 1,
        /// <summary>
        /// ��Ա�ʺų�ֵ
        /// </summary>
        [EnumMember]
        [Enum("��Ա�ʺų�ֵ")]
        RF = 2,
        /// <summary>
        /// ��Ա�ʺ��˿�
        /// </summary>
        [Enum("��Ա�ʺ��˿�")]
        [EnumMember]
        AV = 3,
        /// <summary>
        /// ������λ����Ա�ʺŵ��ˣ���������
        /// </summary>
        [Enum("������λ����Ա�ʺŵ��ˣ���������")]
        [EnumMember]
        AJ = 4,
        /// <summary>
        /// �ʽ��˻������ʽ�
        /// </summary>
        [Enum("�ʽ��˻������ʽ�")]
        [EnumMember]
        GI = 5,
        /// <summary>
        /// �ʽ��˻������ʽ�
        /// </summary>
        [Enum("�ʽ��˻������ʽ�")]
        [EnumMember]
        RD = 6,
        /// <summary>
        /// �ʽ��˻�ת��
        /// </summary>
        [Enum("�ʽ��˻�ת��")]
        [EnumMember]
        VI = 7,
        /// <summary>
        /// ��λ�����˸���
        /// </summary>
        [Enum("��λ�����˸���")]
        [EnumMember]
        PY = 8,
        /// <summary>
        /// ��λ�������տ�
        /// </summary>
        [Enum("��λ�������տ�")]
        [EnumMember]
        GT = 9,
        /// <summary>
        /// �������֧��
        /// </summary>
        [Enum("�������֧��")]
        [EnumMember]
        RK = 10,
        /// <summary>
        /// ���۳���
        /// </summary>
        [Enum("���۳���")]
        [EnumMember]
        SL = 11,
        /// <summary>
        /// --��δ��
        /// </summary>
        [EnumMember]
        [Enum("��δ��")]
        LR = 12,
        /// <summary>
        /// �����˻���
        /// </summary>
        [Enum("�����˻���")]
        [EnumMember]
        SO = 13,
        /// <summary>
        /// �����˻���
        /// </summary>
        [Enum("�����˻���")]
        [EnumMember]
        SI = 14,
        /// <summary>
        /// ��Ʒ��Ӯ��
        /// </summary>
        [Enum("��Ʒ��Ӯ��")]
        [EnumMember]
        BS = 15,
        /// <summary>
        /// ��Ʒ�̿���
        /// </summary>
        [Enum("��Ʒ�̿���")]
        [EnumMember]
        LS = 16,
        /// <summary>
        /// ������ⵥ
        /// </summary>
        [Enum("������ⵥ")]
        [EnumMember]
        TSI = 17,
        /// <summary>
        /// �������ⵥ
        /// </summary>
        [Enum("�������ⵥ")]
        [EnumMember]
        TSO = 18,
        ///// <summary>
        ///// Eyesee���Ϲ��򣬶���
        ///// </summary>
        //[EnumArrtibute("Eyesee���Ϲ��򣬶���")]
        //[EnumMember]
        //SRT = 19,
        /// <summary>
        /// �����걨��
        /// </summary>
        [Enum("�����걨��")]
        [EnumMember]
        RE = 20,
        /// <summary>
        /// �ɹ���
        /// </summary>
        [Enum("�ɹ���")]
        [EnumMember]
        PH = 21,
        /// <summary>
        /// �̵�ƻ���
        /// </summary>
        [Enum("�̵�ƻ���")]
        [EnumMember]
        IV = 22,
        /// <summary>
        /// ��Ա���ֵ�
        /// </summary>
        [Enum("��Ա���ֵ�")]
        [EnumMember]
        MM = 23,
        /// <summary>
        /// ������ˮ���
        /// </summary>
        [Enum("������ˮ���")]
        [EnumMember]
        SR = 24,
        /// <summary>
        /// �˻�����
        /// </summary>
        [Enum("�˻�����")]
        [EnumMember]
        TH = 26,

        /// <summary>
        /// ����������ⵥ
        /// </summary>
        [Enum("����������ⵥ")]
        [EnumMember]
        TR = 27,

        /// <summary>
        /// ���ԭ·�˻أ��������ϣ�,����SDB
        /// </summary>
        [Enum("���ԭ·�˻أ��������ϣ�,����SDB")]
        [EnumMember]
        DB = 28,

        /// <summary>
        /// �ӹ���
        /// </summary>
        [Enum("�ӹ���")]
        [EnumMember]
        LP = 29,

        /// <summary>
        /// ��Ʒ��ϲ��
        /// </summary>
        [Enum("��Ʒ��ϲ��")]
        [EnumMember]
        MS = 30,

        /// <summary>
        /// ���뵥
        /// </summary>
        [Enum("���뵥")]
        [EnumMember]
        BI = 31,

        /// <summary>
        /// ���뷵����
        /// </summary>
        [Enum("���뷵����")]
        [EnumMember]
        BO = 32,

        /// <summary>
        /// �����
        /// </summary>
        [Enum("�����")]
        [EnumMember]
        LO = 33,

        /// <summary>
        /// ���������
        /// </summary>
        [Enum("���������")]
        [EnumMember]
        LI = 34,

        /// <summary>
        /// ������è������
        /// </summary>
        [Enum("������è������")]
        [EnumMember]
        JT = 35,

        /// <summary>�����
        /// </summary>
        [Enum("�����")]
        [EnumMember]
        CI = 36,

        /// <summary>������
        /// </summary>
        [Enum("������")]
        [EnumMember]
        CO = 37,

        /// <summary>������è�궩����
        /// </summary>
        [Enum("������è�궩����")]
        [EnumMember]
        HD = 38,

        /// <summary>
        /// ��������
        /// </summary>
        [Enum("��������")]
        [EnumMember]
        HH = 39,

        /// <summary>
        /// �ڲ��ɹ�
        /// </summary>
        [Enum("�ڲ��ɹ�")]
        [EnumMember]
        CC = 40,
    }
}
