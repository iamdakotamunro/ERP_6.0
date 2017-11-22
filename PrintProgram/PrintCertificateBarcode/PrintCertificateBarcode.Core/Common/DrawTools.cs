using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Printing;
using System.Text;

namespace PrintCertificateBarcode.Core.Common
{
    public class DrawTools
    {
        public const float CURRENT_DPI = 96.0f;

        /// <summary>
        /// 毫米转像素
        /// </summary>
        /// <param name="mm"></param>
        /// <returns></returns>
        public static int Millimeter2Pix(float mm)
        {
            return Millimeter2Pix(mm, CURRENT_DPI);
        }

        ///// <summary>
        ///// 毫米转像素
        ///// </summary>
        ///// <param name="mm"></param>
        ///// <param name="multiple">放大倍数</param>
        ///// <returns></returns>
        //public static int Millimeter2Pix(float mm, int multiple)
        //{
        //    return Millimeter2Pix(mm, multiple * CURRENT_DPI);
        //}

        /// <summary>
        /// 毫米转像素
        /// </summary>
        /// <param name="mm"></param>
        /// <param name="dpi"> </param>
        /// <returns></returns>
        public static int Millimeter2Pix(float mm, float dpi)
        {
            return (int)(mm / 25.4f * dpi);
        }

        public static PrinterSettings.StringCollection PrinterCollection
        {
            get { return PrinterSettings.InstalledPrinters; }
        }

        ///// <summary>
        ///// 以逆时针为方向对图像进行旋转
        ///// </summary>
        ///// <param name="b">位图流</param>
        ///// <param name="angle">旋转角度[0,360]</param>
        ///// <returns></returns>
        //public static Image RotateImg(Image b, int angle)
        //{
        //    angle = angle % 360;
        //    //弧度转换
        //    double radian = angle * Math.PI / 180.0;
        //    double cos = Math.Cos(radian);
        //    double sin = Math.Sin(radian);
        //    //原图的宽和高
        //    int w = b.Width;
        //    int h = b.Height;
        //    int W = (int)(Math.Max(Math.Abs(w * cos - h * sin), Math.Abs(w * cos + h * sin)));
        //    int H = (int)(Math.Max(Math.Abs(w * sin - h * cos), Math.Abs(w * sin + h * cos)));
        //    //目标位图
        //    Bitmap dsImage = new Bitmap(W, H);
        //    Graphics g = Graphics.FromImage(dsImage);
        //    g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Bilinear;
        //    g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
        //    //计算偏移量
        //    Point Offset = new Point((W - w) / 2, (H - h) / 2);
        //    //构造图像显示区域：让图像的中心与窗口的中心点一致
        //    Rectangle rect = new Rectangle(Offset.X, Offset.Y, w, h);
        //    Point center = new Point(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);
        //    g.TranslateTransform(center.X, center.Y);
        //    g.RotateTransform(360 - angle);
        //    //恢复图像在水平和垂直方向的平移
        //    g.TranslateTransform(-center.X, -center.Y);
        //    g.DrawImage(b, rect);
        //    //重至绘图的所有变换
        //    g.ResetTransform();
        //    g.Save();
        //    g.Dispose();
        //    //保存旋转后的图片
        //    b.Dispose();
        //    return dsImage;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="labelText"></param>
        ///// <param name="width"></param>
        ///// <param name="height"></param>
        ///// <param name="multiple"> </param>
        ///// <param name="isShowLabel"> </param>
        ///// <returns></returns>
        //public static Bitmap DrawBarcodeImage(string labelText, int width, int height, int multiple, bool isShowLabel)
        //{
        //    var bitmap = new Bitmap(width, height, PixelFormat.Format64bppPArgb);
        //    bitmap.SetResolution(CURRENT_DPI * multiple, CURRENT_DPI * multiple);
        //    var g = Graphics.FromImage(bitmap);
        //    g.Save();
        //    g.Dispose();
        //    return bitmap;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="txtString"></param>
        /// <returns></returns>
        public static IList<string> ProcessStringLines(string txtString)
        {
            var lineWidth = 32f;
            var cnChrWidth = 4f;
            var enChrWidth = 1.7f;
            var totalLen = Encoding.Default.GetByteCount(txtString);
            if (totalLen > 32)
            {
                cnChrWidth = 3.5f;
            }

            IList<string> lines = new List<string>();
            var sb = new StringBuilder();
            float lenWidth = 0f;
            for (int i = 0; i < txtString.Length; i++)
            {
                var chr = txtString[i];
                if (lenWidth >= lineWidth)
                {
                    lines.Add(sb.ToString());
                    sb.Clear();
                    lenWidth = 0f;
                }
                sb.Append(chr);
                if (chr > 127)
                {
                    lenWidth += cnChrWidth;
                }
                else
                {
                    lenWidth += enChrWidth;
                }
                if ((i + 1) == txtString.Length)
                {
                    lines.Add(sb.ToString());
                }
            }
            return lines;
        }
    }
}
