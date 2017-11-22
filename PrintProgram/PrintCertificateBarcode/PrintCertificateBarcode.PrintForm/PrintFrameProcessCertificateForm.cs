using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Threading;
using System.Windows.Forms;
using Framework.Common;
using PrintCertificateBarcode.Core;
using PrintCertificateBarcode.Core.Common;
using PrintCertificateBarcode.Core.Draw;
using PrintCertificateBarcode.Core.Model;

namespace PrintCertificateBarcode.UIForm
{
    public partial class PrintFrameProcessCertificateForm : Form
    {
        private static Image _printImage;
        private static string _printer;
        private static Guid _personnelId;

        public PrintFrameProcessCertificateForm()
        {
            InitializeComponent();
            tb_AccountNo.MouseEnter += (o, e) => tb_AccountNo.SelectAll();
            tb_ProcessNo.MouseEnter += (o, e) => tb_ProcessNo.SelectAll();
            Icon = Core.Resources.Icon.Tag;
        }

        private void tb_AccountNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                ThreadPool.QueueUserWorkItem(wk => ReadName());
            }
        }

        private void ReadName()
        {
            if (tb_AccountNo.InvokeRequired)
            {
                var act = new Action(InputRealName);
                Invoke(act);
            }
            else
            {
                InputRealName();
            }
        }

        private void InputRealName()
        {
            tb_AccountNo.Enabled = false;
            var accountNo = tb_AccountNo.Text.Trim();
            if (!accountNo.IsNumericString())
            {
                tb_AccountNo.Enabled = true;
                MessageBox.Show(@"输入工号不对！");
                tb_AccountNo.Focus();
                tb_AccountNo.SelectAll();
                tb_ProcessNo.Enabled = false;
            }
            else
            {
                var realName = PersonnelManager.GetRealNameByAccountNo(accountNo, out _personnelId);
                if (realName == string.Empty)
                {
                    tb_AccountNo.Enabled = true;
                    MessageBox.Show(@"未找到此工号的员工！");
                    tb_AccountNo.Focus();
                    tb_AccountNo.SelectAll();
                    lb_RealName.Text = string.Empty;
                    tb_ProcessNo.Enabled = false;
                }
                else
                {
                    lb_RealName.Text = realName;
                    tb_AccountNo.Enabled = true;
                    tb_ProcessNo.Enabled = true;
                    tb_ProcessNo.Focus();
                }
            }
        }

        private void PrintFrameProcessCertificateForm_Load(object sender, EventArgs e)
        {
            InitPrinter();
            tb_AccountNo.Focus();
            tb_ProcessNo.Enabled = false;
        }

        private void InitPrinter()
        {
            foreach (object obj2 in DrawTools.PrinterCollection)
            {
                cb_SelectPrinter.Items.Add(obj2);
            }
        }

        private void tb_ProcessNo_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                var processNo = tb_ProcessNo.Text.Trim();
                if (processNo.IsNullOrEmpty())
                {
                    MessageBox.Show(@"请扫描加工单号！");
                    tb_ProcessNo.Focus();
                    tb_ProcessNo.SelectAll();
                }
                else
                {
                    var info = FrameProcessManager.GetCertificateInfo(processNo);
                    info.ProcessNo = processNo;
                    if (string.IsNullOrEmpty(info.OrderNo))
                    {
                        info.OrderNo = processNo.Split('-')[0];
                    }
                    var realName = lb_RealName.Text;
                    if (realName != string.Empty && !info.OrderNo.IsNullOrEmpty() || !info.Optician.IsNullOrEmpty())
                    {
                        ThreadPool.QueueUserWorkItem(wk => PrintCertificate(info, realName));
                        tb_ProcessNo.Focus();
                        tb_ProcessNo.SelectAll();
                    }
                    else
                    {
                        if (info.FrameGoodsName.IsNullOrEmpty() || info.LeftEyeInfo.IsNullOrEmpty())
                        {
                            MessageBox.Show(@"加工单号数据有异常！");
                            tb_ProcessNo.Focus();
                            tb_ProcessNo.SelectAll();
                        }
                        else if (info.Optician.IsNullOrEmpty())
                        {
                            MessageBox.Show(@"此加工单没有配镜师信息！");
                            tb_ProcessNo.Focus();
                            tb_ProcessNo.SelectAll();
                        }
                    }
                }
            }
        }

        private void PrintCertificate(ERP.Model.FrameProcessCertificateInfo info, string checker)
        {
            var orderLabelInfo = new OrderLabelInfo
            {
                OrderNo = info.OrderNo,
                FrameGoodsName = info.FrameGoodsName,
                GlassGoodsName = info.GlassGoodsName,
                RightEyeInfo = info.RightEyeInfo,
                LeftEyeInfo = info.LeftEyeInfo
            };

            var logoName = @"kede";
            if (AppSetting.JingTuoSaleFilialeID.Equals(info.SaleFilialeID))
            {
                logoName = @"han";
            }

            //打印镜片
            _printImage = DrawCertificateLabel.DrawToOpticImage(AppSetting.Label.With, AppSetting.Label.Height, AppSetting.Label.DPI, info.Optician, info.OperationTime, orderLabelInfo, checker, logoName);
            var docPrint = new PrintDocument
                {
                    PrinterSettings = { PrinterName = _printer }
                };
            docPrint.PrintPage += Document_PrintPage;
            docPrint.Print();

            //记录工作日志
            FrameProcessManager.AddCheckGlassOperation(_personnelId, lb_RealName.Text,info.OrderId, tb_ProcessNo.Text.Trim());
        }


        private void Document_PrintPage(object sender, PrintPageEventArgs e)
        {
            if (_printImage != null)
            {
                e.Graphics.DrawImage(_printImage, 0, 0);
            }
        }

        private void cb_SelectPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            tb_AccountNo.Enabled = !string.IsNullOrEmpty(cb_SelectPrinter.Text.Trim());
            _printer = cb_SelectPrinter.Text;
        }
    }
}
