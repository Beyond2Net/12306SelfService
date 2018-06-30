using _12306BySelfService.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
using TrainCommon.Model;
using static _12306BySelfService.Common.TrainTicketManage;

namespace _12306BySelfService.SubPages
{
    /// <summary>
    /// TrainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TrainWindow : Page
    {
        private readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        public static string GlobalRepeatSubmitToken { get; set; }
        public TrainWindow()
        {
            InitializeComponent();
            //加载出发站、到达站数据源
            LoadedUserControlStation(this.tb_startStation);
            LoadedUserControlStation(this.tb_endStation);

            OutputLog("登录成功！", false);
            this.Loaded += TrainWindow_Loaded;
            _timer.Interval = 1000;
            _timer.Tick += Timer_Tick;
            tb_startStation.MouseEnter += Tb_startStation_MouseEnter;
            tb_endStation.MouseEnter += Tb_endStation_MouseEnter;
        }

        private void Tb_endStation_MouseEnter(object sender, MouseEventArgs e)
        {
            popEndStation.IsOpen = true;
        }

        private void Tb_startStation_MouseEnter(object sender, MouseEventArgs e)
        {
            popStartStation.IsOpen = true;
        }

        private void TrainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //this.dateTake.BlackoutDates.Add(new CalendarDateRange(new DateTime(), DateTime.Now.Date.AddDays(-1)));
                this.dateTake.SelectedDate = DateTime.Now;
                this.dateTake.DisplayDateStart = DateTime.Now;
                this.dateTake.DisplayDateStart = Convert.ToDateTime(SystemCache.GetCache().GetObjByKey(Constant.OtherMindate));
                this.dateTake.DisplayDateEnd = Convert.ToDateTime(SystemCache.GetCache().GetObjByKey(Constant.OtherMaxdate));
                this.dateTakeMore.DisplayDateStart = DateTime.Now;
                this.dateTakeMore.DisplayDateStart = Convert.ToDateTime(SystemCache.GetCache().GetObjByKey(Constant.OtherMindate));
                this.dateTakeMore.DisplayDateEnd = Convert.ToDateTime(SystemCache.GetCache().GetObjByKey(Constant.OtherMaxdate));

                //加载常用联系人
                LoadedContracts();
                //从缓存中加载上一次查询列车信息提高速度
                var trainList = SystemCache.GetCache().GetObjByKey(Constant.TrainList);
                if (trainList != null)
                {
                    this.dataGridTrainList.ItemsSource = trainList as List<Train>;
                }
                //从缓存中加载已选择车次
                var list = SystemCache.GetCache().GetObjByKey(Constant.SelectedTrainList);
                if (list != null)
                {
                    dataGridCheckedTrain.ItemsSource = list as List<Train>;
                }
                //从缓存中加载已选联系人
                var listPassenger = SystemCache.GetCache().GetObjByKey(Constant.SelectedPassengers);
                if (listPassenger != null)
                {
                    dataGridPassenger.ItemsSource = listPassenger as List<Passenger>;
                }
                //初始化已选日期
                InitSelectedDate();
                //输出区域
                OutputInstruction();
                string latestMsg = StringHelper.GetConfigValByKey(Constant.LatestQueryTime);
                if (!String.IsNullOrEmpty(latestMsg))
                {
                    LatestQueryTime.Content = StringHelper.GetConfigValByKey(Constant.LatestQueryTime);
                    lblLatest.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                OutputLog(ex.Message, true);
                LogHelper.Info(ex);
            }
        }

        private void InitSelectedDate()
        {
            //添加缓存(初始化已选乘车日期)
            List<Object> list = new List<Object>();
            list.Add(new { TrainDateMore = DateTime.Now.ToString("yyyy-MM-dd") });
            SystemCache.SetSysObj(Constant.SelectedTakeTrainDateList, list);
            dataGridCheckedDate.ItemsSource = SystemCache.GetCache().GetObjByKey(Constant.SelectedTakeTrainDateList) as List<Object>;
        }

        //加载输入区域和使用说明区域
        private void OutputInstruction()
        {
            //加载使用说明
            TextBlock tbk = null;
            string[] arr = StringHelper.GetConfigValByKey(Constant.Instruction, false).Split('$');
            foreach (var item in arr)
            {
                tbk = new TextBlock();
                //tbk.Height = 210;
                tbk.Width = 160;
                tbk.Text = item;
                tbk.Foreground = Brushes.Black;
                tbk.TextWrapping = TextWrapping.Wrap;
                this.listBoxNote.Items.Add(tbk);
            }
            //如果已开启抢票线程则继续启动该线程
            var isStartOrder = Convert.ToBoolean(SystemCache.GetCache().GetObjByKey(Constant.IsStartOrder));
            if (isStartOrder)
            {
                btnStop.Visibility = Visibility.Visible;
                btnSubmit.Foreground = Brushes.Red;
                _timer.Start();
            }
        }

        //DataGrid右键快捷键事件
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                DataRowView drv = dataGridTrainList.SelectedItem as DataRowView;
                //int index = dataGridTrainList.SelectedIndex;
                MenuItem menu = sender as MenuItem;
                switch (menu.Name)
                {
                    case "menuDelete":
                        this.dataGridTrainList.Items.Remove(drv);
                        //this.dataGridTrainList.Items.RemoveAt(index);
                        OutputLog(String.Format("您刚刚删除了{0}列车", drv.Row["TrainID"]), true);
                        break;
                    case "menuRefresh":
                        this.dataGridTrainList.Items.Refresh();
                        OutputLog("刷新成功!", false);
                        break;
                    case "menuGetDetails":
                        MessageBox.Show(drv.Row["TrainID"].ToString(), "信息", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                        break;
                }
            }
            catch (Exception ex)
            {
                OutputLog(String.Format("出错了:{0}", ex.Message), true);
                LogHelper.Error(ex);
            }

        }
        //点击预定
        private void BtnReserve_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Train train = dataGridTrainList.SelectedItem as Train;
                //系统维护时间内不可预定
                if (String.IsNullOrEmpty(train.TrainSecretStr))
                    return;
                //获取已选中车次
                List<Train> selectedTrainList = SystemCache.GetCache().GetObjByKey(Constant.SelectedTrainList) as List<Train>;
                if (selectedTrainList == null)
                    selectedTrainList = new List<Train>();
                if (!selectedTrainList.Contains(train))
                {
                    selectedTrainList.Add(train);
                    SystemCache.SetSysObj(Constant.SelectedTrainList, selectedTrainList);
                    //将所选中的车次添加至设置区域的(已选车次里面)
                    dataGridCheckedTrain.ItemsSource = null;
                    dataGridCheckedTrain.ItemsSource = selectedTrainList;
                    OutputLog(String.Format("您选择了 {0} 列车", train.TrainID), false);
                }
                else
                {
                    OutputLog(String.Format("您已经选择了 {0} 列车, 不能重复添加！", train.TrainID), true);
                }
            }
            catch (Exception ex)
            {
                OutputLog(String.Format("出错了:{0}", ex.Message), true);
                LogHelper.Error(ex);
            }
        }
        //删除已选列车
        private void delectTrain_MouseLeftButtonDown(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;
            try
            {
                List<Train> selectedTrainList = SystemCache.GetCache().GetObjByKey(Constant.SelectedTrainList) as List<Train>;
                Train train = dataGridCheckedTrain.SelectedItem as Train;
                selectedTrainList.Remove(train);
                dataGridCheckedTrain.ItemsSource = null;
                dataGridCheckedTrain.ItemsSource = selectedTrainList;
                SystemCache.SetSysObj(Constant.SelectedTrainList, selectedTrainList);
                OutputLog(String.Format("您取消了 {0} 列车", train.TrainID), true);
            }
            catch (Exception ex)
            {
                OutputLog(String.Format("出错了:{0}", ex.Message), true);
                LogHelper.Log(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, ex.Message);
                LogHelper.Error(ex);
            }
        }

        //点击查询列车信息
        private void BtnSearch_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Station startStation = SystemCache.GetCache().GetTrainStation(tb_startStation.Text);
                Station endStation = SystemCache.GetCache().GetTrainStation(tb_endStation.Text);
                if (startStation == null)
                {
                    MessageBox.Show("出发站不存在", "信息", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else if (endStation == null)
                {
                    MessageBox.Show("到达站不存在", "信息", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                else
                {
                    btnSearch.Content = "加载中...";
                    OutputLog(btnSearch.Content.ToString(), false);
                    Dispatcher.Invoke(() =>
                    {
                        string fromStation = startStation.StationCode;
                        string toStation = endStation.StationCode;
                        List<Train> list = TrainTicketManage.GetTrain().GetTrainTableList(fromStation, toStation, (DateTime)this.dateTake.SelectedDate, " ", 0);
                        this.dataGridTrainList.ItemsSource = list;
                        if (list != null)
                        {
                            string msg = String.Format("共查询到了 {0} 列车", list.Count);
                            OutputLog(msg, true);
                            this.trainCount.Content = list.Count.ToString();
                            StringHelper.SetConfig(Constant.StartStation, tb_startStation.Text);
                            StringHelper.SetConfig(Constant.EndStation, tb_endStation.Text);
                            StringHelper.SetConfig(Constant.LatestQueryTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
                            LogHelper.Info(msg);
                            LogHelper.Auth(msg);
                        }
                        else
                        {
                            OutputLog("共查询到了 0 列车", true);
                            this.trainCount.Content = 0;
                        }
                    });
                    btnSearch.Content = "查  询";
                }
                StringHelper.SetConfig(Constant.LatestQueryTime, DateTime.Now.ToString("yyyy-MM-dd HH:mm"));

                ////这是测试
                //List<Train> list = TrainTicketManage.GetTrain().GetTrainTableList("", "", (DateTime)this.dateTake.SelectedDate, " ", 0);
                //this.dataGridTrainList.ItemsSource = list;
            }
            catch (Exception ex)
            {
                LogHelper.Error(ex);
                OutputLog(ex.Message, true);
            }
        }

        //交换出发站---到达站
        private void imgSwitch_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            string temp = String.Empty;
            temp = tb_startStation.Text;
            tb_startStation.Text = tb_endStation.Text;
            tb_endStation.Text = temp;
        }

        //选择时间段下拉框事件
        private void CombTimeSection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        //勾选火车类型
        private void cbSelectTrainType_Checked(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;
            if (cbx.IsChecked == false)
                selectAllTrainType.IsChecked = false;
            else
            {
                bool isAllcheck = true;
                foreach (var item in stackpAllTrainType.Children)
                {
                    if (item is CheckBox)
                    {
                        if (((CheckBox)item).IsChecked == false)
                        {
                            isAllcheck = false;
                        }
                    }
                }
                if (isAllcheck)
                {
                    selectAllTrainType.IsChecked = true;
                }
            }
        }

        //全选列车类型
        private void selectAllTrainType_Checked(object sender, RoutedEventArgs e)
        {
            if (selectAllTrainType.IsChecked == true)
            {
                foreach (var item in stackpAllTrainType.Children)
                {
                    if (item is CheckBox)
                    {
                        ((CheckBox)item).IsChecked = true;
                    }
                }
            }
            else
            {
                foreach (var item in stackpAllTrainType.Children)
                {
                    if (item is CheckBox)
                    {
                        ((CheckBox)item).IsChecked = false;
                    }
                }
            }
        }

        //全选关注
        private void CheckBoxAllFocus_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in dataGridTrainList.Items)
            {
                DataGridTemplateColumn templeColumn = dataGridTrainList.Columns[0] as DataGridTemplateColumn;
                FrameworkElement fwElement = dataGridTrainList.Columns[0].GetCellContent(item);

                if (fwElement != null)
                {
                    CheckBox cBox = templeColumn.CellTemplate.FindName("cbitem", fwElement) as CheckBox;
                    if (cBox.IsChecked == true)
                    {
                        cBox.IsChecked = false;
                    }
                    else
                    {
                        if (cBox != null)
                        {
                            cBox.IsChecked = true;
                        }
                        else
                        {
                            cBox.IsChecked = false;
                        }
                    }

                }

            }
        }
        //全选所有席座类型
        private void CbSelectAllSeat_Click(object sender, RoutedEventArgs e)
        {
            if (cbSelectAllSeat.IsChecked == true)
            {
                Int32 flag = 4;
                foreach (var item in stackpAllSeatType.Children)
                {
                    if (item is CheckBox)
                    {
                        ((CheckBox)item).IsChecked = true;
                        dataGridTrainList.Columns[flag].Visibility = Visibility.Visible;
                        flag++;
                    }
                }
            }
            else
            {
                foreach (var item in stackpAllSeatType.Children)
                {
                    if (item is CheckBox)
                    {
                        ((CheckBox)item).IsChecked = false;
                    }
                }
                for (int i = 4; i < dataGridTrainList.Columns.Count - 1; i++)
                {
                    dataGridTrainList.Columns[i].Visibility = Visibility.Collapsed;
                }
            }
        }

        //动态隐藏指定席座类型
        private void CbSelectSeatType_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;
            Int32 index = Int32.Parse(cbx.Uid);
            if (cbx.IsChecked == true)
            {
                dataGridTrainList.Columns[index].Visibility = Visibility.Visible;
                bool isAllcheck = true;
                foreach (var item in stackpAllSeatType.Children)
                {
                    if (item is CheckBox)
                    {
                        if (((CheckBox)item).IsChecked == false)
                        {
                            isAllcheck = false;
                        }
                    }
                }
                if (isAllcheck)
                {
                    cbSelectAllSeat.IsChecked = true;
                }
            }
            else
            {
                dataGridTrainList.Columns[index].Visibility = Visibility.Collapsed;
                cbSelectAllSeat.IsChecked = null;
            }
        }

        private void LoadedContracts()
        {
            string msg = string.Empty;
            try
            {
                //初始化联系人globalRepeatSubmitToken
                //SystemCache.GetCache().GetObjByKey(Constant.GlobalRepeatSubmitToken) == null
                if (SystemCache.GetCache().GetObjByKey(Constant.GlobalRepeatSubmitToken) == null)
                {
                    string repeatSubmit = StringHelper.GetConfigValByKey(Constant.InitClientContractUrl, false);
                    HttpItem item = new HttpItem()
                    {
                        URL = repeatSubmit,
                        PostData = new Dictionary<string, string>() { { "_json_att", "" } },
                        Cookie = HttpRequest.JSessionCookie + "tk=" + Login.AppTk,
                        Referer = @"https://kyfw.12306.cn/otn/leftTicket/init"
                    };
                    string repeatTokenRet = HttpRequest.HttpPost(item);
                    string token = StringHelper.GetSubstring(repeatTokenRet, Constant.GlobalRepeatSubmitToken, ';').Replace("'", "");
                    GlobalRepeatSubmitToken = token;
                    //将globalRepeatSubmitToken添加至系统缓存
                    SystemCache.SetSysObj(Constant.GlobalRepeatSubmitToken, token);

                    //将leftTicketStr添加至系统缓存
                    string leftTicketStr = StringHelper.GetSubstring(repeatTokenRet, Constant.LeftTicketStr, ',');
                    SystemCache.SetSysObj(Constant.LeftTicketStr, leftTicketStr);
                    //将key_Chceck_isChange添加至系统缓存
                    string keyCheckIsChange = StringHelper.GetSubstring(repeatTokenRet, Constant.KeyCheckIsChange, ',');
                    SystemCache.SetSysObj(Constant.KeyCheckIsChange, keyCheckIsChange);
                    //将tour_flag添加至系统缓存
                    string tour_flag = StringHelper.GetSubstring(repeatTokenRet, Constant.TourFlag, ',');//***********************************************************看看为什么没有值
                    //将purpose_codes添加至系统缓存
                    string purpose_codes = StringHelper.GetSubstring(repeatTokenRet, Constant.PurposeCodes, ',');
                    SystemCache.SetSysObj(Constant.PurposeCodes, purpose_codes);

                    //这一步是为了获取提交订单时的表单数据,该字段数据是从下面请求返回的JS文件中查找的
                    string checkOrderInfoUrl = StringHelper.GetConfigValByKey(Constant.CheckOrderInfoJSUrl, false);
                    HttpItem itemx = new HttpItem()
                    {
                        URL = checkOrderInfoUrl,
                        Cookie = HttpRequest.JSessionCookie + "tk=" + Login.AppTk,
                        Referer = "https://kyfw.12306.cn/otn/confirmPassenger/initDc"
                    };
                    string checkOrderRet = HttpRequest.HttpGet(itemx).ToString();
                    string cancel_flag = StringHelper.GetSubstring(checkOrderRet, "cancel_flag", ',').Replace("\"", "");
                    SystemCache.SetSysObj(Constant.CancelFlag, cancel_flag);
                    string bed_level_order_num = StringHelper.GetSubstring(checkOrderRet, "bed_level_order_num", ',').Replace("\"", "");
                    SystemCache.SetSysObj(Constant.BedLevelOrderNum, bed_level_order_num);
                }

                //获取联系人
                //SystemCache.GetCache().GetObjByKey(Constant.NormalPassengers) == null
                //if (SystemCache.GetCache().GetObjByKey(Constant.NormalPassengers) == null)
                //{
                //    string contractUrl = StringHelper.GetConfigValByKey(Constant.ConstractUrl, false);
                //    HttpItem item2 = new HttpItem()
                //    {
                //        URL = contractUrl,
                //        Cookie = HttpRequest.JSessionCookie + "tk=" + Login.AppTk,
                //        Referer = @"https://kyfw.12306.cn/otn/confirmPassenger/initDc"
                //    };
                //    string contracts = HttpRequest.HttpPost(item2);
                //    JObject json = JObject.Parse(contracts);
                //    JArray result = JObject.Parse(json["data"].ToString())[Constant.NormalPassengers] as JArray;
                //    if (result == null)
                //    {
                //        msg = "获取联系人失败";
                //        MessageBox.Show("获取联系人失败", "信息", MessageBoxButton.OK, MessageBoxImage.Warning);
                //    }
                //    else
                //    {
                //        LoadContractComponent(result);
                //        SystemCache.SetSysObj(Constant.NormalPassengers, contracts);
                //    }
                //}
                //else
                //{
                //    //从缓存中读取提高加载速度
                //    //这是测试
                //    //string temp = File.ReadAllText(StringHelper.VirtualPath + "test.txt");
                //    //string mat = "", mat2 = "";
                //    //mat = StringHelper.GetSubstring(temp, Constant.KeyChceckIsChange, ',');
                //    //mat2 = StringHelper.GetSubstring(temp, Constant.LeftTicketStr, ',');
                //    //string tour_flag = StringHelper.GetSubstring(temp, Constant.TourFlag, ',');

                //    string contracts = SystemCache.GetCache().GetObjByKey(Constant.NormalPassengers).ToString();
                //    //string contracts = "{\"validateMessagesShowId\":\"_validatorMessage\",\"status\":true,\"httpstatus\":200,\"data\":{\"isExist\":true,\"exMsg\":\"\",\"two_isOpenClick\":[\"93\",\"95\",\"97\",\"99\"],\"other_isOpenClick\":[\"91\",\"93\",\"98\",\"99\",\"95\",\"97\"],\"normal_passengers\":[{\"code\":\"10\",\"passenger_name\":\"徐坤\",\"sex_code\":\"M\",\"sex_name\":\"男\",\"born_date\":\"1989 - 09 - 19 00:00:00\",\"country_code\":\"CN\",\"passenger_id_type_code\":\"1\",\"passenger_id_type_name\":\"二代身份证\",\"passenger_id_no\":\"41282919890919403X\",\"passenger_type\":\"3\",\"passenger_flag\":\"0\",\"passenger_type_name\":\"学生\",\"mobile_no\":\"18317187560\",\"phone_no\":\"18317894130\",\"email\":\"henu_xk@126.com\",\"address\":\"\",\"postalcode\":\"463600\",\"first_letter\":\"\",\"recordCount\":\"12\",\"total_times\":\"99\",\"index_id\":\"0\"},{\"code\":\"1\",\"passenger_name\":\"陈静梅\",\"sex_code\":\"F\",\"sex_name\":\"女\",\"born_date\":\"2014 - 01 - 19 00:00:00\",\"country_code\":\"CN\",\"passenger_id_type_code\":\"1\",\"passenger_id_type_name\":\"二代身份证\",\"passenger_id_no\":\"412829199109143247\",\"passenger_type\":\"1\",\"passenger_flag\":\"0\",\"passenger_type_name\":\"成人\",\"mobile_no\":\"18317894130\",\"phone_no\":\"\",\"email\":\"henu_xk@126.com\",\"address\":\"河南开封\",\"postalcode\":\"463600\",\"first_letter\":\"CJM\",\"recordCount\":\"12\",\"total_times\":\"99\",\"index_id\":\"1\"},{\"code\":\"2\",\"passenger_name\":\"丁莹峰\",\"sex_code\":\"M\",\"sex_name\":\"男\",\"born_date\":\"2014 - 08 - 07 00:00:00\",\"country_code\":\"CN\",\"passenger_id_type_code\":\"1\",\"passenger_id_type_name\":\"二代身份证\",\"passenger_id_no\":\"410181199305108591\",\"passenger_type\":\"3\",\"passenger_flag\":\"0\",\"passenger_type_name\":\"学生\",\"mobile_no\":\"18317894130\",\"phone_no\":\"\",\"email\":\"\",\"address\":\"\",\"postalcode\":\"\",\"first_letter\":\"DYF\",\"recordCount\":\"12\",\"total_times\":\"99\",\"index_id\":\"2\"},{\"code\":\"3\",\"passenger_name\":\"韩林果\",\"sex_code\":\"\",\"born_date\":\"2015 - 05 - 14 00:00:00\",\"country_code\":\"CN\",\"passenger_id_type_code\":\"1\",\"passenger_id_type_name\":\"二代身份证\",\"passenger_id_no\":\"41282219930424118X\",\"passenger_type\":\"1\",\"passenger_flag\":\"0\",\"passenger_type_name\":\"成人\",\"mobile_no\":\"\",\"phone_no\":\"\",\"email\":\"\",\"address\":\"\",\"postalcode\":\"\",\"first_letter\":\"HLG\",\"recordCount\":\"12\",\"total_times\":\"99\",\"index_id\":\"3\"},{\"code\":\"4\",\"passenger_name\":\"乔阳阳\",\"sex_code\":\"M\",\"sex_name\":\"男\",\"born_date\":\"2014 - 08 - 08 00:00:00\",\"country_code\":\"CN\",\"passenger_id_type_code\":\"1\",\"passenger_id_type_name\":\"二代身份证\",\"passenger_id_no\":\"410327199310145314\",\"passenger_type\":\"3\",\"passenger_flag\":\"0\",\"passenger_type_name\":\"学生\",\"mobile_no\":\"\",\"phone_no\":\"\",\"email\":\"\",\"address\":\"\",\"postalcode\":\"\",\"first_letter\":\"QYY\",\"recordCount\":\"12\",\"total_times\":\"99\",\"index_id\":\"4\"},{\"code\":\"5\",\"passenger_name\":\"王娜利\",\"sex_code\":\"\",\"born_date\":\"1900 - 01 - 01 00:00:00\",\"country_code\":\"CN\",\"passenger_id_type_code\":\"1\",\"passenger_id_type_name\":\"二代身份证\",\"passenger_id_no\":\"410327198107150420\",\"passenger_type\":\"1\",\"passenger_flag\":\"0\",\"passenger_type_name\":\"成人\",\"mobile_no\":\"\",\"phone_no\":\"\",\"email\":\"\",\"address\":\"\",\"postalcode\":\"\",\"first_letter\":\"WNL\",\"recordCount\":\"12\",\"total_times\":\"99\",\"index_id\":\"5\"},{\"code\":\"6\",\"passenger_name\":\"魏勇\",\"sex_code\":\"M\",\"sex_name\":\"男\",\"born_date\":\"1974 - 07 - 15 00:00:00\",\"country_code\":\"CN\",\"passenger_id_type_code\":\"1\",\"passenger_id_type_name\":\"二代身份证\",\"passenger_id_no\":\"412829197407154199\",\"passenger_type\":\"1\",\"passenger_flag\":\"0\",\"passenger_type_name\":\"成人\",\"mobile_no\":\"18217046465\",\"phone_no\":\"\",\"email\":\"\",\"address\":\"\",\"postalcode\":\"\",\"first_letter\":\"WY\",\"recordCount\":\"12\",\"total_times\":\"99\",\"index_id\":\"6\"},{\"code\":\"7\",\"passenger_name\":\"徐璠玙\",\"sex_code\":\"M\",\"sex_name\":\"男\",\"born_date\":\"2014 - 08 - 07 00:00:00\",\"country_code\":\"CN\",\"passenger_id_type_code\":\"1\",\"passenger_id_type_name\":\"二代身份证\",\"passenger_id_no\":\"412828199312110014\",\"passenger_type\":\"3\",\"passenger_flag\":\"0\",\"passenger_type_name\":\"学生\",\"mobile_no\":\"18317894129\",\"phone_no\":\"\",\"email\":\"\",\"address\":\"\",\"postalcode\":\"\",\"first_letter\":\"X\",\"recordCount\":\"12\",\"total_times\":\"99\",\"index_id\":\"7\"},{\"code\":\"9\",\"passenger_name\":\"徐猛\",\"sex_code\":\"M\",\"sex_name\":\"男\",\"born_date\":\"1993 - 01 - 12 00:00:00\",\"country_code\":\"CN\",\"passenger_id_type_code\":\"1\",\"passenger_id_type_name\":\"二代身份证\",\"passenger_id_no\":\"41282919910505401X\",\"passenger_type\":\"3\",\"passenger_flag\":\"0\",\"passenger_type_name\":\"学生\",\"mobile_no\":\"18317894130\",\"phone_no\":\"\",\"email\":\"henu_xk@126.com\",\"address\":\"\",\"postalcode\":\"\",\"first_letter\":\"XM\",\"recordCount\":\"12\",\"total_times\":\"99\",\"index_id\":\"8\"},{\"code\":\"8\",\"passenger_name\":\"徐中原\",\"sex_code\":\"M\",\"sex_name\":\"男\",\"born_date\":\"1980 - 01 - 01 00:00:00\",\"country_code\":\"CN\",\"passenger_id_type_code\":\"1\",\"passenger_id_type_name\":\"二代身份证\",\"passenger_id_no\":\"412829196709154018\",\"passenger_type\":\"1\",\"passenger_flag\":\"0\",\"passenger_type_name\":\"成人\",\"mobile_no\":\"\",\"phone_no\":\"\",\"email\":\"\",\"address\":\"\",\"postalcode\":\"\",\"first_letter\":\"XZY\",\"recordCount\":\"12\",\"total_times\":\"98\",\"index_id\":\"9\"},{\"code\":\"12\",\"passenger_name\":\"左银发\",\"sex_code\":\"M\",\"sex_name\":\"男\",\"born_date\":\"1900 - 01 - 01 00:00:00\",\"country_code\":\"CN\",\"passenger_id_type_code\":\"1\",\"passenger_id_type_name\":\"二代身份证\",\"passenger_id_no\":\"411322198804284212\",\"passenger_type\":\"3\",\"passenger_flag\":\"0\",\"passenger_type_name\":\"学生\",\"mobile_no\":\"18317894130\",\"phone_no\":\"\",\"email\":\"\",\"address\":\"\",\"postalcode\":\"\",\"first_letter\":\"ZYF\",\"recordCount\":\"12\",\"total_times\":\"99\",\"index_id\":\"10\"},{\"code\":\"11\",\"passenger_name\":\"周玉琴\",\"sex_code\":\"F\",\"sex_name\":\"女\",\"born_date\":\"2014 - 12 - 15 00:00:00\",\"country_code\":\"CN\",\"passenger_id_type_code\":\"1\",\"passenger_id_type_name\":\"二代身份证\",\"passenger_id_no\":\"412829196806194046\",\"passenger_type\":\"1\",\"passenger_flag\":\"0\",\"passenger_type_name\":\"成人\",\"mobile_no\":\"18317894130\",\"phone_no\":\"\",\"email\":\"\",\"address\":\"\",\"postalcode\":\"\",\"first_letter\":\"ZYQ\",\"recordCount\":\"12\",\"total_times\":\"99\",\"index_id\":\"11\"}],\"dj_passengers\":[]},\"messages\":[],\"validateMessages\":{}}";
                //    JObject json = JObject.Parse(contracts);
                //    JArray result = JObject.Parse(json["data"].ToString())[Constant.NormalPassengers] as JArray;
                //    LoadContractComponent(result);
                //}

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + msg, "信息", MessageBoxButton.OK, MessageBoxImage.Warning);
                LogHelper.Log(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, ex.Message + "\r\n" + msg);
                OutputLog(String.Format("出错了:{0} {1}", ex.Message, msg), true);
                LogHelper.Error(ex);
            }

        }

        /// <summary>
        /// 动态加载常用联系人控件
        /// </summary>
        /// <param name="result"></param>
        private void LoadContractComponent(JArray result)
        {
            StackPanel panel = null;
            CheckBox cbAdult = null;
            CheckBox cbStudent = null;
            Passenger passenger = null;
            List<Passenger> _passengerList = new List<Passenger>();
            foreach (var _item in result)
            {
                panel = new StackPanel();
                cbAdult = new CheckBox();
                cbStudent = new CheckBox();
                //给CheckBox注册点击事件
                cbAdult.Click += CheckPassengerTicketType_Click;
                cbStudent.Click += CheckPassengerTicketType_Click;
                panel.HorizontalAlignment = HorizontalAlignment.Left;
                panel.VerticalAlignment = VerticalAlignment.Top;
                panel.Orientation = Orientation.Vertical;
                cbAdult.Uid = _item["passenger_id_no"].ToString();
                cbAdult.Content = _item["passenger_name"].ToString();
                cbAdult.Tag = _item["sex_code"].ToString();
                cbAdult.Foreground = Brushes.Black;
                panel.Children.Add(cbAdult);
                if (_item["passenger_type"].ToString() == "3")//1-成人票 2-儿童 3-学生 4-伤残军人
                {
                    cbStudent.Content = _item["passenger_name"].ToString() + "(学生)";
                    cbStudent.Foreground = Brushes.Black;
                    cbStudent.Uid = _item["passenger_id_no"].ToString();
                    cbStudent.Tag = _item["sex_code"].ToString();
                    panel.Children.Add(cbStudent);
                }
                panel.Margin = new Thickness(10, 0, 0, 0);
                stackPassengers.Children.Add(panel);
                passenger = new Passenger()
                {
                    Address = StringHelper.GetFormatVal(_item["address"]),                            //地址
                    BornDate = StringHelper.GetFormatVal(_item["born_date"]),                         //出生日期
                    Code = StringHelper.GetFormatVal(_item["code"]),                                  //code
                    CountryCode = StringHelper.GetFormatVal(_item["country_code"]),                   //country_code
                    Email = StringHelper.GetFormatVal(_item["email"]),                                //email
                    FirstLetter = StringHelper.GetFormatVal(_item["first_letter"]),                   //first_letter
                    IndexID = StringHelper.GetFormatVal(_item["index_id"]),                           //index_id
                    MobileNo = StringHelper.GetFormatVal(_item["mobile_no"]),                         //mobile_no
                    PassengerFlag = StringHelper.GetFormatVal(_item["passenger_flag"]),
                    PassengerIDNo = StringHelper.GetFormatVal(_item["passenger_id_no"].ToString()),
                    PassengerIDTypeCode = StringHelper.GetFormatVal(_item["passenger_id_type_code"]),
                    PassengerIDTypeName = StringHelper.GetFormatVal(_item["passenger_id_type_name"]),
                    PassengerName = StringHelper.GetFormatVal(_item["passenger_name"]),
                    PassengerType = StringHelper.GetFormatVal(_item["passenger_type"]),
                    PassengerTypeName = StringHelper.GetFormatVal(_item["passenger_type_name"]),
                    PhoneNo = StringHelper.GetFormatVal(_item["phone_no"]),
                    PostalCode = StringHelper.GetFormatVal(_item["postalcode"]),
                    RecordCount = StringHelper.GetFormatVal(_item["recordCount"]),
                    SexCode = StringHelper.GetFormatVal(_item["sex_code"]),
                    SexName = StringHelper.GetFormatVal(_item["sex_name"]),
                    TotalTimes = StringHelper.GetFormatVal(_item["total_times"])
                };
                _passengerList.Add(passenger);
            }
            //加入系统缓存
            SystemCache.SetSysObj(Constant.PassengerList, _passengerList);
        }
        //点击选中/取消成人票或者学生票
        private void CheckPassengerTicketType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox cb = sender as CheckBox;
                if (cb.IsChecked == true)
                {
                    List<Passenger> SelectedPassengerList = SystemCache.GetCache().GetObjByKey(Constant.SelectedPassengers) as List<Passenger>;
                    if (SelectedPassengerList == null)
                        SelectedPassengerList = new List<Passenger>();
                    //foreach (var item in SelectedPassengerList)
                    //{
                    //    if (item.PassengerIDNo == cb.Uid) return;
                    //}
                    SelectedPassengerList.Add(new Passenger() { PassengerName = cb.Content.ToString(), PassengerIDNo = cb.Uid, SexCode = cb.Tag.ToString() });
                    dataGridPassenger.ItemsSource = null;
                    dataGridPassenger.ItemsSource = SelectedPassengerList;
                    SystemCache.SetSysObj(Constant.SelectedPassengers, SelectedPassengerList);
                    OutputLog(String.Format("您选择了乘客: {0} {1}", cb.Content, cb.Tag.ToString() == "M" ? "先生" : "女士"), false);
                }
                else
                {
                    List<Passenger> SelectedPassengerList = SystemCache.GetCache().GetObjByKey(Constant.SelectedPassengers) as List<Passenger>;
                    foreach (var item in SelectedPassengerList)
                    {
                        if (item.PassengerIDNo == cb.Uid)
                        {
                            SelectedPassengerList.Remove(item);
                            dataGridPassenger.ItemsSource = null;
                            dataGridPassenger.ItemsSource = SelectedPassengerList;
                            SystemCache.SetSysObj(Constant.SelectedPassengers, SelectedPassengerList);
                            OutputLog(String.Format("您取消选择了乘客: {0} {1}", cb.Content, cb.Tag.ToString() == "M" ? "先生" : "女士"), true);
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                OutputLog(String.Format("出错了:{0}", ex.Message), true);
                LogHelper.Error(ex);
            }
        }

        //隐藏/显示设置区域
        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (borderShowSetting.Uid == "0")
            {
                dataGridTrainList.Height = dataGridTrainList.Height + 225;
                lblBor.Content = "显示设置区域";
                down1.Source = new BitmapImage(new Uri("/12306BySelfService;component/Resources/Images/up.jpg", UriKind.RelativeOrAbsolute));
                down2.Source = new BitmapImage(new Uri("/12306BySelfService;component/Resources/Images/up.jpg", UriKind.RelativeOrAbsolute));
                borderShowSetting.Uid = "1";

                this.wrapSetting.Visibility = Visibility.Collapsed;
            }
            else
            {
                dataGridTrainList.Height = dataGridTrainList.Height - 225;
                lblBor.Content = "隐藏设置区域";
                down1.Source = new BitmapImage(new Uri("/12306BySelfService;component/Resources/Images/down.jpg", UriKind.RelativeOrAbsolute));
                down2.Source = new BitmapImage(new Uri("/12306BySelfService;component/Resources/Images/down.jpg", UriKind.RelativeOrAbsolute));
                borderShowSetting.Uid = "0";

                this.wrapSetting.Visibility = Visibility.Visible;
            }
        }

        //提交订单
        private void BtnSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (_timer.Enabled)
            {
                MessageBox.Show("我们正在为您拼命抢票，请耐心等候哦！");
                return;
            }
            OutputLog("您已开启抢票线程!", true);
            btnSubmit.Foreground = Brushes.Red;
            btnStop.Visibility = Visibility.Visible;
            _timer.Start();
            SystemCache.SetSysObj(Constant.IsStartOrder, _timer.Enabled);

            ////见证奇迹的时候到了，提交订单测试
            //string submitUrl = StringHelper.GetConfigValByKey(Constant.SubmitOrderRequest, false);
            //string secretStr = (SystemCache.GetCache().GetObjByKey(Constant.SelectedTrainList) as List<Train>)[0].TrainSecretStr;
            //secretStr = System.Web.HttpUtility.UrlDecode(secretStr);
            //HttpItem ite = new HttpItem()
            //{
            //    URL = submitUrl,
            //    Referer = "https://kyfw.12306.cn/otn/leftTicket/init",
            //    Cookie = HttpRequest.JSessionCookie + "tk=" + Login.AppTk,
            //    PostData = new Dictionary<string, string>()
            //        {
            //            { "secretStr", secretStr },
            //            { "train_date", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") },
            //            { "back_train_date", DateTime.Now.ToString("yyyy-MM-dd") },
            //            { "tour_flag", "dc" },
            //            { "purpose_codes", "ADULT" },
            //            { "query_from_station_name", "信阳" },
            //            { "query_to_station_name", "上海" },
            //            { "undefined", "" }
            //        }
            //};
            //string subret = HttpRequest.HttpPost(ite);

            //CheckOrderInfo();
        }

        //在正式提交订单之前填充购票信息（非常重要）
        private Boolean CheckOrderInfo()
        {
            //passengerTicketStr字符串格式，多个乘客以_下划线分隔，注意结尾不带_
            //var passengerTicketStr = seat_type + ",0," + ticket_type + "," + passengerName + "," + passenger_id_type_code + "," + passenger_id_no + "," + mobile_no == null ? "" : mobile_no + "," + "N");
            //oldPassengerStr，多个乘客以_下划线分隔，注意结尾要带_
            //var oldPassengerStr = passengerName + "," + passenger_id_type_code + "," + passenger_id_no + ticket_type + "_";
            string passengerTicketStr = "1,0,1,徐坤,1,41282919890919403X,,N";
            string oldPassengerStr = "徐坤,1,41282919890919403X,1_";
            var listTrain = (List<Train>) SystemCache.GetCache().GetObjByKey(Constant.SelectedTrainList);
            if (listTrain != null)
            {

            }
            else
            {
                //MessageBox.Show("您还没有选择车次哦！", "信息", MessageBoxButton.OK, MessageBoxImage.Warning);
                //return false;
            }

            var listPassen = (List<Passenger>) SystemCache.GetCache().GetObjByKey(Constant.SelectedPassengers);
            if (listPassen != null)
            {

            }
            else
            {
                //MessageBox.Show("您还没有选择乘客哦！", "信息", MessageBoxButton.OK, MessageBoxImage.Warning);
                //return false;
            }

            var listSeat = (List<Seat>) SystemCache.GetCache().GetObjByKey(Constant.SelectedSeatList);
            if (listSeat != null)
            {

            }
            else
            {
                //MessageBox.Show("您还没有选择座位哦！", "信息", MessageBoxButton.OK, MessageBoxImage.Warning);
                //return false;
            }
            //提交订单第一步
            string orderInfoUrl = StringHelper.GetConfigValByKey(Constant.CheckOrderInfoUrl, false);
            HttpItem item = new HttpItem()
            {
                URL = orderInfoUrl,
                Referer = "https://kyfw.12306.cn/otn/confirmPassenger/initDc",
                Cookie = HttpRequest.JSessionCookie + "tk=" + Login.AppTk,
                PostData = new Dictionary<string, string>()
                {
                    { Constant.CancelFlag, SystemCache.GetCache().GetObjByKey(Constant.CancelFlag).ToString() },
                    { Constant.BedLevelOrderNum, SystemCache.GetCache().GetObjByKey(Constant.BedLevelOrderNum).ToString() },
                    { "tour_flag", "dc" },//SystemCache.GetCache().GetObjByKey(Constant.TourFlag).ToString()
                    { "REPEAT_SUBMIT_TOKEN", SystemCache.GetCache().GetObjByKey(Constant.GlobalRepeatSubmitToken).ToString() },
                    { "passengerTicketStr", passengerTicketStr },
                    { "oldPassengerStr", oldPassengerStr },
                    { "whatsSelect", "1" },
                    { "_json_att", "" },
                    { "randCode", "" }
                }
            };
            string ret = HttpRequest.HttpPost(item);

            //提交订单第二步
            string getQueue = StringHelper.GetConfigValByKey("GetQueueCountUrl", false);
            HttpItem item2 = new HttpItem()
            {
                URL = getQueue,
                Referer = "https://kyfw.12306.cn/otn/confirmPassenger/initDc",
                Cookie = HttpRequest.JSessionCookie + "tk=" + Login.AppTk,
                PostData = new Dictionary<string, string>()
                {
                    { "train_date", DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") },
                    { "train_no", "40000K11080C" },
                    { "stationTrainCode", "K1108" },
                    { "seatType", "1"},
                    { "fromStationTelecode", "XUN" },
                    { "toStationTelecode", "SHH" },
                    { "leftTicket", SystemCache.GetCache().GetObjByKey(Constant.LeftTicketStr).ToString() },
                    { "purpose_codes", SystemCache.GetCache().GetObjByKey(Constant.PurposeCodes).ToString() },
                    { "train_location", "F2" },
                    { "_json_att", "" },
                    { "REPEAT_SUBMIT_TOKEN", SystemCache.GetCache().GetObjByKey(Constant.GlobalRepeatSubmitToken).ToString() }
                }
            };
            string getQu = HttpRequest.HttpPost(item2);

            //提交订单第三步
            string confirmQueue = StringHelper.GetConfigValByKey(Constant.ConfirmSingleForQueue, false);
            HttpItem item3 = new HttpItem()
            {
                URL = confirmQueue,
                Referer = "https://kyfw.12306.cn/otn/confirmPassenger/initDc",
                Cookie = HttpRequest.JSessionCookie + "tk=" + Login.AppTk,
                PostData = new Dictionary<string, string>()
                {
                    { "passengerTicketStr", passengerTicketStr },
                    { "oldPassengerStr",oldPassengerStr },
                    { "randCode","" },
                    { "purpose_codes", SystemCache.GetCache().GetObjByKey(Constant.PurposeCodes).ToString() },
                    { "key_check_isChange", SystemCache.GetCache().GetObjByKey(Constant.KeyCheckIsChange).ToString() },
                    { "leftTicketStr", SystemCache.GetCache().GetObjByKey(Constant.LeftTicketStr).ToString() },
                    { "train_location", "F2" },
                    { "choose_seats", "" },
                    { "seatDetailType" ,"000" },
                    { "whatsSelect", "1" },
                    { "roomType","00"},
                    { "dwAll", "N" },
                    { "_json_att", "" },
                    { "REPEAT_SUBMIT_TOKEN", SystemCache.GetCache().GetObjByKey(Constant.GlobalRepeatSubmitToken).ToString() }
                }
            };
            string confirm = HttpRequest.HttpPost(item3).ToString();

            //提交订单第四步
            string queryOrder = StringHelper.GetConfigValByKey(Constant.QueryOrderWaitTime, false);
            HttpItem item4 = new HttpItem()
            {
                URL = queryOrder,
                Referer = "https://kyfw.12306.cn/otn/confirmPassenger/initDc",
                Cookie = HttpRequest.JSessionCookie + "tk=" + Login.AppTk,
                PostData = new Dictionary<string, string>()
                {
                    { "random", new Random().Next(100000000, 1000000000).ToString() + new Random().Next(1000, 10000).ToString() },
                    { "tourFlag", "dc" },//SystemCache.GetCache().GetObjByKey(Constant.TourFlag).ToString()
                    { "_json_att", "" },
                    { "REPEAT_SUBMIT_TOKEN", SystemCache.GetCache().GetObjByKey(Constant.GlobalRepeatSubmitToken).ToString() }
                }
            };
            string myOrderInfo1 = HttpRequest.HttpGet(item4).ToString();

            //提交订单第五步
            HttpItem item5 = new HttpItem()
            {
                URL = queryOrder,
                Referer = "https://kyfw.12306.cn/otn/confirmPassenger/initDc",
                Cookie = HttpRequest.JSessionCookie + "tk=" + Login.AppTk,
                PostData = new Dictionary<string, string>()
                {
                    { "random", new Random().Next(100000000, 1000000000).ToString() + new Random().Next(1000, 10000).ToString() },
                    { "tourFlag", "dc" },//SystemCache.GetCache().GetObjByKey(Constant.TourFlag).ToString()
                    { "_json_att", "" },
                    { "REPEAT_SUBMIT_TOKEN", SystemCache.GetCache().GetObjByKey(Constant.GlobalRepeatSubmitToken).ToString() }
                }
            };
            string myOrderInfo2 = HttpRequest.HttpGet(item5).ToString();
            JObject json = JObject.Parse(myOrderInfo2);
            if (JObject.Parse(json["data"].ToString())["orderId"] == null)
            {
                while (true)
                {
                    object obj = SubmitOrder(queryOrder);
                    //继续提交订单
                    if (obj != null)
                    {
                        if (!String.IsNullOrEmpty(obj.ToString()))
                        {
                            MessageBox.Show(String.Format("恭喜你提交订单成功!\r\n订单编号是: {0}", obj.ToString()), "预定成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                            break;
                        }
                    }
                    //停歇3秒钟提交
                    System.Threading.Thread.Sleep(3000);
                }
            }
            else
            {
                if (!String.IsNullOrEmpty(JObject.Parse(json["data"].ToString())["orderId"].ToString()))
                {
                    MessageBox.Show(String.Format("恭喜你提交订单成功!\r\n订单编号是: {0}", String.IsNullOrEmpty(JObject.Parse(json["data"].ToString())["orderId"].ToString()), "预定成功", MessageBoxButton.OK, MessageBoxImage.Asterisk));
                }
                else
                {
                    while (true)
                    {
                        object obj = SubmitOrder(queryOrder);
                        //继续提交订单
                        if (obj != null)
                        {
                            if (!String.IsNullOrEmpty(obj.ToString()))
                            {
                                MessageBox.Show(String.Format("恭喜你提交订单成功!\r\n订单编号是: {0}", obj.ToString()), "预定成功", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                                break;
                            }
                        }
                        //停歇6秒钟提交(否则提交频率太快可能被检测监控)
                        System.Threading.Thread.Sleep(6000);
                    }
                }
            }

            //Stopwatch sw = new Stopwatch();
            //sw.Start();
            //List<Task> ttk = new List<Task>();
            //ParallelLoopResult loop = Parallel.For(0, 10, (i, state) =>
            // {
            //     ttk.Add(Task.Factory.StartNew(() =>
            //     {
            //         //CheckOrderInfo();
            //     }));
            // });
            //bool iscomp = loop.IsCompleted;
            //Task tkx = Task.WhenAny(ttk);
            //sw.Stop();

            //Console.WriteLine(String.Format("运行耗时: {0}", sw.ElapsedMilliseconds));

            return true;
        }

        private object SubmitOrder(string queryOrder)
        {
            //提交订单第五步
            string msg = "";
            try
            {
                HttpItem item5 = new HttpItem()
                {
                    URL = queryOrder,
                    Referer = "https://kyfw.12306.cn/otn/confirmPassenger/initDc",
                    Cookie = HttpRequest.JSessionCookie + "tk=" + Login.AppTk,
                    PostData = new Dictionary<string, string>()
                    {
                        { "random", new Random().Next(100000000, 1000000000).ToString() + new Random().Next(1000, 10000).ToString() },
                        { "tourFlag", "dc" },//SystemCache.GetCache().GetObjByKey(Constant.TourFlag).ToString()
                        { "_json_att", "" },
                        { "REPEAT_SUBMIT_TOKEN", SystemCache.GetCache().GetObjByKey(Constant.GlobalRepeatSubmitToken).ToString() }
                    }
                };
                string myOrderInfo2 = HttpRequest.HttpGet(item5).ToString();
                msg = myOrderInfo2;
                JObject json = JObject.Parse(myOrderInfo2);
                return JObject.Parse(json["data"].ToString())["orderId"];
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message + "\r\n" + msg, "信息", MessageBoxButton.OK, MessageBoxImage.Error);
                OutputLog(String.Format("出错了:{0}  {1}", ex.Message, msg), true);
                LogHelper.Error(msg, ex);
                return null;
            }
        }

        string scale = String.Empty;
        string btnCont = "正在拼命为你抢票";
        private void Timer_Tick(object sender, EventArgs e)
        {
            scale += ".";
            btnSubmit.Content = btnCont + scale;
            if (scale.Length == 6)
            {
                scale = String.Empty;
            }
        }

        //说明区域不允许出现右键选择功能
        private void listBoxNote_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.RightButton == MouseButtonState.Pressed)
            {
                e.Handled = false;
            }
        }

        public void OutputLog(string log, bool isWarn)
        {
            TextBlock tbk = new TextBlock();
            tbk.Text = String.Format("{0} {1}", DateTime.Now.ToString("HH:mm:ss"), log);
            tbk.TextWrapping = TextWrapping.WrapWithOverflow;
            if (isWarn) tbk.Foreground = Brushes.Red;
            listLog.Items.Add(tbk);
            //scrollViewer.ScrollToEnd();
            listLog.ScrollIntoView(listLog.Items[listLog.Items.Count - 1]);
        }

        private void BtnStop_Click(object sender, RoutedEventArgs e)
        {
            _timer.Stop();
            btnStop.Visibility = Visibility.Collapsed;
            btnSubmit.Content = "开 始 抢 票";
            btnSubmit.Foreground = Brushes.Green;
            OutputLog("已停止抢票线程!", true);
        }

        //最后确认选中席位类型
        private void CbxCheckSeatType_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CheckBox cbx = sender as CheckBox;
                List<string> checkSeatList = new List<string>();
                if (cbx.IsChecked == true)
                {
                    List<Seat> SelectedSeatList = SystemCache.GetCache().GetObjByKey(Constant.SelectedSeatList) as List<Seat>;
                    if (SelectedSeatList == null)
                        SelectedSeatList = new List<Seat>();
                    SelectedSeatList.Add(new Seat() { SeatName = cbx.Content.ToString(), SeatCode = cbx.Tag.ToString(), SeatType = cbx.Uid });
                    SystemCache.SetSysObj(Constant.SelectedSeatList, SelectedSeatList);
                    OutputLog(String.Format("您选择了 {0}", cbx.Content), false);
                }
                else
                {
                    List<Seat> SelectedSeatList = SystemCache.GetCache().GetObjByKey(Constant.SelectedSeatList) as List<Seat>;
                    foreach (var item in SelectedSeatList)
                    {
                        if (item.SeatType == cbx.Uid)
                        {
                            SelectedSeatList.Remove(item);
                            SystemCache.SetSysObj(Constant.SelectedSeatList, SelectedSeatList);
                            OutputLog(String.Format("您取消了 {0}", cbx.Content), true);
                            break;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                OutputLog(String.Format("错误信息:{0}", exception.Message), true);
            }
        }

        //左下角复选框二次选择乘客事件
        private void CbHasCheckPassenger_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;
            if (cbx.IsChecked == true)
            {
                OutputLog(String.Format("您再次选择了乘客 {0} {1}", cbx.Content, cbx.Tag.ToString() == "M" ? "先生" : "女士"), false);
            }
            else
            {
                OutputLog(String.Format("您再次取消选择乘客 {0} {1}", cbx.Content, cbx.Tag.ToString() == "M" ? "先生" : "女士"), true);
            }
        }
        //添加更多乘车日期
        private void dateTakeMore_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            List<Object> list = SystemCache.GetCache().GetObjByKey(Constant.SelectedTakeTrainDateList) as List<Object>;
            string item = Convert.ToDateTime(dateTakeMore.SelectedDate).ToString("yyyy-MM-dd");
            List<object> lst = new List<object>();
            lst = (from t in list where t.ToString().Contains(item) select t).ToList();
            if (lst.Count == 0)
            {
                dataGridCheckedDate.ItemsSource = null;
                list.Add(new { TrainDateMore = item });
                SystemCache.SetSysObj(Constant.SelectedTakeTrainDateList, list);
                dataGridCheckedDate.ItemsSource = list;
                OutputLog(String.Format("您新添加了乘车日期: {0}", item), false);
            }
            else
            {
                OutputLog(String.Format("您已经添加了乘车日期: {0}，不能重复添加！", item), true);
            }
        }

        private void dateTakeMore_MouseEnter(object sender, MouseEventArgs e)
        {
            popMoreDate.IsOpen = true;
        }

        //删除乘车日期
        private void cbxCheckMoreDate_Click(object sender, RoutedEventArgs e)
        {
            CheckBox cbx = sender as CheckBox;
            List<Object> list = SystemCache.GetCache().GetObjByKey(Constant.SelectedTakeTrainDateList) as List<Object>;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].ToString().Contains(cbx.Content.ToString()))
                {
                    list.RemoveAt(i);
                    break;
                }
            }
            dataGridCheckedDate.ItemsSource = null;
            dataGridCheckedDate.ItemsSource = list;
            SystemCache.SetSysObj(Constant.SelectedTakeTrainDateList, list);
            OutputLog(String.Format("您再次取消了乘车日期: {0}", cbx.Content), true);
        }

        //删除乘客
        private void psengDelete_Click(object sender, RoutedEventArgs e)
        {
            Passenger psg = dataGridPassenger.SelectedItem as Passenger;
            List<Passenger> SelectedPassengerList = SystemCache.GetCache().GetObjByKey(Constant.SelectedPassengers) as List<Passenger>;
            SelectedPassengerList.Remove(psg);
            dataGridPassenger.ItemsSource = null;
            dataGridPassenger.ItemsSource = SelectedPassengerList;
            List<Passenger> allPsgList = SystemCache.GetCache().GetObjByKey(Constant.PassengerList) as List<Passenger>;
            Passenger temp = allPsgList.Find(p => p.PassengerIDNo == psg.PassengerIDNo) as Passenger;
            //Passenger temp = (from p in allPsgList where p.PassengerIDNo == psg.PassengerIDNo select p).ToList()[0];
            OutputLog(String.Format("您删除了乘客 {0} {1}", temp.PassengerName, temp.SexCode == "M" ? "先生" : "女士"), true);
        }
    }
}
