using System;
using System.IO;
using System.Linq;
using System.Text;
using ERP.Model.Goods;
using ERP.SAL.Goods;
using ERP.SAL.Interface;
using ERP.UI.Web.Base;
using ERP.UI.Web.Common;

namespace ERP.UI.Web.Windows
{
    public partial class AddOrEditGoodsAttrWords : WindowsPage
    {
        readonly IGoodsCenterSao _goodsAttributeGroupSao = new GoodsCenterSao();
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["GroupId"] != null)
                {
                    var attributeGroupInfo = _goodsAttributeGroupSao.GetAttrGroupList().Where(p => p.GroupId.Equals(int.Parse(Request.QueryString["GroupId"]))).ToList().FirstOrDefault();
                    if (attributeGroupInfo != null)
                    {
                        if (!attributeGroupInfo.IsUploadImage)
                        {
                            attrWordImage.Visible = false;
                        }
                        if (!string.IsNullOrEmpty(Request.QueryString["WordId"]))
                        {
                            var attributeWordInfo = _goodsAttributeGroupSao.GetAttrWordsListByGroupId(int.Parse(Request.QueryString["GroupId"])).FirstOrDefault(p => p.WordId.Equals(int.Parse(Request.QueryString["WordId"])));
                            if (attributeWordInfo != null)
                            {
                                txt_Word.Text = attributeWordInfo.Word;
                                ckb_IsShow.Checked = attributeWordInfo.IsShow;
                                txt_OrderIndex.Text = attributeWordInfo.OrderIndex.ToString();
                                ddl_CompareType.SelectedValue = attributeWordInfo.CompareType.ToString();
                                txt_WordValue.Text = attributeWordInfo.WordValue;
                                txt_TopValue.Text = attributeWordInfo.TopValue;
                                if (!string.IsNullOrEmpty(attributeWordInfo.AttrWordImage))
                                {
                                    Hid_AttrWordImage.Value = attributeWordInfo.AttrWordImage;
                                    UploadImgName.Text = attributeWordInfo.AttrWordImage.Substring(attributeWordInfo.AttrWordImage.LastIndexOf('/') + 1);
                                    PreA.HRef = attributeWordInfo.AttrWordImage;
                                }
                            }
                        }
                    }
                }
            }
        }

        //保存数据
        protected void btn_Save_Click(object sender, EventArgs e)
        {
            var groupId = Request.QueryString["GroupId"];
            if (groupId != null)
            {
                #region 验证数据
                var errorMsg = CheckData();
                if (!string.IsNullOrEmpty(errorMsg))
                {
                    MessageBox.Show(this, errorMsg);
                    return;
                }
                #endregion

                var attributeWordInfo = new AttributeWordInfo
                {
                    GroupId = int.Parse(groupId),
                    Word = txt_Word.Text,
                    IsShow = ckb_IsShow.Checked,
                    OrderIndex = int.Parse(txt_OrderIndex.Text),
                    CompareType = int.Parse(ddl_CompareType.SelectedValue),
                    WordValue = txt_WordValue.Text,
                    TopValue = txt_TopValue.Text,
                    AttrWordImage = Hid_AttrWordImage.Value
                };

                string errorMessage;
                var result = false;

                if (string.IsNullOrEmpty(Request.QueryString["WordId"]))
                {
                    string filePath = UploadAttrWordImage(string.Empty);
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        attributeWordInfo.AttrWordImage = filePath;
                    }
                    result = _goodsAttributeGroupSao.AddAttrWords(attributeWordInfo, out errorMessage);
                }
                else
                {
                    string filePath = UploadAttrWordImage(Hid_AttrWordImage.Value);
                    if (!string.IsNullOrEmpty(filePath))
                    {
                        attributeWordInfo.AttrWordImage = filePath;
                    }
                    else if (string.IsNullOrEmpty(UploadImgName.Text))
                    {
                        attributeWordInfo.AttrWordImage = filePath;
                    }

                    attributeWordInfo.WordId = int.Parse(Request.QueryString["WordId"]);
                    result = _goodsAttributeGroupSao.UpdateAttrWords(attributeWordInfo, out errorMessage);
                }

                if (result)
                {
                    MessageBox.AppendScript(this, "alert('保存成功！');CloseAndRebind();");
                }
                else
                {
                    MessageBox.Show(this, "保存失败！" + errorMessage);
                }
            }
        }

        /// <summary>
        /// 上传属性词图片
        /// </summary>
        /// <returns></returns>
        protected string UploadAttrWordImage(string imagePath)
        {
            //判断是否已经上传文件
            if (UploadImg.HasFile && !string.IsNullOrEmpty(UploadImgName.Text))
            {
                if (UploadImg.PostedFile != null && UploadImg.PostedFile.ContentLength > 0)
                {
                    #region 如果上传文件已经存在，则删除该文件
                    if (!string.IsNullOrEmpty(imagePath) && File.Exists(Server.MapPath(imagePath)))
                    {
                        File.Delete(Server.MapPath(imagePath));
                    }
                    #endregion

                    string ext = Path.GetExtension(UploadImg.PostedFile.FileName).ToLower();
                    if (ext == ".jpg" || ext == ".jepg" || ext == ".bmp" || ext == ".gif" || ext == ".png")
                    {
                        string fileName = DateTime.Now.ToString("yyyyMMddHHmmss") + ext;
                        string folderPath = string.Format("../UserDir/AttrWordsImg/{0}", DateTime.Now.ToString("yyyyMM") + "/");
                        if (!Directory.Exists(Server.MapPath(folderPath)))
                        {
                            Directory.CreateDirectory(Server.MapPath(folderPath));
                        }
                        string filePath = folderPath + fileName;
                        UploadImg.PostedFile.SaveAs(Server.MapPath(filePath));
                        return filePath;
                    }
                    MessageBox.AppendScript(this, "图片格式错误（.jpg|.jepg|.bmp|.gif|.png）！");
                }
                else
                {
                    MessageBox.AppendScript(this, "请选择一张图片！");
                }
            }
            return string.Empty;
        }

        //验证数据
        protected string CheckData()
        {
            var errorMsg = new StringBuilder();
            var word = txt_Word.Text;
            if (string.IsNullOrEmpty(word))
            {
                errorMsg.Append("请填写“属性名称”！").Append("\\n");
            }
            var orderIndex = txt_OrderIndex.Text;
            if (string.IsNullOrEmpty(orderIndex))
            {
                errorMsg.Append("请填写“排序字段”！").Append("\\n");
            }
            var wordValue = txt_WordValue.Text;
            if (string.IsNullOrEmpty(wordValue))
            {
                errorMsg.Append("请填写“比较值”！").Append("\\n");
            }

            #region 比较上限
            if (ddl_CompareType.SelectedValue.Equals("1"))
            {
                var topValue = txt_TopValue.Text;
                if (string.IsNullOrEmpty(topValue))
                {
                    errorMsg.Append("请填写“比较上限”！").Append("\\n");
                }
            }
            #endregion

            return errorMsg.ToString();
        }
    }
}