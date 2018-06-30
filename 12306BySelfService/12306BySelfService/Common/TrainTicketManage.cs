using _12306BySelfService.UserControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainCommon;
using TrainCommon.Model;

namespace _12306BySelfService.Common
{
    public class TrainTicketManage
    {
        protected static TrainTicketManage _train = null;
        public static TrainTicketManage GetTrain()
        {
            if (_train == null)
            {
                _train = new TrainTicketManage();
            }
            return _train;
        }

        public static TrainTicketManage GetTrainSVC()
        {
            if (_train == null)
            {
                _train = new TrainTicketManage();
            }
            return _train;
        }

        /// <summary>
        /// 查询火车信息列表
        /// </summary>
        /// <param name="startStation">出发站</param>
        /// <param name="endStation">到达站</param>
        /// <param name="takeDate">出发日期</param>
        /// <param name="ticketType">席座类型(软卧,硬座等)</param>
        /// <param name="seat">票种(成人,学生,儿童)</param>
        /// <returns></returns>
        public List<Train> GetTrainTableList(string startStation, string endStation, DateTime takeDate, string ticketType, Int32 seat)
        {
            List<Train> trainList = null;
            //DataTable dt = new DataTable("Datas");
            //dt.Columns.Add("序号", Type.GetType("System.Int32"));
            //dt.Columns[0].AutoIncrement = true;
            //dt.Columns[0].AutoIncrementSeed = 1;
            //dt.Columns[0].AutoIncrementStep = 1;
            //dt.Columns.Add("车次", Type.GetType("System.String"));
            //dt.Columns.Add("出发站(s)", Type.GetType("System.String"));
            //dt.Columns.Add("目的站(s)", Type.GetType("System.String"));
            //dt.Columns.Add("历时", Type.GetType("System.String"));
            //dt.Columns.Add("商务座", Type.GetType("System.String"));
            //dt.Columns.Add("一等座", Type.GetType("System.String"));
            //dt.Columns.Add("二等座", Type.GetType("System.String"));
            //dt.Columns.Add("高级软卧", Type.GetType("System.String"));
            //dt.Columns.Add("软卧", Type.GetType("System.String"));
            //dt.Columns.Add("动卧", Type.GetType("System.String"));
            //dt.Columns.Add("硬卧", Type.GetType("System.String"));
            //dt.Columns.Add("软座", Type.GetType("System.String"));
            //dt.Columns.Add("硬座", Type.GetType("System.String"));
            //dt.Columns.Add("无座", Type.GetType("System.String"));
            //dt.Columns.Add("其他", Type.GetType("System.String"));
            //dt.Columns.Add("备注", Type.GetType("System.String"));
            //dt.Rows.Add(new object[] { null, train[3], "郑州 " + train[8], "上海 " + train[9]);

            string url = StringHelper.GetConfigValByKey("QUERYURL", false);
            //席座类型
            //ticketType
            //票种
            string seatType = seat == 0 ? "ADULT" : (seat == 1 ? "0X00" : " ");
            Dictionary<String, String> parames = new Dictionary<string, string> {
                { "leftTicketDTO.train_date",  takeDate.ToString("yyyy-MM-dd")},
                { "leftTicketDTO.from_station", startStation},
                { "leftTicketDTO.to_station",  endStation},
                { "purpose_codes", seatType.ToString() },
            };
            HttpItem item = new HttpItem()
            {
                URL = url,
                Referer = "https://kyfw.12306.cn/otn/leftTicket/init",
                PostData = parames,
                Cookie = HttpRequest.JSessionCookie + "tk=" + Login.AppTk
            };
            string originJSON = TrainCommon.HttpRequest.HttpGet(item).ToString();
            //string originJSON = "{\"data\":{\"flag\":\"1\",\"map\":{\"SHH\":\"上海\",\"SNH\":\"上海南\",\"XUN\":\"信阳\"},\"result\":[\"ANNFYzQXMTSV6Z5AQ32qZwMm2YztAH1wV7p%2FeNEQ1W%2BYjRZ8kpo%2Bhds9JlnsROxfl%2BqluEGylrf8%0ANxQ5aDwJadAJL6u5i1ypTtT9GunzhMFX1Q7ZKA8rVnCph44fYuOFkUY4smS1jhoiSqIo3kV%2FpbOf%0A4jVlb1kwjpHUI3pVlS0CDktXXm38xNcXGeXoCNt08akMCNoHqBGhp%2Bl5L1ilpaAnIP8IsPUYkNkE%0A%2B0%2Fi65aH4qAs70%2FaeWKYDs26lX9kZbpM8MLNYjA%3D|预订|390000K7520G|K752|XUN|SNH|XUN|SNH|13:30|05:42|16:12|Y|IowYIPoZ%2B6ym2KcqJHtu3Gwhvqs47ejPulCrl2HJgIY%2FfuoNfW6we9VDgbs%3D|20180121|3|N2|01|21|0|0||||无|||有||2|有|||||10401030|1413|1\",\"dw5KsNj4bymOaI2zlkbZiLTBD78rRGi4S542V4sUCibDn0h6fJJb16mZ%2F30yi0J3FCMUO8Iufmh3%0AmLomu0QY70xhJu2RSxMo4z7a879uEqzE3rUCFLoDVBYcKmJG0%2BIqLS4XGkx2xpXNmgh4uMB9A76S%0Aod%2FpwlmDb4o6HKDVAW2j%2ByD9cbDg8a5WubmQMTwmJYrObcWQNstt%2FRHaV4gxiAzYzRY%2BgOBF9hI3%0A35uKONBSylE96WTtnAAVFSsXupg1oEGSLg%3D%3D|预订|390000K4620C|K462|PEN|SHH|XUN|SHH|22:00|10:20|12:20|Y|MKMqQGpPXOIE56zzBLrLJBPtCYVvR9%2B%2FheycY60d%2BfEFFWrVcEoBga7%2FcJg%3D|20180121|3|N2|05|16|0|0||||1|||有||无|有|||||10401030|1413|0\"]},\"httpstatus\":200,\"messages\":\"\",\"status\":true}";
            if (!originJSON.Contains("\"flag\":\"1\""))
            {
                LogHelper.Log(new System.Diagnostics.StackTrace().GetFrame(0).GetMethod().Name, "出错了请检查网络");
                return null;
            }
            JObject json = JObject.Parse(originJSON);
            JArray result = JObject.Parse(json["data"].ToString())["result"] as JArray;
            string[] train = null;
            Train trainItem = null;
            if (result != null)
            {
                trainList = new List<Train>();
                foreach (var arry in result)
                {
                    train = arry.ToString().Split('|');
                    trainItem = new Train();
                    trainItem.TrainSecretStr = System.Web.HttpUtility.UrlDecode(train[0]);//车次加密信息解码
                    trainItem.TrainID = GetFormatTicket(train[3]);         //车次
                    trainItem.TrainNo = GetFormatTicket(train[2]);         //车次码
                    trainItem.FromStation = GetFormatTicket(SystemCache._station[train[6]].ToString());//出发站
                    trainItem.ToStation = GetFormatTicket(SystemCache._station[train[5]].ToString());  //到达站
                    trainItem.StartTime = GetFormatTicket(train[8]);       //出发时间
                    trainItem.EndTime = GetFormatTicket(train[9]);         //到达时间
                    trainItem.Duration = GetFormatTicket(train[10]);       //历时
                    trainItem.BusinessSeat = GetFormatTicket(train[32]);   //商务座特等座
                    trainItem.FirstLevelSeat = GetFormatTicket(train[31]); //一等座
                    trainItem.SecondLevelSeat = GetFormatTicket(train[32]);//二等座
                    trainItem.HighSoftBerth = GetFormatTicket(train[25]);  //高级软卧(un)
                    trainItem.SoftBerth = GetFormatTicket(train[23]);      //软卧
                    trainItem.PneumaticBerth = GetFormatTicket(train[27]); //动卧
                    trainItem.HardBerth = GetFormatTicket(train[28]);      //硬卧
                    trainItem.SoftSeate = GetFormatTicket(train[24]);      //软座
                    trainItem.HardSeate = GetFormatTicket(train[29]);      //硬座
                    trainItem.NoSeat = GetFormatTicket(train[26]);         //无座
                    trainItem.Other = GetFormatTicket(train[22]);          //其他
                    trainItem.TrainLocation = GetFormatTicket(train[15]);  //列车坐标
                    trainItem.IsOpen = GetFormatTicket(train[11]);         //是否开票(Y 如14:30起售)或者系统维护时间(IS_TIME_NOT_BUY)

                    trainItem.Remark = GetFormatTicket(train[1]);          //备注
                                                                           //train[13]出发日期
                    trainList.Add(trainItem);
                }
            }
            
            SystemCache.SetSysObj(Constant.TrainList, trainList);
            return trainList;
        }

        private String GetFormatTicket(string ticket)
        {
            if (String.IsNullOrEmpty(ticket))
            {
                return "--";
            }
            return ticket;
        }

        //加载车站控件数据源
        public static void LoadedUserControlStation(AutoCompleteTextBox ctrlBox)
        {
            //sha|上海|SHH|shanghai|sh|10
            AutoCompleteEntry entry = null;
            foreach (String item in SystemCache._stationItems.Keys)
            {
                Station station = SystemCache._stationItems[item] as Station;
                entry = new AutoCompleteEntry(item, station.StationFullName);
                ctrlBox.AddItem(new AutoCompleteEntry(item, station.StationFullName));
            }
            //ctrlBox.Uid == "1";//出发站
            string url = ctrlBox.Uid == "1" ? "STARTSTATION" : "ENDSTATION";
            ctrlBox.Text = StringHelper.GetConfigValByKey(url);
        }
    }
}
