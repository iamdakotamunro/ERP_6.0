namespace PrintLabel.WinForm
{
    partial class ToolForm
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
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox_Label = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox_Width = new System.Windows.Forms.TextBox();
            this.textBox_Height = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.button2 = new System.Windows.Forms.Button();
            this.label_width = new System.Windows.Forms.Label();
            this.label_height = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.Color.White;
            this.pictureBox1.Location = new System.Drawing.Point(32, 93);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(294, 108);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 50);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "输入条码：";
            // 
            // textBox_Label
            // 
            this.textBox_Label.Location = new System.Drawing.Point(83, 46);
            this.textBox_Label.Name = "textBox_Label";
            this.textBox_Label.Size = new System.Drawing.Size(128, 21);
            this.textBox_Label.TabIndex = 2;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(238, 17);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(50, 23);
            this.button1.TabIndex = 3;
            this.button1.Text = "预览";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox_Width
            // 
            this.textBox_Width.Location = new System.Drawing.Point(85, 19);
            this.textBox_Width.Name = "textBox_Width";
            this.textBox_Width.Size = new System.Drawing.Size(25, 21);
            this.textBox_Width.TabIndex = 5;
            this.textBox_Width.TextChanged += new System.EventHandler(this.textBox_Width_TextChanged);
            // 
            // textBox_Height
            // 
            this.textBox_Height.Location = new System.Drawing.Point(152, 19);
            this.textBox_Height.Name = "textBox_Height";
            this.textBox_Height.Size = new System.Drawing.Size(25, 21);
            this.textBox_Height.TabIndex = 6;
            this.textBox_Height.TextChanged += new System.EventHandler(this.textBox_Height_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 22);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(59, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "高宽(M)：";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label_height);
            this.groupBox1.Controls.Add(this.label_width);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.textBox_Label);
            this.groupBox1.Controls.Add(this.textBox_Height);
            this.groupBox1.Controls.Add(this.textBox_Width);
            this.groupBox1.Location = new System.Drawing.Point(32, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(294, 75);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(238, 44);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(50, 23);
            this.button2.TabIndex = 8;
            this.button2.Text = "打印";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // label_width
            // 
            this.label_width.AutoSize = true;
            this.label_width.Location = new System.Drawing.Point(113, 27);
            this.label_width.Name = "label_width";
            this.label_width.Size = new System.Drawing.Size(11, 12);
            this.label_width.TabIndex = 9;
            this.label_width.Text = "-";
            // 
            // label_height
            // 
            this.label_height.AutoSize = true;
            this.label_height.Location = new System.Drawing.Point(181, 27);
            this.label_height.Name = "label_height";
            this.label_height.Size = new System.Drawing.Size(11, 12);
            this.label_height.TabIndex = 10;
            this.label_height.Text = "-";
            // 
            // ToolForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(461, 221);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.pictureBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ToolForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = " 条码预览";
            this.Load += new System.EventHandler(this.ToolForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox_Label;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox_Width;
        private System.Windows.Forms.TextBox textBox_Height;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label label_height;
        private System.Windows.Forms.Label label_width;

    }
}