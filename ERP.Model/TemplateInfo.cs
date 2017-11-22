using System;

namespace Keede.Ecsoft.Model
{
    /// <summary>
    /// 模板模型
    /// </summary>
    public class TemplateInfo
    {
        /// <summary>
        /// 模版ID
        /// </summary>
        public Guid TemplateID { get; set; }

        /// <summary>
        /// 模版标题
        /// </summary>
        public string TemplateCaption { get; set; }

        /// <summary>
        /// 模版内容
        /// </summary>
        public string TemplateContent { get; set; }

        /// <summary>
        /// 模版类型
        /// </summary>
        public int TemplateType { get; set; }

        /// <summary>
        /// 模版状态
        /// </summary>
        public int TemplateState { get; set; }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public TemplateInfo()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="templateID">模版ID</param>
        /// <param name="templateCaption">模版标题</param>
        /// <param name="templateContent">模版内容</param>
        /// <param name="templateType">模版类型</param>
        /// <param name="templateState">模版状态</param>
        public TemplateInfo(Guid templateID, string templateCaption, string templateContent, int templateType, int templateState)
        {
            TemplateID = templateID;
            TemplateCaption = templateCaption;
            TemplateContent = templateContent;
            TemplateType = templateType;
            TemplateState = templateState;
        }
    }
}