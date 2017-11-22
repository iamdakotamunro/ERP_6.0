using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Data;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class ManuallyCheckBillDetail : WindowsPage
    {
        private readonly IManuallyCheckBillDetail _manuallyCheckBillDetail = new ManuallyCheckBillDetailDal();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var state = Request.QueryString["State"];
                if (!string.IsNullOrEmpty(state))
                {
                    switch (state)
                    {
                        case "0":
                            UploadConfirm.Visible = false;
                            btn_Export.Text = "导出对账原始表";
                            break;
                        case "1":
                            UploadConfirm.Visible = true;
                            btn_Export.Text = "导出双方对比表";
                            break;
                        case "2":
                            UploadConfirm.Visible = false;
                            btn_Export.Text = "导出财务确认表";
                            break;
                        case "3":
                            UploadConfirm.Visible = false;
                            btn_Export.Text = "导出对账结果表";
                            break;
                    }
                }
            }
        }

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            GridDataBind();
            RG_ManuallyCheckBillDetail.DataBind();
        }

        #region 数据列表相关
        protected void RG_ManuallyCheckBillDetail_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            #region 查询条件
            Guid manuallyCheckBillId = new Guid(Request.QueryString["Id"]);
            string orderNo = string.Empty;
            int pageIndex = RG_ManuallyCheckBillDetail.CurrentPageIndex + 1;
            int pageSize = RG_ManuallyCheckBillDetail.PageSize;
            if (!string.IsNullOrEmpty(txt_OrderNo.Text))
            {
                orderNo = txt_OrderNo.Text;
            }
            #endregion

            int total;
            var manuallyCheckBillDetailList = _manuallyCheckBillDetail.GetManuallyCheckBillDetailByManuallyCheckBillId(manuallyCheckBillId, orderNo, pageIndex, pageSize, out total);
            RG_ManuallyCheckBillDetail.DataSource = manuallyCheckBillDetailList;
            RG_ManuallyCheckBillDetail.VirtualItemCount = total;
        }
        #endregion

        //导入对账原始表
        protected void btn_Upload_Click(object sender, EventArgs e)
        {
            //var excelName = UploadExcelName.Text;
            //UploadExcelName.Text = string.Empty;

            //#region 数据验证
            //if (string.IsNullOrEmpty(rcb_SalePlatform.SelectedValue) || string.IsNullOrEmpty(Hid_SaleFiliale.Value))
            //{
            //    MessageBox.Show(this, "请选择“销售平台”！");
            //    return;
            //}

            //if (!UploadExcel.HasFile || string.IsNullOrEmpty(excelName))
            //{
            //    MessageBox.Show(this, "请选择格式为“.xls”文件！");
            //    return;
            //}

            //var ext = Path.GetExtension(UploadExcel.FileName);
            //if (ext != null && !ext.Equals(".xls"))
            //{
            //    MessageBox.Show(this, "文件格式错误(.xls)！");
            //    return;
            //}
            //#endregion

            //try
            //{
            //    #region 将上传文件保存至临时文件夹
            //    string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
            //    string folderPath = "~/UserDir/ManuallyCheckBill/InitialTable/ ";
            //    if (!Directory.Exists(Server.MapPath(folderPath)))
            //    {
            //        Directory.CreateDirectory(Server.MapPath(folderPath));
            //    }
            //    string filePath = Server.MapPath(folderPath + fileName);
            //    UploadExcel.PostedFile.SaveAs(filePath);
            //    #endregion

            //    var excelDataTable = ExcelHelper.GetDataSet(filePath).Tables[0];

            //    #region 获取数据之后删除临时文件
            //    if (File.Exists(filePath))
            //    {
            //        File.Delete(filePath);
            //    }
            //    #endregion

            //    List<ManuallyCheckBillDetailInfo> manuallyCheckBillDetailInfoList = new List<ManuallyCheckBillDetailInfo>();
            //    StringBuilder errorMsg = new StringBuilder();
            //    int index = 2;

            //    #region 对账原始表
            //    for (int i = 0; i < excelDataTable.Rows.Count; i++)
            //    {
            //        StringBuilder rowMsg = new StringBuilder();
            //        var systemOrderNo = excelDataTable.Rows[i]["系统订单号"].ToString();
            //        var thirdOrderNo = excelDataTable.Rows[i]["第三方订单号"].ToString();
            //        var isCheck = excelDataTable.Rows[i]["是否对账"].ToString();
            //        var isReceipt = excelDataTable.Rows[i]["是否收款"].ToString();
            //        var orderTime = excelDataTable.Rows[i]["下单日期"].ToString();
            //        var memberId = excelDataTable.Rows[i]["会员名"].ToString();
            //        var systemOrderAmount = excelDataTable.Rows[i]["系统金额"].ToString();
            //        var thirdOrderAmount = excelDataTable.Rows[i]["第三方订单金额"].ToString();
            //        var balance = excelDataTable.Rows[i]["差额"].ToString();
            //        var confirmAmount = excelDataTable.Rows[i]["财务确认金额"].ToString();
            //        var contactsReckoningDifference = excelDataTable.Rows[i]["往来差异"].ToString();

            //        #region 验证数据空值
            //        if (string.IsNullOrEmpty(thirdOrderNo))
            //        {
            //            rowMsg.Append("第").Append(index).Append("行“第三方订单号”为空！").Append("\\n");
            //        }
            //        if (string.IsNullOrEmpty(thirdOrderAmount))
            //        {
            //            rowMsg.Append("第").Append(index).Append("行“第三方订单金额”为空！").Append("\\n");
            //        }
            //        #endregion

            //        #region 验证数据格式
            //        decimal tryThirdOrderAmount;
            //        if (!decimal.TryParse(thirdOrderAmount, out tryThirdOrderAmount))
            //        {
            //            rowMsg.Append("第").Append(index).Append("行“第三方订单金额”格式错误！").Append("\\n");
            //        }
            //        #endregion

            //        #region 验证销售平台
            //        //var goodsOrderInfoList = OrderSao.GetGoodsOrderInfoByThirdOrderNo(new Guid(Hid_SaleFiliale.Value), thirdOrderNo);
            //        //if (goodsOrderInfoList.Count > 0)
            //        //{
            //        //    var goodsOrderInfo = goodsOrderInfoList.First();
            //        //    if (!goodsOrderInfo.SalePlatformId.Equals(new Guid(rcb_SalePlatform.SelectedValue)))
            //        //    {
            //        //        rowMsg.Append("第").Append(index).Append("行“" + thirdOrderNo + "”与所选销售平台不符！").Append("\\n");
            //        //    }
            //        //}
            //        //else
            //        //{
            //        //    rowMsg.Append("第").Append(index).Append("行“" + thirdOrderNo + "”与所选销售平台不符！").Append("\\n");
            //        //}
            //        #endregion

            //        if (string.IsNullOrEmpty(rowMsg.ToString()))
            //        {
            //            #region 保存数据
            //            var manuallyCheckBillDetailInfo = new ManuallyCheckBillDetailInfo
            //            {
            //                Id = Guid.NewGuid(),
            //                ManuallyCheckBillId = Guid.Empty,
            //                SystemOrderNo = string.Empty,
            //                ThirdOrderNo = thirdOrderNo,
            //                IsCheck = 0,
            //                IsReceipt = 0,
            //                OrderTime = DateTime.Parse("1900-01-01"),
            //                MemberId = Guid.Empty,
            //                SystemOrderAmount = 0,
            //                ThirdOrderAmount = decimal.Parse(thirdOrderAmount),
            //                Balance = 0,
            //                ConfirmAmount = 0,
            //                ContactsReckoningDifference = 0
            //            };
            //            manuallyCheckBillDetailInfoList.Add(manuallyCheckBillDetailInfo);
            //            #endregion
            //        }
            //        else
            //        {
            //            errorMsg.Append(rowMsg);
            //        }

            //        index++;
            //    }
            //    #endregion

            //    if (!string.IsNullOrEmpty(errorMsg.ToString()))
            //    {
            //        MessageBox.Show(this, errorMsg.ToString());
            //        return;
            //    }

            //    if (manuallyCheckBillDetailInfoList.Count > 0)
            //    {
            //        var manuallyCheckBillInfo = new ManuallyCheckBillInfo
            //        {
            //            Id = Guid.NewGuid(),
            //            CheckBillPersonnelId = CurrentSession.Personnel.PersonnelId,
            //            SalePlatformId = new Guid(rcb_SalePlatform.SelectedValue),
            //            TradeCode = string.Empty,
            //            CheckState = 0,
            //            ThirdOrderTotalAmount = manuallyCheckBillDetailInfoList.Sum(p => p.ThirdOrderAmount),
            //            UnusualOrderQuantity = 0,
            //            ConfirmTotalAmount = 0,
            //            ReceiptState = 0,
            //            CheckBillDate = DateTime.Now,
            //            Memo = Common.WebControl.RetrunUserAndTime("[【导入】：对账原始表;]")
            //        };

            //        foreach (var item in manuallyCheckBillDetailInfoList)
            //        {
            //            item.ManuallyCheckBillId = manuallyCheckBillInfo.Id;
            //        }

            //        var result = _manuallyCheckBill.AddManuallyCheckBill(manuallyCheckBillInfo);
            //        if (result)
            //        {
            //            _manuallyCheckBillDetail.AddBatchManuallyCheckBillDetail(manuallyCheckBillDetailInfoList);
            //            GridDataBind();
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(this, ex.Message);
            //}
        }
    }
}