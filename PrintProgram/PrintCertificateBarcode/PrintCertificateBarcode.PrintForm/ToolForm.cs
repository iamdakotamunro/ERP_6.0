using System;
using System.Drawing.Printing;
using System.Windows.Forms;
using PrintLabel.Common;

namespace PrintLabel.WinForm
{
    public partial class ToolForm : Form
    {
        private int LabelWidth;
        private int LabelHeight;

        public ToolForm()
        {
            InitializeComponent();
        }

        private void ToolForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            pictureBox1.Width = LabelWidth;
            pictureBox1.Height = LabelHeight;
            pictureBox1.Image = DrawTools.DrawBarcodeImage(textBox_Label.Text, LabelWidth, LabelHeight,2,true);
        }

        private void textBox_Width_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(label_width.Text))
            {
                LabelWidth = (int)DrawTools.Millimeter2Pix(float.Parse(textBox_Width.Text),192f);
                //label_width.Text = LabelWidth.ToString();
            }
        }

        private void textBox_Height_TextChanged(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(label_height.Text))
            {
                LabelHeight = (int)DrawTools.Millimeter2Pix(float.Parse(textBox_Height.Text),192f);
                //label_height.Text = LabelHeight.ToString();
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            var printDoc = new PrintDocument();
            printDoc.PrintPage+=printDoc_PrintPage;
            printDoc.Print();
        }

        private void printDoc_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(pictureBox1.Image,3,3,(int)DrawTools.Millimeter2Pix(float.Parse(textBox_Width.Text)),(int)DrawTools.Millimeter2Pix(float.Parse(textBox_Height.Text)));
        }
    }
}
