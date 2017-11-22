using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using Telerik.Web.UI;
using ERP.Enum;
using System.Configuration;
using System.Text;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Config.Keede.Library;

namespace ERP.UI.Web
{
    public partial class GoodsTypeTaxratePage : BasePage
    {
        private readonly ITaxrateProportion _taxrateProportion=new TaxrateProportionDao();

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                string pageName = Common.WebControl.FileName;
                CanSetting = Common.WebControl.GetPowerOperationPoint(pageName, "Setting");
            }
        }

        public List<decimal> DataSource
        {
            get
            {
                return ViewState["DataSource"] == null ? new List<decimal>()
                    : (List<decimal>)ViewState["DataSource"];
            }
            set { ViewState["DataSource"] = value; }
        }

        public bool CanSetting
        {
            get
            {
                return ViewState["CanSetting"] != null && (bool)ViewState["CanSetting"];
            }
            set { ViewState["CanSetting"] = value; }
        }

        protected void RgGoodsTypeListNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var percentageValues = ConfManager.GetAppsetting("Percentage");
            if (string.IsNullOrEmpty(percentageValues))
            {
                RAM.Alert("配置文件中未配置Percentage(默认税率列表)");
                return;
            }
            var percentages = percentageValues.Split(',').Select(Convert.ToDecimal);
            var otherPercentages = _taxrateProportion.AllPercentage();
            otherPercentages.AddRange(percentages.Where(ent => !otherPercentages.Contains(ent)));
            DataSource = otherPercentages;
            var dics = _taxrateProportion.GetNewPercentages(null).ToDictionary(k => k.GoodsType, v => v);
            IDictionary<Int32, string> goodsTypes = Enum.Attribute.EnumAttribute.GetDict<GoodsKindType>();
            var index = 1;
            var dataSource = new List<ShowGridProportion>();
            foreach (var item in goodsTypes.Where(ent => ent.Key != (int)GoodsKindType.NoSet).OrderBy(ent => ent.Key))
            {
                var proportion = dics.ContainsKey(item.Key) ? dics[item.Key] : new TaxrateProportionInfo();
                dataSource.Add(new ShowGridProportion(index,item.Key, item.Value, proportion.GoodsTypeCode, proportion.Percentage, 0));
                index++;
            }
            RgGoodsTypeList.DataSource = dataSource;
        }

        
        protected void RgGoodsTypeListItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var item = (GridDataItem)e.Item;
                var percentage = Convert.ToDecimal(item.GetDataKeyValue("OriginalValue"));
                var ddlProportion = (DropDownList)e.Item.FindControl("DdlProportion");
                if (ddlProportion != null)
                {
                    ddlProportion.Items.Clear();
                    ddlProportion.Text = "";
                    ddlProportion.Items.Add(new ListItem("0.00%", "0"));
                    foreach (var percent in DataSource.Where(ent=>ent>0).OrderBy(ent=>ent))
                    {
                        ddlProportion.Items.Add(new ListItem(string.Format("{0}%", percent.ToString("F2")), percent.ToString("F2")));
                    }
                    ddlProportion.Items.Add(new ListItem("自定义", string.Format("{0}", byte.MaxValue)));
                    ddlProportion.SelectedValue = string.Format("{0}", percentage);
                }
                var setControl = (TextBox)e.Item.FindControl("TbProportion");
                var lbUnit = (Label)e.Item.FindControl("LbUnit");
                if (setControl!=null && lbUnit != null)
                {
                    setControl.Visible = false;
                    lbUnit.Visible = false;
                }
                var imageBtn = (ImageButton)e.Item.FindControl("BtnSave");
                if (imageBtn != null)
                {
                    imageBtn.Visible = CanSetting;
                }
                var tbCode = (TextBox)e.Item.FindControl("TbTypeCode");
                if (tbCode != null )
                {
                    tbCode.Enabled = string.IsNullOrEmpty(tbCode.Text.Trim()) && CanSetting;
                }
            }
        }


        protected void RGGoodsTyp_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = (GridEditableItem)e.Item;
            //var btn = (ImageButton)sender;
            if (editedItem != null)
            {
                var ddl = ((DropDownList)editedItem.FindControl("DdlProportion"));
                if (ddl != null)
                {
                    var typeName = editedItem.GetDataKeyValue("GoodsTypeName");
                    var key = Convert.ToInt32(editedItem.GetDataKeyValue("GoodsType"));
                    var originalValue = Convert.ToDecimal(editedItem.GetDataKeyValue("OriginalValue"));
                    var tbCode = editedItem.FindControl("TbTypeCode") as TextBox;
                    if (tbCode == null || string.IsNullOrEmpty(tbCode.Text))
                    {
                        RAM.Alert("商品类型编码不能为空");
                        return;
                    }

                    string code = tbCode.Text.Trim();
                    if (!System.Text.RegularExpressions.Regex.IsMatch(code, "^[0-9]{1,}$"))
                    {
                        RAM.Alert("商品类型编码格式不正确(只能输入数字)");
                        return;
                    }
                    decimal newValue = 0;
                    if (ddl.SelectedValue == string.Format("{0}", byte.MaxValue))
                    {
                        var textBox = editedItem.FindControl("TbProportion") as TextBox;
                        if (textBox != null)
                        {
                            if (System.Text.RegularExpressions.Regex.IsMatch(textBox.Text.Trim(), "^\\d+(\\.\\d+)?$"))
                            {
                                newValue = Convert.ToDecimal(textBox.Text);
                            }
                            else
                            {
                                RAM.Alert(string.IsNullOrEmpty(textBox.Text) ? "请输入自定义税率" : "您填写的商品税率必须大于等于0");
                                return;
                            }
                        }
                    }
                    else
                    {
                        newValue = Convert.ToDecimal(ddl.SelectedValue);
                    }
                    var personnelInfo = CurrentSession.Personnel.Get();
                    var lastPercent = _taxrateProportion.GetLastPercentage(key);
                    string remark = string.Format("【{0}】", lastPercent == -1 ? "第一次设置" : string.Format("原税率为：{0}%", lastPercent));
                    if (lastPercent != -1 && lastPercent == newValue)
                    {
                        RAM.Alert(string.Format("{0}-税率未做任何调整", typeName));
                        return;
                    }
                    byte operateType = lastPercent == -1 ? (Byte)OperateType.Add : (Byte)OperateType.Edit;
                    var result = _taxrateProportion.Insert(new TaxrateProportionInfo(Guid.NewGuid(), key, code, newValue, DateTime.Now, personnelInfo.RealName, operateType, remark));
                    if (!result)
                    {
                        RAM.Alert(string.Format("{0}-税率更新失败", typeName));
                        return;
                    }
                    RAM.Alert("您修改的税率修改成功，请确认！");
                    RgGoodsTypeList.Rebind();
                }
            }
        }

        protected void DdlProportionSelectChanged(object sender, EventArgs e)
        {
            var ddl = (DropDownList)sender;
            if (ddl != null)
            {
                var setting = ddl.SelectedValue == string.Format("{0}", byte.MaxValue);
                ddl.Parent.Controls[3].Visible = setting;
                ddl.Parent.Controls[5].Visible = setting;

                var item = (ddl.Parent.Parent.Controls[6]).Controls[1] as ImageButton;
                if (item != null)
                    item.Enabled = !setting;

                var items = ((RadGrid) ddl.Parent.Parent.Parent.Parent.Parent).Items;
                var currentItem= (GridDataItem)ddl.Parent.Parent;
                
                foreach (var dataItem in items)
                {
                    var data = (GridDataItem) dataItem;
                    if(data.GetDataKeyValue("GoodsType") == currentItem.GetDataKeyValue("GoodsType"))continue;
                    var ddlProportion = (DropDownList)data.FindControl("DdlProportion");
                    if(ddlProportion==null)continue;
                    var txt=(TextBox)data.FindControl("TbProportion");
                    if(ddlProportion.SelectedValue!= string.Format("{0}", byte.MaxValue))continue;
                    var lbUnit = (Label)data.FindControl("LbUnit");
                    var originalValue = data.GetDataKeyValue("OriginalValue");
                    if(originalValue==null)continue;
                    if (txt != null && lbUnit != null)
                    {
                        txt.Visible = false;
                        lbUnit.Visible = false;

                        ddlProportion.SelectedValue = string.Format("{0}", Convert.ToDecimal(originalValue)>0?originalValue:"0");
                    }
                }
            }
        }
        
    }

    /// <summary>
    /// 
    /// </summary>
    [Serializable]
    public class ShowGridProportion
    {
        public int OrderIndex { get; set; }

        /// <summary>
        /// 商品类型编号
        /// </summary>
        public int GoodsType { get; set; }

        /// <summary>
        /// 商品类型名称
        /// </summary>
        public string GoodsTypeName { get; set; }

        /// <summary>
        /// 原税率
        /// </summary>
        public decimal OriginalValue { get; set; }

        /// <summary>
        /// 商品类型编码
        /// </summary>
        public string GoodsTypeCode { get; set; }

        /// <summary>
        /// 税率
        /// </summary>
        public decimal NewValue { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orderIndex"></param>
        /// <param name="goodsType"></param>
        /// <param name="goodsTypeName"></param>
        /// <param name="goodsTypeCode"></param>
        /// <param name="originalValue"></param>
        /// <param name="newValue"></param>
        public ShowGridProportion(int orderIndex,int goodsType,string goodsTypeName,string goodsTypeCode,decimal originalValue,decimal newValue)
        {
            OrderIndex = orderIndex;
            GoodsType = goodsType;
            GoodsTypeName = goodsTypeName;
            OriginalValue = originalValue;
            NewValue = newValue;
            GoodsTypeCode = goodsTypeCode;
        }
    }
}