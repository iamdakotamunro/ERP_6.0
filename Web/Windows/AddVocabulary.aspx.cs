using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using ERP.DAL.Interface.IInventory;
using ERP.Enum;
using ERP.Environment;
using ERP.Model;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;
using Framework.Data;
using WebControl = System.Web.UI.WebControls.WebControl;

namespace ERP.UI.Web.Windows
{
    public partial class AddVocabulary : WindowsPage
    {
        private static readonly IVocabulary _vocabulary = new DAL.Implement.Inventory.VocabularyDal(GlobalConfig.DB.FromType.Write);
        protected void Page_Load(object sender, EventArgs e)
        {

        }
        //导入文件
        protected void btn_Upload_Click(object sender, EventArgs e)
        {
            var excelName = UploadExcelName.Text;
            UploadExcelName.Text = string.Empty;

            #region 验证文件
            if (!UploadExcel.HasFile || string.IsNullOrEmpty(excelName))
            {
                MessageBox.Show(this, "请选择格式为“.xls”文件！");
                return;
            }

            var ext = Path.GetExtension(UploadExcel.FileName);
            if (ext != null && !ext.Equals(".xls"))
            {
                MessageBox.Show(this, "文件格式错误(.xls)！");
                return;
            }
            #endregion

            try
            {
                #region 将上传文件保存至临时文件夹
                string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
                string folderPath = "~/UserDir/Vocabulary/";
                if (!Directory.Exists(Server.MapPath(folderPath)))
                {
                    Directory.CreateDirectory(Server.MapPath(folderPath));
                }
                string filePath = Server.MapPath(folderPath + fileName);
                UploadExcel.PostedFile.SaveAs(filePath);
                #endregion

                var excelDataTable = ExcelHelper.GetDataSet(filePath).Tables[0];

                #region 获取数据之后删除临时文件
                if (File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                #endregion

                List<VocabularyInfo> list = new List<VocabularyInfo>();
                List<string> verbList = new List<string>();
                List<string> nounList = new List<string>();
                var personnel = CurrentSession.Personnel.Get();
                for (int i = 0; i < excelDataTable.Rows.Count; i++)
                {
                    var disableWords = excelDataTable.Rows[i]["禁用词"].ToString();
                    var verb = excelDataTable.Rows[i]["动词"].ToString();
                    var noun = excelDataTable.Rows[i]["名词"].ToString();

                    if (!string.IsNullOrEmpty(disableWords))
                    {
                        var vocabularyInfo = new VocabularyInfo
                        {
                            Id = Guid.NewGuid(),
                            VocabularyName = disableWords,
                            State = (int)VocabularyState.Enable,
                            Operator = personnel.RealName,
                            OperatingTime = DateTime.Now
                        };
                        list.Add(vocabularyInfo);
                    }

                    if (!string.IsNullOrEmpty(verb))
                    {
                        verbList.Add(verb);
                    }

                    if (!string.IsNullOrEmpty(noun))
                    {
                        nounList.Add(noun);
                    }
                }

                if (verbList.Count > 0 && nounList.Count > 0)
                {
                    foreach (var itemVerb in verbList)
                    {
                        foreach (var itemNoun in nounList)
                        {
                            var vocabularyInfo = new VocabularyInfo
                            {
                                Id = Guid.NewGuid(),
                                VocabularyName = itemVerb + itemNoun,
                                State = (int)VocabularyState.Enable,
                                Operator = personnel.RealName,
                                OperatingTime = DateTime.Now
                            };
                            list.Add(vocabularyInfo);
                        }
                    }
                }

                if (list.Count > 0)
                {
                    var vocabularyInfoList = _vocabulary.GetVocabularyListByVocabularyName(string.Empty);
                    list = vocabularyInfoList.Aggregate(list, (current, item) => current.Where(p => !p.VocabularyName.Equals(item.VocabularyName)).ToList());
                    if (list.Count > 0)
                    {
                        var result = _vocabulary.AddBatchVocabulary(list);
                        if (result)
                        {
                            MessageBox.AppendScript(this, "setTimeout(function(){ CloseAndRebind(); }, " + GlobalConfig.PageAutoRefreshDelayTime + ");");
                        }
                    }
                    else
                    {
                        MessageBox.Show(this, "导入词重复，无需重复导入!");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message);
            }
        }
    }
}