using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using Framework.Data;
using Keede.Ecsoft.Model;

namespace ERP.UI.Web.Common
{
    /// <summary>
    /// 存储一些常用常量
    /// </summary>
    public static class ConstList
    {
        /// <summary>
        /// 对账时候对账单Excel中快递单号的列名
        /// </summary>
        public const String EXPRESS_NO_FILED_NAME = "面单号";
        /// <summary>
        /// 对账时候对账单Excel中实收货款的列名
        /// </summary>
        public const String EXPRESS_MONEY_FIELD_NAME = "实收货款";

        /// <summary>
        /// 对账时候对账单Excel中订单称重的列名
        /// </summary>
        public const string EXPRESS_WEIGHT = "订单重量";

        /// <summary>
        /// 是否增加退货单为异常
        /// </summary>
        public const bool ADD_RETURN = true;

        public const decimal NORMAL_DIFF_MONEY = 5;

        public const String FILTER = "@*@";
    }
    /// <summary>
    /// Summary description for CheckingData
    /// </summary>
    public class CheckingData
    {
        public IList<CheckingDataInfo> GetCheckingData(SheetType sType, string strFilePath, params string[] columns)
        {
            if (columns.Length < 1)
            {
                throw new ApplicationException("列参数不能为个数为0");
            }
            IList<CheckingDataInfo> dataList = new List<CheckingDataInfo>();
            using (OleDbDataReader rdr = ExcelHelper.GetDataReader(sType, strFilePath))
            {
                try
                {

                    while (rdr.Read())
                    {
                        if (rdr[columns[0]] != DBNull.Value && rdr[columns[0]].ToString() != String.Empty)
                        {
                            var info = new CheckingDataInfo();
                            if (dataList.FirstOrDefault(d => d.ExpressNo == rdr[columns[0]].ToString()) == null)
                            {
                                try
                                {
                                    info.ExpressNo = rdr[columns[0]] == DBNull.Value
                                                         ? String.Empty
                                                         : rdr[columns[0]].ToString();
                                    int expresLength = info.ExpressNo.Length;
                                    if (info.ExpressNo != String.Empty && info.ExpressNo.IndexOf(ConstList.FILTER, StringComparison.Ordinal) == 0)
                                        info.ExpressNo = info.ExpressNo.Substring(3, expresLength - 3);
                                    info.Money = rdr[columns[1]] == DBNull.Value
                                                     ? String.Empty
                                                     : rdr[columns[1]].ToString();
                                }
                                catch (Exception ex)
                                {
                                    throw new ApplicationException(ex.Message + " 列名错误");
                                }
                                dataList.Add(info);
                            }
                            else
                            {
                                try
                                {
                                    info.ExpressNo = rdr[columns[0]] == DBNull.Value
                                                         ? String.Empty
                                                         : rdr[columns[0]].ToString();
                                    info.Money = rdr[columns[1]] == DBNull.Value
                                                     ? String.Empty
                                                     : rdr[columns[1]].ToString();
                                }
                                catch (Exception ex)
                                {
                                    throw new ApplicationException(ex.Message + " 列名错误");
                                }
                                info = dataList.FirstOrDefault(d => d.ExpressNo == rdr[columns[0]].ToString());
                                if (info != null)
                                {
                                    var newinfo = new CheckingDataInfo
                                    {
                                        ExpressNo = info.ExpressNo,
                                        Money = (decimal.Parse(info.Money) +
                                                decimal.Parse(rdr[columns[1]].ToString())).ToString(CultureInfo.InvariantCulture),
                                    };
                                    dataList.Remove(info);
                                    dataList.Add(newinfo);
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(ex.Message);
                }
                finally
                {
                    rdr.Close();
                }
            }
            return dataList;
        }

        public IList<CheckingDataInfo> GetCheckingData(String sheetName, SheetType sType, string strFilePath, params string[] columns)
        {
            if (columns.Length < 1)
            {
                throw new ApplicationException("列参数不能为个数为0");
            }
            IList<CheckingDataInfo> dataList = new List<CheckingDataInfo>();
            try
            {
                using (OleDbDataReader rdr = ExcelHelper.GetDataReader(sType, strFilePath, sheetName))
                {
                    try
                    {
                        while (rdr.Read())
                        {
                            if (rdr[columns[0]] != DBNull.Value && rdr[columns[0]].ToString() != String.Empty)
                            {
                                var info = new CheckingDataInfo();
                                if (dataList.FirstOrDefault(d => d.ExpressNo == rdr[columns[0]].ToString()) == null)
                                {
                                    try
                                    {
                                        info.ExpressNo = rdr[columns[0]] == DBNull.Value
                                                             ? String.Empty
                                                             : rdr[columns[0]].ToString();
                                        info.Money = rdr[columns[1]] == DBNull.Value
                                                         ? String.Empty
                                                         : rdr[columns[1]].ToString();
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new ApplicationException(ex.Message + " 列名错误");
                                    }
                                    dataList.Add(info);
                                }
                                else
                                {
                                    try
                                    {
                                        info.ExpressNo = rdr[columns[0]] == DBNull.Value
                                                             ? String.Empty
                                                             : rdr[columns[0]].ToString();
                                        info.Money = rdr[columns[1]] == DBNull.Value
                                                         ? String.Empty
                                                         : rdr[columns[1]].ToString();
                                    }
                                    catch (Exception ex)
                                    {
                                        throw new ApplicationException(ex.Message + " 列名错误");
                                    }
                                    info = dataList.FirstOrDefault(d => d.ExpressNo == rdr[columns[0]].ToString());
                                    if (info != null)
                                    {
                                        var newinfo = new CheckingDataInfo
                                                          {
                                                              ExpressNo = info.ExpressNo,
                                                              Money = (decimal.Parse(info.Money) +
                                                                      decimal.Parse(rdr[columns[1]].ToString())).ToString(CultureInfo.InvariantCulture)
                                                          };
                                        dataList.Remove(info);
                                        dataList.Add(newinfo);
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new Exception("处理异常", ex);
                    }
                    finally
                    {
                        rdr.Close();
                        rdr.Dispose();
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("处理异常", ex);
            }
            finally
            {
                GC.Collect();
            }
            return dataList;
        }

        /// <summary>
        /// 得到对账后保存在数据的Excel(新版本)
        /// </summary>
        /// <returns></returns>
        public IList<CheckInfo> GetNewCheckHistory(string sCheckingHistoryPath, DateTime dtStart, DateTime dtEnd, String checker, String checkCompany)
        {
            IList<CheckInfo> chList = new List<CheckInfo>();
            if (!File.Exists(sCheckingHistoryPath))
                throw new ApplicationException("not exists");

            var xd = new XmlDocument();
            xd.Load(sCheckingHistoryPath);

            XmlNode root = xd.SelectSingleNode("/root");
            if (root != null)
                foreach (XmlNode node in root.ChildNodes)
                {
                    var chInfo = new CheckInfo(new Guid(node.ChildNodes[0].InnerText), node.ChildNodes[1].InnerText, node.ChildNodes[2].InnerText, Convert.ToDateTime(node.ChildNodes[3].InnerText), node.ChildNodes[4].InnerText, node.ChildNodes[5].InnerText, node.ChildNodes[6].InnerText, node.ChildNodes[7].InnerText, node.ChildNodes[8].InnerText);
                    chInfo.SrcFileName = "UserDir/ZzzCheckingDataFolder/" + chInfo.CheckDate.Year + chInfo.CheckDate.Month + "/" + chInfo.SrcFileName;
                    chInfo.CheckFileName = "UserDir/ZzzCheckingHistoryFolder/" + chInfo.CheckDate.Year + chInfo.CheckDate.Month + chInfo.CheckFileName;
                    chInfo.ReckoningFileName = "UserDir/ZzzCheckingHistoryFolder/" + chInfo.CheckDate.Year + chInfo.CheckDate.Month + chInfo.ReckoningFileName;

                    bool isAdd = true;

                    if (checker != String.Empty)
                    {
                        if (!chInfo.CheckUser.Trim().Contains(checker))
                            isAdd = false;
                    }
                    if (checkCompany != String.Empty)
                    {
                        if (!chInfo.CompanyName.Trim().Contains(checkCompany))
                            isAdd = false;
                    }
                    if (chInfo.CheckDate > dtEnd || chInfo.CheckDate < dtStart)
                    {
                        isAdd = false;
                    }


                    if (isAdd)
                        chList.Add(chInfo);
                }
            return chList;
        }

        /// <summary>
        /// 供应商对账
        /// </summary>
        /// <param name="checkType">往来单据付款类型</param>
        /// <param name="receiptInfo">往来单位收付款</param>
        /// <param name="description">描述备注</param>
        /// <param name="reckoningInfos">待添加的往来帐</param>
        /// <param name="stockOrderNos">入库单据集合</param>
        /// <param name="removeNos"></param>
        /// <param name="receiveBill">收款单</param>
        /// <param name="errorMsg">错误信息</param>
        /// <returns></returns>
        public static bool CheckCompany(int checkType, CompanyFundReceiptInfo receiptInfo,
            string description, IList<ReckoningInfo> reckoningInfos,
            IList<string> stockOrderNos, IList<string> removeNos, out CompanyFundReceiptInfo receiveBill, out string errorMsg)
        {
            receiveBill = null;
            var codeManager = new CodeManager();
            ICompanyCussent companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Write);
            var companyCussentInfo = companyCussent.GetCompanyCussent(receiptInfo.CompanyID); //判断往来单位是否为内部公司
            if (companyCussentInfo == null)
            {
                errorMsg = "往来单位信息获取失败";
                return false;
            }
            var reckoningId = Guid.NewGuid();
            var reckoningInfo = new ReckoningInfo(reckoningId, receiptInfo.FilialeId, receiptInfo.CompanyID, codeManager.GetCode(CodeType.PY), description,
                                    receiptInfo.RealityBalance, (int)ReckoningType.Income,
                                    (int)ReckoningStateType.Currently,
                                    (int)CheckType.IsChecked, (int)AuditingState.Yes,
                                    receiptInfo.ReceiptNo, Guid.Empty)
            {
                IsOut = receiptInfo.IsOut
            };
            IReckoning reckoning=new Reckoning(GlobalConfig.DB.FromType.Write);
            var thirdCompanyInfo = companyCussent.GetCompanyByRelevanceFilialeId(receiptInfo.FilialeId);
            
            #region 如果公司内部付款时 生成对应的已完成的收款往来单据)  
            if (companyCussentInfo.RelevanceFilialeId != Guid.Empty)
            {
                if (thirdCompanyInfo == null)
                {
                    errorMsg = "付款公司未关联供应商";
                    return false;
                }
                receiveBill = new CompanyFundReceiptInfo
                {
                    FilialeId = companyCussentInfo.RelevanceFilialeId,
                    CompanyID = thirdCompanyInfo.CompanyId,
                    PayBankAccountsId = receiptInfo.ReceiveBankAccountId,
                    ReceiptType = (int) CompanyFundReceiptType.Receive,
                    ReceiptStatus = (int) CompanyFundReceiptState.Finish,
                    ReceiptID = Guid.NewGuid(),
                    ReceiptNo = codeManager.GetCode(CodeType.GT),
                    ApplyDateTime = receiptInfo.ApplyDateTime,
                    PurchaseOrderNo = "",
                    ReceiveBankAccountId = Guid.Empty,
                    StockOrderNos = "",
                    DealFlowNo = "",
                    FinishDate = DateTime.Now,
                    AuditingDate = receiptInfo.AuditingDate,
                    AuditorID = receiptInfo.AuditorID,
                    Poundage = 0,
                    ExecuteDateTime = DateTime.Now,
                    Remark = string.Format("[公司内部付款单:{0}]",receiptInfo.ReceiptNo),
                    OtherDiscountCaption = "",
                    DiscountCaption = "",
                    DiscountMoney = 0,
                    DebarStockNos = "",
                    LastRebate = 0,
                    ApplicantID = receiptInfo.ApplicantID,
                    AuditFailureReason = "",
                    CompanyName = thirdCompanyInfo.CompanyName,
                    ExpectBalance = receiptInfo.RealityBalance,
                    HasInvoice = receiptInfo.HasInvoice,
                    RealityBalance =receiptInfo.RealityBalance,
                    IncludeStockNos = "",
                    PaymentDate = receiptInfo.PaymentDate,
                    SettleStartDate= receiptInfo.SettleStartDate,
                    SettleEndDate=receiptInfo.SettleEndDate
                };
            }
            #endregion
            switch (checkType)
            {
                #region //按采购单进行付款   如果往来单位是关联内部公司：ex:可得向可镜采购  那么可得付完款平账完成  可镜收款完成 平账
                case 1:
                    var purchaseOrderNos = receiptInfo.PurchaseOrderNo.Split(',');
                    if (purchaseOrderNos.Length > 1)
                    {
                        IPurchasing purchasing=new Purchasing(GlobalConfig.DB.FromType.Read);
                        foreach (var purchaseOrderNo in purchaseOrderNos)
                        {
                            var totalPrice = purchasing.GetPurchasingAmount(purchaseOrderNo);
                            if (receiptInfo.DiscountMoney != 0)
                            {
                                totalPrice -= Math.Abs(receiptInfo.DiscountMoney);
                            }
                            if (receiptInfo.LastRebate != 0)
                            {
                                totalPrice -= Math.Abs(receiptInfo.LastRebate);
                            }
                            reckoningInfos.Add(new ReckoningInfo(Guid.NewGuid(), receiptInfo.FilialeId, receiptInfo.CompanyID, codeManager.GetCode(CodeType.PY), description,
                                    totalPrice, (int)ReckoningType.Income,
                                    (int)ReckoningStateType.Currently,
                                    (int)CheckType.IsChecked, (int)AuditingState.Yes,
                                    purchaseOrderNo, Guid.Empty)
                            {
                                IsOut = receiptInfo.IsOut,
                                LinkTradeType = (int)ReckoningLinkTradeType.PurchasingNo
                            });

                            if (companyCussentInfo.RelevanceFilialeId != Guid.Empty)
                            {
                                reckoningInfos.Add(new ReckoningInfo(Guid.NewGuid(), companyCussentInfo.RelevanceFilialeId, thirdCompanyInfo.CompanyId, codeManager.GetCode(CodeType.PY), description,
                                    -totalPrice, (int)ReckoningType.Defray,
                                    (int)ReckoningStateType.Currently,
                                    (int)CheckType.IsChecked, (int)AuditingState.Yes,
                                    purchaseOrderNo, Guid.Empty)
                                {
                                    IsOut = receiptInfo.IsOut,
                                    LinkTradeType = (int)ReckoningLinkTradeType.PurchasingNo
                                });
                            }
                        }
                        //将物流公司销售出库给销售公司的往来账平掉
                        reckoning.CheckByPurchaseOrder(purchaseOrderNos, reckoningInfo.FilialeId, receiptInfo.CompanyID, receiptInfo.SettleStartDate);
                        if (companyCussentInfo.RelevanceFilialeId != Guid.Empty)
                        {
                            reckoning.CheckByPurchaseOrder(purchaseOrderNos, companyCussentInfo.RelevanceFilialeId, thirdCompanyInfo.CompanyId, receiptInfo.SettleStartDate);
                        }
                    }
                    else
                    {
                        reckoningInfo.LinkTradeCode = receiptInfo.PurchaseOrderNo;
                        reckoningInfo.LinkTradeType = (int)ReckoningLinkTradeType.PurchasingNo;
                        reckoningInfos.Add(reckoningInfo);
                        //将物流公司销售出库给销售公司的往来账平掉
                        reckoning.CheckByPurchaseOrder(purchaseOrderNos, reckoningInfo.FilialeId, receiptInfo.CompanyID, receiptInfo.SettleStartDate);
                        if (companyCussentInfo.RelevanceFilialeId != Guid.Empty)
                        {
                            reckoningInfos.Add(new ReckoningInfo(Guid.NewGuid(), companyCussentInfo.RelevanceFilialeId, thirdCompanyInfo.CompanyId, codeManager.GetCode(CodeType.PY), description,
                                    -receiptInfo.RealityBalance, (int)ReckoningType.Defray,
                                    (int)ReckoningStateType.Currently,
                                    (int)CheckType.IsChecked, (int)AuditingState.Yes,
                                     receiptInfo.PurchaseOrderNo, Guid.Empty)
                            {
                                IsOut = receiptInfo.IsOut,
                                LinkTradeType = (int)ReckoningLinkTradeType.PurchasingNo
                            });
                            reckoning.CheckByPurchaseOrder(purchaseOrderNos, companyCussentInfo.RelevanceFilialeId, receiptInfo.CompanyID, receiptInfo.SettleStartDate);
                        }    
                    }

                    break;
                #endregion

                case 2:  //按入库单进行付款
                    var stockNos = receiptInfo.StockOrderNos.Split(',');
                    reckoning.CheckByStorageTradeCode(stockNos, reckoningInfo.FilialeId, receiptInfo.CompanyID, receiptInfo.SettleStartDate);
                    reckoningInfo.LinkTradeType = (int)ReckoningLinkTradeType.StockIn;
                    reckoningInfos.Add(reckoningInfo);

                    if (companyCussentInfo.RelevanceFilialeId != Guid.Empty)
                    {
                        reckoningInfos.Add(new ReckoningInfo(Guid.NewGuid(), companyCussentInfo.RelevanceFilialeId, thirdCompanyInfo.CompanyId, codeManager.GetCode(CodeType.PY), description,
                                    -receiptInfo.RealityBalance, (int)ReckoningType.Defray,
                                    (int)ReckoningStateType.Currently,
                                    (int)CheckType.IsChecked, (int)AuditingState.Yes,
                                    receiptInfo.ReceiptNo, Guid.Empty)
                        {
                            IsOut = receiptInfo.IsOut
                        });
                        reckoning.CheckByStorageTradeCode(stockNos, companyCussentInfo.RelevanceFilialeId, thirdCompanyInfo.CompanyId, receiptInfo.SettleStartDate);
                    }
                    break;
                case 3: //按日期付款  
                    reckoningInfo.LinkTradeType = (int)ReckoningLinkTradeType.CompanyFundReceipt;
                    var result = reckoning.CheckByDate(receiptInfo.CompanyID,  reckoningInfo.FilialeId ,receiptInfo.SettleStartDate, receiptInfo.SettleEndDate.AddDays(1).AddSeconds(-1),(int)CheckType.NotCheck, stockOrderNos, removeNos);
                    if (result)
                    {
                        if (companyCussentInfo.RelevanceFilialeId != Guid.Empty)
                        {
                            reckoningInfos.Add(new ReckoningInfo(Guid.NewGuid(), companyCussentInfo.RelevanceFilialeId, thirdCompanyInfo.CompanyId, codeManager.GetCode(CodeType.PY), description,
                                        -receiptInfo.RealityBalance, (int)ReckoningType.Defray,
                                        (int)ReckoningStateType.Currently,
                                        (int)CheckType.IsChecked, (int)AuditingState.Yes,
                                        receiptInfo.ReceiptNo, Guid.Empty)
                            {
                                IsOut = receiptInfo.IsOut
                            });
                            //反向更新往来单位和公司的帐务
                            result = reckoning.CheckByDate(thirdCompanyInfo.CompanyId, companyCussentInfo.RelevanceFilialeId, receiptInfo.SettleStartDate, receiptInfo.SettleEndDate.AddDays(1).AddSeconds(-1), (int)CheckType.NotCheck, stockOrderNos, removeNos);
                            if (!result)
                            {
                                errorMsg = "反向更新往来单位和公司间往来帐状态失败!";
                                return false;
                            }
                        }
                    }
                    else
                    {
                        errorMsg = "按日期结算往来帐查询无效!";
                        return false;
                    }
                    reckoningInfos.Add(reckoningInfo);
                    break;
                default:
                    if (companyCussentInfo.RelevanceFilialeId != Guid.Empty)
                    {
                        reckoningInfos.Add(new ReckoningInfo(Guid.NewGuid(), companyCussentInfo.RelevanceFilialeId, thirdCompanyInfo.CompanyId, codeManager.GetCode(CodeType.PY), description,
                                    -receiptInfo.RealityBalance, (int)ReckoningType.Defray,
                                    (int)ReckoningStateType.Currently,
                                    (int)CheckType.IsChecked, (int)AuditingState.Yes,
                                    receiptInfo.ReceiptNo, Guid.Empty)
                        {
                            IsOut = receiptInfo.IsOut
                        });
                    }
                    reckoningInfo.LinkTradeType = (int)ReckoningLinkTradeType.CompanyFundReceipt;
                    reckoningInfos.Add(reckoningInfo);
                    break;
            }
            errorMsg = "";
            return true;
        }
    }
}