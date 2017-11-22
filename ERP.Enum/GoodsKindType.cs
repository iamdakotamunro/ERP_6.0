using System;
using ERP.Enum.Attribute;

namespace ERP.Enum
{
    /// <summary>
    /// ��Ʒ�������ö��,���ݲ�ѯ������Ϣ������
    /// </summary>
    [Serializable]
    public enum GoodsKindType
    {
        /// <summary>
        /// ������,ȫ������
        /// </summary>
        [EnumAttribute("δ����")]
        NoSet = 0,
        /// <summary>
        /// �����۾�
        /// </summary>
        [EnumAttribute("�����۾�")]
        ContactLenses = 1,
        /// <summary>
        /// ��ɫ�����۾�
        /// </summary>
        [EnumAttribute("��ɫ�����۾�")]
        ColorContactLenses = 2,
        /// <summary>
        /// ����Һ
        /// </summary>
        [EnumAttribute("����Һ")]
        LensSolution = 3,
        /// <summary>
        /// ����۾�
        /// </summary>
        [EnumAttribute("����۾�")]
        Frames = 4,
        /// <summary>
        /// ��Ƭ
        /// </summary>
        [EnumAttribute("��Ƭ")]
        Glasse = 5,
        /// <summary>
        /// ̫���۾�
        /// </summary>
        [EnumAttribute("̫���۾�")]
        SunFrame = 6,
        /// <summary>
        /// �˶��۾�
        /// </summary>
        [EnumAttribute("�˶��۾�")]
        SportFrame = 7,
        /// <summary>
        /// �����۾�
        /// </summary>
        [EnumAttribute("�����۾�")]
        FunctionFrame = 8,
        /// <summary>
        /// ������Ʒ
        /// </summary>
        [EnumAttribute("������Ʒ")]
        CareProducts = 9,
        /// <summary>
        /// ����
        /// </summary>
        [EnumAttribute("����")]
        Other = 10,

        ///// <summary>
        ///// �����Ʒ
        ///// </summary>
        //[EnumArrtibute("�����Ʒ")]
        //PackGoods = 11,

        /// <summary>
        /// �Ǵ���ҩ
        /// </summary>
        [EnumAttribute("�Ǵ���ҩ")]
        Medicinal = 12,

        /// <summary>
        /// ����ʳƷ/ʳƷ
        /// </summary>
        [EnumAttribute("����ʳƷ/ʳƷ")]
        HealthProducts = 13,

        /// <summary>
        /// ��ױ����Ʒ
        /// </summary>
        [EnumAttribute("��ױ����Ʒ")]
        CosmeticSkinCare = 14,

        /// <summary>
        /// ������Ʒ
        /// </summary>
        [EnumAttribute("������Ʒ")]
        FamilyPlanningProuducts = 15,

        /// <summary>
        /// ҽ����е
        /// </summary>
        [EnumAttribute("ҽ����е")]
        MedicalEquipment = 16,

        /// <summary>
        /// ����ҩ
        /// </summary>
        [EnumAttribute("����ҩ")]
        PrescriptionMedicine = 17,

        /// <summary>
        /// ��ҩ��Ƭ/����
        /// </summary>
        [EnumAttribute("��ҩ��Ƭ/����")]
        ChineseMedicineYinPian = 18,

        /// <summary>
        /// ��ɱ����Ʒ
        /// </summary>
        [EnumAttribute("��ɱ����Ʒ")]
        DisinfectionTypeGoods = 19,
    }
}
