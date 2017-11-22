using System;
using ERP.DAL.Factory;
using ERP.DAL.Interface.IBasis;
using Keede.Ecsoft.Model;

namespace ERP.BLL.Implement.Basis
{
    public class WebRudder : BllInstance<WebRudder>
    {
        private readonly IWebRudder _webRudder;

        public WebRudder(IWebRudder webRudder)
        {
            _webRudder = webRudder;
        }

        public WebRudder(Environment.GlobalConfig.DB.FromType fromType = Environment.GlobalConfig.DB.FromType.Read)
        {
            _webRudder = BasisInstance.GetWebRudderDao(fromType);
        }
        
        /// <summary>
        /// �������ֽ�
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public decimal CurrencyValue(decimal value)
        {
            var webRudderInfo = _webRudder.GetWebRudder()??new WebRudderInfo();
            switch (webRudderInfo.CurrencyDecimalType)
            {
                case 1://��������
                    value = Math.Round(value, webRudderInfo.CurrencyDecimalDigits);
                    break;
                case 2://����ȡ��
                    value = Math.Round(value, webRudderInfo.CurrencyDecimalDigits, MidpointRounding.AwayFromZero);
                    break;
                case 3://����ȡ��
                    value = Math.Round(value, webRudderInfo.CurrencyDecimalDigits, MidpointRounding.ToEven);
                    break;
            }
            return value;
        }
    }
}
