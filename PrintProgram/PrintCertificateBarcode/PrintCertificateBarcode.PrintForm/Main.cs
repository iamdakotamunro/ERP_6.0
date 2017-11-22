using System;
using System.Windows.Forms;

namespace PrintCertificateBarcode.UIForm
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            Icon = Core.Resources.Icon.Tag;
            //OperationLog.Core.Init.InitSyncConnectionName("");
            timer1.Tick += (o,e) => RefreshTime();
            tssl_StatusTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        private void btn_GlassInStockBarcodePrint_Click(object sender, EventArgs e)
        {
            new GlassBarcodePrintForm().ShowDialog();
        }

        private void RefreshTime()
        {
            tssl_StatusTime.Text = DateTime.Now.ToString("HH:mm:ss");
        }

        //private void brn_Tool_Click(object sender, EventArgs e)
        //{
        //    new ToolForm().Show();
        //}

        private void btn_CertificateLabelPrint_Click(object sender, EventArgs e)
        {
            PrintFrameProcessCertificateForm form = new PrintFrameProcessCertificateForm();
            form.ShowDialog();

            //PrintCertificate(new ERP.Model.FrameProcessCertificateInfo
            //    {
            //         FrameGoodsName="框架眼镜",
            //          GlassGoodsName="镜片",
            //           LeftEyeInfo="RRRRRRRRRRRRRRRRRRRR",
            //           RightEyeInfo ="lllllllllllllllllll",
            //            OperationTime=DateTime.Now,
            //             Optician="阮剑锋",
            //              OrderNo="RT12345688",
            //               ProcessNo="LP124575424"
            //    }, "阮剑锋");

        }

        //private void PrintCertificate(ERP.Model.FrameProcessCertificateInfo info, string checker)
        //{
        //    const float DPI = 300f;
        //    const int WIDTH = 38;
        //    const int HEIGHT = 125;
        //    var orderLabelInfo = new OrderLabelInfo
        //    {
        //        OrderNo = info.OrderNo,
        //        FrameGoodsName = info.FrameGoodsName,
        //        GlassGoodsName = info.GlassGoodsName,
        //        RightEyeInfo = info.RightEyeInfo,
        //        LeftEyeInfo = info.LeftEyeInfo
        //    };
        //    var img = DrawCertificateLabel.DrawToImage(WIDTH, HEIGHT, DPI, info.Optician, info.OperationTime, orderLabelInfo, checker);
        //    var view = new PrintReview(img) { StartPosition = FormStartPosition.CenterScreen };
        //    view.ShowDialog();
        //}

        private void Main_Load(object sender, EventArgs e)
        {
            tssl_StatusTime.Text = DateTime.Now.ToString("HH:mm:ss");
            timer1.Start();
        }

        private void Main_FormClosed(object sender, FormClosedEventArgs e)
        {
            timer1.Stop();
        }
    }
}
