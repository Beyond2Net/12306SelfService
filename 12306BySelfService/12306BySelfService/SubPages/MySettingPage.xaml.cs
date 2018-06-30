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
using NativeWifi;
using System.Data;
using System.Threading;
using TrainCommon;
using System.Windows.Threading;
using System.Configuration;

namespace _12306BySelfService.SubPages
{
    /// <summary>
    /// MySettingPage.xaml 的交互逻辑
    /// </summary>
    public partial class MySettingPage : Page
    {
        private IntPtr clientHandle = new IntPtr();
        Wlan.WlanAvailableNetwork[] wlanAvailableNetworks;
        EmailHelper emailHelper = new EmailHelper();
        private delegate void UpdateProgressBarDelegate(DependencyProperty dp, Object value);
        public MySettingPage()
        {
            InitializeComponent();
            this.Loaded += MySettingPage_Loaded;
            string aa = SystemParameters.WorkArea.Width.ToString();
        }

        private void MySettingPage_Loaded(object sender, RoutedEventArgs e)
        {
            List<Object> dataList = GetAvailableNetworkList1();
            dataGridWlan1.ItemsSource = dataList != null ? GetAvailableNetworkList1() : null;
            dataGridWlan2.ItemsSource = GetAvailableNetworkList2();
            string ipv4 = PathUtil.GetIPv4();
            Wlan.WlanConnectionAttributes wlanAttr = WlanClient.GetWlanConnectInfo();
            if (wlanAttr.profileName != null)
            {
                string macAddress = GetMacAddress(wlanAttr.wlanAssociationAttributes.dot11Bssid);
                tbState.Text = String.Format("当前设备已连接到Wlan: {0}, Mac地址: {1} IPv4地址: {2}", wlanAttr.profileName, macAddress, PathUtil.GetIPv4());
            }
            else
            {
                tbState.Text = string.Format("网络数据连接IPv4: {0}", PathUtil.GetIPV4ByManagementClass());
            }

            InitializeEmailConfiguration();
        }

        private List<Object> GetAvailableNetworkList1()
        {
            WlanClient wlanClient = null;
            WlanClient.WlanInterface wlanInterface = null;
            List<Object> wlanList = null;
            try
            {
                wlanClient = new WlanClient();
            }
            catch (Exception exception)
            {
                Wlan.WlanCloseHandle(clientHandle, IntPtr.Zero);
                LogHelper.Log(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, exception.Message);
                LogHelper.Error(exception);
                MessageBox.Show(exception.Message, "信息", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            if (wlanClient != null && wlanClient.Interfaces.Length != 0)
            {
                wlanList = new List<object>();
                wlanInterface = wlanClient.Interfaces[0];
                WlanClient.WlanInterface wlanInterfaceTmp = wlanClient.Interfaces[0];
                Wlan.WlanAvailableNetwork[] availableNetworks = wlanInterfaceTmp.GetAvailableNetworkList(Wlan.WlanGetAvailableNetworkFlags.IncludeAllAdhocProfiles);
                wlanAvailableNetworks = availableNetworks;
                
                int index = 1;
                foreach (Wlan.WlanAvailableNetwork wlanAvailableNetworkTmp in availableNetworks)
                {
                    bool isConnected = Wlan.WlanAvailableNetworkFlags.Connected.ToString() == wlanAvailableNetworkTmp.flags.ToString().Split(',')[0] && wlanAvailableNetworkTmp.networkConnectable;
                    wlanList.Add(new
                    {
                        IndexID = index,
                        NetworkName = GetStringForSSID(wlanAvailableNetworkTmp.dot11Ssid),
                        SignalQuality = wlanAvailableNetworkTmp.wlanSignalQuality.ToString() + "%",
                        SignalStrength = StringHelper.ClassifySignalByStrength(Convert.ToInt32(wlanAvailableNetworkTmp.wlanSignalQuality)),
                        NetworkSecurity = wlanAvailableNetworkTmp.securityEnabled ? "安全" : "不安全",
                        NetworkFlag = Convert.ToInt32(wlanAvailableNetworkTmp.flags),
                        Connectable = wlanAvailableNetworkTmp.networkConnectable ? "可连接" : "不可访问",
                        ConnectState = isConnected ? "已连接使用中" : "未连接",
                    });
                    index++;
                }
            }
            return wlanList;
        }

        private List<Object> GetAvailableNetworkList2()
        {
            WlanClient wlanClient = null;
            WlanClient.WlanInterface wlanInterface = null;
            List<Object> wlanList = new List<object>();
            try
            {
                wlanClient = new WlanClient();
            }
            catch (Exception exception)
            {
                Wlan.WlanCloseHandle(clientHandle, IntPtr.Zero);
                LogHelper.Log(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, exception.Message);
                LogHelper.Error(exception);
            }

            if (wlanClient != null && wlanClient.Interfaces.Length != 0)
            {
                wlanInterface = wlanClient.Interfaces[0];
                WlanClient.WlanInterface wlanInterfaceTmp = wlanClient.Interfaces[0];
                Wlan.WlanBssEntry[] bssworks = wlanInterface.GetNetworkBssList();
                string temp = string.Empty;
                int index = 1;
                foreach (Wlan.WlanBssEntry bsswork in bssworks)
                {
                    bool isConnected = bsswork.inRegDomain;
                    temp = GetMacAddress(bsswork.dot11Bssid);
                    int rssi = bsswork.rssi;
                    string apssid = GetStringForSSID(bsswork.dot11Ssid);
                    wlanList.Add(new
                    {
                        IndexID = index,
                        NetworkName = GetStringForSSID(bsswork.dot11Ssid),
                        SignalQuality = bsswork.linkQuality.ToString() + "%",
                        SignalStrength = StringHelper.ClassifySignalByStrength(Convert.ToInt32(bsswork.linkQuality)),
                        NetworkSecurity = bsswork.inRegDomain ? "安全" : "不安全",
                        NetworkFlag = Convert.ToInt32(bsswork.chCenterFrequency),
                        Connectable = bsswork.inRegDomain ? "可连接" : "不可访问",
                        ConnectState = isConnected ? "已连接使用中" : "未连接",
                        PhysicsId = bsswork.phyId,
                        Frequency = bsswork.hostTimestamp
                    });
                    index++;
                }
            }

            return wlanList;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            MenuItem menu = sender as MenuItem;
            object drv = dataGridWlan1.SelectedItem as object;
            switch (menu.Name)
            {
                case "ItemDelete":
                    List<object> list = GetAvailableNetworkList1();
                    list.Remove(drv);
                    Dispatcher.Invoke(() =>
                    {
                        this.dataGridWlan1.ItemsSource = null;
                        this.dataGridWlan1.ItemsSource = list;
                    });
                    break;
                case "ItemEdit":
                    break;
                case "ItemRefresh":
                    Dispatcher.Invoke(() => {
                        List<Object> dataList = GetAvailableNetworkList1();
                        if (dataList == null) return;
                        dataGridWlan1.ItemsSource = dataList;
                    });
                    break;
                case "ItemAdd":
                    break;
            }
        }

        private void btnConnectWlan_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                WlanClient wlanClientTmp = new WlanClient();

                if (wlanClientTmp.Interfaces.Length != 0)
                {
                    WlanClient.WlanInterface wlanInterfaceTmp = wlanClientTmp.Interfaces[0];
                    wlanInterfaceTmp.WlanConnectionNotification += new WlanClient.WlanInterface.WlanConnectionNotificationEventHandler(wlanInterfaceTmp_WlanConnectionNotification);
                    
                    int index = dataGridWlan1.SelectedIndex;
                    wlanInterfaceTmp.Connect(Wlan.WlanConnectionMode.Profile, wlanAvailableNetworks[index].dot11BssType, wlanAvailableNetworks[index].profileName);
                    
                    tbState.Text = "正在连接网络：" + wlanAvailableNetworks[index].profileName;
                }
            }
            catch (Exception exp)
            {
                tbState.Foreground = Brushes.Red;
                tbState.Text = exp.Message + "1.网络不可用  2.或者您没有权限连接.";
                MessageBox.Show(exp.Message + "\r\n原因：\r\n1.网络不可用.\r\n2.您没有权限连接", "信息", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void wlanInterfaceTmp_WlanConnectionNotification(Wlan.WlanNotificationData notifyData, Wlan.WlanConnectionNotificationData connNotifyData)
        {
            if (connNotifyData.profileName != "")
            {
                WlanClient wlanClientTmp = new WlanClient();
                WlanClient.WlanInterface wlanInterfaceTmp = wlanClientTmp.Interfaces[0];
                Dispatcher.Invoke(() =>
                {
                    List<Object> dataList = GetAvailableNetworkList1();
                    if (dataList == null) return;
                    dataGridWlan1.ItemsSource = dataList;
                    if (wlanInterfaceTmp.IsWlanConnection())
                    {
                        Wlan.WlanConnectionAttributes wlanAttr = wlanInterfaceTmp.CurrentConnection;
                        if (wlanAttr.isState == Wlan.WlanInterfaceState.Connected)
                        {
                            string macAddress = GetMacAddress(wlanAttr.wlanAssociationAttributes.dot11Bssid);
                            tbState.Text = String.Format("当前设备已连接到Wlan: {0}, Mac地址: {1} IPv4地址: {2}", wlanAttr.profileName, macAddress, PathUtil.GetIPv4());

                        }
                    }
                });
            }
        }

        public string GetMacAddress(byte[] macAddr)
        {
            string tMac = "";
            for (int i = 0; i < macAddr.Length; i++)
            {
                tMac += macAddr[i].ToString("x2").PadLeft(2, '0').Insert(2, ":");
            }
            return tMac.TrimEnd(':');
        }

        public string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.ASCII.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        private void InitializeEmailConfiguration()
        {
            try
            {
                this.txtServerName.Text = StringHelper.GetConfigValByKey("MailServerName");
                this.txtServerPort.Text = StringHelper.GetConfigValByKey("MailServerPort");
                this.txtUserName.Text = StringHelper.GetConfigValByKey("UserName");
                this.txtPassword.Password = StringHelper.GetConfigValByKey("Password");
                this.txtPasswordConfirm.Password = this.txtPassword.Password;
                this.comboxMailPority.SelectedIndex = Int32.Parse(StringHelper.GetConfigValByKey("mail_priority"));
                this.txtReceive.Text = StringHelper.GetConfigValByKey("mail_to_address");
                this.isSSL.IsChecked = StringHelper.GetConfigValByKey("mail_isSSL") == "1" ? true : false;

                string[] contractEmail = ConfigurationManager.AppSettings["ContractsEamilAddress"].Split('$');
                foreach (var item in contractEmail)
                {
                    if (!string.IsNullOrEmpty(item.Trim()))
                    {
                        this.comboBox_CC.Items.Add(item);
                    }
                }
            }
            catch (Exception ex)
            {
                EmailHelper.ShowMessage(ex);
            }
        }

        private void buttonSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.Valid())
                {
                    StringHelper.SetConfig("MailServerName", this.txtServerName.Text);
                    StringHelper.SetConfig("MailServerPort", this.txtServerPort.Text);
                    StringHelper.SetConfig("UserName", this.txtUserName.Text);
                    StringHelper.SetConfig("Password", this.txtPassword.Password);
                    StringHelper.SetConfig("mail_priority", this.comboxMailPority.SelectedIndex.ToString());
                    StringHelper.SetConfig("mail_cc_addresses", this.comboBox_CC.Text);
                    StringHelper.SetConfig("mail_to_address", this.txtReceive.Text);
                    StringHelper.SetConfig("mail_isSSL", this.isSSL.IsChecked == true ? "1" : "0");

                    MessageBox.Show("保存成功！", "信息", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                }
            }
            catch (Exception ex)
            {
                EmailHelper.ShowMessage(ex);
            }
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void buttonCheckConnect_Click(object sender, RoutedEventArgs e)
        {
            if (this.Valid())
            {
                this.buttonCheckConnect.Margin = new Thickness(buttonCheckConnect.Margin.Left, buttonCheckConnect.Margin.Top - 15, buttonCheckConnect.Margin.Right, buttonCheckConnect.Margin.Bottom);
                Dispatcher.Invoke(() =>
                {
                    this.emailConnBar.Visibility = Visibility.Visible;
                    this.buttonSave.IsEnabled = false;
                });

                //this.timer.Start();
                try
                {
                    emailHelper.MailServer = this.txtServerName.Text.Trim();
                    emailHelper.MailPort = Convert.ToInt32(this.txtServerPort.Text.Trim());
                    emailHelper.UserName = this.txtUserName.Text.Trim();
                    emailHelper.Password = this.txtPassword.Password.Trim();
                    emailHelper.MailTo = this.txtReceive.Text.Trim();
                    emailHelper.MailPority = this.comboxMailPority.SelectedIndex;
                    emailHelper.MailCC = this.comboBox_CC.Text.Trim();
                    emailHelper.IsSSL = this.isSSL.IsChecked == true ? true : false;

                    BeginUpdateProgressBar();
                    //bool conn = await this.emailHelper.CheckSmtp(this.txtServerName.Text, int.Parse(txtServerPort.Text), txtUserName.Text, txtPasswordConfirm.Password);
                    int ret = await emailHelper.SelfServiceSendMail();
                    //ret返回值 0失败; 1成功
                    Dispatcher.Invoke(() =>
                    {
                        this.buttonCheckConnect.Content = "测试连接";
                        this.buttonCheckConnect.IsEnabled = true;
                        buttonSave.IsEnabled = true;
                        this.emailConnBar.Visibility = Visibility.Hidden;
                        this.buttonCheckConnect.Margin = new Thickness(buttonCheckConnect.Margin.Left, buttonCheckConnect.Margin.Top + 15, buttonCheckConnect.Margin.Right, buttonCheckConnect.Margin.Bottom);
                    });
                }
                catch (Exception ex)
                {
                    EmailHelper.ShowMessage(ex);
                }
            }
        }

        private void BeginUpdateProgressBar()
        {
            this.emailConnBar.Maximum = 100;
            this.emailConnBar.Value = 0;
            UpdateProgressBarDelegate updatePbDelegate = new UpdateProgressBarDelegate(this.emailConnBar.SetValue);
            for (int i = 0; i < 100; i++)
            {
                System.Threading.Thread.Sleep(5);
                Dispatcher.Invoke(updatePbDelegate,
                    DispatcherPriority.Background,
                    new object[] { ProgressBar.ValueProperty, Convert.ToDouble(i + 1) });

                Dispatcher.Invoke(() =>
                {
                    this.buttonCheckConnect.Content = "正在连接测试请勿关闭......";
                    this.buttonCheckConnect.IsEnabled = false;
                });
            }

        }

        private void isSSL_Checked(object sender, RoutedEventArgs e)
        {
            this.isSSL.Foreground = Brushes.Red;
        }

        private bool Valid()
        {
            if (string.IsNullOrEmpty(this.txtServerName.Text))
            {
                EmailHelper.ShowMessage("邮箱服务器名称不能为空！");
                return false;
            }

            if (string.IsNullOrEmpty(this.txtServerPort.Text))
            {
                MessageBox.Show("邮箱服务端口号不能为空！");
                return false;
            }
            int port = 0;
            if (!Int32.TryParse(this.txtServerPort.Text, out port))
            {
                EmailHelper.ShowMessage("邮箱服务端口号必须为整数！");
                return false;
            }

            if (string.IsNullOrEmpty(this.txtUserName.Text))
            {
                EmailHelper.ShowMessage("发件人邮箱用户名不能为空！");
                return false;
            }

            if (!emailHelper.VerifyEmailFormat(this.txtUserName.Text))
            {
                EmailHelper.ShowMessage("发件人邮箱格式不正确！");
                return false;
            }

            if (string.IsNullOrEmpty(this.txtReceive.Text))
            {
                EmailHelper.ShowMessage("收件人邮箱用户名不能为空！");
                return false;
            }

            if (!emailHelper.VerifyEmailFormat(this.txtReceive.Text))
            {
                EmailHelper.ShowMessage("收件人邮箱格式不正确！");
                return false;
            }

            string password = this.txtPassword.Password;
            string confirmPassword = this.txtPasswordConfirm.Password;
            if (string.IsNullOrEmpty(password))
            {
                EmailHelper.ShowMessage("密码不能为空！");
                return false;
            }
            if (password != confirmPassword)
            {
                EmailHelper.ShowMessage("两次密码输入不一致！");
                return false;
            }
            return true;
        }
    }
}
