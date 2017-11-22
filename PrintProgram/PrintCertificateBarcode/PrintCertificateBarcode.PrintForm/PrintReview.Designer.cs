namespace PrintCertificateBarcode.UIForm
{
    partial class PrintReview
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintReview));
            this.pb_PrintView = new System.Windows.Forms.PictureBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.Menu_Operation = new System.Windows.Forms.ToolStripMenuItem();
            this.Menu_Print = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.pb_PrintView)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pb_PrintView
            // 
            this.pb_PrintView.BackColor = System.Drawing.Color.White;
            this.pb_PrintView.Location = new System.Drawing.Point(12, 50);
            this.pb_PrintView.Name = "pb_PrintView";
            this.pb_PrintView.Size = new System.Drawing.Size(116, 252);
            this.pb_PrintView.TabIndex = 0;
            this.pb_PrintView.TabStop = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Operation});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(273, 25);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // Menu_Operation
            // 
            this.Menu_Operation.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Menu_Print});
            this.Menu_Operation.Name = "Menu_Operation";
            this.Menu_Operation.Size = new System.Drawing.Size(44, 21);
            this.Menu_Operation.Text = "操作";
            // 
            // Menu_Print
            // 
            this.Menu_Print.Name = "Menu_Print";
            this.Menu_Print.Size = new System.Drawing.Size(152, 22);
            this.Menu_Print.Text = "打印";
            this.Menu_Print.Click += new System.EventHandler(this.Menu_Print_Click);
            // 
            // PrintReview
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(273, 357);
            this.Controls.Add(this.pb_PrintView);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PrintReview";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "条码打印预览";
            this.Load += new System.EventHandler(this.ReviewPrint_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pb_PrintView)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb_PrintView;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem Menu_Operation;
        private System.Windows.Forms.ToolStripMenuItem Menu_Print;
    }
}