using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace _12306BySelfService.SubPages
{
    /// <summary>
    /// AboutPage.xaml 的交互逻辑
    /// </summary>
    public partial class AboutPage : Page
    {
        public AboutPage()
        {
            InitializeComponent();
        }

        private void RightAllReserve_MouseDown(object sender, MouseButtonEventArgs e)
        {
            //打开本地浏览器
            Process ie = new Process();
            ie.StartInfo.FileName = "IEXPLORE.EXE";
            ie.StartInfo.Arguments = "http://www.cnblogs.com/";
            ie.Start();
            //亦可如下
            //Process.Start("IEXPLORE.EXE", "http://www.cnblogs.com/");
        }
    }
}
