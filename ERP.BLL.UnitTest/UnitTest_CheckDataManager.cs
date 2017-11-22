using System;
using ERP.BLL.Implement.Inventory;
using ERP.Enum;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.BLL.UnitTest
{
    /************************************************************************************ 
     * 创建人：  文雯
     * 创建时间：2016/6/1 17:00:55 
     * 描述    :
     * =====================================================================
     * 修改时间：2016/6/1 17:00:55 
     * 修改人  ：  
     * 描述    ：
     */
    [TestClass]
    public class UnitTest_CheckDataManager
    {
        private static readonly CheckDataManager _checkDataManagerWrite = CheckDataManager.WriteInstance;

        [TestMethod]
        public void TestGetCheckDataList()
        {
            try
            {
                _checkDataManagerWrite.GetCheckDataList(Guid.Empty, -1, "", DateTime.Now, DateTime.Now, null);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }


        [TestMethod]
        public void TestGetCheckDataInfoById()
        {
            try
            {
                _checkDataManagerWrite.GetCheckDataInfoById(Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }



        [TestMethod]
        public void TestGetTotalMoney()
        {
            try
            {
                _checkDataManagerWrite.GetTotalMoney(Guid.Empty); //实际对账金额 (实收金额累计)
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }

        [TestMethod]
        public void TestUpdateState()
        {
            try
            {
                _checkDataManagerWrite.UpdateState(Guid.Empty, (int)CheckDataState.Checked);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }


        [TestMethod]
        public void TestDeleteConfirmData()
        {
            try
            {
                _checkDataManagerWrite.DeleteConfirmData(Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }

        
    }
}
