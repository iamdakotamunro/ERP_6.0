using System;
using ERP.BLL.Implement;
using ERP.BLL.Interface;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.BLL.UnitTest
{
    /************************************************************************************ 
     * 创建人：  文雯 
     * 创建时间：2016/5/30 16:39:08 
     * 描述    :
     * =====================================================================
     * 修改时间：2016/5/30 16:39:08 
     * 修改人  ：  
     * 描述    ：
     */
    [TestClass]
    public class UnitTest_OperationLogManager
    {
        private static readonly OperationLogManager _operationLogManager = new OperationLogManager(); 


        [TestMethod]
        public void TestGetOperationLogList()
        {
            try
            {
                _operationLogManager.GetOperationLogList(null);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }


        [TestMethod]
        public void TestGetOperationLogListByKey()
        {
            try
            {
                _operationLogManager.GetOperationLogList(Guid.Empty);
            }
            catch (Exception ex)
            {
                Assert.IsTrue(!string.IsNullOrEmpty(ex.Message));
            }
        }
    }
}
