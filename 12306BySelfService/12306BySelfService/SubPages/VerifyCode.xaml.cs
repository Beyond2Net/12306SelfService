using _12306BySelfService.Common;
using _12306BySelfService.SelfServiceReference;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using TrainCommon;
using TrainCommon.Model;

namespace _12306BySelfService
{
    //定义委托
    public delegate void ChangeTextHandler(Boolean callback);
    /// <summary>
    /// VerifyCode.xaml 的交互逻辑
    /// </summary>
    public partial class VerifyCode : Window
    {
        //定义事件（在父窗口实现）
        public event ChangeTextHandler ChangeTextEvent;
        public Login _loginWindow { get; set; }
        public bool isPass { get; set; }

        BitmapImage bitmapImage;
        List<int> passCodeList = new List<int>();
        public VerifyCode()
        {
            InitializeComponent();
            this.ResizeMode = ResizeMode.NoResize;
        }

        public VerifyCode(string sourcePath)
        {
            InitializeComponent();
            //this.imgValidation.Source = new BitmapImage(new Uri(sourcePath, UriKind.RelativeOrAbsolute));
            //不再给BitmapImage直赋filePath，而是先根据filePath读取图片的二进制格式，赋给BitmapImage的Source，这样就可以在图片读取完毕后关闭流）,
            //否则原验证码图片一直被进程占用
            InitBackgroundImage(this.gridMain, sourcePath);
        }

        /// <summary>
        /// 提交验证码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnOK_Click(object sender, RoutedEventArgs e)
        {
            string[] retArr = null;
            string[] TemplateCodeList = StringHelper.GetConfigValByKey(Constant.PasscodeRange, false).Replace('[', ' ').Replace(']', ' ').Split('|');
            if (this.passCodeList.Count > 0)
            {
                retArr = new string[passCodeList.Count];
                for (int i = 0; i < passCodeList.Count; i++)
                {
                    retArr[i] = TemplateCodeList[passCodeList[i]];
                }
                string temp = String.Join(",", retArr);
                temp = temp.Replace('{', ' ').Replace('}', ' ');
                string result = Regex.Replace(temp, @"\s", "");

                //提交验证码
                string url = StringHelper.GetConfigValByKey(Constant.CheckCodeUrl, false);
                Dictionary<String, String> parames = new Dictionary<String, String> {
                        { "login_site","E"},
                        { "rand","sjrand"},
                        { "answer",result}
                    };
                try
                {
                    //TrainServiceClient client = TrainClientSVC.GetTrainSVC().GetClientService();
                    //string resp = client.HttpPostAsync(url, parames).ToString();
                    HttpItem item = new HttpItem() {
                        URL = url,
                        PostData = parames,
                        Cookie = TrainCommon.HttpRequest.BeforLoginCookie
                    };
                    string resp = TrainCommon.HttpRequest.HttpPost(item);
                    //反序列化
                    BackData back = JsonConvert.DeserializeObject<BackData>(resp);
                    /*-校验验证码
                        - 没携带cookie：           { "result_message":"验证码校验失败,信息为空","result_code":"8" }
                        -携带cookie但点错了：      { "result_message":"验证码校验失败","result_code":"5" }
                        -携带cookie并且点击正确：  { "result_message":"验证码校验成功","result_code":"4" }
                        -停留时间过长：            { "result_message":"验证码已经过期","result_code":"7" }*/
                    if (back != null && back.result_code == "4")
                    {
                        //触发父窗口事件方法
                        ChangeTextEvent?.Invoke(true);

                        this.isPass = true;
                        this._loginWindow.Show();
                        this.DialogResult = true;
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("验证失败\r\n" + back.result_message, "信息", MessageBoxButton.OK, MessageBoxImage.Warning);
                        LogHelper.Log(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, back.result_message);
                        LogHelper.Info("验证失败" + back.result_message);
                        LogHelper.Error("验证失败" + back.result_message);
                        RefreshPasscode(sender, e);//验证失败刷新验证码
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("验证失败了。\r\n" + ex.Message, "信息提示", MessageBoxButton.OK, MessageBoxImage.Error);
                    RefreshPasscode(sender, e);//验证失败刷新验证码
                    LogHelper.Log(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, ex.Message);
                    LogHelper.Error(ex);
                }
            }
            else
            {
                MessageBox.Show("请点击图中验证码", "信息", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        /// <summary>
        /// 刷新验证码右上角图标
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Image_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Image img = sender as System.Windows.Controls.Image;
            switch (img.Name)
            {
                case "imgRefresh":
                    RefreshPasscode(sender, e);
                    break;
            }
        }

        /// <summary>
        /// 根据图片路径读取文件流给Image控件加载图片
        /// </summary>
        /// <param name="filePath"></param>
        private void InitImage(System.Windows.Controls.Image img, string filePath)
        {
            //第一种方法读取文件流
            //using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            //{
            //    FileInfo fi = new FileInfo(filePath);
            //    byte[] bytes = reader.ReadBytes((int)fi.Length);
            //    reader.Close();
            //    bitmapImage = new BitmapImage();
            //    bitmapImage.BeginInit();
            //    bitmapImage.StreamSource = new MemoryStream(bytes);
            //    bitmapImage.EndInit();
            //    img.Source = bitmapImage;//给图片Source属性赋值
            //    bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
            //}
            //第一种方法
            img.Source = new BitmapImage(new Uri("/12306BySelfService;component/Resources/Images/select.jpg", UriKind.RelativeOrAbsolute));
        }

        /// <summary>
        /// 根据图片路径读取文件流给Control控件加载背景图片
        /// </summary>
        /// <param name="control"></param>
        /// <param name="filePath"></param>
        private void InitBackgroundImage(Grid grid, string filePath)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                FileInfo fi = new FileInfo(filePath);
                byte[] bytes = reader.ReadBytes((int)fi.Length);
                reader.Close();
                Stream stream = new MemoryStream(bytes);
                Bitmap bitmap = new Bitmap(stream);
                ImageBrush brush = new ImageBrush();
                IntPtr ip = bitmap.GetHbitmap();
                brush.ImageSource = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(ip, IntPtr.Zero, Int32Rect.Empty, BitmapSizeOptions.FromEmptyOptions());
                grid.Background = brush;
                stream.Close();
                bitmap.Dispose();
            }
        }

        /// <summary>
        /// 验证码点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Border_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Border border = sender as Border;
            Border borderItem = null;
            //刷新验证码清除图标
            if (border == null)
            {
                UIElementCollection Childrens = this.gridPassImg.Children;
                foreach (var item in Childrens)
                {
                    borderItem = (Border)item;
                    if (borderItem != null)
                    {
                        borderItem.Child = null;
                    }
                }
                passCodeList.Clear();
            }
            else
            {
                //new Label().Foreground = System.Windows.Media.Brushes.Red;//设置背景色
                if (border.Child == null)
                {
                    System.Windows.Controls.Image img = new System.Windows.Controls.Image();
                    InitImage(img, StringHelper.VirtualPath + @"Temp\select.jpg");
                    //img.Opacity = 0.5;//设置透明度
                    img.Width = border.ActualWidth / 2;
                    img.Height = border.ActualHeight / 2;
                    border.Child = img;

                    //int RowIndex = Grid.GetRow(border);      //获取所在Grid行索引
                    //int ColumnIndex = Grid.GetColumn(border);//获取所在Grid列索引
                    passCodeList.Add(Int32.Parse(border.Uid));
                }
                else
                {
                    border.Child = null;
                    passCodeList.Remove(Int32.Parse(border.Uid));
                }
            }
        }

        /// <summary>
        /// 刷新验证码按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, RoutedEventArgs e)
        {
            RefreshPasscode(sender, e as MouseButtonEventArgs);
        }

        /// <summary>
        /// 重新刷新验证码
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void RefreshPasscode(object sender, object eventArgs)
        {
            try
            {
                TrainServiceClient client = TrainClientSVC.GetTrainSVC().GetClientService();
                string url = StringHelper.GetConfigValByKey(Constant.VerifyCodeUrl, false) + "&" + new Random().NextDouble().ToString() + new Random().Next(0, 10);
                string path = StringHelper.VirtualPath;
                //path = client.HttpGet(url, path).ToString();
                path = TrainCommon.HttpRequest.HttpGet(url, path).ToString();
                this.InitBackgroundImage(this.gridMain, path);
                this.Border_MouseDown(sender, eventArgs as MouseButtonEventArgs);
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
            }
            
        }

        /// <summary>
        /// 手动关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void VerifyCode_OnClosing(object sender, CancelEventArgs e)
        {
            //触发父窗口事件方法
            this._loginWindow.isManualClose = false;
            if (!this.isPass)
            {
                ChangeTextEvent?.Invoke(false);//父窗口callback 为false
                this._loginWindow.isManualClose = true;
            }
            this.DialogResult = true;
            this._loginWindow.Show();
        }
    }
}
