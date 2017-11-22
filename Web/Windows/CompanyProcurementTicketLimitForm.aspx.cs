using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web.Windows
{
    /// <summary>▄︻┻┳═一 采购索票额度详情   ADD 2014-12-22  陈重文
    /// Modify by liangcanren at 2015-04-16  需求：1639
    /// </summary>
    public partial class CompanyProcurementTicketLimitForm : WindowsPage
    {
        private readonly ProcurementTicketLimitDAL _procurementTicketLimitDal=new ProcurementTicketLimitDAL(GlobalConfig.DB.FromType.Write);
        readonly ICompanyCussent _companyCussent = new CompanyCussent(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            RadGridCompanyProcurementTicketLimit.ItemDataBound += RadGridCompanyProcurementTicketLimit_OnItemDataBound;
            GridViewBind(RadGridCompanyProcurementTicketLimit, "CompanyId");
        }
        #region [文本框触发保存]
        /// <summary>
        /// 文本框TextChanged事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void OnTextChanged_TakerTicketLimit(object sender, EventArgs e)
        {
            try
            {
                var tbTakerTicketLimit = (TextBox)sender;
                var dataItem = (GridDataItem)tbTakerTicketLimit.Parent.Parent;
                var companyId = dataItem.GetDataKeyValue("CompanyId");
                var takerTicketLimit = WebControl.NumberRecovery(tbTakerTicketLimit.Text);
                //正则表达式
                if (Regex.IsMatch(takerTicketLimit, @"^(([1-9]\d{0,9})|0)(\.\d{1,2})?$"))
                {
                    var info = new ProcurementTicketLimitInfo
                    {
                        CompanyId = new Guid(companyId.ToString()),
                        FilialeId = new Guid(tbTakerTicketLimit.ID),
                        DateYear = DateYear,
                        DateMonth = DateMonth,
                        TakerTicketLimit = Convert.ToDecimal(takerTicketLimit)
                    };
                    var result = _procurementTicketLimitDal.SaveProcurementTicketLimit(info);
                    if (!result)
                    {
                        RAM.Alert("系统提示：数据保存异常，请尝试刷新后继续操作！");
                        return;
                    }
                    var ticketInfo=TicketLimitBindInfos.FirstOrDefault(act => act.CompanyId == info.CompanyId);
                    if (ticketInfo!=null && ticketInfo.LimitSettingsList.ContainsKey(info.FilialeId))
                    {
                        ticketInfo.LimitSettingsList[info.FilialeId] = info.TakerTicketLimit;
                    }
                    RAM.ResponseScripts.Add("Rebind()");
                }
                else
                {
                    RAM.Alert("系统提示：输入金额格式不正确！");
                }
            }
            catch (Exception)
            {
                RAM.Alert("系统提示：保存异常，请尝试刷新后继续操作！");
            }
            RadGridCompanyProcurementTicketLimit.Rebind();
        }

        #endregion

        #region [行绑定事件]

        protected void RadGridCompanyProcurementTicketLimit_OnItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                var dataItem = (GridDataItem)e.Item;
                var companyId = new Guid(dataItem.GetDataKeyValue("CompanyId").ToString());
                var filialeList = CacheCollection.Filiale.GetHeadList().ToList();

                //对应公司金额
                foreach (var filialeInfo in filialeList)
                {
                    var filiale=e.Item.FindControl(string.Format("{0}",filialeInfo.ID)) as TextBox;
                    if (filiale != null)
                    {
                        filiale.Enabled = _companyCussent.GetCompanyIsBindingFiliale(companyId, filialeInfo.ID);
                        var info =
                            ProcurementTicketLimitList.FirstOrDefault(
                                act => act.CompanyId == companyId && act.FilialeId == filialeInfo.ID);
                        if(info!=null)
                            filiale.Text = WebControl.NumberSeparator(info.TakerTicketLimit);
                    }
                }
            }
        }

        #endregion

        public IList<TicketLimitBindInfo> TicketLimitBindInfos
        {
            get
            {
                if(ViewState["TicketLimitBindInfos"]==null)return new List<TicketLimitBindInfo>();
                return (IList<TicketLimitBindInfo>)ViewState["TicketLimitBindInfos"];
            }
            set { ViewState["TicketLimitBindInfos"] = value; }
        }

        #region  新增动态创建列

        /// <summary>
        /// 绑定生成GridView
        /// </summary>
        /// <param name="gdv">要绑定的GridView</param>
        /// <param name="strDataKey">GridView的DataKeyNames</param>
        public void GridViewBind(RadGrid gdv, string strDataKey)
        {
            gdv.Columns.Clear();

            gdv.AutoGenerateColumns = false;
            var filialeList = CacheCollection.Filiale.GetHeadList();
            if (!IsPostBack)
            {
                var dataList = ProcurementTicketLimitList;
                var companyIds = dataList.Select(act => act.CompanyId).Distinct();
                IList<TicketLimitBindInfo> ticketLimitBindInfos = (from companyId in companyIds
                                                                   let companyInfo = dataList.First(act => act.CompanyId == companyId)
                                                                   let dics = dataList.Where(act => act.CompanyId == companyId && act.FilialeId != Guid.Empty)
                     .ToDictionary(k => k.FilialeId, v => v.TakerTicketLimit)
                                                                   select new TicketLimitBindInfo
                                                                   {
                                                                       CompanyId = companyId,
                                                                       ComapnyName = companyInfo != null ? companyInfo.CompanyName : string.Empty,
                                                                       LimitSettingsList = dics,
                                                                   }).ToList();
                TicketLimitBindInfos = ticketLimitBindInfos.OrderByDescending(act=>act.LimitSettingsList.Sum(k=>k.Value)).ToList();
            }
            gdv.DataSource = TicketLimitBindInfos;
            gdv.MasterTableView.DataKeyNames = new [] { strDataKey };

            var bfColumn = new GridBoundColumn { DataField = "ComapnyName", HeaderText = "供应商" };
            bfColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
            gdv.Columns.Add(bfColumn);
            bool allowEdit = CheckIsCanSave();
            foreach (var t in filialeList)
            {
                //添加文本框模板列
                var tf = new GridTemplateColumn
                {
                    ItemTemplate = new NewTemplate("TbFiliale", DataControlRowType.DataRow, string.Format("{0}", t.ID), allowEdit, OnTextChanged_TakerTicketLimit),
                    HeaderTemplate = new NewTemplate(t.Name, DataControlRowType.Header, "",false,null)
                };
                tf.ItemStyle.HorizontalAlign = HorizontalAlign.Center;
                gdv.Columns.Add(tf);
            }
            gdv.DataBind();//绑定
        }
        #endregion

        /// <summary>判断是否可以保存
        /// </summary>
        /// <returns></returns>
        protected bool CheckIsCanSave()
        {
            var year = Convert.ToInt32(DateYear);
            var month = Convert.ToInt32(DateMonth);
            DateTime dateTime = Convert.ToDateTime(month == 12 ? string.Format("{0}-{1}-01 00:00", year + 1, 1) : string.Format("{0}-{1}-01 00:00", year, month + 1));
            return dateTime > DateTime.Now;
        }

        #region ViewState

        /// <summary>其他公司该供应商额度
        /// </summary>
        protected IList<ProcurementTicketLimitInfo> ProcurementTicketLimitList
        {
            get
            {
                var pageIndex = RadGridCompanyProcurementTicketLimit.CurrentPageIndex + 1;
                var pageSize = RadGridCompanyProcurementTicketLimit.PageSize;
                int totalCount;
                return _procurementTicketLimitDal.GetProcurementTicketLimitDetailList(Guid.Empty,
                    DateYear, DateMonth, pageIndex, pageSize, out totalCount);
            }

        }

        /// <summary>年份
        /// </summary>
        private int DateYear
        {
            get
            {
                return Convert.ToInt32(Request.QueryString["DateYear"]);
            }
        }

        /// <summary>月份
        /// </summary>
        private int DateMonth
        {
            get
            {
                return Convert.ToInt32(Request.QueryString["DateMonth"]);
            }
        }

        #endregion
    }

    /// <summary>
    /// 动态绑定时添加绑定模版
    /// add by liangcanren at 2015-04-16
    /// </summary>
    [Serializable]
    public class NewTemplate : ITemplate
    {
        private readonly string _strColumnName;
        private readonly DataControlRowType _dcrtColumnType;
        private readonly string _controlId;
        private readonly EventHandler _eventHandler;  //文本框TextChanged事件绑定
        private readonly bool _enable;

        public NewTemplate()
        {
            
        }

        /// <summary>
        /// 动态添加模版列
        /// </summary>
        /// <param name="strColumnName">列名</param>
        /// <param name="dcrtColumnType">列的类型</param>
        /// <param name="controlId">控件名称</param>
        /// <param name="allowEdit"></param>
        /// <param name="handler">绑定事件</param>
        public NewTemplate(string strColumnName, DataControlRowType dcrtColumnType,string controlId,bool allowEdit,EventHandler handler)
        {
            _strColumnName = strColumnName;
            _dcrtColumnType = dcrtColumnType;
            _controlId = controlId;
            _eventHandler = handler;
            _enable = allowEdit;
        }

        public void InstantiateIn(Control container)
        {
            switch (_dcrtColumnType)
            {
                case DataControlRowType.Header: //列标题

                    //如果头部使用标题则使用以下代码
                    var ltr = new Literal {Text = _strColumnName};
                    container.Controls.Add(ltr);
                    break;
                case DataControlRowType.DataRow:
                    var cb = new TextBox { ID = string.Format("{0}", _controlId), Text = "0",AutoPostBack = true,Enabled = _enable};
                    cb.TextChanged += _eventHandler;
                    container.Controls.Add(cb);
                    break;
            }
        }
    }
}