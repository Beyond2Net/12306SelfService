using NativeWifi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TrainCommon;

namespace _12306BySelfService
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        public Login _loginWindow { get; set; }
        private double textScrollWidth;
        private bool _isWlan;
        public MainWindow()
        {
            InitializeComponent();
            this.Loaded += MainWindow_Loaded;
            _timer.Interval = 10;
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            textScrollWidth = textBlockHead.ActualWidth;
            textBlockCurrentTime.Text = String.Format("{0}({1})", DateTime.Now.ToString("yyyy-MM-dd dddd HH:mm:ss"),CalendarHelper.GetLunarDay(DateTime.Now));
            double dl = textBlockHead.Margin.Left;
            dl -= 0.7;
            textBlockHead.Margin = new Thickness(dl, 0, 0, 0);
            if (textBlockHead.Margin.Left <= -(textScrollWidth + StackPanelUser.ActualWidth))
            {
                textBlockHead.Margin = new Thickness(stackPanel1.Width, 0, 0, 0);
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            GetNetwork();
            this.textBlockHead.Text = StringHelper.GetConfigValByKey("NOTICE", false);
            this.mainFrame.Source = new Uri(this.train.Tag.ToString(), UriKind.RelativeOrAbsolute);
            this.train.IsChecked = true;
            //添加缓存(初始化已选乘车日期)
            List<Object> list = new List<Object>();
            list.Add(new { TrainDateMore = DateTime.Now.ToString("yyyy-MM-dd") });
            SystemCache.SetSysObj(Constant.SelectedTakeTrainDateList, list);
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            foreach (var it in mainMenu.Items)
            {
                ((MenuItem)it).IsChecked = false;
            }
            var menuItem = (MenuItem) sender;
            if (menuItem != null)
            {
                menuItem.IsChecked = true;
                this.groupBox.Header = menuItem.Header;
            }

            var item = (MenuItem) e.Source;
            if (item?.Tag != null)
            {
                mainFrame.Source = new Uri(item.Tag.ToString(), UriKind.RelativeOrAbsolute);
            }
            else
            {
               MessageBoxResult ret = MessageBox.Show("确定要退出吗？", "提示", MessageBoxButton.OKCancel, MessageBoxImage.Question);
                if (ret == MessageBoxResult.OK)
                {
                    this.Hide();
                    _loginWindow.Show();
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);//退出应用程序
        }

        //预定
        private void btnReserve_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("此功能我们正在破解，请耐心等候", "信息", MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        private void GetNetwork()
        {
            Wlan.WlanConnectionAttributes wlanAttr = WlanClient.GetWlanConnectInfo();
            if (wlanAttr.profileName != null)
            {
                ImageWlan.Source = new BitmapImage(new Uri("Resources/Images/wifi.png", UriKind.RelativeOrAbsolute));
                _isWlan = true;
            }
            else
            {
                ImageWlan.Source = new BitmapImage(new Uri("Resources/Images/wifi2.png", UriKind.RelativeOrAbsolute));
            }
        }


        private void ImageWlan_OnMouseEnter(object sender, MouseEventArgs e)
        {
            LabelTip.Content = _isWlan ? "无线连接" : "有线连接";
            popWlan.IsOpen = true;
        }
    }
}
