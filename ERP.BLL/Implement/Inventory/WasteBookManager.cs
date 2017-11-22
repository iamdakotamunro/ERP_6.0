using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IInventory;
using ERP.Model;

namespace ERP.BLL.Implement.Inventory
{
    /// <summary>�{��ߩרTһ �ʽ���ҵ���  ����޸��ύ  ������  2014-12-25  ��ȫ���¹�˾��ϵSQL�����Ż���ȥ�����÷�����
    /// </summary>
    public class WasteBookManager : BllInstance<WasteBookManager>
    {
        readonly IWasteBook _wasteBookDao;

        public WasteBookManager(Environment.GlobalConfig.DB.FromType fromType)
        {
            _wasteBookDao = InventoryInstance.GetWasteBookDao(fromType);
        }

        public WasteBookManager(IWasteBook wasteBook)
        {
            _wasteBookDao = wasteBook;
        }

        #region ��ȡ�ʽ��� ��ط���

        /// <summary>
        /// ��ȡ��˾�ʽ������б�
        /// </summary>
        /// <param name="keepyear"></param>
        /// <param name="year"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="bankName"></param>
        /// <returns></returns>
        public IList<FundPaymentDaysInfo> GetFundPaymentDaysInfos(int keepyear,int year, Guid saleFilialeId, string bankName)
        {
            var fundPaymentDaysInfoList = new List<FundPaymentDaysInfo>();
            var lastFundInfoList = _wasteBookDao.GetFundPaymentDaysBankInfos(keepyear,saleFilialeId, year, bankName);//��ĩ���
            var inFundInfoList= _wasteBookDao.GetFundPaymentDaysInfos(keepyear,year,saleFilialeId,bankName);//�տ����list
            if ((lastFundInfoList == null || lastFundInfoList.Count == 0) &&
                (inFundInfoList == null || inFundInfoList.Count == 0)) return fundPaymentDaysInfoList;//��û�н��
            if ((lastFundInfoList != null && lastFundInfoList.Count > 0) &&
                (inFundInfoList == null || inFundInfoList.Count == 0))//��ĩ�����
            {
                fundPaymentDaysInfoList.AddRange(
                    lastFundInfoList.Select(fundPaymentDaysInfo => new FundPaymentDaysInfo
                    {
                        BankName = fundPaymentDaysInfo.BankName,
                        BankAccountsId = fundPaymentDaysInfo.BankAccountsId,
                        MaxJan = fundPaymentDaysInfo.MaxJan,
                        MaxFeb = fundPaymentDaysInfo.MaxFeb,
                        MaxMar = fundPaymentDaysInfo.MaxMar,
                        MaxApr = fundPaymentDaysInfo.MaxApr,
                        MaxMay = fundPaymentDaysInfo.MaxMay,
                        MaxJun = fundPaymentDaysInfo.MaxJun,
                        MaxJuly = fundPaymentDaysInfo.MaxJuly,
                        MaxAug = fundPaymentDaysInfo.MaxAug,
                        MaxSept = fundPaymentDaysInfo.MaxSept,
                        MaxOct = fundPaymentDaysInfo.MaxOct,
                        MaxNov = fundPaymentDaysInfo.MaxNov,
                        MaxDecember = fundPaymentDaysInfo.MaxDecember
                    }));
                return fundPaymentDaysInfoList;
            }
            if ((lastFundInfoList == null || lastFundInfoList.Count == 0) &&
                (inFundInfoList != null && inFundInfoList.Count > 0))//���տ�
            {
                fundPaymentDaysInfoList.AddRange(
                    inFundInfoList.Select(fundPaymentDaysInfo => new FundPaymentDaysInfo
                    {
                        BankName = fundPaymentDaysInfo.BankName,
                        BankAccountsId = fundPaymentDaysInfo.BankAccountsId,
                        Jan = fundPaymentDaysInfo.Jan,
                        Feb = fundPaymentDaysInfo.Feb,
                        Mar = fundPaymentDaysInfo.Mar,
                        Apr = fundPaymentDaysInfo.Apr,
                        May = fundPaymentDaysInfo.May,
                        Jun = fundPaymentDaysInfo.Jun,
                        July = fundPaymentDaysInfo.July,
                        Aug = fundPaymentDaysInfo.Aug,
                        Sept = fundPaymentDaysInfo.Sept,
                        Oct = fundPaymentDaysInfo.Oct,
                        Nov = fundPaymentDaysInfo.Nov,
                        December = fundPaymentDaysInfo.December
                    }));
                return fundPaymentDaysInfoList;
            }
            if (lastFundInfoList!=null && lastFundInfoList.Count>0 && inFundInfoList!=null)//����
            {
                var newFundPaymentDaysInfoList = new List<FundPaymentDaysInfo>();
                newFundPaymentDaysInfoList.AddRange(
                    lastFundInfoList.Select(fundPaymentDaysInfo => new FundPaymentDaysInfo
                    {
                        BankName = fundPaymentDaysInfo.BankName,
                        BankAccountsId = fundPaymentDaysInfo.BankAccountsId,
                        MaxJan = fundPaymentDaysInfo.MaxJan,
                        MaxFeb = fundPaymentDaysInfo.MaxFeb,
                        MaxMar = fundPaymentDaysInfo.MaxMar,
                        MaxApr = fundPaymentDaysInfo.MaxApr,
                        MaxMay = fundPaymentDaysInfo.MaxMay,
                        MaxJun = fundPaymentDaysInfo.MaxJun,
                        MaxJuly = fundPaymentDaysInfo.MaxJuly,
                        MaxAug = fundPaymentDaysInfo.MaxAug,
                        MaxSept = fundPaymentDaysInfo.MaxSept,
                        MaxOct = fundPaymentDaysInfo.MaxOct,
                        MaxNov = fundPaymentDaysInfo.MaxNov,
                        MaxDecember = fundPaymentDaysInfo.MaxDecember
                    }));
                fundPaymentDaysInfoList=newFundPaymentDaysInfoList;
                foreach (var fundPaymentDaysInfo in inFundInfoList)
                {
                    var bankId = newFundPaymentDaysInfoList.FirstOrDefault(f => f.BankAccountsId == fundPaymentDaysInfo.BankAccountsId);
                    if (bankId != null)
                    {
                        bankId.Jan = fundPaymentDaysInfo.Jan;
                        bankId.Feb = fundPaymentDaysInfo.Feb;
                        bankId.Mar = fundPaymentDaysInfo.Mar;
                        bankId.Apr = fundPaymentDaysInfo.Apr;
                        bankId.May = fundPaymentDaysInfo.May;
                        bankId.Jun = fundPaymentDaysInfo.Jun;
                        bankId.July = fundPaymentDaysInfo.July;
                        bankId.Aug = fundPaymentDaysInfo.Aug;
                        bankId.Sept = fundPaymentDaysInfo.Sept;
                        bankId.Oct = fundPaymentDaysInfo.Oct;
                        bankId.Nov = fundPaymentDaysInfo.Nov;
                        bankId.December = fundPaymentDaysInfo.December;
                    }
                    else
                    {
                        bankId=new FundPaymentDaysInfo
                        {
                            BankName = fundPaymentDaysInfo.BankName,
                            BankAccountsId = fundPaymentDaysInfo.BankAccountsId,
                            Jan = fundPaymentDaysInfo.Jan,
                            Feb = fundPaymentDaysInfo.Feb,
                            Mar = fundPaymentDaysInfo.Mar,
                            Apr = fundPaymentDaysInfo.Apr,
                            May = fundPaymentDaysInfo.May,
                            Jun = fundPaymentDaysInfo.Jun,
                            July = fundPaymentDaysInfo.July,
                            Aug = fundPaymentDaysInfo.Aug,
                            Sept = fundPaymentDaysInfo.Sept,
                            Oct = fundPaymentDaysInfo.Oct,
                            Nov = fundPaymentDaysInfo.Nov,
                            December = fundPaymentDaysInfo.December
                        };
                    }
                    var fundInfo = fundPaymentDaysInfoList.FirstOrDefault(f => f.BankAccountsId == bankId.BankAccountsId);
                    if (fundInfo!=null)
                    {
                        fundPaymentDaysInfoList.Remove(fundInfo);
                    }
                    fundPaymentDaysInfoList.Add(bankId);
                }
            }
            return fundPaymentDaysInfoList; 
        }
        #endregion
    }
}
