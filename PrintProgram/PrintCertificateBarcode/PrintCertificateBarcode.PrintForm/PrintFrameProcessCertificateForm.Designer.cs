namespace PrintCertificateBarcode.UIForm
{
    partial class PrintFrameProcessCertificateForm
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lb_RealName = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_AccountNo = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tb_ProcessNo = new System.Windows.Forms.TextBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.cb_SelectPrinter = new System.Windows.Forms.ComboBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.lb_RealName);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.tb_AccountNo);
            this.groupBox1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox1.Location = new System.Drawing.Point(12, 118);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(388, 138);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "检验师";
            // 
            // lb_RealName
            // 
            this.lb_RealName.AutoSize = true;
            this.lb_RealName.Location = new System.Drawing.Point(169, 86);
            this.lb_RealName.Name = "lb_RealName";
            this.lb_RealName.Size = new System.Drawing.Size(0, 27);
            this.lb_RealName.TabIndex = 3;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(132, 27);
            this.label2.TabIndex = 2;
            this.label2.Text = "检验师姓名：";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.label1.Location = new System.Drawing.Point(26, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(112, 27);
            this.label1.TabIndex = 1;
            this.label1.Text = "输入工号：";
            // 
            // tb_AccountNo
            // 
            this.tb_AccountNo.Enabled = false;
            this.tb_AccountNo.Font = new System.Drawing.Font("Consolas", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tb_AccountNo.Location = new System.Drawing.Point(174, 28);
            this.tb_AccountNo.Name = "tb_AccountNo";
            this.tb_AccountNo.Size = new System.Drawing.Size(139, 31);
            this.tb_AccountNo.TabIndex = 0;
            this.tb_AccountNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_AccountNo_KeyPress);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.tb_ProcessNo);
            this.groupBox2.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox2.Location = new System.Drawing.Point(12, 267);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(388, 123);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "加工单";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(11, 55);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(72, 27);
            this.label3.TabIndex = 1;
            this.label3.Text = "单号：";
            // 
            // tb_ProcessNo
            // 
            this.tb_ProcessNo.Location = new System.Drawing.Point(86, 52);
            this.tb_ProcessNo.Name = "tb_ProcessNo";
            this.tb_ProcessNo.Size = new System.Drawing.Size(276, 34);
            this.tb_ProcessNo.TabIndex = 0;
            this.tb_ProcessNo.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tb_ProcessNo_KeyPress);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.cb_SelectPrinter);
            this.groupBox3.Font = new System.Drawing.Font("微软雅黑", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134)));
            this.groupBox3.Location = new System.Drawing.Point(12, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(388, 90);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "选择打印机";
            // 
            // cb_SelectPrinter
            // 
            this.cb_SelectPrinter.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cb_SelectPrinter.FormattingEnabled = true;
            this.cb_SelectPrinter.Location = new System.Drawing.Point(31, 33);
            this.cb_SelectPrinter.Name = "cb_SelectPrinter";
            this.cb_SelectPrinter.Size = new System.Drawing.Size(331, 35);
            this.cb_SelectPrinter.TabIndex = 7;
            this.cb_SelectPrinter.SelectedIndexChanged += new System.EventHandler(this.cb_SelectPrinter_SelectedIndexChanged);
            // 
            // PrintFrameProcessCertificateForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(412, 402);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintFrameProcessCertificateForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "框架加工合格证打印";
            this.Load += new System.EventHandler(this.PrintFrameProcessCertificateForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_AccountNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lb_RealName;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox tb_ProcessNo;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.ComboBox cb_SelectPrinter;
    }
}