using System;
using System.Web.UI;
using AllianceShop.Contract.DataTransferObject;
using ERP.BLL.Implement.Inventory;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.SAL;
using ERP.UI.Web.Common;
using Keede.Ecsoft.Model;

namespace ERP.UI.Web.Windows
{
    public partial class ReviewRechargeManager : Page
    {
        /// <summary>
        /// 充值id
        /// </summary>
        public Guid RechargeId
        {
            get
            {
                if (ViewState["RechargeId"] == null)
                {
                    return new Guid(Request.QueryString["RechargeId"]);
                }
                return (Guid)ViewState["RechargeId"];
            }
        }

        public Guid SaleFilialeId
        {
            get
            {
                if (ViewState["SaleFilialeId"] == null)
                {
                    return new Guid(Request.QueryString["SaleFilialeId"]);
                }
                return (Guid)ViewState["SaleFilialeId"];
            }
        }

        public RechargeDTO RechargeDto
        {
            get
            {
                return (RechargeDTO)ViewState["RechargeDto"];
            }
            set { ViewState["RechargeDto"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack) return;
            if (!string.IsNullOrEmpty(Request.QueryString["RechargeId"]))
            {
                var result = ShopSao.SelectRechargeById(RechargeId, SaleFilialeId);
                if (result == null)
                {
                    return;
                }
                if (result.IsSuccess)
                {
                    var shop = result.Data;
                    lblShopName.Text = shop.ShopName;
                    lblBankAccountName.Text = shop.BankAccountName;
                    lblMoney.Text = WebControl.NumberSeparator(shop.Money.ToString("0.00"));
                    lblBankTradeNo.Text = shop.BankTradeNo;
                    //      TB_Remark.Text = shop.Remark;
                    RechargeDto = shop;
                    lblAccountTotalled.Text = WebControl.NumberSeparator(shop.AccountTotalled.ToString("0.00"));
                }
                else
                    RAM.Alert(result.Message);
            }
        }

        readonly CodeManager _codeBll = new CodeManager();

        protected void BtnSelect_Click(object sender, EventArgs e)
        {
            var result = ShopSao.ConfirmRechargeState(SaleFilialeId, RechargeId, Convert.ToInt32(rbSelectState.SelectedValue), TB_Remark.Text.Length > 0 ? string.Format("[{0}]", TB_Remark.Text.Trim()) : string.Empty);
            if (result == null)
            {
                RAM.Alert("服务连接失败");
                return;
            }
            //往来帐 add by lcj at 2015.9.23
            //Start
            var reckNo = _codeBll.GetCode(CodeType.GT);
            var filialeId = CacheCollection.Filiale.Get(SaleFilialeId).ParentId;
            var reckoningInfo = new ReckoningInfo(Guid.NewGuid(), filialeId, SaleFilialeId, reckNo,
                TB_Remark.Text.Length > 0 ? string.Format("[{0}]", TB_Remark.Text.Trim()) : "门店充值确认",
                RechargeDto.Money,
                (int)ReckoningType.Defray,
                (int)ReckoningStateType.Currently,
                (int)CheckType.NotCheck, (int)AuditingState.Yes,
                RechargeDto.No, Guid.Empty)
            {
                LinkTradeType = (int)ReckoningLinkTradeType.Recharge
            };
            //End
            if (!result.IsSuccess)
            {
                IReckoning reckoning = new Reckoning(GlobalConfig.DB.FromType.Write);
                string message;
                reckoning.Insert(reckoningInfo, out message);//添加ERP本地往来帐 add by lcj at 2015.9.23 对应bug11031
                RAM.Alert(result.Message);
                return;
            }
            RAM.Alert("操作成功!");
            RAM.ResponseScripts.Add("setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
        }
    }
}