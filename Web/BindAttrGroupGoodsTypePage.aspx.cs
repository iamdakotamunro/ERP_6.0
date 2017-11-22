using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.UI.WebControls;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class BindAttrGroupGoodsTypePage : BasePage
    {
        readonly IGoodsCenterSao _goodsAttributeGroupSao = new GoodsCenterSao();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BindGoodsType();
                BindGroups(RTV_GoodsType.SelectedNode);
            }
        }

        private IList<AttributeGroupInfo> AttrGroupList
        {
            get { return _goodsAttributeGroupSao.GetAttrGroupList().OrderBy(p => p.OrderIndex).ToList(); }
        }

        protected string GetMatchType(object matchType)
        {
            return EnumAttribute.GetKeyName((MatchType)matchType);
        }

        /// <summary> 添加根节点
        /// </summary>
        protected void BindGoodsType()
        {
            RTV_GoodsType.Nodes.Clear();
            RadTreeNode rootNode = CreateNode("商品类型", true, string.Empty);
            rootNode.Category = "Type";
            rootNode.Selected = true;
            RTV_GoodsType.Nodes.Add(rootNode);
            CreateTypeNode(rootNode);
        }

        /// <summary> 树形控件添加商品类型节点
        /// </summary>
        /// <param name="node"></param>
        private void CreateTypeNode(RadTreeNode node)
        {
            IDictionary<Int32, string> lstEnum = EnumAttribute.GetDict<GoodsKindType>();
            foreach (KeyValuePair<Int32, string> kp in lstEnum)
            {
                if (kp.Key == 0)
                {
                    continue;
                }
                RadTreeNode childNode = CreateNode(kp.Value, false, string.Format("{0}", kp.Key));
                node.Nodes.Add(childNode);
            }
        }

        /// <summary> 创建节点
        /// </summary>
        /// <param name="text"></param>
        /// <param name="expanded"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private RadTreeNode CreateNode(string text, bool expanded, string id)
        {
            var node = new RadTreeNode(text, id) { ToolTip = text, Expanded = expanded };
            return node;
        }

        protected void Rtv_GoodsType_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            BindGroups(e.Node);
        }

        private void BindGroups(RadTreeNode node)
        {
            if (node.Level == 1)
            {
                int goodsType = int.Parse(node.Value);
                IList<AttributeGroupInfo> list = _goodsAttributeGroupSao.GetAttrGroupList(goodsType).ToList();
                foreach (GridDataItem dataItem in GroupGrid.Items)
                {
                    var cbIsChecked = (CheckBox)dataItem.FindControl("cbIsChecked");
                    var labGoodsQuantity = (Label)dataItem.FindControl("Lab_GoodsQuantity");
                    var info = list.FirstOrDefault(w => w.GroupId == Convert.ToInt32(dataItem.GetDataKeyValue("GroupId").ToString()));
                    if (info != null)
                    {
                        cbIsChecked.Checked = info.IsSelect;
                        labGoodsQuantity.Text = string.Format("{0}", info.GoodsQuantity);
                        if (info.GoodsQuantity > 0)
                        {
                            cbIsChecked.InputAttributes.Add("onclick", "javascript:return confirm('该属性下有绑定商品，取消将删除所有商品对应属性！');");
                        }
                    }
                    else
                    {
                        cbIsChecked.Checked = false;
                        labGoodsQuantity.Text = "-";
                    }
                }
            }
            else
            {
                GroupGrid.Rebind();
            }
        }

        protected void GroupGrid_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            GroupGrid.DataSource = AttrGroupList;
        }

        protected void LbtnUpdate_OnClick(object sender, EventArgs e)
        {
            if (RTV_GoodsType.SelectedNode.Level < 1)
            {
                return;
            }
            int goodsType = Convert.ToInt32(RTV_GoodsType.SelectedNode.Level == 1
                                    ? RTV_GoodsType.SelectedNode.Value
                                    : RTV_GoodsType.SelectedNode.ParentNode.Value);
            var attrGroupList = (from GridDataItem dataItem in GroupGrid.Items let cbIsChecked = (CheckBox)dataItem.FindControl("cbIsChecked") where cbIsChecked.Checked select Convert.ToInt32(dataItem.GetDataKeyValue("GroupId").ToString())).ToList();
            try
            {
                using (var ts = new TransactionScope(TransactionScopeOption.Required))
                {
                    string errorMessage;
                    bool isSuccess = _goodsAttributeGroupSao.SetAttrGroupGoodsType(goodsType, attrGroupList, out errorMessage);
                    if (!isSuccess)
                    {
                        RAM.Alert("操作无效！" + errorMessage);
                        return;
                    }
                    ts.Complete();
                }

                ////记录工作日志
                //var pinfo = CurrentSession.Personnel.Get();
                //var type = HRS.Enum.OperationTypePoint.OperationLogTypeEnum.AttributeGroupTypeBind.GetBusinessInfo();
                //var point = HRS.Enum.OperationTypePoint.AttributeGroupTypeBindState.Update.GetBusinessInfo();
                //var logInfo = new HRS.Model.OperationLogInfo
                //{
                //    LogId = Guid.NewGuid(),
                //    PersonnelId = pinfo.PersonnelId,
                //    RealName = pinfo.RealName,
                //    TypeId = new Guid(type.Key),
                //    PointId = new Guid(point.Key),
                //    OperateTime = DateTime.Now,
                //    Description = type.Value + "--" + point.Value,
                //    IdentifyKey = Convert.ToString(goodsType)
                //};
                //OperationLogManager.InsertOperationLog(logInfo);

                RAM.Alert("更新成功!");
            }
            catch (Exception)
            {
                RAM.Alert("更新失败!");
            }
        }
    }
}