using _12306BySelfService.Common;
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
using mshtml;
using TrainCommon;
using TrainCommon.Model;

namespace _12306BySelfService.SubPages
{
    /// <summary>
    /// QuerySaleTime.xaml 的交互逻辑
    /// </summary>
    public partial class QuerySaleTime : Page
    {
        public QuerySaleTime()
        {
            InitializeComponent();
            this.webBrowser.LoadCompleted += WebBrowser_LoadCompleted;
            //加载车站数据源
            try
            {
                TrainTicketManage.LoadedUserControlStation(this.tb_SaleStation);
                var dict = SystemCache.GetCache().GetObjByKey(Constant.AllSaleStation) as Dictionary<String, List<String>>;
                if (dict != null)
                {
                    cbxSaleTime.ItemsSource = dict.Keys;
                }
                popStartStation.MouseEnter += PopStartStation_MouseEnter;
                cbQueryItem.SelectionChanged += cbQuery_SelectionChanged;
                cbxSaleTime.SelectionChanged += cbxSaleTime_SelectionChanged;
                tkCurDate.Text = String.Format("今天是: {0}  [农历] {1}", DateTime.Now.ToString("yyyy-MM-dd dddd"), CalendarHelper.GetLunarDate(DateTime.Now));
                DateTime deadTime = Convert.ToDateTime(SystemCache.GetCache().GetObjByKey(Constant.OtherMaxdate));
                //DateTime deadTime = DateTime.Now.AddDays(30);
                tkDeadDate.Text = String.Format("暂售至: {0}  [农历] {1}", deadTime.ToString("yyyy-MM-dd dddd"), CalendarHelper.GetLunarDate(deadTime));
            }
            catch (Exception ex)
            {
                LogHelper.Error("加载失败！", ex);
            }
        }
        /// <summary>
        /// 待HTML页面加载完毕修改HTML内容
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            try
            {
                HTMLDocument doc = webBrowser.Document as HTMLDocument;//定义HTML
                if (doc == null)
                {
                    MessageBox.Show("页面加载失败！", "信息", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                };
                //获取窗体
                IHTMLWindow2 window = doc.parentWindow;
                //注入javascript
                StringBuilder script = new StringBuilder();
                script.Append("document.getElementsByTagName('iframe')[0].parentNode.parentNode.parentNode.parentNode.parentNode.style.display='none';");
                //script.Append("document.body.childNodes[7].style.display = 'none';");
                script.Append("document.body.getElementsByTagName('table')[3].style.display = 'none';");
                script.Append("document.body.getElementsByTagName('table')[0].style.display = 'none';");
                script.Append("document.body.getElementsByTagName('div')[0].style.display='none';");
                window.execScript(script.ToString(), "javascript");
            }
            catch (Exception exception)
            {
                LogHelper.Error(exception);
                MessageBox.Show(exception.Message, "信息", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void PopStartStation_MouseEnter(object sender, MouseEventArgs e)
        {
            popStartStation.IsOpen = true;
        }

        private void cbQuery_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            switch (cbQueryItem.SelectedIndex)
            {
                case 0:
                    stackStation.Visibility = Visibility.Visible;
                    stackSaleTime.Visibility = Visibility.Collapsed;
                    stackStationText.Visibility = Visibility.Visible;
                    stackSaleTimeText.Visibility = Visibility.Collapsed;
                    stackQuery.Visibility = Visibility.Visible;
                    stackTrainNumberInfo.Visibility = Visibility.Collapsed;
                    WrapPanelNote.Visibility = Visibility.Visible;
                    StackTrainNo.Visibility = Visibility.Collapsed;
                    break;
                case 1:
                    stackSaleTime.Visibility = Visibility.Visible;
                    stackStation.Visibility = Visibility.Collapsed;
                    stackSaleTimeText.Visibility = Visibility.Visible;
                    stackStationText.Visibility = Visibility.Collapsed;
                    stackQuery.Visibility = Visibility.Collapsed;
                    stackTrainNumberInfo.Visibility = Visibility.Collapsed;
                    WrapPanelNote.Visibility = Visibility.Visible;
                    StackTrainNo.Visibility = Visibility.Collapsed;
                    break;
                case 2:
                    stackStation.Visibility = Visibility.Collapsed;
                    stackSaleTime.Visibility = Visibility.Collapsed;
                    stackStationText.Visibility = Visibility.Collapsed;
                    stackSaleTimeText.Visibility = Visibility.Collapsed;
                    stackQuery.Visibility = Visibility.Visible;
                    WrapPanelNote.Visibility = Visibility.Collapsed;
                    StackTrainNo.Visibility = Visibility.Visible;
                    break;
            }
        }

        //根据选择的时间段加载车站列表
        private void cbxSaleTime_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dict = SystemCache.GetCache().GetObjByKey(Constant.AllSaleStation) as Dictionary<String, List<String>>;
            if (dict != null)
            {
                string[] stationArr = dict[cbxSaleTime.SelectedValue.ToString()].ToArray();
                stackSaleTimeTextCont.Text = String.Join("、", stationArr);
            }
        }

        //根据输入车站名称模糊匹配站点
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cbQueryItem.SelectedIndex == 2)//车次查询
            {
                //模拟浏览器页面导航
                string url = StringHelper.GetConfigValByKey(Constant.QueryTrainByNumber, false) + tb_TrainNo.Text.Trim();
                this.webBrowser.Navigate(new Uri(url));
                this.webBrowser.Height = 600;
                stackTrainNumberInfo.Visibility = Visibility.Visible;
            }
            else
            {
                var dict = SystemCache.GetCache().GetObjByKey(Constant.AllSaleStation) as Dictionary<String, List<String>>;
                if (dict != null)
                {
                    Station startStation = SystemCache.GetCache().GetTrainStation(tb_SaleStation.Text);
                    if (startStation != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        string value = tb_SaleStation.Text.Trim();
                        List<String> list;
                        Dictionary<String, List<String>> dictTemp = new Dictionary<String, List<String>>();
                        bool has = false;
                        foreach (var itst in dict)
                        {
                            list = new List<string>();
                            foreach (var it in itst.Value)
                            {
                                if (it.Contains(value))
                                {
                                    has = true;
                                    list.Add(it);
                                }
                            }
                            if (has) dictTemp.Add(itst.Key, list);
                            has = false;
                        }
                        foreach (var itemx in dictTemp)
                        {
                            foreach (var item in itemx.Value)
                            {
                                sb.AppendFormat("{0}:{1}   ", item, itemx.Key);
                            }
                        }
                        tkStationText.Text = sb.ToString();
                    }
                    else
                    {
                        MessageBox.Show("站点不存在，请重新输入！", "信息", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                }
            }
        }
    }
}
