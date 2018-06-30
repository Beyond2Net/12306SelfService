using _12306BySelfService.Common;
using _12306BySelfService.SelfServiceReference;
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
using System.Windows.Shapes;
using System.Windows.Threading;
using TrainCommon.Model;
using TrainCommon;

namespace _12306BySelfService
{
    /// <summary>
    /// Loaded.xaml 的交互逻辑
    /// </summary>
    public partial class Loaded : Window
    {
        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, Object value);
        TrainServiceClient client;
        public Loaded()
        {
            InitializeComponent();
            log4net.Config.XmlConfigurator.ConfigureAndWatch(new System.IO.FileInfo("Log4Net.config"));
            //log4net.Config.XmlConfigurator.Configure();
            this.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            client = TrainClientSVC.GetTrainSVC().GetClientService();
            Dispatcher.Invoke(() =>
            {
                Task.Factory.StartNew(InitializeStation);
                Task.Run(() => InitAllSaleStation());
            });
        }

        private void InitAllSaleStation()
        {
            string url = StringHelper.GetConfigValByKey(Constant.StartSaleTime, false);
            HttpItem item = new HttpItem()
            {
                URL = url
            };
            string ret = TrainCommon.HttpRequest.HttpGet(item).ToString();
            string retx = StringHelper.GetValueBetween(ret, "<div id=\"pretime\">", "</div>");
            string temp = retx.Replace("<p>", "").Replace("</p>", "").Replace("<b>", "").Replace("</b>", "").Replace("&nbsp;", "").Replace("<br />", "").Trim();
            string[] tempArr = temp.Split('。');
            for (int i = 0; i < tempArr.Length - 1; i++)
            {
                string[] tempArr2 = tempArr[i].Trim().Replace("\n", "").Replace("起售车站", "$").Trim().Split('$');
                Dictionary<String, List<String>> dict = SystemCache.GetCache().GetObjByKey(Constant.AllSaleStation) as Dictionary<String, List<String>>;
                if (dict == null)
                    dict = new Dictionary<String, List<String>>();
                dict.Add(tempArr2[0].Trim(), tempArr2[1].Trim().Split('、').ToList());
                SystemCache.SetSysObj(Constant.AllSaleStation, dict);
            }
        }

        private void InitializeStation()
        {
            string urlStation = StringHelper.GetConfigValByKey(Constant.StationNameUrl, false);
            string urlInitial = StringHelper.GetConfigValByKey(Constant.InitialUrl, false);
            string urlAuth = StringHelper.GetConfigValByKey(Constant.Auth, false);
            //LogHelper.WriteTextLog("Loaded", url);
            LogHelper.Info(urlStation);
            Station station = null;
            String[] stations = null;
            try
            {
                //stations = client.HttpGet(url, String.Empty).ToString().Split('@');
                HttpItem item = new HttpItem() { URL = urlStation };
                stations = HttpRequest.HttpGet(item).ToString().Split('@');
                //初始化生成客户端环境Cookie
                HttpItem itemInit = new HttpItem()
                {
                    URL = urlInitial,
                    Referer = @"http://www.12306.cn/mormhweb/",
                    IsInitialLoaded = true
                };
                HttpRequest.HttpGet(itemInit);

                HttpItem item2 = new HttpItem()
                {
                    URL = urlAuth,
                    Referer = @"https://kyfw.12306.cn/otn/login/init",
                    PostData = new Dictionary<string, string>() { { "appid", "otn" } },
                    IsInitialLoaded = true
                };
                HttpRequest.HttpPost(item2);
            }
            catch (Exception ex)
            {
                MessageBoxResult dialog = MessageBox.Show("请排查以下可能故障：\r\n1.请检查网络故障 \r\n2.请确认12306官网是否可以登录\r\n3." + ex.Message + "4.关闭程序重新打开", "出错了", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                if (dialog == MessageBoxResult.OK)
                {
                    Task.Factory.StartNew(InitializeStation);
                    LogHelper.Error(ex.Message);
                    return;
                }
                else
                {
                    Environment.Exit(0);
                }
            }
            UpdateProgressBarDelegate updatePbDelegate = this.InitialProgressBar(stations.Length);
            var i = 0;
            //item = "sha|上海|SHH|shanghai|sh|10"
            foreach (var item in stations)
            {
                if (item.Contains("station_names"))
                {
                    continue;
                }
                if (!String.IsNullOrEmpty(item))
                {
                    var items = item.Split('|');
                    station = new Station(items[0], items[1], items[2], items[3], items[4], items[5]);
                    SystemCache.SetTrainStation(item.Split('|')[1], station);
                }
                i++;
                this.UpdateUIInfo(updatePbDelegate, i, station);
            }
            Dispatcher.Invoke(() =>
            {
                this.Hide();
                new Login().Show();
                string msg = String.Format("加载完毕，共{0}个车站。", stations.Length);
                LogHelper.Auth(msg);
                LogHelper.Info(msg);
                LogHelper.Log(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, msg);
            });
        }

        private void UpdateUIInfo(UpdateProgressBarDelegate updatePbDelegate, int progressBarValue, Station station)
        {
            Dispatcher.Invoke(updatePbDelegate,
                              DispatcherPriority.Background,
                              new object[] { ProgressBar.ValueProperty, Convert.ToDouble(progressBarValue + 1) }
                             );
            Dispatcher.Invoke(() =>
            {
                string str = String.Format("正在加载车站:{0},编号{1}, {2}, {3}, {4}, {5}, ", station.StationName, station.StationNo, station.StationCode, station.StationFullName, station.StationID, station.StationSimpleName);
                this.lblInfo.Content = string.Format(str, progressBarValue, station.StationName, station.StationCode);
            });
        }

        private UpdateProgressBarDelegate InitialProgressBar(Int32 value)
        {
            Dispatcher.Invoke(new Action(delegate
            {
                this.progressBar.Maximum = value;
                this.progressBar.Value = 0;
            }));
            return new UpdateProgressBarDelegate(this.progressBar.SetValue);
        }

    }
}
