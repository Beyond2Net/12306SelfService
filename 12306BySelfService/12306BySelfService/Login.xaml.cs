using _12306BySelfService.Common;
using _12306BySelfService.SelfServiceReference;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
using Newtonsoft.Json.Linq;

namespace _12306BySelfService
{
    /// <summary>
    /// Login.xaml 的交互逻辑
    /// </summary>
    public partial class Login : Window
    {
        TrainServiceClient client;
        internal Boolean callBack = false;
        public bool isManualClose { get; set; }
        private readonly System.Windows.Forms.Timer _timer = new System.Windows.Forms.Timer();
        private readonly System.Windows.Forms.Timer _timer2 = new System.Windows.Forms.Timer();
        private static string _loginStatus = "正在获取验证码";
        public string UserName { get; set; }
        public string Password { get; set; }
        /// <summary>
        /// 登录成功之后的App token 
        /// </summary>
        public static string AppTk { get; set; }

        public Login()
        {
            InitializeComponent();
            client = TrainClientSVC.GetTrainSVC().GetClientService();
            this.Loaded += Login_Loaded;
            _timer.Interval = 500;
            _timer.Tick += _timer_Tick;

            _timer2.Interval = 2000;
            _timer2.Tick += _timer2_Tick;
            _timer2.Start();
        }

        private void Login_Loaded(object sender, RoutedEventArgs e)
        {
            this.tbUserName.Text = StringHelper.GetConfigValByKey(Constant.UserName);
            this.tbPassword.Password = StringHelper.GetConfigValByKey(Constant.Password);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MessageBoxResult mbr = MessageBox.Show("确定退出吗？", "信息", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbr == MessageBoxResult.Yes)
            {
                //Application.Current.Shutdown();
                LogHelper.Log(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, "退出程序");
                SystemCache.ClearCache();
                Environment.Exit(0);//退出应用程序
            }
            else
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BtnLogin_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateLogin())
                {
                    ChangeUserNameReadOnly(true);
                    Process();
                    Dispatcher.Invoke(() =>
                    {
                        this.btnLogin.Content = "登    录";
                    });
                }
            }
            catch (Exception ex)
            {
                _timer.Stop();
                MessageBoxResult dialog = MessageBox.Show("下载验证码失败！\r\n" + ex.Message + "\r\n确定重试 ？", "信息提示", MessageBoxButton.OKCancel, MessageBoxImage.Error);
                LogHelper.Log(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, ex.Message);
                if (dialog == MessageBoxResult.OK)
                {
                    Process();
                }
                else
                {
                    Environment.Exit(0);//退出应用程序
                }
            }
        }

        /// <summary>
        /// 当子窗口验证码校验成功时则触发委托事件
        /// </summary>
        /// <param name="callback"></param>
        private void VerifyForm_ChangeTextEvent(bool callback)
        {
            //校验是否验证通过
            this.callBack = callback;
        }

        /// <summary>
        /// 异步登录验证
        /// </summary>
        private async void Process()
        {
            try
            {
                _timer.Start();
                string url = StringHelper.GetConfigValByKey(Constant.VerifyCodeUrl, false) + "&" + new Random().NextDouble().ToString() + new Random().Next(0, 10);
                string path = StringHelper.VirtualPath;
                //path = client.HttpGet(url, path).ToString();
                path = HttpRequest.HttpGet(url, path).ToString();
                VerifyCode verifyForm = new VerifyCode(path)
                {
                    _loginWindow = this
                };
                verifyForm.ChangeTextEvent += VerifyForm_ChangeTextEvent;
                bool? close = verifyForm.ShowDialog();
                if (close != null && ((Boolean)close && callBack)) //仅仅点击提交按钮并且验证通过表示登录成功
                {
                    _loginStatus = "正在登录";
                    Dictionary<String, String> parames = new Dictionary<string, string>()
                        {
                            {"username", UserName},
                            {"password", Password},
                            {"appid", "otn"}
                        };
                    string loginUrl = StringHelper.GetConfigValByKey("LOGINURL", false);
                    //string ret = client.HttpPost(loginUrl, parames).ToString();
                    await Task.Run(() =>
                    {
                        HttpItem item = new HttpItem()
                        {
                            URL = loginUrl,
                            PostData = parames,
                            Cookie = HttpRequest.BeforLoginCookie
                        };
                        string ret = HttpRequest.HttpPost(item).ToString();
                        BackData back = JsonConvert.DeserializeObject<BackData>(ret);
                        /**************************************************************************************************************
                         * { "result_message":"密码输入错误。如果输错次数超过4次，用户将被锁定。","result_code":1 }
                         * { "result_message":"登录名不存在。","result_code":1 }
                         * { "result_message":"登录成功","result_code":0,"uamtk":"tWDQtPie_z22IWMknmFOymUpDRzvLE4CfzREJBzS9NwrwL2L0" }
                         *************************************************************************************************************/
                        if(back == null)//校验失败
                        {
                            back = new BackData()
                            {
                                result_code = "-1", result_message = "校验失败", uamtk = "ABCD"
                            };
                        }

                        if(back.result_code == "0") //登录成功
                        {
                            _timer.Stop(); _timer.Dispose();
                            if (SystemCache.GetCache().GetObjByKey(Constant.AppToken) == null)
                                this.AuthClient(back.uamtk); //初始化验证信息
                            if (SystemCache.GetCache().GetObjByKey(Constant.JSessionCookieID) == null)
                                this.MakeClientJSession();
                            if (SystemCache.GetCache().GetObjByKey(Constant.LeftTicketQueryUrl) == null)
                                this.InitLeftTicketQueryUrl();

                            Dispatcher.Invoke(() =>
                            {
                                this.btnLogin.Content = "登 录 成 功";
                                this.Hide();
                                MainWindow main = new MainWindow();
                                main._loginWindow = this;
                                main.Show();
                                this.UserName = this.tbUserName.Text.Trim();
                                this.Password = this.tbPassword.Password.Trim();
                            });
                            //缓存用户名密码
                            StringHelper.SetConfig(Constant.UserName, UserName);
                            StringHelper.SetConfig(Constant.Password, Password);
                            LogHelper.Log(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, back.result_message);
                        }
                        else //登录失败
                        {
                            _timer.Stop();_timer.Dispose();
                            MessageBox.Show(back.result_message, "信息", MessageBoxButton.OK, MessageBoxImage.Warning);
                            Dispatcher.Invoke(() =>
                            {
                                this.btnLogin.Content = "登    录";
                                this.btnLogin.Foreground = Brushes.Black;
                            });
                        }
                    });
                    ChangeUserNameReadOnly(false);
                }
                //仅仅手动关闭验证码窗口
                if (close != null && ((Boolean)close) && !callBack && isManualClose)
                {
                    Dispatcher.Invoke(() => ChangeUserNameReadOnly(false));
                }
            }
            catch (Exception ex)
            {
                //登录失败
                MessageBox.Show(ex.Message, "信息", MessageBoxButton.OK, MessageBoxImage.Warning);
                Process();

                LogHelper.Error(ex);
            }
        }
        //登录验证
        private bool ValidateLogin()
        {
            if (String.IsNullOrEmpty(this.tbUserName.Text.Trim()))
            {
                this.popUserName.IsOpen = true;
            }
            else if (String.IsNullOrEmpty(this.tbPassword.Password.Trim()))
            {
                this.popPassword.IsOpen = true;
                //popUserName.Placement = System.Windows.Controls.Primitives.PlacementMode.Mouse;
            }
            else
            {
                this.UserName = this.tbUserName.Text.Trim();
                this.Password = this.tbPassword.Password.Trim();
                return true;
            }
            return false;
        }
        /// <summary>
        /// 根据INITIALURL生成的Cookie1生成全局Cookie 如:Set-Cookie: JSESSIONID=E033C660B967E8A0A6F16EAF565EA049; Path=/otn  以后获取联系人以及下订单需要应用
        /// </summary>
        private void MakeClientJSession()
        {
            string sessionUrl = StringHelper.GetConfigValByKey(Constant.JSessionIDUrl, false);
            HttpItem item = new HttpItem();
            item.URL = sessionUrl;
            item.Referer = @"https://kyfw.12306.cn/otn/login/init";
            item.Cookie = HttpRequest.InitialCookie + HttpRequest.BeforLoginCookie;
            item.NeedSetJSessionID = true;
            item.NeedInitReqCookieContainer = true;
            //设置JSessionCookie
            HttpRequest.HttpGet(item);
            string JSessionCookie = HttpRequest.JSessionCookie;
            SystemCache.SetSysObj(Constant.JSessionCookieID, JSessionCookie);
        }
        /// <summary>
        /// 由于余票查询连接经常变动所以通过该方法实时获取连接
        /// </summary>
        private void InitLeftTicketQueryUrl()
        {
            string ticketUrl = StringHelper.GetConfigValByKey(Constant.InitQueryUrl, false);
            HttpItem item = new HttpItem()
            {
                URL = ticketUrl,
                Cookie = HttpRequest.JSessionCookie + "tk=" + AppTk,
                Referer = "https://kyfw.12306.cn/otn/index/initMy12306"
            };
            string ret = HttpRequest.HttpGet(item).ToString();
            string token = StringHelper.GetSubstring(ret, "CLeftTicketUrl", ';').Replace("'", "");
            SystemCache.SetSysObj(Constant.PassportAppId, StringHelper.GetSubstring(ret, Constant.PassportAppId, ';'));
            string queryUrl = StringHelper.GetConfigValByKey(Constant.QueryUrl, false);
            //更新余票查询连接
            string newQueryUrl = queryUrl.Replace("leftTicket/queryA", token);
            StringHelper.SetConfig(Constant.QueryUrl, newQueryUrl, false);
            LogHelper.Log(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, "余票查询连接 " + newQueryUrl);
            SystemCache.SetSysObj("LeftTicketQueryUrl", newQueryUrl);

            string QueryMinDate = StringHelper.GetSubstring(ret, "otherMindate", ';').Replace("'", "");
            string QueryMaxDate = StringHelper.GetSubstring(ret, "otherMaxdate", ';').Replace("'", "");
            string StudentMindate = StringHelper.GetSubstring(ret, "studentMindate", ';').Replace("'", "");
            string StudentMaxdate = StringHelper.GetSubstring(ret, "studentMaxdate", ';').Replace("'", "");
            SystemCache.SetSysObj(Constant.OtherMindate, QueryMinDate);    //最小查询日期
            SystemCache.SetSysObj(Constant.OtherMaxdate, QueryMaxDate);    //最大查询日期
            SystemCache.SetSysObj(Constant.StudentMindate, StudentMindate);//学生票最小查询日期
            SystemCache.SetSysObj(Constant.StudentMaxdate, StudentMaxdate);//学生票最大查询日期

        }

        private void AuthClient(string uamtk)
        {
            //根据uamtk生成 newapptk
            string authUrl = StringHelper.GetConfigValByKey(Constant.Auth, false);
            string uamauthUrl = StringHelper.GetConfigValByKey(Constant.UamAuthClient, false);
            HttpItem item = new HttpItem()
            {
                URL = authUrl,
                PostData = new Dictionary<string, string>() { { "appid", "otn" } },
                Cookie = HttpRequest.BeforLoginCookie + ";" + "uamtk=" + uamtk,
                Referer = @"https://kyfw.12306.cn/otn/passport?redirect=/otn/login/userLogin"
            };
            string authRet = HttpRequest.HttpPost(item);
            JObject json = JObject.Parse(authRet);
            string newapptk = json["newapptk"].ToString();
            AppTk = newapptk;
            SystemCache.SetSysObj(Constant.AppToken, newapptk);
            //获取验证信息(好像只显示用户名)
            HttpItem item2 = new HttpItem()
            {
                URL = uamauthUrl,
                Cookie = HttpRequest.JSessionCookie,
                PostData = new Dictionary<String, String>() { { "tk", newapptk } }
            };
            //string uamauthRet = HttpRequest.HttpPost(item2);
            //JObject json2 = JObject.Parse(uamauthRet);
            //string userName = json2["username"].ToString();
            //SystemCache.SetSysObj(Constant.CurrentUserName, UserName);
        }

        private void ChangeUserNameReadOnly(bool isReadOnly)
        {
            this.tbUserName.IsEnabled = !isReadOnly;
            this.tbPassword.IsEnabled = !isReadOnly;
            this.btnLogin.IsEnabled = !isReadOnly;
            this.btnLogin.Foreground = Brushes.Black;
            if (!isReadOnly)
            {
                this._timer.Stop();this._timer.Dispose();
                this.btnLogin.Content = "登    录";
                this.btnLogin.Foreground = Brushes.Black;
            }
        }
        
        string scale = String.Empty;
        private void _timer_Tick(object sender, EventArgs e)
        {
            Dispatcher.InvokeAsync(() =>
            {
                scale += ".";
                this.btnLogin.Content = _loginStatus + scale;
                this.btnLogin.Foreground = Brushes.Red;
                if (scale.Length == 6)
                {
                    scale = string.Empty;
                }
                Console.WriteLine(this.btnLogin.Content);
            });
        }

        private int index = 1;
        private void _timer2_Tick(object sender, EventArgs e)
        {
            if (index > 11)
            {
                index = 1;
            }
            this.imageLogo.Source = new BitmapImage(new Uri(String.Format("Resources/Images/logo{0:D2}.png", index), UriKind.RelativeOrAbsolute));
            Console.WriteLine(String.Format("Resources/Images/logo{0:D2}.png", index));
            index++;
        }
    }
}
