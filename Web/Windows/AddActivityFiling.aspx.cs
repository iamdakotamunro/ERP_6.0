using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using AllianceShop.Common.Extension;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Common;
using Keede.Ecsoft.Model;
using Telerik.Web.UI;
using Telerik.Web.UI.Calendar;

namespace ERP.UI.Web.Windows
{
    public partial class AddActivityFiling : WindowsPage
    {
        private readonly IGoodsCenterSao _goodsCenterSao = new GoodsCenterSao();
        static readonly DAL.Implement.Inventory.ActivityFiling _activityFiling=new DAL.Implement.Inventory.ActivityFiling(GlobalConfig.DB.FromType.Write);
        static readonly ActivityOperateLog _activityOperateLog = new ActivityOperateLog(GlobalConfig.DB.FromType.Write);
        private int _requestParm;
        private Guid _activityId;
        private int _isUpdate;

        #region 初始化
        override protected void OnInit(EventArgs e)
        {
            InitializeComponent();
            base.OnInit(e);
        }

        private void InitializeComponent()
        {
            AddRadGoods.ItemsRequested += (ActivityFiling_Goods_OnItemsRequested);
        }
        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            _requestParm =Request.QueryString["parm"]!=null?Convert.ToInt32(Request.QueryString["parm"]):0;
            _isUpdate =Request.QueryString["IsUpdate"]!=null? Convert.ToInt32(Request.QueryString["IsUpdate"]):0;
            _activityId=Request.QueryString["id"]!=null? new Guid(Request.QueryString["id"]):Guid.Empty;
            if (!IsPostBack)
            {
                if (_requestParm == 2 && _isUpdate == 0)
                {
                    BtnOK.Text = "审核通过";
                    BtnCannel.Text = "审核不通过";
                }
                else
                {
                    BtnOK.Text = "确定";
                    BtnCannel.Text = "取消";
                }
                BandingSaleTerrace();
                BindInStock();
                //采购员
                addRadPursePersonnel.DataSource = PersonnelList;
                addRadPursePersonnel.SelectedIndex = 0;
                addRadPursePersonnel.DataBind();
                if (_requestParm == 0 && _isUpdate == 0)
                {
                    Extract("新建活动报备单",false,false,false);
                    addStartDateTime.MinDate = DateTime.Now;
                    addEndDateTime.MinDate = DateTime.Now;
                }
                if (_requestParm == 1 && _isUpdate == 0)
                {
                    Extract("修改活动报备单",true,true,false);
                }
                if (_requestParm == 2 && _isUpdate == 0)
                {
                    Extract("审核活动报备单",true,true,false);
                }
                if (_requestParm == 3 && _isUpdate == 0)
                {
                    Extract("活动报备单",true,true,true);
                    BtnOK.Visible = false;
                    BtnCannel.Visible = false;
                }
                if (_requestParm == 4 && _isUpdate == 0)
                {
                    Extract("活动报备单",true,true,false);
                    BtnOK.Visible = false;
                    BtnCannel.Visible = false;
                }
                if (_isUpdate == 1 && (_requestParm == 1 || _requestParm==4)) //运营申报----修改
                {  
                    Extract("修改活动报备单",false,false,false);
                }
                if (_isUpdate == 1 && _requestParm == 2) //采购申报-----修改
                {
                    Extract("修改活动报备单", true, true, false);
                } 
                BandingInfo();
            }
        }
        /// <summary>
        /// 将重复的控件显示隐藏代码提取
        /// </summary>
        /// <param name="pageTitle"></param>
        /// <param name="pursePersonnelVisible"></param>
        /// <param name="prospectSaleNumberVisible"></param>
        /// <param name="actualSaleNumberVisible"></param>
        public void Extract(string pageTitle,bool pursePersonnelVisible,bool prospectSaleNumberVisible,bool actualSaleNumberVisible)
        {
            Page.Title = pageTitle;
            addPursePersonnel.Visible = pursePersonnelVisible;       //采购员label
            addRadPursePersonnel.Visible = pursePersonnelVisible;    //采购员RadComboBox
            addLadProspectSaleNumber.Visible = prospectSaleNumberVisible;//预估备货label
            addTxtProspectSaleNumber.Visible = prospectSaleNumberVisible;//预估备货textbox
            addTxtActualSaleNumber.Visible = actualSaleNumberVisible; //实际销量textbox
            addLabActualSaleNumber.Visible = actualSaleNumberVisible; //实际销量label
        }

        /// <summary>
        /// 绑定信息
        /// </summary>
        public void BandingInfo()
        {
            var result = _activityFiling.SelectFilingInfoById(_activityId);
            if (result==null)
            {
                return;
            }
            addActivityFiling_Title.Text = result.ActivityFilingTitle;
            addStartDateTime.DbSelectedDate = result.ActivityStateDate;
            addEndDateTime.DbSelectedDate = result.ActivityEndDate;
            AddRadSaleTerrace.SelectedValue = result.FilingCompanyID.ToString();
            AddRadTerraceFiling.DataSource = CacheCollection.SalePlatform.GetListByFilialeId(result.FilingCompanyID).Where(w=>w.IsActive).ToList();
            AddRadTerraceFiling.DataTextField = "Name";
            AddRadTerraceFiling.DataValueField = "ID";
            AddRadTerraceFiling.DataBind();
            AddRadTerraceFiling.SelectedValue = result.FilingTerraceID.ToString();
            //AddRadGoods.Items.Clear();
            AddRadGoods.Items.Add(new RadComboBoxItem{Text = result.GoodsName,Value = result.GoodsID.ToString(),Selected = true});
            HdGoodsID.Value = result.GoodsID.ToString();
            AddRadWarehouse.SelectedValue = result.WarehouseID.ToString();
            GoodsSaleLab.Text = result.NormalSaleNumber.ToString(CultureInfo.InvariantCulture);
            TxtProspectReadyNumber.Text = result.ProspectSaleNumber.ToString(CultureInfo.InvariantCulture);
            if (_isUpdate==1 && (_requestParm==1 || _requestParm==4))
            {
                addActivityFiling_Title.Enabled = true;
                addStartDateTime.Enabled = true;
                addEndDateTime.Enabled = true;
                AddRadSaleTerrace.Enabled = true;
                AddRadTerraceFiling.Enabled = true;
                AddRadGoods.Enabled = true;
                AddRadWarehouse.Enabled = true;
                TxtProspectReadyNumber.Enabled = true;
            }
            else 
            {
                addActivityFiling_Title.Enabled = false;
                addStartDateTime.Enabled = false;
                addEndDateTime.Enabled = false;
                AddRadSaleTerrace.Enabled = false;
                AddRadTerraceFiling.Enabled = false;
                AddRadGoods.Enabled = false;
                AddRadWarehouse.Enabled = false;
                TxtProspectReadyNumber.Enabled = false;
            }
            if (_requestParm==2 && _isUpdate==0)//采购申报
            {
                addRadPursePersonnel.SelectedValue = result.PurchasePersonnelID.ToString();
                addRadPursePersonnel.Enabled = false;
                addTxtProspectSaleNumber.Text = result.ProspectReadyNumber.ToString(CultureInfo.InvariantCulture);
                addTxtProspectSaleNumber.Enabled = false;
            }
            if (_requestParm == 2 && _isUpdate == 1)
            {
                addRadPursePersonnel.SelectedValue = result.PurchasePersonnelID.ToString();
                addRadPursePersonnel.Enabled = true;
                addTxtProspectSaleNumber.Text = result.ProspectReadyNumber.ToString(CultureInfo.InvariantCulture);
                addTxtProspectSaleNumber.Enabled = true;
            }
            if (_requestParm==3)//申报通过
            {
                addRadPursePersonnel.SelectedValue = result.PurchasePersonnelID.ToString();
                addRadPursePersonnel.Enabled = false;
                addTxtProspectSaleNumber.Text = result.ProspectReadyNumber.ToString(CultureInfo.InvariantCulture);
                addTxtProspectSaleNumber.Enabled = false;
                //计算实际销量
                if (result.ActivityEndDate < DateTime.Now &&
                    result.ActivityFilingState == (int) ActivityFilingState.FilingPass)
                {
                    addLabActualSaleNumber.Text = result.ActualSaleNumber + "个";
                }
                else
                {
                    var realSale = _activityFiling.GetGoodsRealSale(result.GoodsID,result.WarehouseID, result.FilingCompanyID, result.FilingTerraceID, result.ActivityStateDate,result.ActivityEndDate.AddDays(1));
                    addLabActualSaleNumber.Text = realSale + "个";
                }
            }
            
            if (_requestParm == 4 && _isUpdate==0)//申报不通过
            {
                addRadPursePersonnel.SelectedValue = result.PurchasePersonnelID.ToString();
                addRadPursePersonnel.Enabled = false;
                addTxtProspectSaleNumber.Text = result.ProspectReadyNumber.ToString(CultureInfo.InvariantCulture);
                addTxtProspectSaleNumber.Enabled = false;
            }
        }

        /// <summary>
        /// 采购人员
        /// </summary>
        protected IList<PersonnelInfo> PersonnelList
        {
            get
            {
                if (ViewState["PersonnelList"] == null)
                {
                    var systemBranchId = new Guid("D9D6002C-196C-4375-B41A-E7040FE12B09"); //系统部门ID
                    var systemPostionList = MISService.GetAllSystemPositionList().ToList();
                    var positonIds = systemPostionList.Where(
                        act => act.ParentSystemBranchID == systemBranchId || act.SystemBranchID == systemBranchId)
                        .Select(act => act.SystemBranchPositionID);
                    IList<PersonnelInfo> list = new PersonnelSao().GetList().Where(ent => positonIds.Contains(ent.SystemBrandPositionId) && ent.IsActive).ToList();
                    ViewState["PersonnelList"] = list;
                }
                return (IList<PersonnelInfo>)ViewState["PersonnelList"];
            }
        }
        #region 绑定销售平台、申报公司、仓库

        public void BandingSaleTerrace()
        {
            //var saleTerraceList = CacheCollection.SalePlatform.GetList();
            //AddRadTerraceFiling.DataSource = saleTerraceList;
            //AddRadTerraceFiling.DataTextField = "Name";
            //AddRadTerraceFiling.DataValueField = "ID";
            //AddRadTerraceFiling.DataBind();
            AddRadTerraceFiling.Items.Insert(0, new RadComboBoxItem("销售平台列表", Guid.Empty.ToString()));
            //RadSaleTerrace.MaxHeight = 100;

            var companyList = CacheCollection.Filiale.GetHeadList();
            AddRadSaleTerrace.DataSource = companyList;
            AddRadSaleTerrace.DataTextField = "Name";
            AddRadSaleTerrace.DataValueField = "ID";
            AddRadSaleTerrace.DataBind();
            AddRadSaleTerrace.Items.Insert(0,new RadComboBoxItem("公司列表",Guid.Empty.ToString()));
        }

        /// <summary>
        /// 绑定授权仓库
        /// </summary>
        private void BindInStock()
        {
            AddRadWarehouse.DataSource = WMSSao.GetWarehouseAuthDics(CurrentSession.Personnel.Get().PersonnelId);
            AddRadWarehouse.DataTextField = "Value";
            AddRadWarehouse.DataValueField = "Key";
            AddRadWarehouse.DataBind();
            AddRadWarehouse.Items.Insert(0, new RadComboBoxItem("请选择仓库", Guid.Empty.ToString()));
        }
        #endregion
        /// <summary>
        /// 活动商品
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ActivityFiling_Goods_OnItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            var combo = (RadComboBox)sender;
            combo.Items.Clear();
            if (!string.IsNullOrEmpty(e.Text))
            {
                var dic = _goodsCenterSao.GetGoodsSelectList(e.Text);
                Int32 totalCount = dic.Count;
                if (e.NumberOfItems >= totalCount)
                    e.EndOfItems = true;
                else
                {
                    foreach (var keyValuePair in dic)
                    {
                        var item = new RadComboBoxItem
                        {
                            Text = keyValuePair.Value,
                            Value = keyValuePair.Key
                        };
                        combo.Items.Add(item);
                    }
                }
            }
        }

        /// <summary>
        /// 商品 选择取历史销量汇总
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ActivityFiling_OnSelectedDateChanged(object sender, EventArgs e)
        {
            GoodsNormalSale();
            GoodsInfo();
        }

        /// <summary>
        /// 仓库选择
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void ActivityFiling_WarehouseSelect(object sender,EventArgs e)
        {
            GoodsNormalSale();
            GoodsInfo();
        }

        /// <summary>
        /// 获取历史销量汇总
        /// </summary>
        /// <param name="goodsID"></param>
        /// <param name="warehouseID"></param>
        /// <param name="saleFilialeId"></param>
        /// <param name="salePlatformId"></param>
        /// <returns></returns>
        public int GetGoodsSaleInfo(Guid goodsID, Guid warehouseID, Guid saleFilialeId, Guid salePlatformId)
        {
            //计算前第一个备货周期的销售额
            int minDays = 1;
            int maxDay = -30;
            var childSaleList1 = _activityFiling.GetGoodsSale(goodsID, warehouseID, saleFilialeId, salePlatformId, minDays, maxDay);
            //计算前第二个备货周期的销售额
            minDays = -30;
            maxDay = -60;
            var childSaleList2 = _activityFiling.GetGoodsSale(goodsID, warehouseID, saleFilialeId, salePlatformId, minDays, maxDay);

            //计算前第三个备货周期的销售额
            minDays = -60;
            maxDay = -90;
            var childSaleList3 = _activityFiling.GetGoodsSale(goodsID, warehouseID, saleFilialeId, salePlatformId, minDays, maxDay);
            return MargeGoods(childSaleList1, childSaleList2, childSaleList3);
        }
        /// <summary>
        /// 合并
        /// </summary>
        /// <param name="saleNumber1"></param>
        /// <param name="saleNumber2"></param>
        /// <param name="saleNumber3"></param>
        /// <returns></returns>
        public int MargeGoods(int saleNumber1, int saleNumber2, int saleNumber3)
        {
            var sumSaleNumber = (saleNumber1 * 3 + saleNumber2 * 2 + saleNumber3);
            var sumWeightedNumber = ((saleNumber1 == 0 ? 0 : 3) + (saleNumber2 == 0 ? 0 : 2) + (saleNumber3 == 0 ? 0 : 1));
            if (sumWeightedNumber == 0)
            {
                return 0;
            }
            return Math.Round(Convert.ToDecimal((sumSaleNumber / sumWeightedNumber)/30)).ToInt();
        }
        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddActivityFiling_Click(object sender, EventArgs e)
        {
            if (!CanSubmit())
            {
                RAM.Alert("请不要重复提交!");
                return;
            }
            ExecuteSubmit((ctx) =>
            {
                if (CheckInfo())
                {
                    var personnelInfo = CurrentSession.Personnel.Get();
                    var activityOperateLogModel = new ActivityOperateLogModel
                    {
                        OperatePersonnelID = personnelInfo.PersonnelId,
                        OperatePersonnelName = personnelInfo.RealName,
                        OperateDate = DateTime.Now
                    };
                    if (_requestParm == 0)//添加----运营申报
                    {
                        var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(AddRadGoods.SelectedValue.ToGuid());
                        string goodsCode = goodsInfo != null ? goodsInfo.GoodsCode : string.Empty;
                        var activityFilingInfo = new ActivityFilingInfo
                        {
                            ActivityFilingTitle = addActivityFiling_Title.Text,
                            ActivityStateDate =
                                addStartDateTime.SelectedDate.ToDateTime().ToString("yyyy-MM-dd").ToDateTime(),
                            ActivityEndDate = addEndDateTime.SelectedDate.ToDateTime().ToString("yyyy-MM-dd").ToDateTime(),
                            GoodsID = AddRadGoods.SelectedValue.ToGuid(),
                            GoodsName = AddRadGoods.Text,
                            GoodsCode = goodsCode,
                            WarehouseID = AddRadWarehouse.SelectedValue.ToGuid(),
                            NormalSaleNumber = Convert.ToInt32(GoodsSaleLab.Text),
                            ProspectSaleNumber = Convert.ToInt32(TxtProspectReadyNumber.Text),
                            FilingCompanyID = AddRadSaleTerrace.SelectedValue.ToGuid(),
                            FilingCompanyName = AddRadSaleTerrace.Text,
                            FilingTerraceID = AddRadTerraceFiling.SelectedValue.ToGuid(),
                            FilingTerraceName = AddRadTerraceFiling.Text,
                            OperatePersonnelID = personnelInfo.PersonnelId,
                            OperatePersonnelName = personnelInfo.RealName,
                            CreateDate = DateTime.Now,
                            ActivityFilingState = (int)ActivityFilingState.OperateFiling,
                            ID = Guid.NewGuid()
                        };
                        activityOperateLogModel.ActivityFilingID = activityFilingInfo.ID;
                        activityOperateLogModel.Description = "[" + activityOperateLogModel.OperateDate + activityOperateLogModel.OperatePersonnelName + "]新建活动报备单";
                        var result = AddInfo(activityFilingInfo, activityOperateLogModel);
                        if (result)
                        {
                            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                        }
                        else
                        {
                            RAM.Alert("活动报备单添加失败！");
                            ctx.SetFail();
                        }
                    }
                    else if (_requestParm == 1 && _isUpdate == 0)//修改-----采购申报
                    {
                        var pursePersonnelName = addRadPursePersonnel.Text;
                        var pursePersonnelId = new Guid(addRadPursePersonnel.SelectedValue);
                        var number = Convert.ToInt32(addTxtProspectSaleNumber.Text);
                        var result = _activityFiling.UpdateFilingInfo(_activityId, pursePersonnelId,
                            pursePersonnelName, number, (int)ActivityFilingState.PurchaseFiling);
                        if (result)
                        {
                            activityOperateLogModel.ActivityFilingID = _activityId;
                            activityOperateLogModel.Description = "[" + activityOperateLogModel.OperateDate + activityOperateLogModel.OperatePersonnelName + "]修改活动报备单";
                            var addLogResult = _activityOperateLog.InsertLog(activityOperateLogModel);
                            if (addLogResult)
                            {
                                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                            }
                            else
                            {
                                RAM.Alert("操作日志添加失败！");
                                ctx.SetFail();
                            }
                        }
                        else
                        {
                            RAM.Alert("修改活动报备单失败！");
                            ctx.SetFail();
                        }
                    }

                    if (_requestParm == 2 && _isUpdate == 0)//审核-----审核通过
                    {
                        var updateResult = _activityFiling.UpdateFilingState(_activityId, (int)ActivityFilingState.FilingPass);
                        if (updateResult)
                        {
                            activityOperateLogModel.ActivityFilingID = _activityId;
                            activityOperateLogModel.Description = "[" + activityOperateLogModel.OperateDate + activityOperateLogModel.OperatePersonnelName + "]申报通过活动报备单";
                            var addLogResult = _activityOperateLog.InsertLog(activityOperateLogModel);
                            if (addLogResult)
                            {
                                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                            }
                            else
                            {
                                RAM.Alert("操作日志添加失败！");
                                ctx.SetFail();
                            }
                        }
                        else
                        {
                            RAM.Alert("活动报备单审核失败！");
                            ctx.SetFail();
                        }
                    }

                    if ((_requestParm == 1 || _requestParm == 4) && _isUpdate == 1)
                    {
                        var goodsInfo = _goodsCenterSao.GetGoodsBaseInfoById(AddRadGoods.SelectedValue.ToGuid());
                        string goodsCode = goodsInfo != null ? goodsInfo.GoodsCode : string.Empty;
                        var activityFilingInfo = new ActivityFilingInfo
                        {
                            ID = _activityId,
                            ActivityFilingTitle = addActivityFiling_Title.Text,
                            ActivityStateDate = addStartDateTime.SelectedDate.ToDateTime().ToString("yyyy-MM-dd").ToDateTime(),
                            ActivityEndDate = addEndDateTime.SelectedDate.ToDateTime().ToString("yyyy-MM-dd").ToDateTime(),
                            GoodsID = AddRadGoods.SelectedValue.ToGuid(),
                            GoodsName = AddRadGoods.Text,
                            GoodsCode = goodsCode,
                            WarehouseID = AddRadWarehouse.SelectedValue.ToGuid(),
                            NormalSaleNumber = Convert.ToInt32(GoodsSaleLab.Text),
                            ProspectSaleNumber = Convert.ToInt32(TxtProspectReadyNumber.Text),
                            FilingCompanyID = AddRadSaleTerrace.SelectedValue.ToGuid(),
                            FilingCompanyName = AddRadSaleTerrace.Text,
                            FilingTerraceID = AddRadTerraceFiling.SelectedValue.ToGuid(),
                            FilingTerraceName = AddRadTerraceFiling.Text,
                            ActivityFilingState = (int)ActivityFilingState.OperateFiling
                        };
                        var updateResult = _activityFiling.UpdateFilingBaseInfo(activityFilingInfo);
                        if (updateResult)
                        {
                            activityOperateLogModel.ActivityFilingID = activityFilingInfo.ID;
                            activityOperateLogModel.Description = "[" + activityOperateLogModel.OperateDate + activityOperateLogModel.OperatePersonnelName + "]修改活动报备单";
                            var addLogResult = _activityOperateLog.InsertLog(activityOperateLogModel);
                            if (addLogResult)
                            {
                                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                            }
                            else
                            {
                                RAM.Alert("操作日志添加失败！");
                                ctx.SetFail();
                            }
                        }
                    }
                    if (_requestParm == 2 && _isUpdate == 1)
                    {
                        var pursePersonnelName = addRadPursePersonnel.Text;
                        var pursePersonnelId = new Guid(addRadPursePersonnel.SelectedValue);
                        var number = Convert.ToInt32(addTxtProspectSaleNumber.Text);
                        var result = _activityFiling.UpdateFilingInfo(_activityId, pursePersonnelId, pursePersonnelName, number);
                        if (result)
                        {
                            activityOperateLogModel.ActivityFilingID = _activityId;
                            activityOperateLogModel.Description = "[" + activityOperateLogModel.OperateDate + activityOperateLogModel.OperatePersonnelName + "]修改活动报备单";
                            var addLogResult = _activityOperateLog.InsertLog(activityOperateLogModel);
                            if (addLogResult)
                            {
                                RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                            }
                            else
                            {
                                RAM.Alert("操作日志添加失败！");
                                ctx.SetFail();
                            }
                        }
                        else
                        {
                            RAM.Alert("修改活动报备单失败！");
                            ctx.SetFail();
                        }
                    }
                }
            });
        }

        /// <summary>
        /// 取消
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void AddCannelActivityFiling_Click(object sender, EventArgs e)
        {
            if (!CanSubmit())
            {
                RAM.Alert("请不要重复提交!");
                return;
            }
            ExecuteSubmit((ctx) =>
            {
                if (_requestParm == 2)
                {
                    var updateResult = _activityFiling.UpdateFilingState(_activityId, (int)ActivityFilingState.FilingFail);
                    if (updateResult)
                    {
                        var personnelInfo = CurrentSession.Personnel.Get();
                        var activityOperateLogModel = new ActivityOperateLogModel
                        {
                            OperatePersonnelID = personnelInfo.PersonnelId,
                            OperatePersonnelName = personnelInfo.RealName,
                            OperateDate = DateTime.Now,
                            ActivityFilingID = _activityId
                        };
                        activityOperateLogModel.Description = "[" + activityOperateLogModel.OperateDate + activityOperateLogModel.OperatePersonnelName + "]申报不通过活动报备单";
                        var addLogResult = _activityOperateLog.InsertLog(activityOperateLogModel);
                        if (addLogResult)
                        {
                            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                        }
                        else
                        {
                            RAM.Alert("操作日志添加失败！");
                            ctx.SetFail();
                        }
                    }
                    else
                    {
                        RAM.Alert("修改活动报备单状态失败！");
                        ctx.SetFail();
                    }
                }
                else
                {
                    RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                }
            });
        }

        /// <summary>
        /// 添加
        /// </summary>
        /// <param name="activityFilingInfo"></param>
        /// <param name="activityOperateLogModel"></param>
        /// <returns></returns>
        public bool AddInfo(ActivityFilingInfo activityFilingInfo, ActivityOperateLogModel activityOperateLogModel)
        {
            var resultFilingInfo = _activityFiling.InsertActivityFiling(activityFilingInfo);
            if (!resultFilingInfo)
            {
                return false;
            }
            var resultLog = _activityOperateLog.InsertLog(activityOperateLogModel);
            if (!resultLog)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 判断信息
        /// </summary>
        /// <returns></returns>
        public bool CheckInfo()
        {
            if (string.IsNullOrEmpty(addActivityFiling_Title.Text))
            {
                RAM.Alert("请输入活动标题！");
                return false;
            }
            var startDate = addStartDateTime.DbSelectedDate.ToDateTime();
            var endDate = addEndDateTime.DbSelectedDate.ToDateTime();
            if (startDate==DateTime.MinValue || endDate==DateTime.MinValue)
            {
                RAM.Alert("请选择活动时间！");
                return false;
            }
            if (startDate>endDate)
            {
                RAM.Alert("活动结束时间不允许大于开始时间！");
                return false;
            }
            if (AddRadSaleTerrace.SelectedValue.ToGuid()==Guid.Empty)
            {
                RAM.Alert("请选择申报公司！");
                return false;
            }
            if (AddRadTerraceFiling.SelectedValue.ToGuid() == Guid.Empty)
            {
                RAM.Alert("请选择申报平台！");
                return false;
            }
            if (AddRadGoods.SelectedValue.ToGuid()==Guid.Empty)
            {
                RAM.Alert("请选择活动商品！");
                return false;
            }
            if (string.IsNullOrEmpty(AddRadGoods.Text))
            {
                RAM.Alert("请选择活动商品！");
                return false;
            }
            if (AddRadWarehouse.SelectedValue.ToGuid()==Guid.Empty)
            {
                RAM.Alert("请选择仓库！");
                return false;
            }
            if (string.IsNullOrEmpty(TxtProspectReadyNumber.Text))
            {
                RAM.Alert("请输入预估销量！");
                return false;
            }
            if ((_requestParm==1 && _isUpdate==0) || (_requestParm==2 && _isUpdate==1))//修改
            {
                if (addRadPursePersonnel.SelectedValue.ToGuid()==Guid.Empty)
                {
                    RAM.Alert("请选择采购员！");
                    return false;
                }
                if (string.IsNullOrEmpty(addTxtProspectSaleNumber.Text))
                {
                    RAM.Alert("请输入预估备货！");
                    return false;
                }
            }
            if (_requestParm==1 && _isUpdate==0)
            {
                if (endDate<DateTime.Now.ToString("yyyy-M-d").ToDateTime())
                {
                    RAM.Alert("该活动结束时间小于当前时间，无法操作；可以在修改页面修改结束时间，继续操作！");
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// 申报公司选择
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void Add_SaleTerraceOnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var radComboBox = o as RadComboBox;
            if (radComboBox == null) return;
            var rcbSaleFiliale = radComboBox.Parent.FindControl("AddRadSaleTerrace") as RadComboBox;
            var rcbSalePlatform = radComboBox.Parent.FindControl("AddRadTerraceFiling") as RadComboBox;
            if (rcbSalePlatform != null) rcbSalePlatform.Items.Clear();
            if (rcbSaleFiliale == null) return;
            var rcbSaleFilialeId = new Guid(rcbSaleFiliale.SelectedValue);
            if (rcbSaleFilialeId == Guid.Empty)
            {
                if (rcbSalePlatform != null) rcbSalePlatform.Items.Clear();
                //SalePlatformId = Guid.Empty;
                return;
            }
            AddRadTerraceFiling.DataSource = rcbSaleFilialeId == Guid.Empty ? CacheCollection.SalePlatform.GetList() : CacheCollection.SalePlatform.GetListByFilialeId(rcbSaleFilialeId).Where(w=>w.IsActive).ToList();
            AddRadTerraceFiling.DataTextField = "Name";
            AddRadTerraceFiling.DataValueField = "ID";
            AddRadTerraceFiling.DataBind();

            AddRadTerraceFiling.Items.Insert(0, new RadComboBoxItem("销售平台列表", Guid.Empty.ToString()));
            //AddRadTerraceFiling.Items.Insert(0, new RadComboBoxItem("全部", Guid.Empty.ToString()));
            AddRadTerraceFiling.SelectedIndex = 0;

            GoodsNormalSale();
            GoodsInfo();
        }

        /// <summary>
        /// 获取商品的正常销量
        /// </summary>
        private void GoodsNormalSale()
        {
            GoodsSaleLab.Text = "0";
            var startDate = addStartDateTime.DbSelectedDate.ToDateTime();
            var endDate = addEndDateTime.DbSelectedDate.ToDateTime().AddDays(1);
            var saleFilialeId = AddRadSaleTerrace.SelectedValue;//销售公司
            var salePlatformId = AddRadTerraceFiling.SelectedValue;//销售平台
            var goodsID = AddRadGoods.SelectedValue;
            var warehouseID = AddRadWarehouse.SelectedValue;
            if (warehouseID.ToGuid() != Guid.Empty && startDate != DateTime.MinValue && endDate != DateTime.MinValue &&
                saleFilialeId.ToGuid() != Guid.Empty && salePlatformId.ToGuid() != Guid.Empty &&
                goodsID.ToGuid() != Guid.Empty)
            {
                var sale = GetGoodsSaleInfo(goodsID.ToGuid(), warehouseID.ToGuid(),saleFilialeId.ToGuid(),salePlatformId.ToGuid());
                var date = Convert.ToInt32((endDate - startDate).TotalDays);
                GoodsSaleLab.Text = (sale*date).ToString(CultureInfo.InvariantCulture);
            }
        }

        /// <summary>
        /// 验证活动商品是否已经存在申报活动中
        /// </summary>
        private void GoodsInfo()
        {
            var saleFilialeId = AddRadSaleTerrace.SelectedValue;//销售公司
            var salePlatformId = AddRadTerraceFiling.SelectedValue;//销售平台
            var goodsID = AddRadGoods.SelectedValue;
            var warehouseID = AddRadWarehouse.SelectedValue;
            if (goodsID.Equals(HdGoodsID.Value))
            {
                return;
            }
            if (warehouseID.ToGuid() != Guid.Empty  &&saleFilialeId.ToGuid() != Guid.Empty && salePlatformId.ToGuid() != Guid.Empty &&goodsID.ToGuid() != Guid.Empty)
            {
                var result = _activityFiling.SelectGoods(saleFilialeId.ToGuid(), goodsID.ToGuid(), salePlatformId.ToGuid(), warehouseID.ToGuid());
                if (result)
                {
                    RAM.Alert("该商品已经存在其他活动申报中！");
                    AddRadGoods.Items.Clear();
                    AddRadGoods.Text = string.Empty;
                }
            }
        }

        /// <summary>
        /// 销售平台选择
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        protected void AddRadTerraceFiling_OnSelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            GoodsInfo();
            GoodsNormalSale();
        }

        /// <summary>
        /// 起始时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void addStartDateTime_OnSelectedDateChanged(object sender, SelectedDateChangedEventArgs e)
        {
            GoodsNormalSale();//正常销量
        }
        /// <summary>
        /// 结束时间
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void addEndDateTime_OnSelectedDateChanged(object sender, SelectedDateChangedEventArgs e)
        {
            GoodsNormalSale();
        }

    }
}