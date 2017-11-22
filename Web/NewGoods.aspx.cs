using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ERP.BLL.Implement.Goods;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using ERP.UI.Web.UserControl;
using MIS.Enum;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    ///<summary>
    /// 新版本商品编辑列表页面
    ///</summary>
    public partial class NewGoods : BasePage
    {
        private static readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private readonly GoodsClassManager _goodsClassManager = new GoodsClassManager(_goodsCenterSao);
        private readonly GoodsManager _goodManager = new GoodsManager(_goodsCenterSao, new PurchaseSet(GlobalConfig.DB.FromType.Write));

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                GetTreeGoodsClass();//加载商品分类
                LoadMoveGoodsClassData();//绑定商品分类(转移)
                LoadGoodsAuditState();//绑定审核状态

                #region 判断登录用户是否有此操作的权限
                //批量强制删除
                if (!GetPowerOperationPoint("MandatoryDel"))
                {
                    btn_ForceDel.Visible = false;
                }
                #endregion
            }

            #region 判断登录用户是否有此操作的权限
            if (!string.IsNullOrEmpty(rcb_GoodsAuditState.SelectedValue))
            {
                var selectedGoodsAuditState = int.Parse(rcb_GoodsAuditState.SelectedValue);
                //审核权限
                if (
                    (selectedGoodsAuditState.Equals((int)GoodsAuditState.PurchasingWaitAudit) && GetPowerOperationPoint("PurchasingWaitAudit")) ||
                    (selectedGoodsAuditState.Equals((int)GoodsAuditState.QualityWaitAudit) && GetPowerOperationPoint("QualityWaitAudit")) ||
                    (selectedGoodsAuditState.Equals((int)GoodsAuditState.CaptainWaitAudit) && GetPowerOperationPoint("CaptainWaitAudit")))
                {
                    btn_Audit.Visible = true;
                }
                else
                {
                    btn_Audit.Visible = false;
                }
            }
            #endregion
        }

        #region 数据准备
        #region 加载商品分类
        private void GetTreeGoodsClass()
        {
            RadTreeNode rootNode = CreateNode("商品分类", true, Guid.Empty.ToString());
            rootNode.Category = "GoodsClass";
            rootNode.Selected = true;
            TVGoodsClass.Nodes.Add(rootNode);
            IList<GoodsClassInfo> goodsClassList = _goodsCenterSao.GetAllClassList().OrderBy(act => act.OrderIndex).ToList();
            RecursivelyGoodsClass(Guid.Empty, rootNode, goodsClassList);
            tree_GoodsClass.Attributes.CssStyle.Value = "display:none;";
        }

        //遍历产品分类
        private void RecursivelyGoodsClass(Guid goodsClassId, IRadTreeNodeContainer node, IList<GoodsClassInfo> goodsClassList)
        {
            IList<GoodsClassInfo> childGoodsClassList = goodsClassList.Where(w => w.ParentClassId == goodsClassId).ToList();
            foreach (GoodsClassInfo goodsClassInfo in childGoodsClassList)
            {
                RadTreeNode goodsClassNode = CreateNode(goodsClassInfo.ClassName, false, goodsClassInfo.ClassId.ToString());
                if (node == null)
                    TVGoodsClass.Nodes.Add(goodsClassNode);
                else
                    node.Nodes.Add(goodsClassNode);
                RecursivelyGoodsClass(goodsClassInfo.ClassId, goodsClassNode, goodsClassList);
            }
        }

        //创建节点
        private static RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { ToolTip = text, Expanded = expanded };
            return node;
        }

        //选择商品分类
        protected void TvGoodsClassNodeClick(object sender, RadTreeNodeEventArgs e)
        {
            Hid_GoodsClassId.Value = Guid.Empty.ToString();
            if (!string.IsNullOrEmpty(e.Node.Value))
            {
                Hid_GoodsClassId.Value = e.Node.Value;
                RG_Goods.CurrentPageIndex = 0;
                RG_Goods.Rebind();
                tree_GoodsClass.Attributes.CssStyle.Value = "display:\"\";";
            }
            Hid_TreeToggle.Value = "1";
        }
        #endregion

        ///<summary>
        /// 绑定商品分类(转移)
        ///</summary>
        public void LoadMoveGoodsClassData()
        {
            ddl_GoodsClass.DataSource = _goodsClassManager.GetGoodsClassListWithRecursion();
            ddl_GoodsClass.DataTextField = "ClassName";
            ddl_GoodsClass.DataValueField = "ClassId";
            ddl_GoodsClass.DataBind();
        }

        ///<summary>
        /// 绑定审核状态
        ///</summary>
        public void LoadGoodsAuditState()
        {
            rcb_GoodsAuditState.Items.Insert(0, new RadComboBoxItem("全部", ""));
            var list = EnumAttribute.GetDict<GoodsAuditState>();
            foreach (var item in list)
            {
                if (item.Key.ToString() == "1" || item.Key.ToString() == "2" || item.Key.ToString() == "3")
                {
                    rcb_GoodsAuditState.Items.Add(
                        new RadComboBoxItem("待" + item.Value, item.Key.ToString()));
                }
                else
                {
                    rcb_GoodsAuditState.Items.Add(
                    new RadComboBoxItem(item.Value, item.Key.ToString()));
                }

            }
        }

        #endregion

        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            GridDataBind();
            RG_Goods.CurrentPageIndex = 0;
            RG_Goods.DataBind();
        }

        #region 数据列表相关
        protected void RG_Goods_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (IsPostBack)
            {
                GridDataBind();
            }
            else
            {
                IList<GoodsInfo> goodsList = new List<GoodsInfo>();
                RG_Goods.DataSource = goodsList;
            }
        }

        //Grid数据源
        protected void GridDataBind()
        {
            var pageIndex = RG_Goods.CurrentPageIndex + 1;
            int pageSize = RG_Goods.PageSize;
            int totalCount = 0;
            int? saleStockType = null;
            if (!string.IsNullOrEmpty(ddl_SaleStockType.SelectedValue))
            {
                saleStockType = int.Parse(ddl_SaleStockType.SelectedValue);
            }

            int? goodsAuditState = null;
            if (!string.IsNullOrEmpty(rcb_GoodsAuditState.SelectedValue))
            {
                goodsAuditState = int.Parse(rcb_GoodsAuditState.SelectedValue);
            }

            var goodsList = _goodsCenterSao.GetGoodsListToPage(new Guid(Hid_GoodsClassId.Value), txt_GoodsNameOrCode.Text.Trim(), string.IsNullOrEmpty(ddl_HasInformation.SelectedValue) ? (bool?)null : bool.Parse(ddl_HasInformation.SelectedValue), saleStockType, goodsAuditState, pageIndex, pageSize, out totalCount).ToList();
            RG_Goods.DataSource = goodsList;
            RG_Goods.VirtualItemCount = totalCount;
        }

        #region 列表显示辅助方法
        /// <summary>
        /// 卖库存状态
        /// </summary>
        /// <param name="saleStockState"></param>
        /// <returns></returns>
        public string GetSaleStockState(int saleStockState)
        {
            string applyName = "申请";
            if (saleStockState == (int)SaleStockState.Apply)
            {
                applyName = "待审核";
            }
            else if (saleStockState == (int)SaleStockState.ApplyNotPass)
            {
                applyName = "重新申请";
            }
            return applyName;
        }
        #endregion
        #endregion

        //批量转移
        protected void btn_Move_Click(object sender, EventArgs e)
        {
            if (Hid_GoodsClassId.Value.Equals(Guid.Empty.ToString()))
            {
                MessageBox.Show(this, "请选择商品分类！");
                return;
            }

            if (Hid_GoodsClassId.Value.Equals(ddl_GoodsClass.SelectedValue))
            {
                MessageBox.Show(this, "您所选分类和商品当前分类一致,无需移动！");
                return;
            }

            if (Request["ckId"] != null)
            {
                var errorMsg = new StringBuilder();
                var isMove = false;
                var newGoodsClassId = new Guid(ddl_GoodsClass.SelectedValue);
                var goodsIdsAndGoodsNamesAndGoodsAuditState = Request["ckId"].Split(',');
                var goodsAuditStateList = goodsIdsAndGoodsNamesAndGoodsAuditState.Select(item => item.Split('&')[2]).Where(p => !p.Equals(((int)GoodsAuditState.Pass).ToString()));
                if (goodsAuditStateList.Any())
                {
                    MessageBox.Show(this, "非“审核通过”状态不允许转移！");
                    return;
                }
                foreach (var item in goodsIdsAndGoodsNamesAndGoodsAuditState)
                {
                    var goodsId = new Guid(item.Split('&')[0]);
                    string goodsName = item.Split('&')[1];
                    //获取要转移的商品属性列表
                    var goodsfieldList = _goodsCenterSao.GetFieldDetailByGoodsId(goodsId).ToList();
                    //转移目标商品分类的属性列表
                    var goodsClassFieldList = newGoodsClassId == Guid.Empty ? new List<Guid>() : _goodsCenterSao.GetClassDetail(newGoodsClassId).GoodsClassFieldList;
                    //如果所选商品和转移目标分类都是没有属性的则可以转移
                    if (goodsfieldList.Count == 0 && goodsClassFieldList.Count == 0)
                    {
                        isMove = true;
                    }
                    else
                    {
                        foreach (var info in goodsfieldList)
                        {
                            foreach (var guid in goodsClassFieldList)
                            {
                                isMove = info.FieldId == guid;
                            }
                        }
                    }
                    if (!isMove) continue;
                    try
                    {
                        string failMessage;
                        var result = _goodsCenterSao.UpdateGoodsClass(goodsId, newGoodsClassId, out failMessage);
                        if (!result)
                        {
                            errorMsg.Append("“").Append(goodsName).Append("”").Append(failMessage).Append("！").Append("\\n");
                        }
                    }
                    catch
                    {
                        errorMsg.Append("“").Append(goodsName).Append("”转移失败！").Append("\\n");
                    }
                }
                if (!string.IsNullOrEmpty(errorMsg.ToString()))
                {
                    MessageBox.Show(this, errorMsg.ToString());
                }
                else
                {
                    GridDataBind();
                    RG_Goods.DataBind();
                    MessageBox.AppendScript(this, "alert('商品转移成功！');moveHide();");
                }
            }
            else
            {
                MessageBox.AppendScript(this, "alert('请选择要转移的商品！');moveShow();");
            }
        }

        //批量删除
        protected void btn_Del_Click(object sender, EventArgs e)
        {
            if (Request["ckId"] != null)
            {
                IGoodsOrder goodsOrder = new GoodsOrder(GlobalConfig.DB.FromType.Read);
                var purchasingManager = new PurchasingManager(new Purchasing(GlobalConfig.DB.FromType.Read), _goodsCenterSao, null, null, null);
                IStorageRecordDao storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Read);

                var errorMsg = new StringBuilder();
                var goodsIdList = new List<Guid>();
                var goodsIdsAndGoodsNamesAndGoodsAuditState = Request["ckId"].Split(',');
                List<int> stockStates = new List<int>
                {
                    (int) StorageRecordState.WaitAudit,
                    (int) StorageRecordState.Refuse,
                    (int) StorageRecordState.Refuse,
                    (int) StorageRecordState.Approved,
                    (int) StorageRecordState.Finished
                };
                foreach (var item in goodsIdsAndGoodsNamesAndGoodsAuditState)
                {
                    var goodsId = new Guid(item.Split('&')[0]);
                    string goodsName = item.Split('&')[1];
                    if (goodsOrder.SelectSemiStockAtOneYearByGoodsId(goodsId, null, null, 365, stockStates))
                    {
                        errorMsg.Append("“").Append(goodsName).Append("”该商品1年内有进行过出入库记录,不允许删除！").Append("\\n");
                        continue;
                    }
                    if (purchasingManager.SelectPurchasingNoCompleteByGoodsId(goodsId, null))
                    {
                        errorMsg.Append("“").Append(goodsName).Append("”该商品存在未完成的采购单,不允许删除！").Append("\\n");
                        continue;
                    }
                    if (storageRecordDao.IsExistNormalStorageRecord(goodsId, null))
                    {
                        errorMsg.Append("“").Append(goodsName).Append("”该商品存在未审核的出入库单据,不允许删除！").Append("\\n");
                        continue;
                    }
                    if (CacheCollection.Filiale.GetHeadList().Where(f => f.FilialeTypes.Contains((int)FilialeType.EntityShop)).Any(source => StockSao.IsExistGoodsStock(source.ID, goodsId, new List<Guid>())))
                    {
                        errorMsg.Append("“").Append(goodsName).Append("”此商品门店有库存,不允许删除！").Append("\\n");
                        continue;
                    }

                    goodsIdList.Add(goodsId);
                }
                if (!string.IsNullOrEmpty(errorMsg.ToString()))
                {
                    MessageBox.Show(this, errorMsg.ToString());
                }
                else
                {
                    //删除主商品
                    string errorMessage;
                    var personnel = CurrentSession.Personnel.Get();
                    var isSuccess = _goodManager.DeleteGoods(goodsIdList, personnel.RealName, personnel.PersonnelId, out errorMessage);
                    if (isSuccess)
                    {
                        MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                        MessageBox.Show(this, "主商品删除成功！");
                    }
                    else
                    {
                        MessageBox.Show(this, "主商品删除失败！");
                    }
                }
            }
            else
            {
                MessageBox.Show(this, "请选择相关数据！");
            }
        }

        //批量强制删除
        protected void btn_ForceDel_Click(object sender, EventArgs e)
        {
            if (Request["ckId"] != null)
            {
                var goodsIdsAndGoodsNamesAndGoodsAuditState = Request["ckId"].Split(',');
                var goodsIdList = goodsIdsAndGoodsNamesAndGoodsAuditState.Select(item => new Guid(item.Split('&')[0])).ToList();

                //删除主商品
                string errorMessage;
                var personnel = CurrentSession.Personnel.Get();
                var isSuccess = _goodManager.DeleteGoods(goodsIdList, personnel.RealName, personnel.PersonnelId, out errorMessage);
                if (isSuccess)
                {
                    MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    MessageBox.Show(this, "主商品删除成功！");
                }
                else
                {
                    MessageBox.Show(this, "主商品删除失败！");
                }
            }
            else
            {
                MessageBox.Show(this, "请选择相关数据！");
            }
        }

        //是否缺货
        protected void ccb_IsStockScarcity_CheckedChanged(object sender, EventArgs e)
        {
            var ccb = sender as ConfirmCheckBox;
            if (ccb != null)
            {
                var item = ccb.Parent.Parent as GridDataItem;
                if (item != null)
                {
                    var goodsId = new Guid(item.GetDataKeyValue("GoodsId").ToString());
                    var isScarcity = ccb.Checked;
                    try
                    {
                        string errorMsg;
                        var personnel = CurrentSession.Personnel.Get();
                        var result = _goodsCenterSao.SetGoodsIsScarcity(goodsId, isScarcity, personnel.RealName, personnel.PersonnelId, out errorMsg);
                        if (result)
                        {
                            new LogUtility().WriteException(isScarcity ? new Exception("非异常，记录缺货!") : new Exception("非异常，记录不缺货!"), goodsId.ToString());
                            RG_Goods.Rebind();
                        }
                        else
                        {
                            MessageBox.Show(this, "设置商品是否缺货失败！" + errorMsg);
                        }
                    }
                    catch
                    {
                        MessageBox.Show(this, "设置商品是否缺货失败！");
                    }
                }
            }
        }

        //是否上下架
        protected void ccb_IsOnShelf_CheckedChanged(object sender, EventArgs e)
        {
            var ccb = sender as ConfirmCheckBox;
            if (ccb != null)
            {
                var item = ccb.Parent.Parent as GridDataItem;
                if (item != null)
                {
                    var goodsId = new Guid(item.GetDataKeyValue("GoodsId").ToString());
                    bool isOnShelf = ccb.Checked;
                    string errorManage;
                    try
                    {
                        var personnel = CurrentSession.Personnel.Get();
                        var result = _goodsCenterSao.SetPurchaseState(goodsId, isOnShelf, personnel.RealName, personnel.PersonnelId, out errorManage);
                        if (result)
                        {
                            RG_Goods.Rebind();
                        }
                        else
                        {
                            MessageBox.Show(this, "设置商品是否上架失败！" + errorManage);
                        }
                    }
                    catch
                    {
                        MessageBox.Show(this, "设置商品是否上架失败！");
                    }
                }
            }
        }

        /// <summary>
        /// 取得用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            const string PAGE_NAME = "NewGoods.aspx";
            return WebControl.GetPowerOperationPoint(PAGE_NAME, powerName);
        }

        /// <summary>
        /// 审核权限
        /// </summary>
        protected int GetApproved(int goodsAuditState)
        {
            int result = 0;

            if (goodsAuditState.Equals((int)GoodsAuditState.PurchasingWaitAudit) && GetPowerOperationPoint("PurchasingWaitAudit"))
            {
                result = 1;
            }
            else if (goodsAuditState.Equals((int)GoodsAuditState.QualityWaitAudit) && GetPowerOperationPoint("QualityWaitAudit"))
            {
                result = 1;
            }
            else if (goodsAuditState.Equals((int)GoodsAuditState.CaptainWaitAudit) && GetPowerOperationPoint("CaptainWaitAudit"))
            {
                result = 1;
            }
            else if (!GetPowerOperationPoint("PurchasingWaitAudit") && !GetPowerOperationPoint("QualityWaitAudit") && !GetPowerOperationPoint("CaptainWaitAudit"))
            {
                result = 0;
            }
            else if (goodsAuditState.Equals((int)GoodsAuditState.NoPass) || goodsAuditState.Equals((int)GoodsAuditState.Pass))
            {
                result = 0;
            }
            return result;
        }

        //批量审核
        protected void btn_Audit_Click(object sender, EventArgs e)
        {
            string selectedGoodsAuditState = rcb_GoodsAuditState.SelectedValue;
            if (string.IsNullOrEmpty(selectedGoodsAuditState))
            {
                MessageBox.Show(this, "请选择审核状态！");
                return;
            }

            if (Request["ckId"] == null)
            {
                MessageBox.Show(this, "请选择相关数据！");
                return;
            }

            if (!(int.Parse(selectedGoodsAuditState).Equals((int)GoodsAuditState.PurchasingWaitAudit) && GetPowerOperationPoint("PurchasingWaitAudit")) && !(int.Parse(selectedGoodsAuditState).Equals((int)GoodsAuditState.QualityWaitAudit) && GetPowerOperationPoint("QualityWaitAudit")) && !(int.Parse(selectedGoodsAuditState).Equals((int)GoodsAuditState.CaptainWaitAudit) && GetPowerOperationPoint("CaptainWaitAudit")))
            {
                MessageBox.Show(this, "您不具有“" + EnumAttribute.GetKeyName((GoodsAuditState)int.Parse(selectedGoodsAuditState)) + "”的审核权限！");
                return;
            }

            var errorMsg = string.Empty;
            var goodIdList = new List<Guid>();
            var goodsIdsAndGoodsNamesAndGoodsAuditState = Request["ckId"].Split(',');
            foreach (var item in goodsIdsAndGoodsNamesAndGoodsAuditState)
            {
                var goodsId = new Guid(item.Split('&')[0]);
                string goodsAuditState = item.Split('&')[2];
                string goodsCode = item.Split('&')[3];
                if (goodsAuditState != selectedGoodsAuditState)
                {
                    errorMsg += goodsCode + ",";
                }
                else
                {
                    goodIdList.Add(goodsId);
                }
            }
            if (!string.IsNullOrEmpty(errorMsg))
            {
                MessageBox.Show(this, "商品编号：" + errorMsg.Substring(0, errorMsg.Length - 1) + " 审核状态与已选择审核状态不一致！");
                return;
            }

            #region 批量核准
            if (goodIdList.Any())
            {
                int auditState = int.Parse(selectedGoodsAuditState);
                string auditStateMemo = string.Empty;
                switch ((GoodsAuditState)int.Parse(selectedGoodsAuditState))
                {
                    case GoodsAuditState.PurchasingWaitAudit:
                        auditState = (int)GoodsAuditState.QualityWaitAudit;
                        auditStateMemo = "【采购经理审核】:审核通过;";
                        break;
                    case GoodsAuditState.QualityWaitAudit:
                        auditState = (int)GoodsAuditState.CaptainWaitAudit;
                        auditStateMemo = "【质管部审核】:审核通过;";
                        break;
                    case GoodsAuditState.CaptainWaitAudit:
                        auditState = (int)GoodsAuditState.Pass;
                        auditStateMemo = "【负责人终审】:审核通过;";
                        break;
                }
                auditStateMemo = WebControl.RetrunUserAndTime("[" + auditStateMemo + "]");
                string failMessage;
                bool result = _goodManager.PlUpdateGoodsAuditStateAndAuditStateMemo(goodIdList, auditState, auditStateMemo, out failMessage);
                if (result)
                {
                    MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    MessageBox.Show(this, "保存成功！");
                }
                else
                {
                    MessageBox.Show(this, "保存失败！" + failMessage);
                }
            }
            #endregion
        }
    }
}
