using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace PrintCertificateBarcode.UIForm
{
    public partial class PrintReview : Form
    {
        private readonly Image _printImage;

        public PrintReview(Image img, float width, float height)
        {
            InitializeComponent();
            _printImage = img;

            pb_PrintView.Parent.Width = (int) width + 30;
            pb_PrintView.Parent.Height = (int)height + 100 + menuStrip1.Height;
            StartPosition = FormStartPosition.CenterParent;
        }

        public PrintReview(Image img)
        {
            if (img == null) return;
            InitializeComponent();
            _printImage = img;
            pb_PrintView.Parent.Width = img.Width + 30;
            pb_PrintView.Parent.Height = img.Height + 100 + menuStrip1.Height;
            StartPosition = FormStartPosition.CenterParent;
        }

        private void ReviewPrint_Load(object sender, EventArgs e)
        {
            Location = new Point(10, 10);
            pb_PrintView.Width = _printImage.Width;
            pb_PrintView.Height = _printImage.Height;
            pb_PrintView.Image = _printImage;
        }

        private void Menu_Print_Click(object sender, EventArgs e)
        {
            var docPrint = new PrintDocument();
            var printDialog = new PrintDialog { Document = docPrint };
            printDialog.Document.PrintPage += Document_PrintPage;
            
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                docPrint.Print();
            }
        }

        private void Document_PrintPage(object sender, PrintPageEventArgs e)
        {
            e.Graphics.DrawImage(pb_PrintView.Image, 0, 0);
        }
    }
}
