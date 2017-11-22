using System;
using System.Drawing;
using System.Drawing.Imaging;
using PrintLabel.Common;
using PrintLabel.Model;

namespace PrintLabel.Draw
{
    public class DrawCertificateLabel
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="widthMM"></param>
        /// <param name="heightMM"></param>
        /// <param name="dpi"></param>
        /// <param name="optician">配镜师</param>
        /// <param name="operationTime"></param>
        /// <param name="orderLabelInfo"></param>
        /// <param name="checker">检验师</param>
        /// <returns></returns>
        public static Image DrawToImage(int widthMM, int heightMM, float dpi, string optician,DateTime operationTime, OrderLabelInfo orderLabelInfo, string checker)
        {
            float bs = dpi/DrawTools.CURRENT_DPI;

            var widthPX = DrawTools.Millimeter2Pix(widthMM, dpi);
            var heightPX = DrawTools.Millimeter2Pix(heightMM, dpi);
            var allBitmap = new Bitmap(widthPX, heightPX, PixelFormat.Format32bppPArgb);
            Graphics graphics = Graphics.FromImage(allBitmap);
            allBitmap.SetResolution(dpi, dpi);
            graphics.Clear(Color.White);
            var y = 1;
            const int X = 0;


            // 间隔空隙两张
            y += DrawTools.Millimeter2Pix(50,dpi);

            //绘画配镜师标签
            var opticianLabel = DrawOpticianLabel(optician,operationTime, dpi, bs);
            DrawImage(graphics, opticianLabel, X, y);

            //下一张间隔
            //y += opticianLabel.Height-14;
            y += DrawTools.Millimeter2Pix(14, dpi);

            //绘画订单信息标签
            var orderLabel = DrawOrderLabel(orderLabelInfo, dpi, bs);
            DrawImage(graphics, orderLabel, X, y);

            //下一张间隔
            y += orderLabel.Height;
            y += DrawTools.Millimeter2Pix(5, dpi);

            //绘画检验师
            var checkLabel = DrawChecker(checker, orderLabelInfo.OrderNo, dpi, bs);
            DrawImage(graphics, checkLabel, X, y);

            //下一张间隔
            y += checkLabel.Height;
            //y += DrawTools.Millimeter2Pix(5, dpi);

            //同样再绘画检验师
            DrawImage(graphics, checkLabel, X, y);

            
            graphics.Save();
            graphics.Dispose();
            return allBitmap;
        }

        private static void DrawString(Graphics graphics, string text, Font font, Color color, int x, int y)
        {
            graphics.DrawString(text, font, new SolidBrush(color), new PointF(x, y));
        }

        private static void DrawImage(Graphics graphics, Bitmap bitmap, int x, int y)
        {
            graphics.DrawImage(bitmap,x,y,bitmap.Width,bitmap.Height);
        }

        private static Bitmap DrawOpticianLabel(string optician, DateTime operationTime, float dpi, float bs)
        {
            var fontCN8 = new Font("Arial", 8f * bs);
            var fontEN6 = new Font("Arial", 6f * bs);
            var widthPX = DrawTools.Millimeter2Pix(38, dpi);
            var heightPX = DrawTools.Millimeter2Pix(20, dpi);
            var bitmap = new Bitmap(widthPX, heightPX, PixelFormat.Format32bppPArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            bitmap.SetResolution(dpi, dpi);
            graphics.Clear(Color.White);

            //设定Y坐标
            var y = 3;
            var x = DrawTools.Millimeter2Pix(12, dpi);

            //开始绘画
            DrawString(graphics,"配镜师："+ optician, fontCN8, Color.Black, x, y);
            y += fontCN8.Height;
            DrawString(graphics, operationTime.ToString("yyyy-MM-dd HH:mm"), fontEN6, Color.Black, x, y);

            graphics.Save();
            graphics.Dispose();
            return bitmap;
        }

        private static Bitmap DrawOrderLabel(OrderLabelInfo orderLabelInfo, float dpi, float bs)
        {
            var fontEN = new Font("Arial", 6f * bs);
            //var fontCN = new Font("微软雅黑", 6f * bs);
            var widthPX = DrawTools.Millimeter2Pix(38, dpi);
            var heightPX = DrawTools.Millimeter2Pix(20, dpi);
            var bitmap = new Bitmap(widthPX, heightPX, PixelFormat.Format32bppPArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            bitmap.SetResolution(dpi, dpi);
            graphics.Clear(Color.White);

            //设定Y坐标
            var y = 3;
            var x = DrawTools.Millimeter2Pix(3, dpi);

            //开始绘画
            DrawString(graphics, orderLabelInfo.OrderNo, fontEN, Color.Black, x, y);
            y += fontEN.Height;
            DrawString(graphics, orderLabelInfo.FrameGoodsName, fontEN, Color.Black, x, y);
            y += fontEN.Height;
            DrawString(graphics, orderLabelInfo.GlassGoodsName, fontEN, Color.Black, x, y);
            y += fontEN.Height;
            DrawString(graphics, orderLabelInfo.RightEyeInfo, fontEN, Color.Black, x, y);
            y += fontEN.Height;
            DrawString(graphics, orderLabelInfo.LeftEyeInfo, fontEN, Color.Black, x, y);

            //做旋转
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipY);
            bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
            graphics.Save();
            graphics.Dispose();
            return bitmap;
        }

        private static Bitmap DrawChecker(string checker,string orderNo, float dpi, float bs)
        {
            var fontCN8 = new Font("Arial", 8f * bs);
            var fontEN6 = new Font("Arial", 6f * bs);
            var widthPX = DrawTools.Millimeter2Pix(38, dpi);
            var heightPX = DrawTools.Millimeter2Pix(20, dpi);
            var bitmap = new Bitmap(widthPX, heightPX, PixelFormat.Format32bppPArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            bitmap.SetResolution(dpi, dpi);
            graphics.Clear(Color.White);

            //设定Y坐标
            var y = 2;
            var x = DrawTools.Millimeter2Pix(12, dpi);

            //开始绘画
            DrawString(graphics, "检验师：" + checker, fontCN8, Color.Black, x, y);
            y += fontCN8.Height;
            DrawString(graphics, DateTime.Now.ToString("yyyy-MM-dd HH:mm"), fontEN6, Color.Black, x, y);
            y += fontEN6.Height;
            DrawString(graphics, orderNo, fontEN6, Color.Black, x, y);

            graphics.Save();
            graphics.Dispose();
            return bitmap;
        }
    }
}
