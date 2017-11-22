using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.MobileControls;
using ERP.BLL.Implement.Organization;
using ERP.Model;
using ERP.UI.Web.Common;
using FineUI;
using Telerik.Web.UI;
using ListItem = System.Web.UI.WebControls.ListItem;
using ERP.UI.Web.Base;

namespace ERP.UI.Web.CostReport
{
    public partial class CostReport_DepositRecoverySeleteReportPersonnel : WindowsPage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                LoadReportPersonnelData();
            }
        }

        //处理状态
        private void LoadReportPersonnelData()
        {
            var list = string.IsNullOrEmpty(RTB_PersonnelNameOrEnterpriseNo.Text)
                ? new PersonnelManager().GetList()
                : new PersonnelManager().GetList().Where(p => p.RealName.Contains(RTB_PersonnelNameOrEnterpriseNo.Text) || p.EnterpriseNo.Contains(RTB_PersonnelNameOrEnterpriseNo.Text));
            ckb_ReportPersonnel.DataSource = list;
            ckb_ReportPersonnel.DataTextField = "RealName";
            ckb_ReportPersonnel.DataValueField = "PersonnelId";
            ckb_ReportPersonnel.DataBind();
        }

        /// <summary> 
        /// 添加一个选中人到右边ListBox
        /// </summary>
        protected void AddToRight(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(ckb_ReportPersonnel.SelectedValue))
            {
                if (Request.QueryString["type"] == "1")
                {
                    if (lbx_goodslist1.Items.Count >= 1)
                    {
                        MessageBox.Show(this, "转派人只能选择一人！");
                        return;
                    }
                    var sum = 0;
                    foreach (ListItem li in ckb_ReportPersonnel.Items)
                    {
                        if (li.Selected)
                        {
                            sum += 1;
                            if (sum > 1)
                            {
                                break;
                            }
                        }
                    }
                    if (sum>1)
                    {
                        MessageBox.Show(this, "转派人只能选择一人！");
                        return;
                    }
                }

                foreach (ListItem li in ckb_ReportPersonnel.Items)
                {
                    if (li.Selected)
                    {
                        var result = 0;
                        if (lbx_goodslist1.Items.Count > 0)
                        {
                            foreach (var source in lbx_goodslist1.Items.ToList())
                            {
                                if (source.Value == li.Value)
                                {
                                    result = 1;
                                    break;
                                }
                            }
                        }
                        if (result == 0)
                        {
                            lbx_goodslist1.Items.Add(new RadListBoxItem(li.Text, li.Value));
                        }
                        li.Selected = false;
                        
                    }
                }
                ckb_qx.Checked = false;
            }
            else
            {
                MessageBox.Show(this, "请先选择数据！");
            }
        }



        /// <summary>
        /// 移除右边ListBox一个选中人
        /// </summary>
        protected void RemoveToLeft(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lbx_goodslist1.SelectedValue))
            {
                foreach (var item in lbx_goodslist1.SelectedItems)
                {
                    lbx_goodslist1.Items.Remove(item);
                }
            }
            else
            {
                MessageBox.Show(this, "请先选择数据！");
            }
        }


        /// <summary> 
        /// 将左边人员全部添加到右边RadListBox
        /// </summary>
        protected void AllAddToRight(object sender, EventArgs e)
        {
            if (ckb_ReportPersonnel.Items.Count == 0)
            {
                MessageBox.Show(this, "请先选择数据！");
            }
            else
            {
                if (Request.QueryString["type"] == "1")
                {
                    MessageBox.Show(this, "转派人只能选择一人！");
                    return;
                }
                foreach (ListItem li in ckb_ReportPersonnel.Items)
                {
                    var result = 0;
                    if (lbx_goodslist1.Items.Count > 0)
                    {
                        foreach (var source in lbx_goodslist1.Items.ToList())
                        {
                            if (source.Value == li.Value)
                            {
                                result = 1;
                                break;
                            }
                        }
                    }
                    if (result == 0)
                    {
                        lbx_goodslist1.Items.Add(new RadListBoxItem(li.Text, li.Value));
                    }
                    li.Selected = false;
                }
                ckb_qx.Checked = false;
            }
        }

        /// <summary> 
        /// 移除右边ListBox所有选中人
        /// </summary>
        protected void AllReMoveToLeft(object sender, EventArgs e)
        {
            if (lbx_goodslist1.Items.Count == 0)
            {
                MessageBox.Show(this, "请先选择数据！");
            }
            else
            {
                lbx_goodslist1.Items.Clear();
            }
        }


        /// <summary>
        /// 搜索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void LB_Search_Click(object sender, EventArgs e)
        {
            ckb_qx.Checked = false;
            LoadReportPersonnelData();
        }

        /// <summary>
        /// 确定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void btn_Save_Click(object sender, EventArgs e)
        {
            if (lbx_goodslist1.Items.Count == 0)
            {
                MessageBox.Show(this, "请先选择数据！");
            }
            else
            {
                var personnelId = string.Empty;
                var realName = string.Empty;
                foreach (RadListBoxItem item in lbx_goodslist1.Items)
                {
                    personnelId += item.Value + ",";
                    realName += item.Text + ",";
                }

                var type = Request.QueryString["type"];

                ClientScript.RegisterStartupScript(GetType(), "js", "<script>Save('" + personnelId.Substring(0, personnelId.Length - 1) + "','" + realName.Substring(0, realName.Length - 1) + "','" + type + "');</script>");
            }
        }
    }
}