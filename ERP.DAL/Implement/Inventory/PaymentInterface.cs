using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using Keede.DAL.Helper;
using Keede.Ecsoft.Model;

namespace ERP.DAL.Implement.Inventory
{
    public class PaymentInterface : IPaymentInterface
    {
        public PaymentInterface(Environment.GlobalConfig.DB.FromType fromType)
        {

        }

        private const string SQL_SELECT_PAYMENTINTERFACE = "SELECT PaymentInterfaceId,PaymentInterfaceName FROM lmShop_PaymentInterface WHERE PaymentInterfaceId=@PaymentInterfaceId;";
        private const string SQL_SELECT_PAYMENTINTERFACE_LIST = "SELECT PaymentInterfaceId,PaymentInterfaceName FROM lmShop_PaymentInterface;";
        //ȡ֧�����ʻ���ID
        private const string SQL_SELECT_BANKACCOUNTSID = "SELECT BankAccountsId FROM lmShop_BankAccounts WHERE PaymentInterfaceId=(SELECT PaymentInterfaceId FROM lmShop_PaymentInterface where PaymentInterfaceName='֧����')";
        private const string PARM_PAYMENTINTERFACEID = "@PaymentInterfaceId";

        /// <summary>
        /// ȡ֧�����ʻ���ID
        /// </summary>
        /// <returns></returns>
        public Guid GetBandAccountsId()
        {
            Guid bandId=Guid.Empty;
            try
            {
                object obj = SqlHelper.ExecuteScalar(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_BANKACCOUNTSID, null);
                if (obj != DBNull.Value)
                    bandId = new Guid(obj.ToString());
            }
            catch(Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
            return bandId;
        }
        /// <summary>
        /// ��ȡָ��������֧���ӿ�
        /// </summary>
        /// <param name="paymentInterfaceId">����֧���ӿڱ��</param>
        /// <returns></returns>
        public PaymentInterfaceInfo GetPaymentInterface(Guid paymentInterfaceId)
        {
            PaymentInterfaceInfo paymentInterfaceInfo;
            var parm = new SqlParameter(PARM_PAYMENTINTERFACEID, SqlDbType.UniqueIdentifier)
                           {Value = paymentInterfaceId};

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_PAYMENTINTERFACE, parm))
            {
                paymentInterfaceInfo = rdr.Read() ? new PaymentInterfaceInfo(rdr.GetGuid(0), rdr.GetString(1)) : new PaymentInterfaceInfo();
            }
            return paymentInterfaceInfo;
        }

        /// <summary>
        /// ��ȡ����֧���ӿ���Ϣ��
        /// </summary>
        /// <returns></returns>
        public IList<PaymentInterfaceInfo> GetPaymentInterfaceList()
        {
            IList<PaymentInterfaceInfo> paymentInterfaceInfoList = new List<PaymentInterfaceInfo>();

            using (var rdr = SqlHelper.ExecuteReader(GlobalConfig.ERP_DB_NAME, true, SQL_SELECT_PAYMENTINTERFACE_LIST, null))
            {
                while (rdr.Read())
                {
                    var paymentInterfaceInfo = new PaymentInterfaceInfo(rdr.GetGuid(0), rdr.GetString(1));
                    paymentInterfaceInfoList.Add(paymentInterfaceInfo);
                }
            }
            return paymentInterfaceInfoList;
        }
    }
}
