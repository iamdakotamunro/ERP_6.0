using System;
using System.Linq;
using ERP.BLL.Implement.Inventory;
using ERP.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.BLL.UnitTest
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/5/26 10:53:28 
     * 描述    :
     * =====================================================================
     * 修改时间：2016/5/26 10:53:28 
     * 修改人  ：  
     * 描述    ：
     */
     [TestClass]
    public class UnitTest_WarehouseManager
    {
         readonly WarehouseManager _warehouse = new WarehouseManager(); 


         [TestMethod]
         public void TestGetWarehouseIsPermission()
         {
             try
             {
                 _warehouse.GetWarehouseIsPermission(new Guid("7ae62af0-eb1f-49c6-8fd1-128d77c84698"), new Guid("c365d6e2-22ea-4295-9333-b2476351648a"), new Guid("176a425e-1dc2-4068-84ad-d5e37f1efce3"));
             }
             catch (Exception ex)
             {
                 Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
             }
         }


         [TestMethod]
         public void TestGetWarehouse()
         {
             try
             {
                 _warehouse.GetWarehouse(new Guid("84B303F5-2AA6-437D-9D23-3488AD55D278"));
             }
             catch (Exception ex)
             {
                 Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
             }
         }

         [TestMethod]
         public void GetWarehouseListByOrderState()
         {
             try
             {
                 var wIdlist = WarehouseManager.GetList().Where(w => w.State == (int)WarehouseState.Enable).Select(w => w.WarehouseId).ToList();
                 //获取授权仓库列表
                 var warehouseList = _warehouse.GetWarehouseIsPermission(new Guid("7ae62af0-eb1f-49c6-8fd1-128d77c84698"), new Guid("c365d6e2-22ea-4295-9333-b2476351648a"), new Guid("176a425e-1dc2-4068-84ad-d5e37f1efce3")).ToList();
                 var idList = warehouseList.Where(w => wIdlist.Contains(w.WarehouseId) && (w.WarehouseType == (int)WarehouseType.MainStock || w.WarehouseType == (int)WarehouseType.AfterSaleStock)).Select(w => w.WarehouseId).ToList();
                 OrderState[] orderStateArray = { OrderState.StockUp, OrderState.Redeploy };
                 _warehouse.GetWarehouseListByOrderState(orderStateArray, idList);
             }
             catch (Exception ex)
             {
                 Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
             }
         }
    }
}
