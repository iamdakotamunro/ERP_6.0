using System;
using System.Collections.Generic;
using ERP.BLL.Implement.Company;
using ERP.Enum;
using ERP.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.BLL.UnitTest
{
    [TestClass]
    public class UnitTest_QualificationManager
    {
        readonly QualificationManager _qualificationManager=new QualificationManager();

        /// <summary>
        /// 判断是否资料完整
        /// </summary>
        [TestMethod]
        public void TestIsComplete()
        {
            //没有资质
            IList<SupplierInformationInfo> dataList =new List<SupplierInformationInfo>();
            var result = _qualificationManager.IsComplete(dataList);
            Assert.IsTrue(result==(int)SupplierCompleteType.NoComplete);

            //有资质、不完整或者完整但过期
            dataList.Add(new SupplierInformationInfo
            {
                QualificationType = (int)SupplierQualificationType.TaxRegistrationCertificate,
                OverdueDate = DateTime.Now
            });
            var result1 = _qualificationManager.IsComplete(dataList);
            Assert.IsTrue(result1 == (int)SupplierCompleteType.NoComplete);

            dataList.Add(new SupplierInformationInfo
            {
                QualificationType = (int)SupplierQualificationType.BusinessLicense,
                OverdueDate = DateTime.Now
            });
            var result2 = _qualificationManager.IsComplete(dataList);
            Assert.IsTrue(result2 == (int)SupplierCompleteType.NoComplete);

            dataList.Add(new SupplierInformationInfo
            {
                QualificationType = (int)SupplierQualificationType.OrganizationCodeCertificate,
                OverdueDate = DateTime.Now.AddDays(-10)
            });
            var result3 = _qualificationManager.IsComplete(dataList);
            Assert.IsTrue(result3 ==(int)SupplierCompleteType.NoComplete);

            //有资质、完整
            dataList=new List<SupplierInformationInfo>{
                new SupplierInformationInfo
                {
                    QualificationType = (int)SupplierQualificationType.BusinessLicense,
                    OverdueDate = DateTime.Now
                },
                new SupplierInformationInfo
                {
                    QualificationType = (int)SupplierQualificationType.OrganizationCodeCertificate,
                    OverdueDate = DateTime.Now
                },
                new SupplierInformationInfo
                {
                    QualificationType = (int)SupplierQualificationType.TaxRegistrationCertificate,
                    OverdueDate = DateTime.Now
                }
            };
            var result7 = _qualificationManager.IsComplete(dataList);
            Assert.IsTrue(result7 ==(int)SupplierCompleteType.Complete);
        }

        /// <summary>
        /// 判断是否资料过期
        /// </summary>
        [TestMethod]
        public void TestIsExpire()
        {
            IList<SupplierInformationInfo> dataList = new List<SupplierInformationInfo>();
            var result = _qualificationManager.IsExpire(dataList, 30);
            Assert.IsTrue(result=="0");

            dataList.Add(new SupplierInformationInfo
            {
                QualificationType = (int)SupplierQualificationType.BusinessLicense,
                OverdueDate = DateTime.Now.AddDays(-10)
            });
            var result1 = _qualificationManager.IsExpire(dataList, 30);
            Assert.IsTrue(result1 == "3");

            dataList.Add(new SupplierInformationInfo
            {
                QualificationType = (int)SupplierQualificationType.TaxRegistrationCertificate,
                OverdueDate = DateTime.Now.AddDays(10)
            });
            var result2 = _qualificationManager.IsExpire(dataList, 30);
            Assert.IsTrue(result2 == "3,2");

            dataList.Add(new SupplierInformationInfo
            {
                QualificationType = (int)SupplierQualificationType.MedicalDeviceBusinessLicense,
                OverdueDate = DateTime.Now.AddDays(60)
            });
            var result3= _qualificationManager.IsExpire(dataList, 30);
            Assert.IsTrue(result3== "3,2,1");

            dataList.Add(new SupplierInformationInfo
            {
                QualificationType = (int)SupplierQualificationType.OrganizationCodeCertificate,
                OverdueDate = DateTime.Now.AddDays(50)
            });
            var result4 = _qualificationManager.IsExpire(dataList, 30);
            Assert.IsTrue(result4== "3,2,1");
        }
    }
}
