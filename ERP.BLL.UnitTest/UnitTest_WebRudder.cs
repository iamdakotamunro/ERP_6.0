using ERP.BLL.Implement.Basis;
using ERP.DAL.Interface.IBasis.Fakes;
using Keede.Ecsoft.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ERP.BLL.UnitTest
{
    /// <summary>
    /// WebRudderTest 的摘要说明
    /// </summary>
    [TestClass]
    public class UnitTestWebRudder
    {
        private static readonly StubIWebRudder _stubiWebRudder = new StubIWebRudder();
        private WebRudder _webRudder;

        [TestMethod]
        public void TestCurrencyValue()
        {
            _stubiWebRudder.GetWebRudder = () => null;
            _webRudder=new WebRudder(_stubiWebRudder);
            var result = _webRudder.CurrencyValue((decimal)10.49); //四舍五入
            Assert.AreEqual(result, 10);

            var result1 = _webRudder.CurrencyValue((decimal)10.55); //四舍五入
            Assert.AreEqual(result1, 11);

            _stubiWebRudder.GetWebRudder = () => new WebRudderInfo { CurrencyDecimalType =2};  //向上取整
            _webRudder = new WebRudder(_stubiWebRudder);
            var result2 = _webRudder.CurrencyValue((decimal)10.55); 
            Assert.AreEqual(result2, 11);

            _stubiWebRudder.GetWebRudder = () => new WebRudderInfo { CurrencyDecimalType = 3 }; //向下取整
            _webRudder = new WebRudder(_stubiWebRudder);
            var result3 = _webRudder.CurrencyValue((decimal)10.45); 
            Assert.AreEqual(result3, 10);
        }
    }
}
