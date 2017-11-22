namespace PrintCertificateBarcode.UIForm
{
    public partial class GlassBarcodePrintForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GlassBarcodePrintForm));
            this.tb_InputKey = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cb_SelectLabelType = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cb_SelectPrinter = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_Print = new System.Windows.Forms.Button();
            this.btn_PrintView = new System.Windows.Forms.Button();
            this.radioButton_KD = new System.Windows.Forms.RadioButton();
            this.radioButton_KEDE = new System.Windows.Forms.RadioButton();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // tb_InputKey
            // 
            this.tb_InputKey.Font = new System.Drawing.Font("新宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.tb_InputKey.Location = new System.Drawing.Point(158, 20);
            this.tb_InputKey.Name = "tb_InputKey";
            this.tb_InputKey.Size = new System.Drawing.Size(124, 21);
            this.tb_InputKey.TabIndex = 1;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cb_SelectLabelType);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.cb_SelectPrinter);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.tb_InputKey);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(299, 82);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "标签打印";
            // 
            // cb_SelectLabelType
            // 
            this.cb_SelectLabelType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_SelectLabelType.FormattingEnabled = true;
            this.cb_SelectLabelType.Location = new System.Drawing.Point(69, 20);
            this.cb_SelectLabelType.Name = "cb_SelectLabelType";
            this.cb_SelectLabelType.Size = new System.Drawing.Size(83, 20);
            this.cb_SelectLabelType.TabIndex = 8;
            this.cb_SelectLabelType.SelectedIndexChanged += new System.EventHandler(this.cb_SelectLabelType_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 56);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "打印机：";
            // 
            // cb_SelectPrinter
            // 
            this.cb_SelectPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_SelectPrinter.FormattingEnabled = true;
            this.cb_SelectPrinter.Location = new System.Drawing.Point(69, 53);
            this.cb_SelectPrinter.Name = "cb_SelectPrinter";
            this.cb_SelectPrinter.Size = new System.Drawing.Size(213, 20);
            this.cb_SelectPrinter.TabIndex = 6;
            this.cb_SelectPrinter.SelectedIndexChanged += new System.EventHandler(this.cb_SelectPrinter_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "类  型：";
            // 
            // btn_Print
            // 
            this.btn_Print.Enabled = false;
            this.btn_Print.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btn_Print.Location = new System.Drawing.Point(317, 68);
            this.btn_Print.Name = "btn_Print";
            this.btn_Print.Size = new System.Drawing.Size(114, 26);
            this.btn_Print.TabIndex = 5;
            this.btn_Print.Text = "打印标签";
            this.btn_Print.UseVisualStyleBackColor = true;
            this.btn_Print.Click += new System.EventHandler(this.btn_Print_Click);
            // 
            // btn_PrintView
            // 
            this.btn_PrintView.Enabled = false;
            this.btn_PrintView.Font = new System.Drawing.Font("微软雅黑", 9F);
            this.btn_PrintView.Location = new System.Drawing.Point(317, 39);
            this.btn_PrintView.Name = "btn_PrintView";
            this.btn_PrintView.Size = new System.Drawing.Size(114, 26);
            this.btn_PrintView.TabIndex = 6;
            this.btn_PrintView.Text = "打印预览";
            this.btn_PrintView.UseVisualStyleBackColor = true;
            this.btn_PrintView.Click += new System.EventHandler(this.btn_PrintView_Click);
            // 
            // radioButton_KD
            // 
            this.radioButton_KD.AutoSize = true;
            this.radioButton_KD.Location = new System.Drawing.Point(318, 17);
            this.radioButton_KD.Name = "radioButton_KD";
            this.radioButton_KD.Size = new System.Drawing.Size(35, 16);
            this.radioButton_KD.TabIndex = 7;
            this.radioButton_KD.Text = "KD";
            this.radioButton_KD.UseVisualStyleBackColor = true;
            // 
            // radioButton_KEDE
            // 
            this.radioButton_KEDE.AutoSize = true;
            this.radioButton_KEDE.Location = new System.Drawing.Point(359, 17);
            this.radioButton_KEDE.Name = "radioButton_KEDE";
            this.radioButton_KEDE.Size = new System.Drawing.Size(47, 16);
            this.radioButton_KEDE.TabIndex = 8;
            this.radioButton_KEDE.Text = "KEDE";
            this.radioButton_KEDE.UseVisualStyleBackColor = true;
            // 
            // GlassBarcodePrintForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(445, 107);
            this.Controls.Add(this.radioButton_KEDE);
            this.Controls.Add(this.radioButton_KD);
            this.Controls.Add(this.btn_PrintView);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btn_Print);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GlassBarcodePrintForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "框架条码标签打印";
            this.Load += new System.EventHandler(this.GlassInStockBarcodePrintForm_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_InputKey;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_Print;
        private System.Windows.Forms.ComboBox cb_SelectPrinter;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cb_SelectLabelType;
        private System.Windows.Forms.Button btn_PrintView;
        private System.Windows.Forms.RadioButton radioButton_KD;
        private System.Windows.Forms.RadioButton radioButton_KEDE;
    }
}