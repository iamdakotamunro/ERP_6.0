using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.WebControls;
using ERP.BLL.Implement;
using ERP.BLL.Implement.Inventory;
using ERP.Cache;
using ERP.DAL.Implement.Company;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.ICompany;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Enum.Attribute;
using ERP.Environment;
using ERP.Model;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using ERP.UI.Web.UserControl;
using Keede.Ecsoft.Model;
using MIS.Enum;
using Telerik.Web.UI;
using WebControl = ERP.UI.Web.Common.WebControl;

namespace ERP.UI.Web
{
    public partial class BankWebSitePage : BasePage
    {
        //其他公司
        private readonly Guid _reckoningElseFilialeid = new Guid(ConfigManage.GetConfigValue("RECKONING_ELSE_FILIALEID"));
        private readonly IBankAccounts _bankAccountsWrite = new BankAccounts(GlobalConfig.DB.FromType.Write);
        private readonly IBankAccountDao _bankAccountDaoWrite = new BankAccountDao(GlobalConfig.DB.FromType.Write);

        private Guid TargetId
        {
            get { return new Guid(ViewState["TargetId"].ToString()); }
            set { ViewState["TargetId"] = value.ToString(); }
        }

        public class TreeMode
        {
            public string Name { get; set; }
            public Guid Id { get; set; }
            public Guid ParentId { get; set; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                RecursivelyFiliale();
                TargetId = Guid.Empty;
            }
        }

        private void RecursivelyFiliale()
        {
            var filiales = new List<FilialeInfo>();
            filiales.AddRange(CacheCollection.Filiale.GetHostingAndSaleFilialeList());
            filiales.Add(
                new FilialeInfo
                {
                    ID = _reckoningElseFilialeid,
                    Name = "ERP"
                });
            var list = filiales.Select(info => new TreeMode
            {
                Id = info.ID,
                Name = info.Name,
                ParentId = Guid.Empty
            }).ToList();
            list.AddRange(CacheCollection.SalePlatform.GetList().Select(info => new TreeMode
                            {
                                Id = info.ID,
                                Name = info.Name,
                                ParentId = info.FilialeId
                            }));
            RTVWebSite.DataSource = list;
            RTVWebSite.DataTextField = "Name";
            RTVWebSite.DataValueField = "Id";
            RTVWebSite.DataFieldID = "Id";
            RTVWebSite.DataFieldParentID = "ParentId";
            RTVWebSite.DataBind();
            RTVWebSite.ExpandAllNodes();
        }

        protected string GetFromSourceName(string bankAccountsId)
        {
            if (TargetId == _reckoningElseFilialeid || TargetId == Guid.Empty)
            {
                return "-";
            }
            var selectSalePlatformInfo = CacheCollection.SalePlatform.Get(TargetId);
            if (selectSalePlatformInfo != null)
            {
                return RTVWebSite.SelectedNode.Text;
            }
            if (bankAccountsId.Length != 0)
            {
                var bankList = _bankAccountDaoWrite.GetListByBankAccountId(new Guid(bankAccountsId));
                foreach (var bankAccountsInfo in bankList)
                {
                    if (bankAccountsInfo.TargetId != TargetId)
                    {
                        var info = CacheCollection.SalePlatform.Get(bankAccountsInfo.TargetId);
                        if (info != null)
                            return info.Name;
                    }
                }
            }
            return "-";
        }

        protected string GetFilialeName()
        {
            if (TargetId == _reckoningElseFilialeid || TargetId==Guid.Empty)
            {
                return "-";
            }
            var selectSalePlatformInfo = CacheCollection.SalePlatform.Get(TargetId);
            if (selectSalePlatformInfo != null)
            {
                return CacheCollection.Filiale.GetName(selectSalePlatformInfo.FilialeId);
            }
            return CacheCollection.Filiale.Get(new Guid(RTVWebSite.SelectedNode.Value)).Name;
        }

        protected void Rg_BankWebSite_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            IList<BankAccountInfo> list;
            if (TargetId == _reckoningElseFilialeid)
            {
                RG_BankWebSite.MasterTableView.Columns.FindByUniqueName("filialeName").Visible = false;
                RG_BankWebSite.MasterTableView.Columns.FindByUniqueName("FromSourceName").Visible = false;
                RG_BankWebSite.MasterTableView.Columns.FindByUniqueName("Delete").Visible = false;
                RG_BankWebSite.MasterTableView.Columns.FindByUniqueName("IsMain").Visible = false;
                RG_BankWebSite.MasterTableView.Columns.FindByUniqueName("Transfer").Visible = true;
                list = _bankAccountsWrite.GetList().Where(ent => ent.IsMain == false).ToList();
            }
            else
            {
                RG_BankWebSite.MasterTableView.Columns.FindByUniqueName("filialeName").Visible = true;
                RG_BankWebSite.MasterTableView.Columns.FindByUniqueName("FromSourceName").Visible = true;
                RG_BankWebSite.MasterTableView.Columns.FindByUniqueName("IsMain").Visible = true;
                RG_BankWebSite.MasterTableView.Columns.FindByUniqueName("Transfer").Visible = false;
                //var filiale=CacheCollection.Filiale.Get(TargetId);
                list = _bankAccountDaoWrite.GetListByTargetId(TargetId);
                //_bankAccountDaoWrite.GetListByTargetId(TargetId).Where(ent => ent.IsMain).ToList();
            }
            RG_BankWebSite.DataSource = list;
        }

        protected void RtvWebSite_NodeClick(object sender, RadTreeNodeEventArgs e)
        {
            TargetId = new Guid(e.Node.Value);
            RG_BankWebSite.Rebind();
        }

        protected void Rg_BankWebSite_InsertCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = (GridEditableItem)e.Item;
            var bankAccountsId = new Guid(((DropDownList)editedItem.FindControl("DDL_Bank")).SelectedValue);
            TargetId = new Guid(RTVWebSite.SelectedValue);
            var bwInfo = new BankAccountInfo
            {
                BankAccountsId = bankAccountsId,
                TargetId = TargetId
            };
            try
            {
                _bankAccountsWrite.InsertBindBankAccounts(TargetId, bankAccountsId);
            }
            catch
            {
                RAM.Alert("插入该资金账户失败!");
            }
            if (CacheCollection.SalePlatform.Get(bwInfo.TargetId) != null || CacheCollection.Filiale.IsB2CFiliale(bwInfo.TargetId))
            {
                try
                {
                    B2CSao.AddBankAccountBinding(bwInfo.TargetId, bwInfo.BankAccountsId);
                }
                catch (Exception exp)
                {
                    RAM.Alert("同步失败，错误信息：" + exp.Message);
                }
            }
        }

        protected void Rg_BankWebSite_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = (GridEditableItem)e.Item;
            if (editedItem != null)
            {
                var oldBankAccountsId = new Guid(((Label)editedItem.FindControl("lbOldBankAccountsId")).Text);
                var bankinfo = _bankAccountsWrite.GetBankAccounts(oldBankAccountsId);
                if (bankinfo.IsMain)
                {
                    RAM.Alert("系统提示：该资金帐号为主账号不允许编辑！");
                    return;
                }
                var bankAccountsId = new Guid(((DropDownList)editedItem.FindControl("DDL_Bank")).SelectedValue);
                var sourceName = ((Label)editedItem.FindControl("lbFromSourceNameE")).Text;
                BankAccountInfo bwInfo;
                if (sourceName == "-")
                {
                    bwInfo = new BankAccountInfo
                                    {
                                        BankAccountsId = bankAccountsId,
                                        TargetId = new Guid(RTVWebSite.SelectedNode.Value),
                                    };
                }
                else
                {
                    var selectSalePlatformInfo = CacheCollection.SalePlatform.Get(TargetId);
                    if (selectSalePlatformInfo != null)
                    {
                        bwInfo = new BankAccountInfo
                        {
                            BankAccountsId = bankAccountsId,
                            TargetId = new Guid(RTVWebSite.SelectedNode.Value),
                        };
                    }
                    else
                    {
                        RAM.Alert("只可编辑未绑定的账户！");
                        return;
                    }
                }
                try
                {
                    BankAccountInfo isinfo = _bankAccountDaoWrite.Get(bwInfo);
                    if (isinfo == null)
                    {
                        _bankAccountDaoWrite.Update(oldBankAccountsId, bwInfo);
                        BankAccount.Instance.Remove();
                    }
                    else
                    {
                        RAM.Alert("修改该资金账户已存在!");
                        return;
                    }
                }
                catch
                {
                    RAM.Alert("修改该资金账户失败!");
                    return;
                }
                if (CacheCollection.SalePlatform.Get(bwInfo.TargetId) != null || CacheCollection.Filiale.IsB2CFiliale(bwInfo.TargetId))
                {
                    try
                    {
                        B2CSao.RemoveBankAccountBinding(bwInfo.TargetId, oldBankAccountsId);
                        B2CSao.AddBankAccountBinding(bwInfo.TargetId, bwInfo.BankAccountsId);
                    }
                    catch (Exception exp)
                    {
                        RAM.Alert("同步失败，错误信息：" + exp.Message);
                    }
                }
            }
        }

        ///<summary>
        /// 获取银行账户列表
        ///</summary>
        ///<returns></returns>
        public IList<BankAccountInfo> GetBankAccountsList()
        {
            var selectSalePlatformInfo = CacheCollection.SalePlatform.Get(TargetId);
            if (selectSalePlatformInfo != null)
            {
                var bankAccountList = _bankAccountDaoWrite.GetListByTargetId(selectSalePlatformInfo.FilialeId);
                #region [已注释] 允许多绑
                //foreach (var bankAccountsInfo in bankAccountList)
                //{
                //    var iteminfo = new BankAccountsInfo
                //                    {
                //                        BankAccountsId = bankAccountsInfo.BankAccountsId,
                //                        Accounts =bankAccountsInfo.Accounts + " => " + bankAccountsInfo.BankName + " 【" +ERP.Enum.Attribute.EnumAttribute.GetKeyName((PaymentType) bankAccountsInfo.PaymentType) +"】"
                //                    };
                //    needBankAccountList.Add(iteminfo);
                //}
                #endregion

                #region  每个账号只对应一个公司
                var salePlatformList = CacheCollection.SalePlatform.GetListByFilialeId(selectSalePlatformInfo.FilialeId);
                var bindingedBankAccountList = new List<BankAccountInfo>();
                foreach (var salePlatformInfo in salePlatformList)
                {
                    bindingedBankAccountList.AddRange(BankAccountManager.ReadInstance.GetListByTargetId(salePlatformInfo.ID));
                }

                #endregion
                return (from info in bankAccountList
                    where bindingedBankAccountList.All(ent => ent.BankAccountsId != info.BankAccountsId)
                    select new BankAccountInfo
                    {
                        BankAccountsId = info.BankAccountsId, 
                        Accounts = info.Accounts + " => " + info.BankName + "【" + EnumAttribute.GetKeyName((PaymentType) info.PaymentType) + "】"
                    }).ToList();
            }
            var filiale = CacheCollection.Filiale.Get(TargetId);
            IList <BankAccountInfo> oldlist = filiale != null && filiale.FilialeTypes.Contains((int)MIS.Enum.FilialeType.SaleCompany) ?
                _bankAccountsWrite.GetBankAccountsNoBindingList().Where(b => b.IsUse).ToList():_bankAccountsWrite.GetBankAccountsList(new[] { PaymentType.Tradition, PaymentType.Tradition }).Where(b => b.IsUse).ToList();
            return oldlist.Select(binfo => new BankAccountInfo
            {
                BankAccountsId = binfo.BankAccountsId, 
                Accounts = binfo.Accounts + " => " + binfo.BankName + " 【" + EnumAttribute.GetKeyName((PaymentType) binfo.PaymentType) + "】"
            }).ToList();
        }

        protected void Rg_BankWebSite_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            var editedItem = e.Item as GridEditableItem;
            if (editedItem != null)
            {
                var bankAccountsId = new Guid(editedItem.GetDataKeyValue("BankAccountsId").ToString());
                var bankinfo = _bankAccountsWrite.GetBankAccounts(bankAccountsId);
                if (bankinfo.IsMain)
                {
                    RAM.Alert("系统提示：该资金帐号为主账号不允许删除！");
                    return;
                }
                try
                {
                    var selectSalePlatformInfo = CacheCollection.SalePlatform.Get(TargetId);
                    if (selectSalePlatformInfo != null)
                    {
                        _bankAccountDaoWrite.Delete(bankAccountsId, selectSalePlatformInfo.ID);
                    }
                    else
                    {
                        var salePlatformList = CacheCollection.SalePlatform.GetListByFilialeId(TargetId);
                        foreach (var salePlatformInfo in salePlatformList)
                        {
                            _bankAccountDaoWrite.Delete(bankAccountsId, salePlatformInfo.ID);
                        }
                        _bankAccountDaoWrite.Delete(bankAccountsId, TargetId);
                        BankAccount.Instance.Remove();
                    }
                    BankAccount.Instance.Remove();
                }
                catch
                {
                    RAM.Alert("删除该资金账户失败!");
                    return;
                }
                if (CacheCollection.SalePlatform.Get(TargetId) != null ||
                    CacheCollection.Filiale.Get(TargetId) != null)
                {
                    try
                    {
                        var selectSalePlatformInfo = CacheCollection.SalePlatform.Get(TargetId);
                        if (selectSalePlatformInfo != null)
                        {
                            B2CSao.RemoveBankAccountBinding(TargetId, bankAccountsId);
                        }
                        else
                        {
                            var salePlatformList = CacheCollection.SalePlatform.GetListByFilialeId(TargetId);
                            foreach (var salePlatformInfo in salePlatformList)
                            {
                                B2CSao.RemoveBankAccountBinding(salePlatformInfo.ID, bankAccountsId);
                            }
                            B2CSao.RemoveBankAccountBinding(TargetId, bankAccountsId);
                        }
                    }
                    catch (Exception exp)
                    {
                        RAM.Alert("同步失败，错误信息：" + exp.Message);
                    }
                }
            }
        }

        /// <summary>设置资金账户是否主账户
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void CB_IsMain_OnCheckedChanged(object sender, EventArgs e)
        {
            var cbIsMain = (OverwriteCheckBox)sender;
            var dataItem = (GridDataItem)cbIsMain.Parent.Parent;
            var bankAccountsId = dataItem.GetDataKeyValue("BankAccountsId");
            var result =_bankAccountsWrite.SetBankAccountsIsMain(new Guid(bankAccountsId.ToString()), cbIsMain.Checked);
            if (result)
            {
                //镜拓暂不同步
                var list = CacheCollection.Filiale.GetList().Where(ent => ent.FilialeTypes.Contains((int)FilialeType.SaleCompany) && ent.ID != new Guid("43609645-97dd-4ae4-989d-f3c867969a99"));
                foreach (var filialeInfo in list)
                {
                    B2CSao.SetBankAccountsIsMain(filialeInfo.ID, new Guid(bankAccountsId.ToString()), true);
                }
            }
            RG_BankWebSite.Rebind();
        }

        protected void RamAjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            WebControl.RamAjajxRequest(RG_BankWebSite, e);
        }
    }
}
