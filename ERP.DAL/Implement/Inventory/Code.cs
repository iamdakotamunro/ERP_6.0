using System;
using System.Globalization;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using Keede.DAL.Helper;

namespace ERP.DAL.Implement.Inventory
{
    public class Code : ICode
    {
        public Code(Environment.GlobalConfig.DB.FromType fromType) { }

        private const string SQL_SELECT_CODEVALUE = @"
DECLARE @CodeValue int;
SET @CodeValue = 1
IF EXISTS (SELECT CodeType FROM dbo.lmShop_Code with(updlock,rowlock) WHERE CodeType=@CodeType)
	BEGIN 
		UPDATE lmShop_Code with(updlock,rowlock)
		SET @CodeValue=CodeValue=
		(
			CASE WHEN (DATEDIFF(hh,LastDateTime,@DateTimeNow)>0) 
			THEN 0 ELSE  CodeValue END
		)+1
		,LastDateTime=@DateTimeNow 
		WHERE CodeType=@CodeType;
	END
ELSE 
	BEGIN
		INSERT INTO lmShop_Code (CodeType,CodeValue,LastDateTime) VALUES (@CodeType,1,@DateTimeNow)
	END
SELECT @CodeValue
";
        
        private const string PARM_CODETYPE = "@CodeType";

        /// <summary>
        /// 获取指定类型的当前编号
        /// </summary>
        /// <param name="codeType">单据类型</param>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public int GetCodeValue(CodeType codeType, DateTime dateTime)
        {
            var parms = new[] {
                new Parameter(PARM_CODETYPE, codeType),
				new Parameter("@DateTimeNow", dateTime)
            };
            using (var db = DatabaseFactory.Create())
            {
                return db.GetValue<int>(false, SQL_SELECT_CODEVALUE, parms);
            }
        }

        /// <summary>
        /// 获取指定类型的订单号
        /// </summary>
        /// <param name="codeType"></param>
        /// <returns></returns>
        public string GetCode(CodeType codeType)
        {
            DateTime dateTime = DateTime.Now;
            string tradeCode = codeType.ToString() + dateTime.Year.ToString(CultureInfo.InvariantCulture).Substring(2, 2) + dateTime.Month.ToString("D2") + dateTime.Day.ToString("D2") + dateTime.Hour.ToString("D2") + GetCodeValue(codeType, dateTime).ToString("D3");
            return tradeCode;
        }
    }
}
