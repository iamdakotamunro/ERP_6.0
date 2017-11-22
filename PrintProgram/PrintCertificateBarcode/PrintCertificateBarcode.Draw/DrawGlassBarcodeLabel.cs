using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using Fath;
using Framework.Common.Barcode;
using PrintLabel.Common;

namespace PrintLabel.Draw
{
    public class DrawGlassBarcodeLabel
    {
        /// <summary>
        /// 绘制框架条码
        /// </summary>
        /// <param name="barcodeText"></param>
        /// <param name="brandName"></param>
        /// <param name="marketPrice"></param>
        /// <param name="sellPrice"></param>
        /// <param name="origin"></param>
        /// <param name="width">宽度，毫米</param>
        /// <param name="height">高度，毫米</param>
        /// <param name="barcodeWeight">条码尺寸，最小1个单位</param>
        /// <param name="barType"> </param>
        /// <returns></returns>
        public static Image DrawToImage(string barcodeText, string brandName, string marketPrice, string sellPrice, string origin, int width, int height, int barcodeWeight, int barType)
        {
            barcodeText = barcodeText.ToUpper();
            Image image;
            int barWidth = DrawTools.Millimeter2Pix(width) * barcodeWeight;
            int barHeight = DrawTools.Millimeter2Pix(height) * barcodeWeight;
            var bitmap = new Bitmap(barWidth, barHeight, PixelFormat.Format32bppPArgb);
            bitmap.SetResolution(DrawTools.CURRENT_DPI * barcodeWeight, DrawTools.CURRENT_DPI * barcodeWeight);
            Graphics graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.White);
            if (barType > 0)
            {
                var ex2 = new CBarcodeX
                {
                    Data = barcodeText,
                    ShowText = true,
                    Symbology = (bcType)barType,
                    Font = new Font("Arial", 12)
                };
                image = ex2.Image(barWidth / 2, barHeight / 2);
            }
            else
            {
                image = Code128Rendering.MakeBarcodeImage(barcodeText, barcodeWeight, 0.1f * barcodeWeight, true, true);
            }
            int x = (image.Width < (barWidth / 2)) ? (((barWidth / 2) - image.Width) / 2) : 0;
            int y = (image.Height < barHeight) ? (((barHeight - image.Height) / 2) + (5 * barcodeWeight)) : 0;
            //int num5 = image.Width;
            //int num6 = image.Height;
            if (barType > 0)
            {
                x = 15;
                y = 5 * barcodeWeight;
            }
            var font = new Font("微软雅黑", 8f);
            int leftY = y + 5;
            graphics.DrawString("建议零售价：" + marketPrice, font, new SolidBrush(Color.Black), new PointF(x, leftY));
            leftY += font.Height + 15;
            graphics.DrawImage(image, x, leftY, image.Width, image.Height);
            //leftY += image.Height;
            //graphics.DrawString(barcodeText, new Font("Arial", 8), new SolidBrush(Color.Black), new PointF(x, leftY));
            IList<string> list = DrawTools.ProcessStringLines(brandName);
            int num7 = barWidth / 2;
            float rightY = y;
            var font2 = new Font("微软雅黑", 8f);
            foreach (string str in list)
            {
                graphics.DrawString("品牌：" + str, font2, new SolidBrush(Color.Black), new PointF(num7 + DrawTools.Millimeter2Pix(10f), rightY));
                rightY += font2.Height + (5 * barcodeWeight);
            }
            if (origin != string.Empty)
            {
                graphics.DrawString("产地：" + origin, font2, new SolidBrush(Color.Black),
                                    new PointF(num7 + DrawTools.Millimeter2Pix(10f), rightY));
                rightY += font2.Height + (5 * barcodeWeight);
            }
            else
            {
                rightY += 3;
            }
            var font3 = new Font("微软雅黑", 9f, FontStyle.Bold);
            graphics.DrawString("可得价：" + sellPrice, font3, new SolidBrush(Color.Black), new PointF(num7 + DrawTools.Millimeter2Pix(10f), rightY));
            graphics.Save();
            return bitmap;
        }

    }
}
