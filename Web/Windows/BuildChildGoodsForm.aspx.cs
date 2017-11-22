using System;
using System.Collections.Generic;
using System.Linq;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Implement.Order;
using ERP.DAL.Interface.IInventory;
using ERP.DAL.Interface.IOrder;
using ERP.Enum;
using ERP.Model;
using ERP.Model.Goods;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using FineUI;
using Framework.Common;
using MIS.Enum;
using GlobalConfig = ERP.Environment.GlobalConfig;

namespace ERP.UI.Web.Windows
{
    public partial class BuildChildGoods : WindowsPage
    {
        private readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        private readonly IStorageRecordDao _storageRecordDao = new StorageRecordDao(GlobalConfig.DB.FromType.Write);

        #region --> Page_Load
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                List<GoodsFieldInfo> fieldList;
                List<GoodsFieldInfo> selectedFieldList;
                string errorMessage;
                RblDelete.Enabled = GetPowerOperationPoint("MandatoryDel");
                bool result = _goodsCenterSao.GetRealGoodsGoodsEditRequestModel(GoodsId, out fieldList, out selectedFieldList, out errorMessage);
                if (result)
                {
                    BindingField(fieldList);//绑定属性
                    //默认绑定属性
                    BindingDefaultSelectField(fieldList, selectedFieldList);
                    BindingChildGoods();//绑定现有的子商品信息
                }
                else
                {
                    Alert.Show(errorMessage, "加载异常", MessageBoxIcon.Error);
                }
            }
        }
        #endregion

        #region --> ViewState
        #region --> GoodsId
        protected Guid GoodsId
        {
            get
            {
                if (ViewState["GoodsId"] == null)
                {
                    ViewState["GoodsId"] = Request.QueryString["GoodsId"].ToGuid();
                }
                return (Guid)ViewState["GoodsId"];
            }

        }
        #endregion
        #region --> ClassId
        protected Guid ClassId
        {
            get
            {
                if (ViewState["ClassId"] == null)
                {
                    ViewState["ClassId"] = Request.QueryString["ClassId"].ToGuid();
                }
                return (Guid)ViewState["ClassId"];
            }

        }
        #endregion

        protected List<Guid> SelectedFieldIds
        {
            get
            {
                if (ViewState["SelectedFieldIds"] == null)
                {
                    return new List<Guid>();
                }
                return (List<Guid>)ViewState["SelectedFieldIds"];
            }
            set { ViewState["SelectedFieldIds"] = value; }
        }
        #endregion

        #region --> 加载属性

        private void BindingField(IEnumerable<FieldInfo> list)
        {
            foreach (var field in list)
            {
                var childFieldList = field.ChildFields.OrderBy(act => act.OrderIndex).ToList();
                if (field.FieldName == "光度")
                {
                    HF_LightFieldParentId.Text = field.FieldId.ToString();
                    //绑定光度属性
                    CBL_LightFieldList.DataSource = childFieldList;
                    CBL_LightFieldList.DataBind();
                }

                if (field.FieldName == ("散光"))
                {
                    HF_AstigmatismFieldParentId.Text = field.FieldId.ToString();
                    //绑定散光属性
                    CBL_AstigmatismFieldList.DataSource = childFieldList;
                    CBL_AstigmatismFieldList.DataBind();
                }

                if (field.FieldName == ("轴位"))
                {
                    HF_AxialViewFieldParentId.Text = field.FieldId.ToString();
                    //绑定轴位属性
                    CBL_AxialViewFieldList.DataSource = childFieldList;
                    CBL_AxialViewFieldList.DataBind();
                }
            }
        }

        #endregion

        #region --> 指定主商品获取加载子商品信息

        private void BindingChildGoods()
        {
            int totalCount;
            var fieldList = new List<Guid>();
            if (IsPostBack)
            {
                if (DDL_LightFields.Enabled)
                {
                    var light = new Guid(DDL_LightFields.SelectedValue);
                    if (light != Guid.Empty)
                    {
                        fieldList.Add(light);
                    }
                }
                if (DDL_AstigmatismFields.Enabled)
                {
                    var astigmatism = new Guid(DDL_AstigmatismFields.SelectedValue);
                    if (astigmatism != Guid.Empty)
                    {
                        fieldList.Add(astigmatism);
                    }
                }
                if (DDL_AxialFields.Enabled)
                {
                    var axial = new Guid(DDL_AxialFields.SelectedValue);
                    if (axial != Guid.Empty)
                    {
                        fieldList.Add(axial);
                    }
                }
            }
            List<ChildGoodsInfo> childGoodsInfos = _goodsCenterSao.GetRealGoodsListByPage(GoodsId, fieldList, GD_ChildGoodsList.PageIndex + 1,
                                                GD_ChildGoodsList.PageSize, out totalCount).ToList();

            GD_ChildGoodsList.Collapsed = childGoodsInfos.Count == 0;
            form_Field.Collapsed = childGoodsInfos.Count > 0;
            if (childGoodsInfos.Count > 0)
            {
                foreach (var childGoodsInfo in childGoodsInfos)
                {
                    childGoodsInfo.IsActive = !childGoodsInfo.IsActive;
                }
            }
            GD_ChildGoodsList.RecordCount = totalCount;
            GD_ChildGoodsList.DataSource = childGoodsInfos.OrderBy(w => w.Specification).ToList();
            GD_ChildGoodsList.DataBind();
        }

        protected void GridChildGoodsList_PageIndexChange(object sender, GridPageEventArgs e)
        {
            GD_ChildGoodsList.PageIndex = e.NewPageIndex;
            BindingChildGoods();
        }

        #endregion

        #region --> 筛选哪些属性是默认选择

        private void BindingDefaultSelectField(List<GoodsFieldInfo> fieldList, List<GoodsFieldInfo> selectedFieldList)
        {
            var lightFields = new List<string>();
            var astigmatismFields = new List<string>();
            var axialFields = new List<string>();

            var goodsFieldInfo1 = fieldList.FirstOrDefault(w => w.FieldName == "光度");
            if (goodsFieldInfo1 != null)
            {
                List<GoodsFieldInfo> lightFieldList = selectedFieldList.Where(w => w.ParentFieldId == goodsFieldInfo1.FieldId).OrderBy(w => w.FieldValue).ToList();
                var list = new List<GoodsFieldInfo> { new GoodsFieldInfo { FieldId = Guid.Empty, FieldValue = "全部" } };
                list.AddRange(lightFieldList);
                DDL_LightFields.Items.Clear();
                foreach (var info in list)
                {
                    DDL_LightFields.Items.Add(new ListItem(info.FieldValue, info.FieldId.ToString()));
                }

                lightFields.AddRange(lightFieldList.Select(item => item.FieldId.ToString()));
            }
            else
            {
                DDL_LightFields.Enabled = false;
            }
            var goodsFieldInfo2 = fieldList.FirstOrDefault(w => w.FieldName == "散光");
            if (goodsFieldInfo2 != null)
            {
                List<GoodsFieldInfo> astigmatismFieldList = selectedFieldList.Where(w => w.ParentFieldId == goodsFieldInfo2.FieldId).OrderBy(w => w.FieldValue).ToList();
                var list = new List<GoodsFieldInfo> { new GoodsFieldInfo { FieldId = Guid.Empty, FieldValue = "全部" } };
                list.AddRange(astigmatismFieldList);
                DDL_AstigmatismFields.Items.Clear();
                foreach (var info in list)
                {
                    DDL_AstigmatismFields.Items.Add(new ListItem(info.FieldValue, info.FieldId.ToString()));
                }

                astigmatismFields.AddRange(astigmatismFieldList.Select(item => item.FieldId.ToString()));
            }
            else
            {
                DDL_AstigmatismFields.Enabled = false;
            }
            var goodsFieldInfo3 = fieldList.FirstOrDefault(w => w.FieldName == "轴位");
            if (goodsFieldInfo3 != null)
            {
                List<GoodsFieldInfo> axialFieldList = selectedFieldList.Where(w => w.ParentFieldId == goodsFieldInfo3.FieldId).OrderBy(w => int.Parse(w.FieldValue)).ToList();
                var list = new List<GoodsFieldInfo> { new GoodsFieldInfo { FieldId = Guid.Empty, FieldValue = "全部" } };
                list.AddRange(axialFieldList);
                DDL_AxialFields.Items.Clear();
                foreach (var info in list)
                {
                    DDL_AxialFields.Items.Add(new ListItem(info.FieldValue, info.FieldId.ToString()));
                }

                axialFields.AddRange(axialFieldList.Select(item => item.FieldId.ToString()));
            }
            else
            {
                DDL_AxialFields.Enabled = false;
            }

            //赋值绑定
            CBL_LightFieldList.SelectedValueArray = lightFields.ToArray();
            CBL_AstigmatismFieldList.SelectedValueArray = astigmatismFields.ToArray();
            CBL_AxialViewFieldList.SelectedValueArray = axialFields.ToArray();

            SelectedFieldIds = selectedFieldList.Select(w => w.FieldId).ToList();
        }

        #endregion

        #region --> 事件

        #region -->OnSelectedIndexChanged

        protected void CblField_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbl = (CheckBoxList)sender;
            if (SelectedFieldIds.Count > 0)
            {
                foreach (var item in cbl.Items)
                {
                    if (!item.Selected)
                    {
                        if (RblDelete.SelectedValue == "0")//普通删除
                        {
                            var fieldId = new Guid(item.Value);
                            if (SelectedFieldIds.Any(w => w == fieldId))
                            {
                                string errorMessage;
                                var result = AllowDeleted(GoodsId, new List<Guid> { fieldId }, null, out errorMessage);
                                if (!result && !string.IsNullOrEmpty(errorMessage))
                                {
                                    //不允许删除该属性
                                    Alert.Show(errorMessage, "提示", MessageBoxIcon.Warning);
                                    item.Selected = true;
                                }
                                break;
                            }
                        }
                    }
                }
            }

        }

        #endregion

        #region --> OnClick

        protected void BT_SearchChildGoods_OnClick(object sender, EventArgs e)
        {
            BindingChildGoods();
        }

        protected void BT_BuildChildGoods_OnClick(object sender, EventArgs e)
        {
            form_Field.Expanded = false;
            GD_ChildGoodsList.Expanded = true;

            var lightFields = CBL_LightFieldList.SelectedValueArray;
            var astigmatismField = CBL_AstigmatismFieldList.SelectedValueArray;
            var axialFields = CBL_AxialViewFieldList.SelectedValueArray;

            var dict = new Dictionary<Guid, IEnumerable<Guid>>();
            if (lightFields.ToGuids().Any())
            {
                dict.Add(HF_LightFieldParentId.Text.ToGuid(), lightFields.Select(w => new Guid(w)).ToList());
            }
            if (astigmatismField.ToGuids().Any())
            {
                dict.Add(HF_AstigmatismFieldParentId.Text.ToGuid(), astigmatismField.Select(w => new Guid(w)).ToList());
            }
            if (axialFields.ToGuids().Any())
            {
                dict.Add(HF_AxialViewFieldParentId.Text.ToGuid(), axialFields.Select(w => new Guid(w)).ToList());
            }
            string failMessage;
            var dicts = dict.ToDictionary(f => f.Key, f => f.Value.ToList());
            var success = _goodsCenterSao.CreatRealGoods(GoodsId, dicts, out failMessage);
            if (success)
            {
                List<GoodsFieldInfo> fieldList;
                List<GoodsFieldInfo> selectedFieldList;
                string errorMessage;
                bool result = _goodsCenterSao.GetRealGoodsGoodsEditRequestModel(GoodsId, out fieldList, out selectedFieldList, out errorMessage);
                if (result)
                {
                    BindingDefaultSelectField(fieldList, selectedFieldList);
                    BindingChildGoods();//绑定现有的子商品信息
                }
                else
                {
                    Alert.Show(errorMessage, "重新加载异常", MessageBoxIcon.Error);
                }
            }
            else
            {
                Alert.Show(failMessage, "子商品生成提示", MessageBoxIcon.Error);
            }
        }

        protected void BT_IsActive_OnClick(object sender, EventArgs e)
        {
            var isSelected = hfIsSelected.Text == "1";
            var strRealGoodsId = hfRealGoodsId.Text;
            if (!string.IsNullOrEmpty(strRealGoodsId))
            {
                var childGoodsInfo = _goodsCenterSao.GetChildGoodsInfo(new Guid(strRealGoodsId));
                childGoodsInfo.IsActive = !isSelected;
                string msg;
                if (RblDelete.SelectedValue == "0" && isSelected)//普通删除验证
                {
                    bool isAllow = AllowDeleted(childGoodsInfo.GoodsId, null, new List<Guid> { childGoodsInfo.RealGoodsId }, out msg);
                    if (!isAllow)
                    {
                        Alert.Show(msg, "保存子商品", MessageBoxIcon.Error);
                        BindingChildGoods();
                        return;
                    }
                }
                var personnelInfo = CurrentSession.Personnel.Get();
                bool isSuccess = _goodsCenterSao.UpdateChildGoodsInfo(childGoodsInfo, personnelInfo.RealName, personnelInfo.PersonnelId, out msg);
                if (!isSuccess)
                {
                    Alert.Show(msg, "保存子商品", MessageBoxIcon.Error);
                    BindingChildGoods();
                    return;
                }
                //添加这条记录到管理系统
                //var personnelInfo = CurrentSession.Personnel.Get();
                //var goodsInfo = GoodsManager.GetGoodsInfo(GoodsId) ?? new GoodsInfo();
                //_operationLogManager.Add(personnelInfo.PersonnelId, personnelInfo.RealName, childGoodsInfo.RealGoodsId, goodsInfo.GoodsCode, OperationPoint.GoodsSettingManager.RealGoodsDelete.GetBusinessInfo(),
                //                         1, childGoodsInfo.IsActive ? "不删除 [" + childGoodsInfo.Specification + " ]" : "删除 [" + childGoodsInfo.Specification + " ]");
                BindingChildGoods();
            }
        }

        protected void BT_IsScarcity_OnClick(object sender, EventArgs e)
        {
            var isSelected = hfIsSelected.Text == "1";
            var strRealGoodsId = hfRealGoodsId.Text;
            if (!string.IsNullOrEmpty(strRealGoodsId))
            {
                var childGoodsInfo = _goodsCenterSao.GetChildGoodsInfo(new Guid(strRealGoodsId));
                childGoodsInfo.IsScarcity = isSelected;
                string failMessage;
                var personnelInfo = CurrentSession.Personnel.Get();
                var success = _goodsCenterSao.UpdateChildGoodsInfo(childGoodsInfo, personnelInfo.RealName, personnelInfo.PersonnelId, out failMessage);
                if (!success)
                {
                    Alert.Show(failMessage, "保存子商品", MessageBoxIcon.Error);
                }
                //else
                //{
                ////添加这条记录到管理系统
                //var personnelInfo = CurrentSession.Personnel.Get();
                //var goodsInfo = GoodsManager.GetGoodsInfo(GoodsId);
                //_operationLogManager.Add(personnelInfo.PersonnelId, personnelInfo.RealName, childGoodsInfo.RealGoodsId, goodsInfo.GoodsCode,
                //    OperationPoint.GoodsSettingManager.RealGoodsOutOfStock.GetBusinessInfo(), 1, childGoodsInfo.IsScarcity ? "缺货 [" + childGoodsInfo.Specification + " ]" : "不缺货 [" + childGoodsInfo.Specification + " ]");
                //}
                BindingChildGoods();
            }
        }

        protected void BT_Disable_OnClick(object sender, EventArgs e)
        {
            var result = GetPowerOperationPoint("ReceptionIsDisable");//是否有前台禁用权限
            if (!result)
            {
                Alert.Show("系统提示：您没有此操作权限！");
                BindingChildGoods();
            }
            else
            {
                var isSelected = hfIsSelected.Text == "1";
                var strRealGoodsId = hfRealGoodsId.Text;
                if (!string.IsNullOrEmpty(strRealGoodsId))
                {
                    var childGoodsInfo = _goodsCenterSao.GetChildGoodsInfo(new Guid(strRealGoodsId));
                    childGoodsInfo.Disable = isSelected;
                    string failMessage;
                    var personnelInfo = CurrentSession.Personnel.Get();
                    var success = _goodsCenterSao.UpdateChildGoodsInfo(childGoodsInfo, personnelInfo.RealName, personnelInfo.PersonnelId, out failMessage);
                    if (!success)
                    {
                        Alert.Show(failMessage, "保存子商品", MessageBoxIcon.Error);
                    }
                    //else
                    //{
                    //    //添加这条记录到管理系统
                    //    var personnelInfo = CurrentSession.Personnel.Get();
                    //    var goodsInfo = GoodsManager.GetGoodsInfo(GoodsId) ?? new GoodsInfo();
                    //    _operationLogManager.Add(personnelInfo.PersonnelId, personnelInfo.RealName, childGoodsInfo.RealGoodsId, goodsInfo.GoodsCode,
                    //        OperationPoint.GoodsSettingManager.RealGoodsSaleShow.GetBusinessInfo(), 1, childGoodsInfo.Disable ? "前台禁用 [ " + childGoodsInfo.Specification + " ]" : "前台启用 [" + childGoodsInfo.Specification + " ]");
                    //}
                    BindingChildGoods();
                }
            }
        }

        protected void BT_Cancel_OnClick(object sender, EventArgs e)
        {
            BindingChildGoods();
        }

        #endregion

        #region --> OnAfterEdit

        protected void GD_ChildGoodsList_OnAfterEdit(object sender, GridAfterEditEventArgs e)
        {
            var row = GD_ChildGoodsList.Rows[e.RowIndex];
            var realGoodsId = row.DataKeys[0].ToString().ToGuid();
            var childGoodsInfo = _goodsCenterSao.GetChildGoodsInfo(realGoodsId);
            var dict = GD_ChildGoodsList.GetModifiedDict();
            string value = string.Empty;
            var keyValue = dict[e.RowIndex];
            if (keyValue.ContainsKey(e.ColumnID))
            {
                value = keyValue[e.ColumnID];
            }

            if (e.ColumnID == "Column_Barcode")
            {
                childGoodsInfo.Barcode = value;

                string failMessage;
                var personnelInfo = CurrentSession.Personnel.Get();
                var success = _goodsCenterSao.UpdateChildGoodsInfo(childGoodsInfo, personnelInfo.RealName, personnelInfo.PersonnelId, out failMessage);
                if (success)
                {
                    GD_ChildGoodsList.CommitChanges();
                }
                else
                {
                    Alert.Show(failMessage, "保存子商品", MessageBoxIcon.Error);
                    GD_ChildGoodsList.RejectChanges();
                }
            }
            else if (e.ColumnID == "Column_IsScarcity")
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var isScarcity = value.ToLower() == "true";
                    childGoodsInfo.IsScarcity = isScarcity;

                    hfIsSelected.Text = isScarcity ? "1" : "0";
                    hfRealGoodsId.Text = realGoodsId.ToString();
                    var confirm = Confirm.GetShowReference("是否确认执行！", string.Empty, MessageBoxIcon.Question, BT_IsScarcity.GetPostBackEventReference(), BT_Cancel.GetPostBackEventReference());
                    PageContext.RegisterStartupScript(confirm);
                }
            }
            else if (e.ColumnID == "Column_Disable")
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var disable = value.ToLower() == "true";
                    childGoodsInfo.Disable = disable;

                    hfIsSelected.Text = disable ? "1" : "0";
                    hfRealGoodsId.Text = realGoodsId.ToString();
                    var confirm = Confirm.GetShowReference("是否确认执行！", string.Empty, MessageBoxIcon.Question, BT_Disable.GetPostBackEventReference(), BT_Cancel.GetPostBackEventReference());
                    PageContext.RegisterStartupScript(confirm);
                }
            }
            else if (e.ColumnID == "Column_IsActive")
            {
                if (!string.IsNullOrEmpty(value))
                {
                    var isActive = value.ToLower() == "true";
                    childGoodsInfo.IsActive = isActive;
                    //if (!isActive && RblDelete.SelectedValue == "0")
                    //{
                    //    isAllow = AllowDeleted(childGoodsInfo.GoodsId, null, new List<Guid> { childGoodsInfo.RealGoodsId }, out msg);
                    //    if (!isAllow)
                    //        childGoodsInfo.IsActive = true;
                    //}

                    hfIsSelected.Text = isActive ? "1" : "0";
                    hfRealGoodsId.Text = realGoodsId.ToString();
                    var confirm = Confirm.GetShowReference("是否确认执行！", string.Empty, MessageBoxIcon.Question, BT_IsActive.GetPostBackEventReference(), BT_Cancel.GetPostBackEventReference());
                    PageContext.RegisterStartupScript(confirm);
                }
            }

        }
        #endregion.

        #endregion

        #region 取得用户操作权限
        /// <summary>
        /// 取得用户操作权限
        /// </summary>
        protected bool GetPowerOperationPoint(string powerName)
        {
            const string PAGE_NAME = "NewGoods.aspx";
            return WebControl.GetPowerOperationPoint(PAGE_NAME, powerName);
        }
        #endregion

        #region [普通删除验证商品是否可以删除]

        /// <summary>普通删除验证商品是否可以删除
        /// </summary>
        /// <param name="goodsId"></param>
        /// <param name="fieldIds"></param>
        /// <param name="realGoods"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        protected bool AllowDeleted(Guid goodsId, List<Guid> fieldIds, List<Guid> realGoods, out string msg)
        {

            List<Guid> realGoodsIds;
            if (realGoods != null && realGoods.Count != 0)
            {
                realGoodsIds = realGoods;
            }
            else
            {
                int totalCount;
                var realGoodsList = _goodsCenterSao.GetRealGoodsListByPage(GoodsId, fieldIds, 1, int.MaxValue, out totalCount);
                realGoodsIds = realGoodsList.Select(w => w.RealGoodsId).ToList();
            }
            var canDeleteDic = WMSSao.CanDeleteForGoodsSet(realGoodsIds);
            if (!canDeleteDic.All(ent => ent.Value))
            {
                msg = "该属性对应的商品有库存记录，不允许删除！";
                return false;
            }
            IGoodsOrder goodsOrder = new GoodsOrder(GlobalConfig.DB.FromType.Read);
            List<int> stockStates = new List<int>
            {
                    (int) StorageRecordState.WaitAudit,
                    (int) StorageRecordState.Refuse,
                    (int) StorageRecordState.Approved,
                    (int) StorageRecordState.Finished
            };
            if (goodsOrder.SelectSemiStockAtOneYearByGoodsId(GoodsId, realGoodsIds, null, 365, stockStates))
            {
                msg = "该属性对应的商品1年内有进行过出入库记录，不允许删除！";
                return false;
            }
            var purchasing = new PurchasingManager(new Purchasing(GlobalConfig.DB.FromType.Read), _goodsCenterSao, null, null, null);
            if (purchasing.SelectPurchasingNoCompleteByGoodsId(GoodsId, realGoodsIds))
            {
                msg = " 该商品存在未完成的采购单，不允许删除！";
                return false;
            }
            if (CacheCollection.Filiale.GetHeadList().Where(f => f.FilialeTypes.Contains((int)FilialeType.EntityShop)).Any(source => StockSao.IsExistGoodsStock(source.ID, GoodsId, realGoodsIds)))
            {
                msg = "该属性对应的商品门店有库存，不允许删除！";
                return false;
            }

            //子商品删除
            if (_storageRecordDao.IsExistNormalStorageRecord(GoodsId, realGoodsIds))
            {
                msg = " 该商品存在未审核的出入库单据，不允许删除！";
                return false;
            }
            msg = string.Empty;
            return true;
        }

        #endregion
    }
}