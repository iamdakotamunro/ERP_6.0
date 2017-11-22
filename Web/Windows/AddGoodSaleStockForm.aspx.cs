using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using ERP.Enum;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using ERP.Environment;

namespace ERP.UI.Web.Windows
{
    ///<summary>
    /// 新版本买库存商品申请审核页面
    ///</summary>
    public partial class AddGoodSaleStockForm : WindowsPage
    {
        readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();

        protected Boolean IsSaveAndAdd;


        protected SubmitController SubmitController;
        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["SubmitController"] == null)
            {
                SubmitController = new SubmitController(Guid.NewGuid());
                ViewState["SubmitController"] = SubmitController;
            }
            return (SubmitController)ViewState["SubmitController"];
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            SubmitController = CreateSubmitInstance();
            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["GoodsId"]))
                {
                    GoodsId = new Guid(Request.QueryString["GoodsId"]);
                    ApplyOrAudit = Request.QueryString["Type"];
                    BindGoodsField(GoodsId);
                }
            }
        }

        //[商品ID]
        protected Guid GoodsId
        {
            get { return new Guid(ViewState["GoodsId"].ToString()); }
            set { ViewState["GoodsId"] = value.ToString(); }
        }


        //[申请or审核]
        protected string ApplyOrAudit
        {
            get { return ViewState["Type"].ToString(); }
            set { ViewState["Type"] = value; }
        }

        //[加载页面初始值]
        private void BindGoodsField(Guid goodsId)
        {
            GoodsSaleStockInfo info = _goodsCenterSao.GetGoodsSaleStockInfoByGoodsId(goodsId);

            TB_ReplenishmentCycle.Visible = false;
            Lit_ReplenishmentCycle.Visible = false;
            RequiredFieldValidator6.Visible = false;
            REVCost.Visible = false;
            if (ApplyOrAudit == "Apply")
            {
                #region --> Apply
                if (info == null || info.GoodsId == Guid.Empty)
                {
                    GoodsInfo goodsInfo = goodsId == Guid.Empty ? null : _goodsCenterSao.GetGoodsBaseInfoById(goodsId);
                    LitGoodsName.Text = goodsInfo != null ? goodsInfo.GoodsName : string.Empty;
                    LitGoodsCode.Text = goodsInfo != null ? goodsInfo.GoodsCode : string.Empty;
                    Btn_Agree.Visible = false;
                    Btn_UnAgree.Visible = false;
                    TR_AuditReason.Visible = false;
                }
                else
                {
                    LitGoodsName.Text = info.GoodsName;
                    LitGoodsCode.Text = info.GoodsCode;
                    Btn_Agree.Visible = false;
                    Btn_UnAgree.Visible = false;
                    if (info.SaleStockState == (int)SaleStockState.ApplyNotPass)
                    {
                        //审核未通过
                        TR_AuditReason.Visible = true;
                        TB_AuditReason.Text = info.AuditReason;
                        TB_AuditReason.Enabled = false;
                    }
                    else
                    {
                        TR_AuditReason.Visible = false;
                    }

                    #region
                    if (info.SaleStockType == (int)SaleStockType.ShortStock)
                    {
                        //卖完缺货
                        CB_ShortStock.Checked = true;
                        TB_ReplenishmentCycle.Visible = true;
                        Lit_ReplenishmentCycle.Visible = true;
                        RequiredFieldValidator6.Visible = true;
                        REVCost.Visible = true;
                        TB_ReplenishmentCycle.Text = string.Format("{0}", info.ReplenishmentCycle);
                        TB_ApplyReason.Text = info.ApplyReason;
                    }
                    else if (info.SaleStockType == (int)SaleStockType.OutOfStock)
                    {
                        //卖完断货
                        CB_OutOfStock.Checked = true;
                        TB_ReplenishmentCycle.Visible = false;
                        Lit_ReplenishmentCycle.Visible = false;
                        RequiredFieldValidator6.Visible = false;
                        REVCost.Visible = false;
                        TB_ApplyReason.Text = info.ApplyReason;
                    }
                    else
                    {
                        //非卖库存
                        CB_NotSellStock.Checked = true;
                        TB_ReplenishmentCycle.Visible = false;
                        Lit_ReplenishmentCycle.Visible = false;
                        TR_AuditReason.Visible = false;
                        RequiredFieldValidator6.Visible = false;
                        REVCost.Visible = false;
                        TB_ApplyReason.Text = info.ApplyReason;
                    }
                    #endregion
                }
                #endregion
            }
            else
            {
                Btn_Apply.Visible = false;
                Btn_Cancel.Visible = false;

                #region --> Audit
                LitGoodsName.Text = info.GoodsName;
                LitGoodsCode.Text = info.GoodsCode;
                if (info.SaleStockType == (int)SaleStockType.ShortStock)
                {
                    //卖完缺货
                    CB_ShortStock.Checked = true;
                    CB_ShortStock.Enabled = false;
                    CB_OutOfStock.Enabled = false;
                    CB_NotSellStock.Enabled = false;
                    TB_ReplenishmentCycle.Visible = true;
                    Lit_ReplenishmentCycle.Visible = true;
                    TB_ApplyReason.Enabled = false;
                    TB_ReplenishmentCycle.Enabled = false;
                    TB_ReplenishmentCycle.Text = string.Format("{0}", info.ReplenishmentCycle);
                    TB_ApplyReason.Text = info.ApplyReason;
                    TB_AuditReason.Text = info.AuditReason;
                }
                else
                {
                    //卖完断货
                    CB_OutOfStock.Checked = true;
                    CB_ShortStock.Enabled = false;
                    CB_OutOfStock.Enabled = false;
                    CB_NotSellStock.Enabled = false;
                    TB_ReplenishmentCycle.Visible = false;
                    Lit_ReplenishmentCycle.Visible = false;
                    RequiredFieldValidator6.Visible = false;
                    REVCost.Visible = false;
                    TB_ApplyReason.Enabled = false;
                    TB_ApplyReason.Text = info.ApplyReason;
                    TB_AuditReason.Text = info.AuditReason;
                }
                #endregion
            }
        }

        /// <summary>
        /// 申请
        /// </summary>
        private void SaveApply()
        {
            try
            {
                if ((!CB_ShortStock.Checked) && (!CB_OutOfStock.Checked) && (!CB_NotSellStock.Checked))
                {
                    RAM.Alert("请至少选择一个卖库存类型");
                }
                else
                {
                    var personnel = CurrentSession.Personnel.Get();
                    int saleStockType;
                    if (CB_ShortStock.Checked)
                        saleStockType = (int)SaleStockType.ShortStock;
                    else if (CB_OutOfStock.Checked)
                        saleStockType = (int)SaleStockType.OutOfStock;
                    else
                        saleStockType = (int)SaleStockType.NotSellStock;
                    int cycle = 0;
                    if (saleStockType == (int)SaleStockType.ShortStock)
                    {
                        if (string.IsNullOrEmpty(TB_ReplenishmentCycle.Text))
                        {
                            RAM.Alert("补货周期不允许为空");
                            return;
                        }
                        else
                        {
                            cycle = int.Parse(TB_ReplenishmentCycle.Text);
                        }
                    }
                    GoodsSaleStockInfo info = _goodsCenterSao.GetGoodsSaleStockInfoByGoodsId(GoodsId);

                    if (info == null || info.GoodsId == Guid.Empty)
                    {
                        info = new GoodsSaleStockInfo
                        {
                            GoodsId = GoodsId,
                            GoodsName = LitGoodsName.Text,
                            GoodsCode = LitGoodsCode.Text,
                            ApplyReason = TB_ApplyReason.Text,
                            Applicant = CurrentSession.Personnel.Get().PersonnelId,
                            ApplyTime = DateTime.Now,
                            Auditor = Guid.Empty,
                            AuditTime = (DateTime)SqlDateTime.MinValue,
                            AuditReason = "",
                            SaleStockType = saleStockType,
                            ReplenishmentCycle = cycle,
                            SaleStockState = (int)SaleStockState.Apply
                        };
                        string errorMessage;
                        var result = _goodsCenterSao.ApplyGoodsSaleStock(info, personnel.RealName, personnel.PersonnelId, out errorMessage);
                        if (result == false)
                        {
                            RAM.Alert("操作无效！" + errorMessage);
                        }
                        //else
                        //{
                        //    AddOperationLog((SaleStockType)saleStockType, SaleStockState.Apply, info);
                        //}
                        if (saleStockType == (int)SaleStockType.NotSellStock)
                        {
                            new LogUtility().WriteException(saleStockType == (int)SaleStockType.ShortStock ? new Exception("非异常，记录缺货!新增申请") : saleStockType == (int)SaleStockType.OutOfStock ? new Exception("非异常，记录断货!新增申请") : new Exception("非异常，记录非卖库存!新增申请"), GoodsId.ToString());
                        }
                    }
                    else
                    {
                        info.ApplyReason = TB_ApplyReason.Text;
                        info.ReplenishmentCycle = cycle;
                        info.SaleStockType = saleStockType;
                        info.SaleStockState = (int)SaleStockState.Apply;
                        string errorMessage;
                        var result = _goodsCenterSao.ApplyGoodsSaleStock(info, personnel.RealName, personnel.PersonnelId, out errorMessage);
                        if (result == false)
                        {
                            RAM.Alert("操作无效！" + errorMessage);
                        }
                        //else
                        //{
                        //    AddOperationLog((SaleStockType)saleStockType, SaleStockState.Apply, info);
                        //}
                        if (saleStockType == (int)SaleStockType.NotSellStock)
                        {
                            new LogUtility().WriteException(saleStockType == (int)SaleStockType.ShortStock ? new Exception("非异常，记录缺货!修改申请") : saleStockType == (int)SaleStockType.OutOfStock ? new Exception("非异常，记录断货!修改申请") : new Exception("非异常，记录非卖库存!修改申请"), GoodsId.ToString());
                        }
                    }
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
            }
            catch (Exception ex)
            {
                RAM.Alert("保存失败，系统提示：" + ex.Message);
            }
        }

        private void SaveAudit(bool b)
        {
            try
            {
                var isAudit = true;
                if (b == false)
                {
                    if (TB_AuditReason.Text.Trim() == "")
                    {
                        RAM.Alert("请给出审核不同意理由");
                        isAudit = false;
                    }
                }
                else  //设置商品下的子商品是否缺货  zhangfan added at 2013-Oct-10th
                {
                    if (!SetRealGoodsIsScarcity(GoodsId))
                        isAudit = false;
                }
                if (isAudit)
                {
                    GoodsSaleStockInfo info = _goodsCenterSao.GetGoodsSaleStockInfoByGoodsId(GoodsId);
                    if (info == null || info.GoodsId == Guid.Empty)
                    {
                        RAM.Alert("操作无效！");
                        return;
                    }
                    int saleStockState = b ? (int)SaleStockState.Approve : (int)SaleStockState.ApplyNotPass;
                    try
                    {
                        string errorMessage;
                        var personnel = CurrentSession.Personnel.Get();
                        var result = _goodsCenterSao.AuditGoodsSaleStock(GoodsId, saleStockState, personnel.RealName, personnel.PersonnelId, TB_AuditReason.Text, out errorMessage);
                        if (result == false)
                        {
                            RAM.Alert("操作无效！" + errorMessage);
                        }
                        //else
                        //{
                        //    AddOperationLog((SaleStockType)info.SaleStockType, (SaleStockState)saleStockState, info);
                        //    new LogUtility().WriteException(info.SaleStockType == (int)SaleStockType.ShortStock ? new Exception("非异常，记录缺货!") : new Exception("非异常，记录断货!"), GoodsId.ToString());
                        //}
                    }
                    catch
                    {
                        RAM.Alert("设置商品是否为缺货失败！");
                    }
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
            }
            catch (Exception ex)
            {
                RAM.Alert("保存失败，系统提示：" + ex.Message);
            }
        }

        /// <summary>
        /// 设置商品下的子商品是否缺货，在设置商品为卖库存时使用
        /// zhangfan added at 2013-Oct-10th
        /// </summary>
        /// <param name="goodsId"></param>
        /// <returns>是否修改成功</returns>
        private bool SetRealGoodsIsScarcity(Guid goodsId)
        {
            var result = _goodsCenterSao.GetRealGoodsListByGoodsId(new List<Guid> { goodsId });
            IList<ChildGoodsInfo> realGoodsList = result != null ? result.OrderBy(ent => ent.GoodsId).ThenBy(w => w.Specification).ToList()
                : new List<ChildGoodsInfo>();
            if (realGoodsList.Count == 0)
            {
                RAM.Alert("设置商品下的子商品是否缺货 --> 未找到该商品的子商品信息！");
                return false;
            }

            var quantityList = WMSSao.CanSetScarcityForSaleStockGoods(realGoodsList.Select(ent => ent.RealGoodsId));

            if (quantityList == null)
            {
                RAM.Alert("设置商品下的子商品是否缺货 --> 未从库存服务取到库存数据！");
                return false;
            }
            var personnel = CurrentSession.Personnel.Get();
            int scarcityGoodsCount = 0;
            foreach (var item in quantityList.Where(ent => ent.Value))
            {
                _goodsCenterSao.SetRealGoodsIsScarcity(item.Key, true, personnel.RealName, personnel.PersonnelId);
                scarcityGoodsCount++;
            }

            if (realGoodsList.Count == scarcityGoodsCount)  //如果该商品下的子商品都没货，那么主商品下架并缺货
            {
                //卖完缺货  商品没有库存  商品缺货
                //卖完断货  商品没有库存  商品缺货和下架
                string errorMsg;
                _goodsCenterSao.SetGoodsIsScarcity(goodsId, true, personnel.RealName, personnel.PersonnelId, out errorMsg);
                if (CB_OutOfStock.Checked)
                {
                    //卖完断货，商品下架
                    string errorMessage;
                    _goodsCenterSao.SetPurchaseState(goodsId, false, personnel.RealName, personnel.PersonnelId, out errorMessage);
                }
            }
            return true;
        }

        protected void Cb_ShortStock_CheckedChanged(object sender, EventArgs e)
        {
            if (!CB_ShortStock.Checked) return;
            CB_OutOfStock.Checked = false;
            CB_NotSellStock.Checked = false;
            TB_ReplenishmentCycle.Visible = true;
            Lit_ReplenishmentCycle.Visible = true;
            RequiredFieldValidator6.Visible = true;
            REVCost.Visible = true;
        }

        protected void Cb_OutOfStock_CheckedChanged(object sender, EventArgs e)
        {
            if (!CB_OutOfStock.Checked) return;
            CB_ShortStock.Checked = false;
            CB_NotSellStock.Checked = false;
            TB_ReplenishmentCycle.Visible = false;
            Lit_ReplenishmentCycle.Visible = false;
            RequiredFieldValidator6.Visible = false;
            REVCost.Visible = false;
        }

        protected void Cb_NotSellStock_CheckedChanged(object sender, EventArgs e)
        {
            if (!CB_NotSellStock.Checked) return;
            CB_ShortStock.Checked = false;
            CB_OutOfStock.Checked = false;
            TB_ReplenishmentCycle.Visible = false;
            Lit_ReplenishmentCycle.Visible = false;
            RequiredFieldValidator6.Visible = false;
            REVCost.Visible = false;
        }

        //申请
        protected void Btn_Apply_Click(object sender, EventArgs e)
        {
            SaveApply();
        }

        //取消
        protected void Btn_Cancel_Click(object sender, EventArgs e)
        {
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }

        //同意
        protected void Btn_Agree_Click(object sender, EventArgs e)
        {
            SaveAudit(true);
        }

        //不同意
        protected void Btn_UnAgree_Click(object sender, EventArgs e)
        {
            SaveAudit(false);
        }
    }
}
