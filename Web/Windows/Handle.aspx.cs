using System;
using System.Collections.Generic;
using System.Linq;
using ERP.DAL.Implement.Inventory;
using ERP.DAL.Interface.IInventory;
using ERP.SAL;
using ERP.UI.Web.Base;
using ERP.Environment;
using MIS.Enum;

namespace ERP.UI.Web.Windows
{
    public partial class Handle : WindowsPage
    {
        private readonly IInternalPriceSetDao _internalPriceSetDao = new InternalPriceSetDao(GlobalConfig.DB.FromType.Read);
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                String goodsType = Request.QueryString["goodsType"];
                var filialeDic = MISService.GetAllFiliales().Where(act => act.IsActive && (act.FilialeTypes.Contains((int)FilialeType.LogisticsCompany))).ToList();
                foreach (var filiale in filialeDic)
                {
                    try
                    {
                        if (!String.IsNullOrWhiteSpace(Request.QueryString[filiale.ID.ToString()]))
                        {
                            var result = Convert.ToDecimal(Request.QueryString[filiale.ID.ToString()]);
                        }
                        _internalPriceSetDao.UpdateInternalPriceSetInfo(Convert.ToInt32(goodsType), filiale.ID, Request.QueryString[filiale.ID.ToString()] == "" ? "0" : Request.QueryString[filiale.ID.ToString()]);
                    }
                    catch (Exception)
                    {
                        throw new Exception(filiale.Name + ":预留利润比例设置 " + Request.QueryString[filiale.ID.ToString()] + " 输入的格式不正确！");
                    }
                }
            }
        }
    }
}