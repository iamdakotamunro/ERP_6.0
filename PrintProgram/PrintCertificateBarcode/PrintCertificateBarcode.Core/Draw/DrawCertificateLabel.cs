using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using Fath;
using PrintCertificateBarcode.Core.Common;
using PrintCertificateBarcode.Core.Model;

namespace PrintCertificateBarcode.Core.Draw
{
    public class DrawCertificateLabel
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

        public static Bitmap GetThumbnail(Image sourceImage, int destHeight, int destWidth)
        {
            Image imgSource = sourceImage;
            int sW, sH;
            // 按比例缩放           
            int sWidth = imgSource.Width;
            int sHeight = imgSource.Height;
            if (sHeight > destHeight || sWidth > destWidth)
            {
                if ((sWidth * destHeight) > (sHeight * destWidth))
                {
                    sW = destWidth;
                    sH = (destWidth * sHeight) / sWidth;
                }
                else
                {
                    sH = destHeight;
                    sW = (sWidth * destHeight) / sHeight;
                }
            }
            else
            {
                sW = sWidth;
                sH = sHeight;
            }
            Bitmap outBmp = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage(outBmp);
            g.Clear(Color.Transparent);
            // 设置画布的描绘质量         
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;
            g.DrawImage(imgSource, new Rectangle((destWidth - sW) / 2, (destHeight - sH) / 2, sW, sH), 0, 0, imgSource.Width, imgSource.Height, GraphicsUnit.Pixel);
            g.Dispose();
            // 以下代码为保存图片时，设置压缩质量     
            EncoderParameters encoderParams = new EncoderParameters();
            long[] quality = new long[1];
            quality[0] = 100;
            EncoderParameter encoderParam = new EncoderParameter(Encoder.Quality, quality);
            encoderParams.Param[0] = encoderParam;
            imgSource.Dispose();
            return outBmp;
        }

        #region -- 打印镜片

        /// <summary>
        /// 打印镜片标签图片
        /// </summary>
        /// <param name="widthMM"></param>
        /// <param name="heightMM"></param>
        /// <param name="dpi"></param>
        /// <param name="optician">配镜师</param>
        /// <param name="operationTime"></param>
        /// <param name="orderLabelInfo"></param>
        /// <param name="checker">检验师</param>
        /// <param name="logoName"></param>
        /// <returns></returns>
        public static Image DrawToOpticImage(int widthMM, int heightMM, float dpi, string optician, DateTime operationTime, OrderLabelInfo orderLabelInfo, string checker,string logoName)
        {
            float bs = dpi / DrawTools.CURRENT_DPI;
            var widthPX = DrawTools.Millimeter2Pix(widthMM, dpi);
            var heightPX = DrawTools.Millimeter2Pix(heightMM, dpi);
            var allBitmap = new Bitmap(widthPX, heightPX, PixelFormat.Format32bppPArgb);
            Graphics graphics = Graphics.FromImage(allBitmap);
            allBitmap.SetResolution(dpi, dpi);
            graphics.Clear(Color.White);
            var y = 1;
            const int X = 1;

            // 间隔空隙
            y += DrawTools.Millimeter2Pix(2, dpi);
            var logImg = Image.FromFile(Framework.Common.ServerInfo.SystemPath + string.Format(@"\Logo\{0}.jpg",logoName));
            var logbp = GetThumbnail(logImg, DrawTools.Millimeter2Pix(10, dpi), DrawTools.Millimeter2Pix(20, dpi));
            DrawImage(graphics, logbp, (widthPX - logbp.Width) / 2, y);

            // 间隔空隙
            y += logbp.Height + DrawTools.Millimeter2Pix(1, dpi);

            //绘画订单信息标签
            var orderLabel = DrawOpticOrderLabel(widthMM, heightMM - 14, optician, operationTime, orderLabelInfo, checker, dpi, bs);
            DrawImage(graphics, orderLabel, X, y);

            //下一张间隔
            //y += opticianLabel.Height-14;
            //y += DrawTools.Millimeter2Pix(14, dpi);

            //绘画配镜师标签
            //var opticianLabel = DrawOpticOperationLabel(optician,operationTime, dpi, bs);
            //DrawImage(graphics, opticianLabel, X, y);




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


        //private static Bitmap DrawOpticOperationLabel(string optician, DateTime operationTime, float dpi, float bs)
        //{
        //    var fontCN7 = new Font("Arial", 7f * bs);
        //    var fontEN6 = new Font("Arial", 6f * bs);
        //    var widthPX = DrawTools.Millimeter2Pix(38, dpi);
        //    var heightPX = DrawTools.Millimeter2Pix(13, dpi);
        //    var bitmap = new Bitmap(widthPX, heightPX, PixelFormat.Format32bppPArgb);
        //    Graphics graphics = Graphics.FromImage(bitmap);
        //    bitmap.SetResolution(dpi, dpi);
        //    graphics.Clear(Color.White);

        //    //设定Y坐标
        //    var y = 5;
        //    var x = DrawTools.Millimeter2Pix(0, dpi);


        //    graphics.Save();
        //    graphics.Dispose();
        //    return bitmap;
        //}

        private static Bitmap DrawOpticOrderLabel(int widthMM, int heightMM, string optician, DateTime operationTime, OrderLabelInfo orderLabelInfo, string checker, float dpi, float bs)
        {
            var fontNB = new Font("Arial", 6f * bs);
            var fontCN = new Font("Arial", 7f * bs);
            var widthPX = DrawTools.Millimeter2Pix(widthMM, dpi);
            var heightPX = DrawTools.Millimeter2Pix(heightMM, dpi);
            var bitmap = new Bitmap(widthPX, heightPX, PixelFormat.Format32bppPArgb);
            Graphics graphics = Graphics.FromImage(bitmap);
            bitmap.SetResolution(dpi, dpi);
            graphics.Clear(Color.White);

            //设定Y坐标
            var y = 3;
            var x = DrawTools.Millimeter2Pix(3, dpi);

            //开始绘画
            DrawString(graphics, "工单：" + orderLabelInfo.OrderNo, fontCN, Color.Black, x, y);
            y += fontCN.Height;
            DrawString(graphics, "配镜：" + optician, fontCN, Color.Black, x, y);
            y += fontCN.Height;
            DrawString(graphics, "时间： " + operationTime.ToString("yy-MM-dd HH:mm"), fontCN, Color.Black, x, y);
            y += fontCN.Height;
            DrawString(graphics, "质检：" + checker, fontCN, Color.Black, x, y);
            y += fontCN.Height;
            DrawString(graphics, "执行标准：", fontCN, Color.Black, x, y);
            y += fontCN.Height;
            DrawString(graphics, "QB-2506-2001 GB10810.1-2005", fontNB, Color.Black, x, y);
            y += fontNB.Height;
            DrawString(graphics, "QB-2682-2005 GB10810.3-2006", fontNB, Color.Black, x, y);

            y += 35;
            //条码对象
            var ex2 = new CBarcodeX
            {
                Data = orderLabelInfo.OrderNo,
                ShowText = false,
                Symbology = bcType.Code128,
                Font = new Font("Arial", 12)
            };
            var barcodeImage = ex2.Image(widthPX - DrawTools.Millimeter2Pix(3, dpi), DrawTools.Millimeter2Pix(10, dpi));
            DrawImage(graphics, barcodeImage, (widthPX - barcodeImage.Width) / 2, y);//画上条码

            //做旋转
            //bitmap.RotateFlip(RotateFlipType.Rotate180FlipY);
            //bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
            graphics.Save();
            graphics.Dispose();
            return bitmap;
        }

        //private static Bitmap DrawChecker(string checker,string orderNo, float dpi, float bs)
        //{
        //    var fontCN7 = new Font("Arial", 7f * bs);
        //    var fontEN6 = new Font("Arial", 6f * bs);
        //    var widthPX = DrawTools.Millimeter2Pix(38, dpi);
        //    var heightPX = DrawTools.Millimeter2Pix(20, dpi);
        //    var bitmap = new Bitmap(widthPX, heightPX, PixelFormat.Format32bppPArgb);
        //    Graphics graphics = Graphics.FromImage(bitmap);
        //    bitmap.SetResolution(dpi, dpi);
        //    graphics.Clear(Color.White);

        //    //设定Y坐标
        //    var y = 2;
        //    var x = DrawTools.Millimeter2Pix(12, dpi);

        //    //开始绘画
        //    DrawString(graphics, "检验师：" + checker, fontCN7, Color.Black, x, y);
        //    y += fontCN7.Height;
        //    DrawString(graphics, DateTime.Now.ToString("yy-MM-dd HH:mm"), fontEN6, Color.Black, x, y);
        //    y += fontEN6.Height;
        //    DrawString(graphics, orderNo, fontEN6, Color.Black, x, y);

        //    graphics.Save();
        //    graphics.Dispose();
        //    return bitmap;
        //}

        #endregion
    }
}
