using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCommon
{
    public class CalendarHelper
    {
        private static ChineseLunisolarCalendar LunarCalendar = new ChineseLunisolarCalendar();
        public static string[] prifix = { "初", "十", "廿", "三" };
        public static string[] suffix = { "一", "二", "三", "四", "五", "六", "七", "八", "九", "十" };
        public static string[] JQ = { "小 寒", "大 寒", "立 春", "雨 水", "惊 蛰", "春 分", "清 明", "谷 雨", "立 夏", "小 满", "芒 种", "夏 至", "小 暑", "大 暑", "立 秋", "处 暑", "白 露", "秋 分", "寒 露", "霜 降", "立 冬", "小 雪", "大 雪", "冬 至" };
        public static int[] JQData = { 0, 21208, 42467, 63836, 85337, 107014, 128867, 150921, 173149, 195551, 218072, 240693, 263343, 285989, 308563, 331033, 353350, 375494, 397447, 419210, 440795, 462224, 483532, 504758 };

        /// <summary>
        /// 根据当前时间获取农历日期(带农历节气和阳历节日)
        /// </summary>
        /// <param name="Datetime"></param>
        /// <returns></returns>
        public static String GetLunarDay(DateTime Datetime)
        {
            String LunarDay = "";
            int yearLunar = LunarCalendar.GetYear(Datetime);
            int monthLunar = LunarCalendar.GetMonth(Datetime);
            int dayLunar = LunarCalendar.GetDayOfMonth(Datetime);
            int leapMonth = LunarCalendar.GetLeapMonth(yearLunar);//0 则表示没有闰月,如果有闰月,则monthLunar的范围为1-13, 
            bool isLeapMonth = LunarCalendar.IsLeapMonth(Datetime.Year, Datetime.Month);//当前月是否为闰月
            if (leapMonth > 0)
            {
                if (leapMonth == monthLunar)//闰月
                    monthLunar--;
                else if (monthLunar > leapMonth)
                    monthLunar--;
            }
            //闰月无节日
            if (monthLunar + 1 == leapMonth && dayLunar == 1 && isLeapMonth)
            {
                LunarDay = String.Format("闰{0}月", "无正二三四五六七八九十冬腊"[monthLunar]);
            }
            else
            {
                String solarDay = GetSolarTerm(Datetime);
                if (!String.IsNullOrEmpty(solarDay))
                    LunarDay = solarDay;
                else if (monthLunar == 1 && dayLunar == 1)
                    LunarDay = "春 节";
                else if (monthLunar == 1 && dayLunar == 15)
                    LunarDay = "元宵节";
                else if (monthLunar == 5 && dayLunar == 5)
                    LunarDay = "端午节";
                else if (monthLunar == 7 && dayLunar == 7)
                    LunarDay = "七 夕";
                else if (monthLunar == 7 && dayLunar == 15)
                    LunarDay = "中元节";
                else if (monthLunar == 8 && dayLunar == 15)
                    LunarDay = "中秋节";
                else if (monthLunar == 9 && dayLunar == 9)
                    LunarDay = "重阳节";
                else if (monthLunar == 12 && dayLunar == 8)
                    LunarDay = "腊八节";
                else if (LunarCalendar.GetDayOfYear(Datetime) == LunarCalendar.GetDaysInYear(LunarCalendar.GetYear(Datetime)))
                    LunarDay = "除 夕";
                //阳历节日
                else if (Datetime.Month == 1 && Datetime.Day == 1)
                    LunarDay = "元 旦";
                else if (Datetime.Month == 2 && Datetime.Day == 14)
                    LunarDay = "情人节";
                else if (Datetime.Month == 3 && Datetime.Day == 8)
                    LunarDay = "女人节";
                else if (Datetime.Month == 3 && Datetime.Day == 12)
                    LunarDay = "植树节";
                else if (Datetime.Month == 3 && Datetime.Day == 15)
                    LunarDay = "消费者权益日";
                else if (Datetime.Month == 4 && Datetime.Day == 1)
                    LunarDay = "愚人节";
                else if (Datetime.Month == 4 && Datetime.Day == 5)
                    LunarDay = "清明节";
                else if (Datetime.Month == 5 && Datetime.Day == 1)
                    LunarDay = "劳动节";
                else if (Datetime.Month == 5 && Datetime.Day == 4)
                    LunarDay = "青年节";
                else if (Datetime.Month == 5 && Datetime.Day == 12)
                    LunarDay = "护士节";
                else if (Datetime.Month == 6 && Datetime.Day == 1)
                    LunarDay = "儿童节";
                else if (Datetime.Month == 7 && Datetime.Day == 1)
                    LunarDay = "建党节";
                else if (Datetime.Month == 8 && Datetime.Day == 1)
                    LunarDay = "抗战胜利日";
                else if (Datetime.Month == 9 && Datetime.Day == 3)
                    LunarDay = "建军节";
                else if (Datetime.Month == 9 && Datetime.Day == 10)
                    LunarDay = "教师节";
                else if (Datetime.Month == 10 && Datetime.Day == 1)
                    LunarDay = "国庆节";
                else if (Datetime.Month == 11 && Datetime.Day == 1)
                    LunarDay = "万圣节";
                else if (Datetime.Month == 11 && Datetime.Day == 8)
                    LunarDay = "记者节";
                else if (Datetime.Month == 12 && Datetime.Day == 24)
                    LunarDay = "平安夜";
                else if (Datetime.Month == 12 && Datetime.Day == 25)
                    LunarDay = "圣诞节";
                else if (dayLunar > 0 && dayLunar < 32)
                {
                    //农历节日
                    if (dayLunar != 20 && dayLunar != 30)
                        LunarDay = string.Concat(prifix[(dayLunar - 1) / 10], suffix[(dayLunar - 1) % 10]);
                    else
                        LunarDay = string.Concat(suffix[(dayLunar - 1) / 10], prifix[1]);
                }
            }
            if (LunarDay.Length < 3)
            {
                return String.Format("{0}月{1}", "无正二三四五六七八九十冬腊"[monthLunar], LunarDay);
            }
            return LunarDay;
        }
        /// <summary>
        /// 获取农历节气
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetSolarTerm(DateTime dt)
        {
            DateTime dtBase = new DateTime(1900, 1, 6, 2, 5, 0);
            DateTime dtNew;
            double num;
            int y;
            string strReturn = "";
            y = dt.Year;
            for (int i = 1; i <= 24; i++)
            {
                num = 525948.76 * (y - 1900) + JQData[i - 1];
                dtNew = dtBase.AddMinutes(num);
                if (dtNew.DayOfYear == dt.DayOfYear)
                {
                    strReturn = JQ[i - 1];
                    return strReturn;
                }
            }
            return "";
        }

        /// <summary>
        /// 获取农历月
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetLunarMonth(DateTime date)
        {
            int yearLunar = LunarCalendar.GetYear(date);
            //如果有闰月，则month的范围为1-13
            int month = LunarCalendar.GetMonth(date);
            int leapMonth = LunarCalendar.GetLeapMonth(yearLunar);
            int day = LunarCalendar.GetDayOfMonth(date);

            int cmonth = leapMonth > 0 && leapMonth <= month ? month - 1 : month;

            //if (cmonth == 1 && day == 1)
            //{
            //    return "春节";
            //}
            //if (cmonth == 5 && day == 5)
            //{
            //    return "端午";
            //}
            //if (cmonth == 8 && day == 15)
            //{
            //    return "中秋";
            //}

            if (day == 1)
            {
                return String.Format("{0}{1}月", month == leapMonth ? "闰" : "", "无正二三四五六七八九十冬腊"[cmonth]);
            }
            else
            {
                //return string.Format("{0}{1}", "初十廿三"[day == 10 ? 0 : day / 10], "十一二三四五六七八九"[day % 10]);
                return String.Format("{0}月", "无正二三四五六七八九十冬腊"[cmonth]);
            }
        }

        /// <summary>
        /// 获取农历日期(带日期和农历节气)
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string GetLunarDate(DateTime date)
        {
            int yearLunar = LunarCalendar.GetYear(date);
            //如果有闰月，则month的范围为1-13
            int month = LunarCalendar.GetMonth(date);
            int leapMonth = LunarCalendar.GetLeapMonth(yearLunar);
            int day = LunarCalendar.GetDayOfMonth(date);
            
            int cmonth = leapMonth > 0 && leapMonth <= month ? month - 1 : month;
            
            if (day == 1)
            {
                return String.Format("{0}{1}月", month == leapMonth ? "闰" : "", "无正二三四五六七八九十冬腊"[cmonth]);
            }
            else
            {
                string solar = GetSolarTerm(date);
                string solar2 = String.IsNullOrEmpty(solar) ? "" : String.Format("【{0}】", solar);
                return string.Format("{0}月{1}{2} {3}", "无正二三四五六七八九十冬腊"[cmonth], "初十廿三"[day == 10 ? 0 : day / 10], "十一二三四五六七八九"[day % 10], solar2);
            }
        }
    }
}
