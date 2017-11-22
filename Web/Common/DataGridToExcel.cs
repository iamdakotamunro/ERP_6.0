using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace ERP.UI.Web.Common
{
    /************************************************************************************ 
     * 创建人：  张安龙
     * 创建时间：2016/03/01  
     * 描述    :DataGrid到处Excel
     * =====================================================================
     * 修改时间：2016/03/01  
     * 修改人  ：  
     * 描述    ：
     */
    public class DataGridToExcel
    {
        /// <summary>
        /// 用Microsoft自带的Gridview
        /// </summary>
        /// <param name="ds">Gridview数据源</param>
        /// <param name="ExcelName">导出的Excel的名称</param>
        public static void GridViewToExcel(DataSet ds, string ExcelName)
        {
            if (ds != null)
            {
                GridView myGridView = new GridView();
                myGridView.DataSource = ds;
                myGridView.DataBind();

                UploadExcel(myGridView, ExcelName);
            }
            else
            {
                HttpContext.Current.Response.Write("<script>alert('请检查数据源！')</script>");
            }
        }
        /// <summary>
        /// 使用此方法时需要在前台放一个Gridview（此Gridview可以是Microsoft自带的Gridview，也可以是本系统自定义的FGridView1）,同时绑定Gridview每一列的值
        /// </summary>
        /// <param name="DataGridID">前台放置的Gridview的id</param>
        /// <param name="ExcelName">导出的Excel的名称</param>
        public static void GridViewToExcel(GridView DataGridID, string ExcelName)
        {
            if (DataGridID != null)
            {
                UploadExcel(DataGridID, ExcelName, true);
            }
            else
            {
                HttpContext.Current.Response.Write("<script>alert('请检查数据源！')</script>");
            }
        }
        /// <summary>
        /// 当数据源返回类型是IDataReader时使用
        /// </summary>
        /// <param name="IDR"></param>
        /// <param name="ExcelName"></param>
        public static void GridViewToExcel(IDataReader IDR, string ExcelName)
        {
            if (IDR != null)
            {
                GridView myGridView = new GridView();
                myGridView.DataSource = IDR;
                myGridView.DataBind();
                UploadExcel(myGridView, ExcelName);
            }
            else
            {
                HttpContext.Current.Response.Write("<script>alert('请检查数据源！')</script>");
            }
        }
        /// <summary>
        /// 导出Excel的核心方法
        /// </summary>
        /// <param name="DataGridID"></param>
        /// <param name="ExcelName">导出的Excel的名称</param>
        /// <param name="flag">当使用方法三时不保持当前页面的ViewState</param>
        protected static void UploadExcel(GridView DataGridID, string ExcelName, bool flag = false)
        {
            //string style = @"<style>.text { mso-number-format:\@; }</script> ";
            
            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.Write("<meta http-equiv=Content-Type content=text/html;charset=UTF-8>");
            HttpContext.Current.Response.AddHeader("content-disposition", "attachment;filename=" + HttpUtility.UrlEncode(ExcelName, Encoding.UTF8) + ".xls");
            HttpContext.Current.Response.Charset = "UTF-8";
            HttpContext.Current.Response.ContentEncoding = Encoding.GetEncoding("UTF-8");
            HttpContext.Current.Response.ContentType = "application/ms-excel";
            if (flag)
            {
                DataGridID.Page.EnableViewState = false;
            }
            string strStyle = "<style>td{mso-number-format:\"\\@\";}</style>";//导入到excel时,保存表里数字列中前面存在的0 
            StringWriter stringWrite = new StringWriter();
            stringWrite.WriteLine(strStyle);
            HtmlTextWriter htmlWrite = new HtmlTextWriter(stringWrite);

            #region 去除链接功能没有实现（原因是因为日常监管中的GridView都是自动创建的，通过Controls无法找到相应的控件）
            //foreach (GridViewRow R in DataGridID.Rows)//r.Cells.Count
            //{
            //    for (int i = 0; i < R.Cells.Coun t; i++)
            //    {
            //        //HyperLink linkButton = ((HyperLink)(DataGridID.HeaderRow.Cells[i].Controls[0]));//获取标题行第i个单元格的<a>标签(LinkButton)
            //        //string value = linkButton.Text;//保存<a>标签的文本
            //        //DataGridID.HeaderRow.Cells[i].Controls.Clear();//把标题行第i个单元格的所有控件删除(把<a>标签删除)
            //        //DataGridID.HeaderRow.Cells[i].Text = value;//把<a>标签的文本赋给标题行第i个单元格的文本
            //        //linkButton.Dispose();

            //        //R.Cells[i].Attributes.Add("class", "text");
            //        // Cells.Hyperlinks.Delete; 
            //    }
            //}


            //Literal lit = new Literal();
            //for (int i = 0; i < DataGridID.Controls.Count; i++)
            //{
            //    if (DataGridID.Controls[i].GetType() == typeof(HyperLink))
            //    {
            //        lit.Text = (DataGridID.Controls[i] as HyperLink).Text;

            //        DataGridID.Controls.Remove(DataGridID.Controls[i]);

            //        DataGridID.Controls.AddAt(i, lit);
            //    }
            //}
            #endregion

            DataGridID.RenderControl(htmlWrite);

            HttpContext.Current.Response.Write(stringWrite.ToString());
            HttpContext.Current.Response.End();
        }
    }

}
