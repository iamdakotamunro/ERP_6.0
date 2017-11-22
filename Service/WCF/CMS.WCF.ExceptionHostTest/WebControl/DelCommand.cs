using System;
using LonmeShop.CommandBLL;


/// <summary>
/// 删除离现在半个月之前的IsSuccess=1 AND IsSendCommand=0的记录
/// </summary>
public class DelCommand
{
    private readonly int delTime = Convert.ToInt32(Function.GetConfig("DelTime"));
    private DateTime delDate = DateTime.MinValue;
    private readonly Command commandText = new Command();

    public void DeleteCommand()
    {
        if (DateTime.Now.Hour == delTime && delDate.ToString("yyyyMMdd") != DateTime.Now.ToString("yyyyMMdd"))
        {
            try
            {
                 delDate = DateTime.Now;
                 commandText.DelCommand();
            }
            catch (Exception ex) 
            {

                throw new ApplicationException(ex.Message);
            }               
        }
        
    }

}

