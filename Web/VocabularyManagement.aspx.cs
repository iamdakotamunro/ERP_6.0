using ERP.UI.Web.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.UI.Web.Base;
using Telerik.Web.UI;

namespace ERP.UI.Web
{
    public partial class VocabularyManagement : BasePage
    {
        private static readonly IVocabulary _vocabulary = new DAL.Implement.Inventory.VocabularyDal(GlobalConfig.DB.FromType.Write);
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        //查询
        protected void btn_Search_Click(object sender, EventArgs e)
        {
            GridDataBind();
            RG_Vocabulary.DataBind();
        }

        #region 数据列表相关
        protected void RG_Vocabulary_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            GridDataBind();
        }

        //Grid数据源
        protected void GridDataBind()
        {
            RG_Vocabulary.DataSource = _vocabulary.GetVocabularyListByVocabularyName(txt_VocabularyName.Text);
        }
        #endregion

        //批量删除
        protected void btn_BatchDel_Click(object sender, EventArgs e)
        {
            if (Request["ckId"] != null)
            {
                var result = _vocabulary.DelById(Request["ckId"].Split(','));
                if (result)
                {
                    MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                    MessageBox.Show(this, "批量删除成功！");
                }
                else
                {
                    MessageBox.Show(this, "批量删除失败！");
                }
            }
            else
            {
                MessageBox.Show(this, "请选择相关数据！");
            }
        }

        //删除
        protected void btn_Del_Click(object sender, EventArgs e)
        {
            var id = ((Button)sender).CommandArgument;
            var result = _vocabulary.DelById(new string[] { id });
            if (result)
            {
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                MessageBox.Show(this, "删除成功！");
            }
            else
            {
                MessageBox.Show(this, "删除失败！");
            }
        }

        //更改状态
        public void btn_CheckState_Click(object sender, EventArgs e)
        {
            var id = Hid_Id.Value;
            var state = Hid_State.Value;
            var result = _vocabulary.UpdateStateById(new Guid(id), int.Parse(state));
            if (result)
            {
                MessageBox.AppendScript(this, "setTimeout(function(){ refreshGrid(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
            }
        }

        //生成Txt
        protected void btn_Txt_Click(object sender, EventArgs e)
        {
            var vocabularyInfoList = _vocabulary.GetVocabularyListByVocabularyName(string.Empty);
            if (vocabularyInfoList.Count == 0)
            {
                MessageBox.Show(this, "没有可生成的数据！");
                return;
            }
            var errorMsg = string.Empty;
            try
            {
                #region 将上传文件保存至临时文件夹
                string folderPath = "~/UserDir/Vocabulary/Export/";
                if (!Directory.Exists(Server.MapPath(folderPath)))
                {
                    Directory.CreateDirectory(Server.MapPath(folderPath));
                }
                var filePath = Server.MapPath(folderPath + "bannedword.txt");
                #endregion

                #region 创建并写入文件
                //如果文件不存在,创建文件;如果存在,覆盖文件; true:追加, false:覆盖原文件
                StreamWriter sw = new StreamWriter(filePath, false, Encoding.UTF8);
                foreach (var item in vocabularyInfoList)
                {
                    //开始写入
                    sw.Write(item.VocabularyName + "\r\n");
                }
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
                #endregion
            }
            catch (Exception ex)
            {
                errorMsg = ex.Message;
            }
            if (string.IsNullOrEmpty(errorMsg))
            {
                MessageBox.AppendScript(this, "document.getElementById(\"a_Look\").click();");
            }
            else
            {
                MessageBox.Show(this, errorMsg);
            }
        }
    }
}