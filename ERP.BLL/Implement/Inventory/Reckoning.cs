using System;
using System.Collections.Generic;
using System.Transactions;
using ERP.DAL.Factory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Implement.Inventory
{
    public class ReckoningManager : BllInstance<ReckoningManager>
    {
        static IReckoning _reckoningDao;
        readonly IWasteBook _wasteBookManager;

        public ReckoningManager(Environment.GlobalConfig.DB.FromType fromType)
        {
            _wasteBookManager = new WasteBook(fromType);
            _reckoningDao = InventoryInstance.GetReckoningDao(fromType);
        }

        public ReckoningManager(IReckoning reckoning,IWasteBook wasteBook)
        {
            _wasteBookManager = wasteBook;
            _reckoningDao = reckoning;
        }

        /// <summary>
        /// ����
        /// </summary>
        /// <param name="lstModify">��Ҫ�޸�״̬���������б�</param>
        /// <param name="lstAdd">��Ҫ���ӵ��������б�</param>
        /// <param name="wastBookinfo">��Ҫ��ӵ��ʽ�����Ϣ</param>
        public void Checking(IList<ReckoningInfo> lstModify, IList<ReckoningInfo> lstAdd, WasteBookInfo wastBookinfo)
        {
            using (var scop = new TransactionScope())
            {
                try
                {
                    //1.���±���������Ϊ�Ѷ���
                    if (lstModify.Count > 0)
                        _reckoningDao.UpdateCheckState(lstModify, 1);

                    //2.���������ˣ��������ˣ�
                    foreach (ReckoningInfo reckoningInfo in lstAdd)
                    {
                        reckoningInfo.AuditingState = (int)AuditingState.Yes;
                        string errorMessage;
                        _reckoningDao.Insert(reckoningInfo, out errorMessage);
                    }

                    //3.�ʽ��� ����һ������
                    if (wastBookinfo.Income != 0)
                    {
                        _wasteBookManager.Insert(wastBookinfo);
                    }
                    scop.Complete();
                }
                catch (Exception ex)
                {
                    throw new Exception("����ʧ��!", ex);
                }
            }

        }
    }
}
