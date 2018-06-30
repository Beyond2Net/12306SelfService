using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
using TrainCommon;

namespace _12306BySelfService
{
    /// <summary>
    /// Test.xaml 的交互逻辑
    /// </summary>
    public partial class Test : Window
    {
        public Test()
        {
            InitializeComponent();
            this.Loaded += Test_Loaded;
        }

        public string dateTime { get; private set; }

        private void Test_Loaded(object sender, RoutedEventArgs e)
        {
            List<Object> list = new List<object>();
            list.Add(new { dateTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") });
            list.Add(new { dateTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") });
            list.Add(new { dateTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") });
            list.Add(new { dateTime = DateTime.Now.AddDays(1).ToString("yyyy-MM-dd") });
            dataGrid.ItemsSource = list;


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

            Dictionary<String, List<String>> dict2 = SystemCache.GetCache().GetObjByKey(Constant.AllSaleStation) as Dictionary<String, List<String>>;
            cb1.ItemsSource = dict2.Keys.ToList();
            cb1.SelectionChanged += Cb1_SelectionChanged;
        }

        private void Cb1_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            Dictionary<String, List<String>> dict2 = SystemCache.GetCache().GetObjByKey(Constant.AllSaleStation) as Dictionary<String, List<String>>;
            //string value = cb1.SelectedValue.ToString();
            //List<String> list2;
            //Dictionary<String, List<String>> dict3 = new Dictionary<String, List<String>>();
            //bool has = false;
            //foreach (var itst in dict2)
            //{
            //    list2 = new List<string>();
            //    foreach (var it in itst.Value)
            //    {
            //        if (it.Contains(value))
            //        {
            //            has = true;
            //            list2.Add(it);
            //        }
            //    }
            //    if (has) dict3.Add(itst.Key, list2);
            //    has = false;
            //}
            //StringBuilder sb = new StringBuilder();
            //foreach (var itemx in dict3)
            //{
            //    foreach (var itemm in itemx.Value)
            //    {
            //        sb.AppendFormat("{0}: {1}  ", itemx.Key, itemm);
            //    }
            //}
            //tkStationText.Text = sb.ToString();

            string[] stationArr = dict2[cb1.SelectedValue.ToString()].ToArray();
            string ret2 = String.Join("、", stationArr);
            tkStationText.Text = ret2;
        }
    }
}
