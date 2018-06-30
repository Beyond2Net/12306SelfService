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
using System.Xml;
using TrainCommon;
using MessageBox = System.Windows.MessageBox;

namespace _12306BySelfService.SubPages
{
    /// <summary>
    /// LatestNews.xaml 的交互逻辑
    /// </summary>
    public partial class LatestNews : Page
    {
        public LatestNews()
        {
            InitializeComponent();
            this.BrowserLatest.LoadCompleted += (BrowserLatest_OnLoadCompleted);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            BrowserLatest.Navigate(new Uri(StringHelper.GetConfigValByKey(Constant.GetLatestNews, false), UriKind.RelativeOrAbsolute));
        }

        private void BrowserLatest_OnLoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                HTMLDocument dom = BrowserLatest.Document as HTMLDocument; //定义HTML

                HTMLDocument doc = BrowserLatest.Document as HTMLDocument;
                //获取窗体
                IHTMLWindow2 window = doc.parentWindow;
                //注入javascript
                StringBuilder script = new StringBuilder();
                script.Append("if(document.getElementById('menu')){document.getElementById('menu').style.display = 'none';}");
                window.execScript(script.ToString(), "javascript");
            }
            catch (Exception exception)
            {
                LogHelper.Log(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, exception.Message);
            }
        }
    }
}
