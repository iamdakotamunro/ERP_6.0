using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.Enum;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    public partial class EditGoodsAttribute : WindowsPage
    {
        readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //添加商品过来不显示取消按钮
                var isShow = Request.QueryString["IsShow"];
                if (!string.IsNullOrWhiteSpace(isShow))
                {
                    LB_Cancel.Visible = false;
                }
                AttributeList = _goodsCenterSao.GetAttributeListByGoodsId(GoodsId).ToList();
                BindGroup();
            }
        }

        #region [自定义属性]

        /// <summary>
        /// 商品ID
        /// </summary>
        private Guid GoodsId
        {
            get { return new Guid(Request["GoodsId"]); }
        }

        private IList<AttributeInfo> AttributeList
        {
            get
            {
                return ViewState["AttributeList"] == null ? new List<AttributeInfo>() : (List<AttributeInfo>)ViewState["AttributeList"];
            }
            set { ViewState["AttributeList"] = value; }
        }
        #endregion

        /// <summary> 添加根节点
        /// </summary>
        protected void BindGroup()
        {
            GoodsInfo goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(GoodsId);
            if (goodsInfo != null)
            {
                lbGoodsInfo.Text = "商品名称：" + goodsInfo.GoodsName;
                //通过商品类型获取其绑定组
                if (AttributeList.Count == 0)
                {
                    Lb_Msg.Text = "该类型商品尚未绑定任何属性组!";
                    PanelMsg.Visible = true;
                }
                else
                {
                    PanelMsg.Visible = false;
                }

                DL_GroupsList.DataSource = AttributeList;
                DL_GroupsList.DataBind();
            }
        }

        /// <summary> 属性绑定
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void DlGroupsList_OnItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var cAttributeInfo = (AttributeInfo)e.Item.DataItem;
                var rdbGroups = (RadComboBox)e.Item.FindControl("rdb_Groups");
                var txtGroups = (TextBox)e.Item.FindControl("Txt_Groups");
                var rdbOutlet = (RadComboBox)e.Item.FindControl("rdb_Outlet");


                AttributeInfo attributeInfo = null;
                if (AttributeList.Count > 0)
                {
                    attributeInfo = AttributeList.FirstOrDefault(a => a.GroupId == cAttributeInfo.GroupId);
                }
                if (attributeInfo != null)
                {

                    if (attributeInfo.MatchType == 0) //文字匹配
                    {
                        if (!attributeInfo.IsMChoice) //单选
                        {
                            rdbGroups.Visible = true;
                            txtGroups.Visible = false;
                            rdbOutlet.Visible = false;
                            BindRadComboBox(rdbGroups, "Word", "WordId", attributeInfo.AttributeWordList,
                                YesOrNo.Yes);
                            rdbGroups.ErrorMessage = cAttributeInfo.GroupId.ToString();
                            rdbGroups.SelectedValue = attributeInfo.Value;
                        }
                        else
                        {
                            rdbGroups.Visible = false;
                            txtGroups.Visible = false;
                            rdbOutlet.Visible = true;
                            BindRadComboBox(rdbOutlet, "Word", "WordId", attributeInfo.AttributeWordList, YesOrNo.No);
                            rdbOutlet.ErrorMessage = cAttributeInfo.GroupId.ToString();

                            List<string> list_CheckboxTest = new List<string>();
                            //已设置的默认属性词
                            foreach (var titem in rdbOutlet.Controls)
                            {
                                if (titem is RadComboBoxItem)
                                {
                                    var nitem = titem as RadComboBoxItem;
                                    foreach (var control in nitem.Controls)
                                    {
                                        if (control is CheckBox)
                                        {
                                            var cb = control as CheckBox;

                                            if (attributeInfo.Value.Contains(cb.ToolTip))
                                            {
                                                cb.Checked = true;
                                                
                                                int result = 0;
                                                int.TryParse(cb.ToolTip, out result);
                                                var temp = attributeInfo.AttributeWordList.Where(d => d.WordId == result);
                                                if (temp != null && temp.Count() > 0)
                                                {
                                                    list_CheckboxTest.Add(temp.First().Word);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            rdbOutlet.Text = string.Join(",", list_CheckboxTest.ToArray());
                        }
                    }
                    else
                    {
                        rdbGroups.Visible = false;
                        rdbOutlet.Visible = false;
                        txtGroups.Visible = true;
                        txtGroups.Text = attributeInfo.Value;
                    }
                }

            }
        }

        protected string GetGroupName(string groupId)
        {
            AttributeInfo groupInfo = AttributeList.FirstOrDefault(a => string.Format("{0}", a.GroupId) == groupId.ToString());
            if (groupInfo != null)
            {
                return groupInfo.GroupName + "：";
            }
            return "";
        }

        //确定
        protected void LbtnSetGoodsAttribute_OnClick(object sender, EventArgs e)
        {
            string showMsg = string.Empty;
            if (DL_GroupsList.Items.Count == 0)
            {
                return;
            }
            var list = new List<AttributeInfo>();
            bool isFormat = false;
            for (int i = 0; i < DL_GroupsList.Items.Count; i++)
            {
                if (DL_GroupsList.Items[i].ItemType == ListItemType.Item || DL_GroupsList.Items[i].ItemType == ListItemType.AlternatingItem)
                {
                    int groupId = Convert.ToInt32(DL_GroupsList.DataKeys[DL_GroupsList.Items[i].ItemIndex]);
                    var rdbGroups = (RadComboBox)DL_GroupsList.Items[i].FindControl("rdb_Groups");
                    var txt = (TextBox)DL_GroupsList.Items[i].FindControl("Txt_Groups");
                    var rdbOutlet = (RadComboBox)DL_GroupsList.Items[i].FindControl("rdb_Outlet");
                    var groupInfo = AttributeList.FirstOrDefault(a => a.GroupId == groupId);

                    if (groupInfo != null)
                    {
                        if (groupInfo.MatchType == 0)//文字匹配
                        {
                            if (!groupInfo.IsMChoice)
                            {
                                #region 文字匹配单选
                                if (rdbGroups.SelectedItem == null || rdbGroups.SelectedItem.Value == "0")
                                {
                                    isFormat = true;
                                    showMsg = groupInfo.GroupName;
                                    break;
                                }
                                list.Add(new AttributeInfo { GoodsId = GoodsId, GroupId = groupId, Value = rdbGroups.SelectedItem.Value });
                                #endregion
                            }
                            else
                            {
                                #region 文字匹配多选
                                bool isSel = false;
                                foreach (var titem in rdbOutlet.Controls)
                                {
                                    if (titem is RadComboBoxItem)
                                    {
                                        var item = titem as RadComboBoxItem;
                                        foreach (var control in item.Controls)
                                        {
                                            if (control is CheckBox)
                                            {
                                                var cb = control as CheckBox;
                                                if (cb.Checked)
                                                    isSel = true;
                                            }
                                        }
                                    }
                                }
                                if (!isSel) //无选择
                                {
                                    isFormat = true;
                                    showMsg = groupInfo.GroupName;
                                    break;
                                }
                                foreach (var titem in rdbOutlet.Controls)
                                {
                                    if (titem is RadComboBoxItem)
                                    {
                                        var item = titem as RadComboBoxItem;
                                        foreach (var control in item.Controls)
                                        {
                                            if (control is CheckBox)
                                            {
                                                var cb = control as CheckBox;
                                                if (cb.Checked)
                                                {
                                                    var info = new AttributeInfo
                                                    {
                                                        GoodsId = GoodsId,
                                                        GroupId = groupId,
                                                        Value = cb.ToolTip
                                                    };
                                                    list.Add(info);
                                                }

                                            }
                                        }
                                    }
                                }
                                #endregion
                            }
                        }
                        else if (groupInfo.MatchType == 1)//数字匹配
                        {
                            #region 数字匹配
                            string value = txt.Text.Trim();  //填写的值
                            if (string.IsNullOrEmpty(value))
                            {
                                isFormat = true;
                                showMsg = groupInfo.GroupName;
                                break;
                            }

                            IList<AttributeWordInfo> attrWordses = _goodsCenterSao.GetAttrWordsListByGroupId(groupId).ToList();
                            int wordId = 0;
                            for (int j = 0; j < attrWordses.Count; j++)//循环查找匹配属性词
                            {
                                var item = attrWordses[j];
                                double txtValue;
                                double wordValue;
                                switch (item.CompareType)
                                {
                                    case 0:
                                        if (value == item.WordValue)
                                        {
                                            wordId = item.WordId;
                                        }
                                        break;
                                    case 1:
                                        wordValue = double.Parse(item.WordValue);
                                        double topValue = double.Parse(item.TopValue);
                                        if (double.TryParse(value, out txtValue))
                                        {
                                            if (txtValue >= wordValue && txtValue <= topValue)
                                            {
                                                wordId = item.WordId;
                                            }
                                        }
                                        break;
                                    case 2:
                                        wordValue = double.Parse(item.WordValue);
                                        if (double.TryParse(value, out txtValue))
                                        {
                                            if (txtValue <= wordValue)
                                            {
                                                wordId = item.WordId;
                                            }
                                        }
                                        break;
                                    case 3:
                                        wordValue = double.Parse(item.WordValue);
                                        if (double.TryParse(value, out txtValue))
                                        {
                                            if (txtValue >= wordValue)
                                            {
                                                wordId = item.WordId;
                                            }
                                        }
                                        break;
                                }
                                if (wordId != 0) break;
                            }

                            if (wordId != 0)
                            {
                                list.Add(new AttributeInfo { GoodsId = GoodsId, GroupId = groupId, WordId = wordId, Value = value });
                            }
                            else
                            {
                                RAM.Alert(groupInfo.GroupName + "添加的内容不符合规则,请查看好规则后再添加!");
                                return;
                            }
                            #endregion
                        }
                        else if (groupInfo.MatchType == 2)//不匹配
                        {
                            string value = txt.Text.Trim();  //填写的值
                            if (string.IsNullOrEmpty(value))
                            {
                                isFormat = true;
                                showMsg = groupInfo.GroupName;
                                break;
                            }
                            list.Add(new AttributeInfo { GoodsId = GoodsId, GroupId = groupId, WordId = 0, Value = value });
                        }
                    }
                }

            }
            if (isFormat)
            {
                RAM.Alert("商品属性组--" + showMsg + "--值未选定值或为空!");
            }
            else
            {
                try
                {
                    string failMessage;
                    bool result = _goodsCenterSao.SetAttribute(GoodsId, list, out failMessage);
                    if (result)
                    {
                        RAM.Alert("更新成功!");
                        RAM.ResponseScripts.Add("CancelWindow();");
                    }
                    else
                    {
                        RAM.Alert("更新异常!" + failMessage);
                    }
                }
                catch (Exception ex)
                {
                    RAM.Alert("更新失败!" + ex.Message);
                }
            }
        }

        #region [绑定一个RadComboBox控件]

        /// <summary> 绑定一个RadComboBox控件
        /// </summary>
        /// <param name="rcb">控件</param>
        /// <param name="dataTextField">显示文字</param>
        /// <param name="dataValueField">值</param>
        /// <param name="dataSource">数据源</param>
        /// <param name="yesOrNo">是否有第一项,YES,有第一项,NO,没有</param>
        internal static void BindRadComboBox(RadComboBox rcb, string dataTextField, string dataValueField, List<AttributeWordInfo> dataSource, YesOrNo yesOrNo)
        {
            rcb.DataValueField = dataValueField;
            rcb.DataTextField = dataTextField;
            rcb.DataSource = dataSource;
            rcb.DataBind();
            if (yesOrNo == YesOrNo.Yes)
            {
                rcb.Items.Insert(0, new RadComboBoxItem("请选择", "0"));
            }
        }

        //搜索
        protected void rdb_Groups_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var rcb = (RadComboBox)sender;
            var groupId = rcb.ErrorMessage;
            if (string.IsNullOrEmpty(groupId))
            {
                return;
            }
            AttributeInfo attributeInfo = AttributeList.FirstOrDefault(a => a.GroupId == int.Parse(groupId));
            if (attributeInfo == null)
            {
                return;
            }
            rcb.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                var attributeWordList = attributeInfo.AttributeWordList.Where(p => p.Word.Contains(e.Text)).ToList();
                int totalCount = attributeWordList.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var item in attributeWordList)
                    {
                        rcb.Items.Add(new RadComboBoxItem(item.Word, item.WordId.ToString()));
                    }
                }
            }
        }
        #endregion


    }
}