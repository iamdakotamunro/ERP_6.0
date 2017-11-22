using System;
using System.Collections.Generic;
using System.Linq;
using ERP.BLL.Implement;
using ERP.DAL.Implement.Inventory;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Telerik.Web.UI;

namespace ERP.UI.Web.Windows
{
    /// <summary>
    /// 添加折扣和差额说明
    /// </summary>
    public partial class ShowReckoning : WindowsPage
    {
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        readonly CompanySubjectDiscountDal _companySubjectDiscountDao=new CompanySubjectDiscountDal(GlobalConfig.DB.FromType.Read);
        SubmitController _submitController;
        protected SubmitController CreateSubmitInstance()
        {
            if (ViewState["SubmitController"] == null)
            {
                _submitController = new SubmitController(Guid.NewGuid(), 3);
                ViewState["SubmitController"] = _submitController;
            }
            return (SubmitController)ViewState["SubmitController"];
        }

        public Dictionary<Guid, string> FilialeDics
        {
            get
            {
                if (ViewState["FilialeDics"] == null) return new Dictionary<Guid, string>();
                return (Dictionary<Guid, string>)ViewState["FilialeDics"];
            }
            set { ViewState["FilialeDics"] = value; }
        }

        public Guid FilialeID
        {
            get
            {
                if (ViewState["FilialeID"] == null) return Guid.Empty;
                return (Guid)ViewState["FilialeID"];
            }
            set { ViewState["FilialeID"] = value; }
        }

        public Guid CompanyID
        {
            get
            {
                return Request.QueryString["CompanyId"] == null ? Guid.Empty : new Guid(Request.QueryString["CompanyId"]);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            _submitController = CreateSubmitInstance();
            if (!IsPostBack)
            {
                if (Request.QueryString["CompanyId"] != null)
                {
                    FilialeID = Request.QueryString["FilialeId"] != null
                        ? new Guid(Request.QueryString["FilialeId"])
                        : Guid.Empty;
                    BindFilialeList();
                }
            }
        }

        ///<summary>
        ///绑定往来公司
        ///</summary>
        protected void BindFilialeList()
        {
            var data = CacheCollection.Filiale.GetHeadList().ToDictionary(k => k.ID, v => v.Name);
            data.Add(_reckoningElseFilialeid, "ERP");
            FilialeDics = data;
        }

        /// <summary>
        /// 选择往来公司时绑定数据
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void RcbFilialeOnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var rcb = (RadComboBox)o;
            if (rcb != null)
            {
                FilialeID = string.IsNullOrEmpty(rcb.SelectedValue) ? Guid.Empty : new Guid(rcb.SelectedValue);
            }
            else
            {
                FilialeID = Guid.Empty;
            }
        }

        /// <summary>
        /// 数据绑定
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgSubjectDiscountNeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            var companyId = new Guid(Request.QueryString["CompanyId"]);
            var filialeId = Request.QueryString["FilialeId"] != null
                ? new Guid(Request.QueryString["FilialeId"])
                : Guid.Empty;
            var memoType = Request.QueryString["Cost"] != null
                ? 0
                : Request.QueryString["Type"] == "1" ? (int)MemoType.Subject : (int)MemoType.Discount;
            var dataList = _companySubjectDiscountDao.GetCompanySubjectDiscountInfos(companyId, filialeId,
                memoType);
            RgSubjectDiscount.DataSource = dataList;
        }

        /// <summary>
        /// 分页
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        protected void RgSubjectDiscountOnPageIndexChanged(object source, GridPageChangedEventArgs e)
        {

        }


        //protected void BtnAddOnClick(object sender, EventArgs e)
        //{
        //    if (!_submitController.Enabled)
        //    {
        //        RAM.Alert("程序正在处理中，请稍候...");
        //        return;
        //    }
        //    if (FilialeID==Guid.Empty)
        //    {
        //        RAM.Alert("请选择往来公司!");
        //        return;
        //    }
        //    if (!FilialeDics.ContainsKey(FilialeID))
        //    {
        //        RAM.Alert("往来单位与该公司无往来!");
        //        return;
        //    }
        //    var personnelInfo = CurrentSession.Personnel.Get();
        //    if (Request.QueryString["Cost"] != null)
        //    {
        //        var addSubjectResult = _companySubjectDiscountBll.Insert(new CompanySubjectDiscountInfo
        //        {
        //            CompanyId = CompanyID,
        //            FilialeId = FilialeID,
        //            ID = Guid.NewGuid(),
        //            Datecreated = DateTime.Now,
        //            Income = 0,
        //            Memo = string.Format("{0}", TbDescription.Text),
        //            PersonnelName = personnelInfo.RealName,
        //            MemoType = (int)MemoType.Subject
        //        });
        //        if (!addSubjectResult)
        //        {
        //            RAM.Alert(string.Format("添加差额说明失败!"));
        //            return;
        //        }
        //        _submitController.Submit();
        //    }
        //    else
        //    {
        //        int type = Request.QueryString["Type"] != null
        //        ? Request.QueryString["Type"] == "1" ? (int)MemoType.Subject : (int)MemoType.Discount
        //        : 0;
        //        if (type != 0)
        //        {
        //            string message = type == 2 ? "差额说明" : "折扣说明";
        //            if (TbDescription.Text.Trim().Length == 0)
        //            {
        //                RAM.Alert(string.Format("{0}不能为空!", message));
        //                return;
        //            }
        //            if (FilialeID == Guid.Empty)
        //            {
        //                RAM.Alert(string.Format("请选择往来公司!"));
        //                return;
        //            }
        //            var addResult = _companySubjectDiscountBll.Insert(new CompanySubjectDiscountInfo
        //            {
        //                CompanyId = CompanyID,
        //                FilialeId = FilialeID,
        //                ID = Guid.NewGuid(),
        //                Datecreated = DateTime.Now,
        //                Income = 0,
        //                Memo = string.Format("[添加{0}：{1}]", message, TbDescription.Text.Trim()),
        //                PersonnelName = personnelInfo.RealName,
        //                MemoType = type == 2?(int)MemoType.Subject:(int)MemoType.Discount
        //            });
        //            if (!addResult)
        //            {
        //                RAM.Alert(string.Format("添加{0}失败!", message));
        //            }
        //            else
        //            {
        //                WebControl.AddOperationLog(personnelInfo.PersonnelId, personnelInfo.RealName, CompanyID, "",
        //                            Request.QueryString["Type"] == "1"
        //                                ? OperationPoint.ReckongingManage.DifferenceExplain.GetBusinessInfo()
        //                                : OperationPoint.ReckongingManage.DiscountExplain.GetBusinessInfo(), string.Empty);
        //                _submitController.Submit();

        //            }
        //        }
        //    }
        //    RgSubjectDiscount.Rebind();
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filiale"></param>
        /// <returns></returns>
        protected string GetFilialeName(object filiale)
        {
            var filialeId = new Guid(filiale.ToString());
            if (FilialeDics.ContainsKey(filialeId))
                return FilialeDics[filialeId];
            return string.Empty;
        }
    }

}
