using System;
using System.Configuration;
using System.Web.UI;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.Environment;

namespace ERP.UI.Web.UserControl
{
    public partial class GetImg : Page
    {
        static readonly  IGoodsCenterSao _goodsCenterSao=new GoodsCenterSao();

        /// <summary>
        /// 获取HTML控件中IMG控件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            string imgtype = Request.Params["imgtype"];
            string imgid=Request.Params["filename"];
            Response.Write(GetImage(imgtype, imgid));
            Response.End();
        }

        public string GetImage(string imgtype, string imgId)
        {
            GoodsInfo info = _goodsCenterSao.GetGoodsBaseInfoById(new Guid(imgId));
            if (info != null)
            {
                var goodsId = info.GoodsId.ToString();
                var fullPath = GlobalConfig.ResourceServerImg + goodsId + "/" + goodsId + ".jpg";
                return fullPath;
            }
            return string.Empty;
        }
    }
}
