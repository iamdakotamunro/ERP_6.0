using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Fath;
using PrintCertificateBarcode.Core.Common;

namespace PrintCertificateBarcode.Core.Draw
{
    public class DrawShopKedePriceLabel : IDrawLabel
    {
        #region -- 公用方法
        private static void DrawString(Graphics graphics, string text, Font font, Color color, int x, int y)
        {
            graphics.DrawString(text, font, new SolidBrush(color), new PointF(x, y));
        }

        private static void DrawImage(Graphics graphics, Bitmap bitmap, int x, int y)
        {
            graphics.DrawImage(bitmap, x, y, bitmap.Width, bitmap.Height);
        }
        #endregion

        public DrawShopKedePriceLabel(int width,int height,float dpi)
        {
            Width = width;
            Height = height;
            DPI = dpi;
        }

        #region -- 打印镜片

        public int Width { get; private set; }
        public int Height { get; private set; }
        public float DPI { get; private set; }

        /// <summary>
        /// 打印镜片标签图片
        /// </summary>
        /// <param name="drawGoodsInfo">需打印的商品信息</param>
        /// <returns></returns>
        public Image DrawToImage(DrawGoodsInfo drawGoodsInfo)
        {
            float bs = DPI / DrawTools.CURRENT_DPI;
            var widthPX = DrawTools.Millimeter2Pix(Width, DPI);
            var heightPX = DrawTools.Millimeter2Pix(Height, DPI);
            var allBitmap = new Bitmap(widthPX, heightPX, PixelFormat.Format32bppPArgb);
            Graphics graphics = Graphics.FromImage(allBitmap);
            allBitmap.SetResolution(DPI, DPI);
            graphics.Clear(Color.White);
            var y = 1;
            const int X = 0;


            // 间隔空隙两张
            //y += DrawTools.Millimeter2Pix(50,dpi);

            //绘画条码
            var barcodeLabel = DrawBarcodeLabel(Width, drawGoodsInfo.GoodsCode, DPI, bs);
            DrawImage(graphics, barcodeLabel, X, y);

            //下一张间隔
            //y += opticianLabel.Height-14;
            y += DrawTools.Millimeter2Pix((float)14.5, DPI);

            //绘画订单信息标签
            var orderLabel = DrawPriceLabel(Width, drawGoodsInfo.GoodsName, drawGoodsInfo.SellPrice, DPI, bs);
            DrawImage(graphics, orderLabel, X, y);

            ////下一张间隔
            //y += orderLabel.Height;
            //y += DrawTools.Millimeter2Pix(5, dpi);

            //绘画检验师
            //var checkLabel = DrawChecker(checker, orderLabelInfo.OrderNo, dpi, bs);
            //DrawImage(graphics, checkLabel, X, y);

            //下一张间隔
            //y += checkLabel.Height;
            //y += DrawTools.Millimeter2Pix(5, dpi);

            //同样再绘画检验师
            //DrawImage(graphics, checkLabel, X, y);


            graphics.Save();
            graphics.Dispose();
            return allBitmap;
        }


        private Bitmap DrawBarcodeLabel(int width, string barcodeText, float dpi, float bs)
        {
            var fontCN10 = new Font("微软雅黑", 12 * bs);
            var widthPX = DrawTools.Millimeter2Pix(width, dpi);
            var heightPX = DrawTools.Millimeter2Pix(14, dpi);
            var bitmap = new Bitmap(widthPX, heightPX, PixelFormat.Format32bppPArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            bitmap.SetResolution(dpi, dpi);
            graphics.Clear(Color.White);

            //设定Y坐标
            var y = 5;
            var x = 0;

            var barcode = new CBarcodeX
            {
                Data = barcodeText,
                ShowText = false,
                Symbology = bcType.Code128,
                Font = new Font("Arial", 12)
            };

            //画出条码图片
            var barcodeImage = barcode.Image(widthPX - 10, (heightPX - 40) / 2);

            //开始绘画
            DrawImage(graphics, barcodeImage, x + 10, y);

            //绘画条码字
            y += barcodeImage.Height + 10;
            var txtW = barcodeText.Length * 12 * bs;
            DrawString(graphics, barcodeText, fontCN10, Color.Black, (widthPX - (int)txtW) / 2, y);


            graphics.Save();
            graphics.Dispose();
            return bitmap;
        }

        private static Bitmap DrawPriceLabel(int width, string goodsName, string salePrice, float dpi, float bs)
        {
            var fontCN = new Font("微软雅黑", 9 * bs);
            var fontEN8 = new Font("微软雅黑", 9 * bs);
            var widthPX = DrawTools.Millimeter2Pix(width, dpi);
            var heightPX = DrawTools.Millimeter2Pix(18, dpi);
            var bitmap = new Bitmap(widthPX, heightPX, PixelFormat.Format32bppPArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            bitmap.SetResolution(dpi, dpi);
            graphics.Clear(Color.White);

            //设定Y坐标
            var y = 1;
            var x = DrawTools.Millimeter2Pix(3, dpi);

            //开始绘画
            IList<string> list = DrawTools.ProcessStringLines(goodsName);
            if (list.Count > 2) fontCN = new Font("微软雅黑", 8 * bs);
            foreach (string str in list)
            {
                graphics.DrawString(str, fontCN, new SolidBrush(Color.Black), new PointF(3 + DrawTools.Millimeter2Pix(10f), y));
                y += fontCN.Height;
            }
            DrawString(graphics, "全国统一价格：" + salePrice.Split('.')[0] + "元", fontEN8, Color.Black, x, y);

            //做旋转
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipY);
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
            graphics.Save();
            graphics.Dispose();
            return bitmap;
        }

        #endregion
    }
}
