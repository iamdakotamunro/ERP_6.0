using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading;
using System.Windows.Forms;
using ERP.Model.Goods;
using PrintCertificateBarcode.Core;
using PrintCertificateBarcode.Core.Common;
using PrintCertificateBarcode.Core.Draw;
using PrintCertificateBarcode.Core.Model;
using PrintCertificateBarcode.Core.SAL;

namespace PrintCertificateBarcode.UIForm
{
    public partial class GlassBarcodePrintForm : Form
    {
        #region -- Init
        private Image _currentImage;
        private readonly StockManager _stockManager = new StockManager();

        public GlassBarcodePrintForm()
        {
            InitializeComponent();
        }
        #endregion

        private void btn_Print_Click(object sender, EventArgs e)
        {
            if (cb_SelectLabelType.SelectedValue == null)
            {
                MessageBox.Show(@"请选择打印条码标签类型！", @"打印提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if (string.IsNullOrEmpty(tb_InputKey.Text.Trim()))
            {
                MessageBox.Show(@"请选输入需要打印的单据号或者商品编号！", @"打印提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else if (string.IsNullOrEmpty(cb_SelectPrinter.Text))
            {
                MessageBox.Show(@"请选择条码打印机！", @"打印提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                string inputStr = tb_InputKey.Text.Trim();
                string labelType = cb_SelectLabelType.SelectedValue.ToString();
                string printer = cb_SelectPrinter.Text;
                new Thread(() => StartPrintBarcode(labelType, inputStr, printer)).Start();
            }
        }

        private void btn_PrintView_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(tb_InputKey.Text.Trim()))
            {
                MessageBox.Show(@"请选输入需要打印预览的商品编号", @"打印提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
            }
            else
            {
                string goodscode = tb_InputKey.Text.Trim();
                GoodsBarcodeInfo goodsBarcodeInfo = _stockManager.GetGoodsBarcodeInfo(goodscode);
                if (goodsBarcodeInfo != null)
                {
                    DrawBarcodeImage(goodscode, goodsBarcodeInfo.GoodsName, goodsBarcodeInfo.SellPrice);
                    if (_currentImage == null) return;
                    new PrintReview(_currentImage).ShowDialog();
                }
                else
                {
                    MessageBox.Show(@"未能获取商品信息", @"打印提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }

        private void cb_SelectLabelType_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_PrintView.Enabled = cb_SelectLabelType.SelectedValue.ToString() == "1";
        }

        private void cb_SelectPrinter_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_Print.Enabled = !string.IsNullOrEmpty(cb_SelectPrinter.Text.Trim());
        }

        private void DrawBarcodeImage(string goodsCode, string goodsName, string sellPrice)
        {
            IDrawLabel draw;
            if (radioButton_KEDE.Checked)
            {
                draw = new DrawShopKedePriceLabel(39,43,300);
            }
            else if (radioButton_KD.Checked)
            {
                draw = new DrawShopKDPriceLabel(65, 19, 300);
            }
            else
            {
                MessageBox.Show(@"请选择打印店面模版，是KD还是Kede", @"打印提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            var goodsInfo = new DrawGoodsInfo
            {
                GoodsCode = goodsCode,
                GoodsName = goodsName,
                SellPrice = sellPrice
            };
            Image image = draw.DrawToImage(goodsInfo);
            _currentImage = image;
        }

        private void GlassInStockBarcodePrintForm_Load(object sender, EventArgs e)
        {
            InitLabelType();
            InitPrinter();
        }

        private void InitLabelType()
        {
            var itemArray2 = new CombBoxDropDownItem[2];
            var item = new CombBoxDropDownItem
            {
                DisplayField = "出库单号",
                ValueField = 2
            };
            itemArray2[0] = item;
            var item2 = new CombBoxDropDownItem
            {
                DisplayField = "商品编号",
                ValueField = 1
            };
            itemArray2[1] = item2;
            CombBoxDropDownItem[] itemArray = itemArray2;
            cb_SelectLabelType.ValueMember = "ValueField";
            cb_SelectLabelType.DisplayMember = "DisplayField";
            cb_SelectLabelType.DataSource = itemArray;
        }

        private void InitPrinter()
        {
            foreach (object obj2 in DrawTools.PrinterCollection)
            {
                cb_SelectPrinter.Items.Add(obj2);
            }
        }

        private IList<GoodsBarcodeInfo> ReadGoodsBarcode(string labelType, string tradeNo)
        {
            if (labelType == "2")
            {
                return _stockManager.GetGoodsBarcodeListByOutStock(tradeNo);
            }
            return null;
        }

        private void StartPrintBarcode(string labelType, string txtString, string printer)
        {
            IDrawLabel draw;
            if (radioButton_KEDE.Checked)
            {
                draw = new DrawShopKedePriceLabel(39, 43, 300);
            }
            else if (radioButton_KD.Checked)
            {
                draw = new DrawShopKDPriceLabel(65, 19, 300);
            }
            else
            {
                MessageBox.Show(@"请选择打印店面模版，是KD还是Kede", @"打印提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                return;
            }
            if (labelType == "2")
            {
                IList<GoodsBarcodeInfo> goodsBarcodeList = ReadGoodsBarcode(labelType, txtString);
                if ((goodsBarcodeList == null) || (goodsBarcodeList.Count == 0))
                {
                    MessageBox.Show(@"输入的单据号没有相关的出库商品信息！", @"打印提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
                else
                {
                    var service = new GlassGoodsLabelPrintService(printer, goodsBarcodeList);
                    service.StartPrintMany(draw);
                }
            }
            else if (labelType == "1")
            {
                string goodscode = tb_InputKey.Text.Trim();
                GoodsBarcodeInfo goodsBarcodeInfo = _stockManager.GetGoodsBarcodeInfo(goodscode);
                if (goodsBarcodeInfo != null)
                {
                    new GlassGoodsLabelPrintService(printer).PrintGoods(draw, goodsBarcodeInfo);
                }
                else
                {
                    MessageBox.Show("未能获取商品信息", "打印提示", MessageBoxButtons.OK, MessageBoxIcon.Hand);
                }
            }
        }


    }
}
