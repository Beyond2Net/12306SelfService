using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCommon.Model
{
    public class Train
    {
        /// <summary>
        /// 车次
        /// </summary>
        public String TrainID { set; get; }

        /// <summary>
        /// 车次码
        /// </summary>
        public String TrainNo { set; get; }
        /// <summary>
        /// 列车加密信息
        /// </summary>
        public String TrainSecretStr { get; set; }

        /// <summary>
        /// 出发站
        /// </summary>
        public String FromStation { set; get; }
        /// <summary>
        /// 到达站
        /// </summary>
        public String ToStation { set; get; }
        /// <summary>
        /// 出发时间
        /// </summary>
        public String StartTime { set; get; }
        /// <summary>
        /// 到达时间
        /// </summary>
        public String EndTime { set; get; }
        /// <summary>
        /// 历时
        /// </summary>
        public String Duration { set; get; }
        /// <summary>
        /// 商务座
        /// </summary>
        public String BusinessSeat { set; get; }
        /// <summary>
        /// 一等座
        /// </summary>
        public String FirstLevelSeat { set; get; }
        /// <summary>
        /// 二等座
        /// </summary>
        public String SecondLevelSeat { set; get; }
        /// <summary>
        /// 高级软卧
        /// </summary>
        public String HighSoftBerth { set; get; }
        /// <summary>
        /// 软卧
        /// </summary>
        public String SoftBerth { set; get; }
        /// <summary>
        /// 动卧
        /// </summary>
        public String PneumaticBerth { set; get; }
        /// <summary>
        /// 硬卧
        /// </summary>
        public String HardBerth { set; get; }
        /// <summary>
        /// 软座
        /// </summary>
        public String SoftSeate { set; get; }
        /// <summary>
        /// 硬座
        /// </summary>
        public String HardSeate { set; get; }
        /// <summary>
        /// 无座
        /// </summary>
        public String NoSeat { set; get; }
        /// <summary>
        /// 其他
        /// </summary>
        public String Other { set; get; }

        /// <summary>
        /// 列车坐标
        /// </summary>
        public String TrainLocation { set; get; }

        /// <summary>
        /// 是否开票
        /// </summary>
        public String IsOpen { set; get; }

        /// <summary>
        /// 备注
        /// </summary>
        public String Remark { set; get; }
    }
}
