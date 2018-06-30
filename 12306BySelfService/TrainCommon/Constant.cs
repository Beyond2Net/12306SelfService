using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCommon
{
    public class Constant
    {
        public static string[] Letters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z" };
        /// <summary>
        /// 席位类型
        /// </summary>
        public enum SeateType
        {
            /// <summary>
            /// 商务座特等座
            /// </summary>
            BusinessSeat,
            /// <summary>
            /// 一等座
            /// </summary>
            FirstLevelSeat,
            /// <summary>
            /// 二等座
            /// </summary>
            SecondLevelSeat,
            /// <summary>
            /// 高级软卧
            /// </summary>
            HighSoftBerth,
            /// <summary>
            /// 软卧
            /// </summary>
            SoftBerth,
            /// <summary>
            /// 动卧
            /// </summary>
            PneumaticBerth,
            /// <summary>
            /// 硬卧
            /// </summary>
            HardBerth,
            /// <summary>
            /// 软座
            /// </summary>
            SoftSeate,
            /// <summary>
            /// 硬座
            /// </summary>
            HardSeate,
            /// <summary>
            /// 无座
            /// </summary>
            NoSeat,
            /// <summary>
            /// 其他
            /// </summary>
            Other
        }

        /// <summary>
        /// 乘客票种类型
        /// </summary>
        public enum TicketType
        {
            /// <summary>
            /// 成人票
            /// </summary>
            Adult = 1,
            /// <summary>
            /// 儿童票
            /// </summary>
            Children = 2,
            /// <summary>
            /// 学生票
            /// </summary>
            Student = 3,
            /// <summary>
            /// 伤残军人票
            /// </summary>
            DisabledVeterans = 4
        }

        public static Dictionary<String, String> Tour_Flag = new Dictionary<string, string>()
        {
            { "dc","dc" },
            { "wc","wc" },
            { "fc","fc" },
            { "gc","gc" },
            { "lc","lc" },
            { "lc1","l1" },
            { "lc2","l2" }
        };

        //配置文件Key
        public static String StationNameUrl = "STATIONNAMEURL";
        public static String InitialUrl = "INITIALURL";
        public static String PasscodeRange = "PASSCODERANGE";
        public static String CheckCodeUrl = "CHECKCODEURL";
        public static String Auth = "AUTH";
        public static String UserName = "USERNAME";
        public static String Password = "PASSWORD";
        public static String VerifyCodeUrl = "VERIFYCODEURL";
        public static String JSessionIDUrl = "JSESSIONID";
        public static String InitQueryUrl = "INITQUERYURL";
        public static String QueryUrl = "QUERYURL";
        public static String UamAuthClient = "UAMAUTHCLIENT";
        public static String Instruction = "INSTRUCTION";
        public static String StartStation = "STARTSTATION";
        public static String EndStation = "ENDSTATION";
        public static String LatestQueryTime = "LatestQueryTime";
        public static String InitClientContractUrl = "INITCLIENTCONTRACT";
        public static String CheckOrderInfoJSUrl = "CheckOrderInfoJSUrl";
        public static String ConstractUrl = "CONTRACTURL";
        public static String SubmitOrderRequest = "SubmitOrderRequest";
        public static String CheckOrderInfoUrl = "CheckOrderInfoUrl";
        public static String ConfirmSingleForQueue = "ConfirmSingleForQueue";
        public static String QueryOrderWaitTime = "QueryOrderWaitTime";
        public static String QueryTrainByNumber = "QueryTrainByNumber";
        public static String GetLatestNews = "GetLatestNews";
        /// <summary>
        /// 未完成订单
        /// </summary>
        public static String InitNoComplete = "InitNoComplete";
        public static String StartSaleTime = "StartSaleTime";

        //系统缓存Key
        public static String AppToken = "AppToken";
        public static String JSessionCookieID = "JSessionCookieID";
        public static String PassportAppId = "passport_appId";
        //public static String CurrentCaptchaType = "current_captcha_type";
        public static String LeftTicketQueryUrl = "LeftTicketQueryUrl";
        public static String OtherMindate = "OtherMindate";
        public static String OtherMaxdate = "OtherMaxdate";
        public static String StudentMindate = "StudentMindate";
        public static String StudentMaxdate = "StudentMaxdate";
        public static String CurrentUserName = "CurrentUserName";
        public static String PassengerList = "PassengerList";
        public static String GlobalRepeatSubmitToken = "globalRepeatSubmitToken";
        public static String NormalPassengers = "normal_passengers";
        public static String TrainList = "TrainList";
        public static String SelectedPassengers = "SelectedContracts";
        public static String SelectedSeatList = "SelectedSeatList";
        public static String SelectedTrainList = "SelectedTrainList";
        public static String SelectedTakeTrainDateList = "SelectedTakeTrainDateList";
        public static String IsStartOrder = "IsStartOrder";
        public static String CancelFlag = "cancel_flag";
        public static String BedLevelOrderNum = "bed_level_order_num";
        public static String PurposeCodes = "purpose_codes";
        public static String LeftTicketStr = "leftTicketStr";
        public static String KeyCheckIsChange = "key_check_isChange";
        public static String TourFlag = "tourFlag";
        public static String AllSaleStation = "AllSaleStation";


        //public static String SWZ = "商务座";
        //public static String YDZ = "商务座";
        //public static String EDZ = "商务座";
        //public static String GJRW = "高级软卧";
        //public static String RW = "软卧";
        //public static String DW = "动卧";
        //public static String YW = "硬卧";
        //public static String RZ = "软座";
        //public static String YZ = "硬座";
        //public static String WZ = "无座";
        //public static String QT = "其他";
    }
}
