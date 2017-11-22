using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Fath;
using PrintCertificateBarcode.Core.Common;
using System.Reflection.Emit;

namespace PrintCertificateBarcode.Core.Draw
{
    public class DrawShopKDPriceLabel : IDrawLabel
    {
        public DrawShopKDPriceLabel(int width, int height, float dpi)
        {
            Width = width;
            Height = height;
            DPI = dpi;
        }

        public int Width { get; private set; }
        public int Height { get; private set; }
        public float DPI { get; private set; }

        /// <summary>
        /// 绘制框架条码
        /// </summary>
        /// <param name="drawGoodsInfo"></param>
        /// <returns></returns>
        public Image DrawToImage(DrawGoodsInfo drawGoodsInfo)
        {
            var barcodeText = drawGoodsInfo.GoodsCode.ToUpper();
            int barWidth = DrawTools.Millimeter2Pix(Width, DPI);
            int barHeight = DrawTools.Millimeter2Pix(Height, DPI);
            var bitmap = new Bitmap(barWidth, barHeight, PixelFormat.Format32bppPArgb);
            bitmap.SetResolution(DPI, DPI);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);

            //条码对象
            var ex2 = new CBarcodeX
            {
                Data = barcodeText,
                ShowText = false,
                Symbology = bcType.Code128,
                Font = new Font("Arial", 12)
            };
            var barcodeImage = ex2.Image(barWidth / 2 - DrawTools.Millimeter2Pix(4, DPI), DrawTools.Millimeter2Pix(7, DPI));

            //画左侧价格条码
            int x = DrawTools.Millimeter2Pix(3, DPI);
            DrawText(ref graphics, barcodeImage, drawGoodsInfo, barWidth, x, 0);

            //画右侧价格条码
            x = DrawTools.Millimeter2Pix(Width / 2 + 4, DPI);
            DrawText(ref graphics, barcodeImage, drawGoodsInfo, barWidth, x, barWidth / 2 + 7);

            //int y = DrawTools.Millimeter2Pix(3.5f, DPI);
            //graphics.DrawImage(barcodeImage, x, y, barcodeImage.Width, barcodeImage.Height);
            //y = y + barcodeImage.Height + 5;
            //var font = new Font("微软雅黑", 8f);

            //var stringFormat = new StringFormat
            //{
            //    LineAlignment = StringAlignment.Center,
            //    Alignment = StringAlignment.Center
            //};
            //var printTxt = "产品编号：" + drawGoodsInfo.GoodsCode;
            //var lbRect = new Rectangle(0, y, barWidth / 2, (int)font.GetHeight() + 15);
            //graphics.DrawString(printTxt, font, new SolidBrush(Color.Black), lbRect, stringFormat);
            //y = y + font.Height + 20;
            //lbRect.Y = y;
            //printTxt = "全国统一零售价：" + drawGoodsInfo.SellPrice + "元";
            //graphics.DrawString(printTxt, new Font("微软雅黑", 7f), new SolidBrush(Color.Black), lbRect, stringFormat);


            graphics.Save();
            return bitmap;
        }

        void DrawText(ref Graphics graphics, Image barcodeImage, DrawGoodsInfo drawGoodsInfo, int barWidth, int barcodeX, int textX)
        {
            int y = DrawTools.Millimeter2Pix(2f, DPI);
            graphics.DrawImage(barcodeImage, barcodeX, y, barcodeImage.Width, barcodeImage.Height);
            y = y + barcodeImage.Height + 5;

            var stringFormat = new StringFormat
            {
                LineAlignment = StringAlignment.Center,
                Alignment = StringAlignment.Center
            };
            var printTxt = "产品编号：" + drawGoodsInfo.GoodsCode;
            var font = new Font("微软雅黑", 8f);
            var lbRect = new Rectangle(textX, y, barWidth / 2, (int)font.GetHeight() + 15);
            graphics.DrawString(printTxt, font, new SolidBrush(Color.Black), lbRect, stringFormat);
            y = y + font.Height + 20;
            lbRect.Y = y;
            printTxt = "全国统一零售价：" + drawGoodsInfo.SellPrice.Split('.')[0] + "元";
            graphics.DrawString(printTxt, new Font("微软雅黑", 7f), new SolidBrush(Color.Black), lbRect, stringFormat);
        }

    }
}
