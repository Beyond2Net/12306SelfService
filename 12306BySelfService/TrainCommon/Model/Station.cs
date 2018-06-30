using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCommon.Model
{
    public class Station
    {
        public Station(string stationID, string stationName, string stationCode, string fullName, string simpleName, string stationNo)
        {
            this.StationID = stationID;
            this.StationName = stationName;
            this.StationCode = stationCode;
            this.StationFullName = fullName;
            this.StationSimpleName = simpleName;
            this.StationNo = stationNo;
        }
        //shq|上海虹桥|AOH|shanghaihongqiao|shhq|12

        /// <summary>
        /// 车站
        /// </summary>
        public string StationID { get; set; }
        /// <summary>
        /// 车站名称(使用中)
        /// </summary>
        public string StationName { get; set; }
        /// <summary>
        /// 车站代码(使用中)
        /// </summary>
        public string StationCode { get; set; }
        /// <summary>
        /// 车站全拼音全拼
        /// </summary>
        public string StationFullName { get; set; }
        /// <summary>
        /// 车站拼音简拼
        /// </summary>
        public string StationSimpleName { get; set; }
        /// <summary>
        /// 车站编号
        /// </summary>
        public string StationNo { get; set; }
    }
}
