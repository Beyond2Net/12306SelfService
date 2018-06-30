using mshtml;
using System;
using System.Collections.Generic;
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

namespace _12306BySelfService.SubPages
{
    /// <summary>
    /// QuerySubway.xaml 的交互逻辑
    /// </summary>
    public partial class QuerySubway : Page
    {
        private readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        public QuerySubway()
        {
            InitializeComponent();
            this.Loaded += QuerySubway_Loaded;
            this.BrowserSubway.LoadCompleted += BrowserSubway_LoadCompleted;
            _timer.Interval = 100;
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        int scale = 0;
        private void _timer_Tick(object sender, EventArgs e)
        {
            scale += 10;
            ProgressBar.Value = scale;
            if (ProgressBar.Value == ProgressBar.Maximum)
            {
                scale = 0;
            }
        }

        private void QuerySubway_Loaded(object sender, RoutedEventArgs e)
        {
            this.BrowserSubway.Navigate(new Uri("http://www.huochepiao.com/ditie/", UriKind.RelativeOrAbsolute));
        }

        private void BrowserSubway_LoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                HTMLDocument doc = BrowserSubway.Document as HTMLDocument;
                //获取窗体
                IHTMLWindow2 window = doc.parentWindow;
                //注入javascript
                StringBuilder script = new StringBuilder();
                script.Append("if(document.getElementById('loginmenu')){document.getElementById('loginmenu').style.display='none';}");
                script.Append("if(document.getElementById('huoche_nav')){document.getElementById('huoche_nav').style.display='none';}");
                script.Append("if(document.getElementById('huoche_topbar')){document.getElementById('huoche_topbar').style.display='none';}");
                script.Append("if(document.getElementsByTagName('table')[0]){ document.getElementsByTagName('table')[0].children[0].children[0].children[1].style.display='none';}");
                script.Append("if(document.getElementById('footer')){ document.getElementById('footer').style.display='none';}");

                //script.Append("if(document.getElementsByTagName('iframe')[0]){ document.getElementsByTagName('iframe')[1].style.display='none';document.getElementsByTagName('iframe')[0].style.display='none';}");
                //script.Append("if(document.getElementsByClassName('remenqu')[0]){ document.getElementsByClassName('remenqu')[0].style.display='none';}");
                //script.Append("if(document.getElementsByClassName('content')[0].children[4]){ document.getElementsByClassName('content')[0].children[4].style.display='none';}");
                //script.Append("if(document.getElementsByClassName('content')[0]){ document.getElementsByClassName('content')[0].children[0].children[0].children[0].style.display='none';}");
                //script.Append("if(document.getElementsByClassName('content')[0]){ document.getElementsByClassName('content')[0].children[0].children[0].children[2].style.display='none';}");

                script.Append("if(document.getElementById('BAIDU_SSP__wrapper_u616238_0')){document.getElementById('BAIDU_SSP__wrapper_u616238_0').style.display='none';}");
                window.execScript(script.ToString(), "javascript");

                ProgressBar.Visibility = Visibility.Collapsed;
                _timer.Stop();
            }
            catch (Exception exception)
            {
                LogHelper.Error(exception);
                MessageBox.Show(exception.Message, "信息", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
}
