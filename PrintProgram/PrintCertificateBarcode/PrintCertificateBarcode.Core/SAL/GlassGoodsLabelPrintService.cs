using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Printing;
using ERP.Model.Goods;
using PrintCertificateBarcode.Core.Common;
using PrintCertificateBarcode.Core.Draw;

namespace PrintCertificateBarcode.Core.SAL
{
    public class GlassGoodsLabelPrintService
    {
        public GlassGoodsLabelPrintService(string printer)
        {
            PrinterName = printer;
        }

        public GlassGoodsLabelPrintService(string printer, IList<GoodsBarcodeInfo> goodsBarcodeList)
        {
            PrinterName = printer;
            GoodsBarcodeList = goodsBarcodeList;
        }

        private void Print_PrintPage(object sender, PrintPageEventArgs e)
        {
            var document = sender as GlassGoodsPrintDocument;
            if (document != null)
            {
                GoodsBarcodeInfo currentGoodsBarcodeInfo = document.CurrentGoodsBarcodeInfo;
                var goodsInfo = new DrawGoodsInfo
                {
                    GoodsCode = currentGoodsBarcodeInfo.GoodsCode,
                    GoodsName = currentGoodsBarcodeInfo.GoodsName,
                    SellPrice = currentGoodsBarcodeInfo.SellPrice
                };
                Image image = document.DrawLabel.DrawToImage(goodsInfo);
                e.Graphics.DrawImage(image, DrawTools.Millimeter2Pix(1f), DrawTools.Millimeter2Pix(1f), DrawTools.Millimeter2Pix(document.DrawLabel.Width), DrawTools.Millimeter2Pix(document.DrawLabel.Height));
            }
        }

        public void PrintGoods(IDrawLabel drawLabel, GoodsBarcodeInfo goods)
        {
            var document = new GlassGoodsPrintDocument(goods, drawLabel)
            {
                PrinterSettings = { PrinterName = PrinterName }
            };
            PrintController printController = new StandardPrintController();
            document.PrintController = printController;
            document.PrintPage += Print_PrintPage;
            document.Print();
        }

        public void StartPrintMany(IDrawLabel drawLabel)
        {
            foreach (GoodsBarcodeInfo info in GoodsBarcodeList)
            {
                PrintGoods(drawLabel, info);
            }
        }

        public IList<GoodsBarcodeInfo> GoodsBarcodeList { get; private set; }

        public string PrinterName { get; private set; }

        public class GlassGoodsPrintDocument : PrintDocument
        {
            public GlassGoodsPrintDocument(GoodsBarcodeInfo goodsBarcodeInfo, IDrawLabel drawLabel)
            {
                CurrentGoodsBarcodeInfo = goodsBarcodeInfo;
                DrawLabel = drawLabel;
            }

            public IDrawLabel DrawLabel { get; private set; }

            public GoodsBarcodeInfo CurrentGoodsBarcodeInfo { get; private set; }
        }
    }
}

