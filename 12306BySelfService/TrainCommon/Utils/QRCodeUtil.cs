using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Text;
using System.Web;
using ThoughtWorks.QRCode.Codec;

namespace TrainCommon
{
    public class QRCodeUtil
    {
        public static String GetUrl(string account, string password)
        {
            //string acc = HttpUtility.UrlEncode(AESUtil.Encrypt(account));
            //string pwd = HttpUtility.UrlEncode(AESUtil.Encrypt(password));
            //string code = EncryptAndDecodeUtil.Encode(string.Format("account={0}&pwd={1}", acc, pwd));
            //string certificationURL = string.Format("http://www.hr919.com/HRAppServer/Customer/GetServerByQRCode?code={0}", code);
            //return certificationURL;
            return "";
        }

        /// <summary>
        /// 创建二维码图片并返回图片路径
        /// </summary>
        /// <param name="account">账号</param>
        /// <param name="password">密码(明文)</param>
        /// <param name="fileName">图片名称</param>
        /// <returns></returns>
        public static String GetQRCodeUrl(string account, string password, string fileName)
        {
            string certificationURL = GetUrl(account, password);
            QRCodeEncoder qrCodeEncoder = new QRCodeEncoder();
            qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            qrCodeEncoder.QRCodeScale = 100; //设置编码测量度
            qrCodeEncoder.QRCodeVersion = 0; //设置编码版本
            qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.M;//设置编码错误纠正
            Bitmap image = qrCodeEncoder.Encode(certificationURL, Encoding.UTF8);  //生成二维码图片
            Image combinImg = CombinImage(image, StringHelper.VirtualPath + ("Content/images/KyLogo.png"), 300, 300);//添加二维码Logo
            string currentPath = StringHelper.VirtualPath + "Content/Company";
            if (!Directory.Exists(currentPath))
            {
                Directory.CreateDirectory(currentPath);
            }
            fileName = fileName + ".jpg";
            string filePath = Path.Combine(currentPath, fileName);
            combinImg.Save(filePath);
            image.Dispose();
            combinImg.Dispose();

            return String.Format("{0}/Content/Company/" + fileName, StringHelper.VirtualPath);
        }

        /// <summary>    
        /// 调用此函数后使此两种图片合并
        /// </summary>    
        /// <param name="imgBack">粘贴的源图片</param>    
        /// <param name="destImg">粘贴的目标图片</param>    
        public static Image CombinImage(Image imgBack, string destImg, int destHeight, int destWidth)
        {
            Image img = Image.FromFile(destImg); //中间Logo图片
            int sW = 0, sH = 0;//最终二维码图片宽度高度
            // 按比例缩放
            int sWidth = imgBack.Width;
            int sHeight = imgBack.Height;
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
            imgBack = KiResizeImage(imgBack, sW, sH);//确定二维码图像大小
            // 为了插入图片的完整性，我们选择在最中间插入，而且长宽建议为整个二维码的3/7至1/3
            int middleImgW = Math.Min((int)(sW / 3.5), img.Width);//中间图片宽度
            int middleImgH = Math.Min((int)(sH / 3.5), img.Height);//中间图片高度
            img = KiResizeImage(img, middleImgW, middleImgH);//确定二维码中间图像大小

            Graphics g = Graphics.FromImage(imgBack);
            // 设置画布的描绘质量
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.SmoothingMode = SmoothingMode.HighQuality;
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            int middleImgX = sW / 2 - middleImgW / 2;//中间图片X坐标
            int middleImgY = sH / 2 - middleImgH / 2;//中间图片Y坐标
            //相片四周刷一层白色边框
            //g.FillRectangle(Brushes.White, middleImgX - 6, middleImgY - 6, middleImgW + 12, middleImgH + 12);
            FillRoundRectangle(g, Brushes.White, new Rectangle(middleImgX - 6, middleImgY - 6, middleImgW + 12, middleImgH + 12), 8);//圆角
            //嵌入中间图片
            g.DrawImage(img, middleImgX, middleImgY, middleImgW, middleImgH);
            g.Dispose();
            GC.Collect();

            return imgBack;
        }

        /// <summary>    
        /// Resize图片    
        /// </summary>    
        /// <param name="bmp">原始Bitmap</param>    
        /// <param name="newW">新的宽度</param>    
        /// <param name="newH">新的高度</param>    
        /// <param name="Mode">保留着，暂时未用</param>    
        /// <returns>处理以后的图片</returns>    
        public static Image KiResizeImage(Image bmp, int newW, int newH)
        {
            try
            {
                Image b = new Bitmap(newW, newH);
                Graphics g = Graphics.FromImage(b);
                // 插值算法的质量    
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.DrawImage(bmp, new Rectangle(0, 0, newW, newH), new Rectangle(0, 0, bmp.Width, bmp.Height), GraphicsUnit.Pixel);
                g.Dispose();
                return b;
            }
            catch
            {
                return null;
            }
        }

        public static void FillRoundRectangle(Graphics g, Brush brush, Rectangle rect, int cornerRadius)
        {
            using (GraphicsPath path = CreateRoundedRectanglePath(rect, cornerRadius))
            {
                g.FillPath(brush, path);
            }
        }

        public static GraphicsPath CreateRoundedRectanglePath(Rectangle rect, int cornerRadius)
        {
            //重新整理下,圆角矩形
            GraphicsPath roundedRect = new GraphicsPath();
            roundedRect.AddArc(rect.X, rect.Y, cornerRadius * 2, cornerRadius * 2, 180, 90);
            roundedRect.AddLine(rect.X + cornerRadius, rect.Y, rect.Right - cornerRadius * 2, rect.Y);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y, cornerRadius * 2, cornerRadius * 2, 270, 90);
            roundedRect.AddLine(rect.Right, rect.Y + cornerRadius * 2, rect.Right, rect.Y + rect.Height - cornerRadius * 2);
            roundedRect.AddArc(rect.X + rect.Width - cornerRadius * 2, rect.Y + rect.Height - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 0, 90);
            roundedRect.AddLine(rect.Right - cornerRadius * 2, rect.Bottom, rect.X + cornerRadius * 2, rect.Bottom);
            roundedRect.AddArc(rect.X, rect.Bottom - cornerRadius * 2, cornerRadius * 2, cornerRadius * 2, 90, 90);
            roundedRect.AddLine(rect.X, rect.Bottom - cornerRadius * 2, rect.X, rect.Y + cornerRadius * 2);
            roundedRect.CloseFigure();
            return roundedRect;
        }
    }
}