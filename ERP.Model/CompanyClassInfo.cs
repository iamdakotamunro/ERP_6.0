//================================================
// Դ��������˾:�Ϻ���ý��Ϣ�������޹�˾
// �ļ�����ʱ��:2006��4��27��
// �ļ�������:����
// ����޸�ʱ��:2006��4��27��
// ���һ���޸���:����
//================================================
using System;
using System.Runtime.Serialization;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// ������λ����
    /// </summary>
    [Serializable]
    [DataContract]
    public class CompanyClassInfo
    {
        //�����ֶ�

        /// <summary>
        /// Ĭ�Ϲ��캯��
        /// </summary>
        public CompanyClassInfo() { }

        /// <summary>
        /// ��ʼ�����캯��
        /// </summary>
        /// <param name="companyClassId">������λ����Id</param>
        /// <param name="parentCompanyClassId">������λ��������</param>
        /// <param name="companyClassCode">������λ���</param>
        /// <param name="companyClassName">������λ����</param>
        public CompanyClassInfo(Guid companyClassId, Guid parentCompanyClassId, string companyClassCode, string companyClassName)
        {
            CompanyClassId = companyClassId;
            ParentCompanyClassId = parentCompanyClassId;
            CompanyClassCode = companyClassCode;
            CompanyClassName = companyClassName;
        }

        /// <summary>
        /// ������λ������
        /// </summary>
        [DataMember]
        public Guid CompanyClassId { get; private set; }

        /// <summary>
        /// ������λ��������
        /// </summary>
        [DataMember]
        public Guid ParentCompanyClassId { get; set; }

        /// <summary>
        /// ������λ�������
        /// </summary>
        [DataMember]
        public string CompanyClassCode { get; set; }

        /// <summary>
        /// ������λ��������
        /// </summary>
        [DataMember]
        public string CompanyClassName { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="compareObj"></param>
        /// <returns></returns>
        public override bool Equals(object compareObj)
        {
            if (compareObj is CompanyClassInfo)
                return (compareObj as CompanyClassInfo).CompanyClassId == CompanyClassId;
            return base.Equals(compareObj);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            if (CompanyClassId == Guid.Empty) return base.GetHashCode();
            string stringRepresentation = System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.FullName + "#" + CompanyClassId.ToString();
            return stringRepresentation.GetHashCode();
        }
    }
}
