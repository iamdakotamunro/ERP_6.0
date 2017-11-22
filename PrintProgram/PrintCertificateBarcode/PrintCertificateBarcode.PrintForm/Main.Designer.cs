namespace PrintCertificateBarcode.UIForm
{
    partial class Main
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_CertificateLabelPrint = new System.Windows.Forms.Button();
            this.btn_GlassInStockBarcodePrint = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.tssl_StatusTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btn_CertificateLabelPrint);
            this.groupBox2.Location = new System.Drawing.Point(15, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(154, 57);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "标签打印";
            // 
            // btn_CertificateLabelPrint
            // 
            this.btn_CertificateLabelPrint.Location = new System.Drawing.Point(14, 20);
            this.btn_CertificateLabelPrint.Name = "btn_CertificateLabelPrint";
            this.btn_CertificateLabelPrint.Size = new System.Drawing.Size(126, 23);
            this.btn_CertificateLabelPrint.TabIndex = 1;
            this.btn_CertificateLabelPrint.Text = "合格证标签打印";
            this.btn_CertificateLabelPrint.UseVisualStyleBackColor = true;
            this.btn_CertificateLabelPrint.Click += new System.EventHandler(this.btn_CertificateLabelPrint_Click);
            // 
            // btn_GlassInStockBarcodePrint
            // 
            this.btn_GlassInStockBarcodePrint.Location = new System.Drawing.Point(14, 20);
            this.btn_GlassInStockBarcodePrint.Name = "btn_GlassInStockBarcodePrint";
            this.btn_GlassInStockBarcodePrint.Size = new System.Drawing.Size(126, 23);
            this.btn_GlassInStockBarcodePrint.TabIndex = 0;
            this.btn_GlassInStockBarcodePrint.Text = "门店框架价格打印";
            this.btn_GlassInStockBarcodePrint.UseVisualStyleBackColor = true;
            this.btn_GlassInStockBarcodePrint.Click += new System.EventHandler(this.btn_GlassInStockBarcodePrint_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_GlassInStockBarcodePrint);
            this.groupBox1.Location = new System.Drawing.Point(15, 116);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(154, 59);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "条码打印";
            // 
            // statusStrip1
            // 
            this.statusStrip1.AutoSize = false;
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tssl_StatusTime});
            this.statusStrip1.Location = new System.Drawing.Point(0, 206);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.statusStrip1.Size = new System.Drawing.Size(181, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // tssl_StatusTime
            // 
            this.tssl_StatusTime.Name = "tssl_StatusTime";
            this.tssl_StatusTime.Size = new System.Drawing.Size(0, 17);
            // 
            // timer1
            // 
            this.timer1.Interval = 1000;
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(181, 228);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox2);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "标签|条码打印";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.Main_FormClosed);
            this.Load += new System.EventHandler(this.Main_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_GlassInStockBarcodePrint;
        private System.Windows.Forms.Button btn_CertificateLabelPrint;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel tssl_StatusTime;
        private System.Windows.Forms.Timer timer1;
    }
}

