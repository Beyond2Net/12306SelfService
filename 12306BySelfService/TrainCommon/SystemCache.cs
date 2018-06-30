using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainCommon.Model;

namespace TrainCommon
{
    public class SystemCache
    {
        /// <summary>
        /// 缓存
        /// </summary>
        protected static SystemCache _cache = null;
        /// <summary>
        /// 缓存系统变量
        /// </summary>
        public static Hashtable _objItems = Hashtable.Synchronized(new Hashtable());
        /// <summary>
        /// 缓存全国各个代售车站对象信息["上海", Station]
        /// </summary>
        public static Hashtable _stationItems = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 缓存全国各个代售车站对象信息["SHH", "上海"]
        /// </summary>
        public static Hashtable _station = Hashtable.Synchronized(new Hashtable());

        /// <summary>
        /// 清除系统缓存
        /// </summary>
        public static void ClearCache()
        {
            if (_cache != null)
            {
                _stationItems.Clear();
                _station.Clear();
                _objItems.Clear();
            }
        }

        public static SystemCache GetCache()
        {
            if (_cache == null)
            {
                _cache = new SystemCache();
            }
            return _cache;
        }

        /// <summary>
        /// 缓存代售站信息
        /// </summary>
        /// <param name="stationName">站点名称 如:上海</param>
        /// <param name="station">站点对象</param>
        public static void SetTrainStation(string stationName, Station station)
        {
            if (!_stationItems.ContainsKey(stationName))
            {
                _stationItems[stationName] = station;
                _station[station.StationCode] = stationName;
            }
        }

        /// <summary>
        /// 读取代售站信息
        /// </summary>
        /// <param name="station">站点名称如上海</param>
        /// <returns></returns>
        public Station GetTrainStation(string station)
        {
            lock (_stationItems)
            {
                if (!_stationItems.ContainsKey(station))
                {
                    return null;
                }
                return _stationItems[station] as Station;
            }
        }

        /// <summary>
        /// 将键值对缓存系统缓存中(如不存在Key值则添加否则更新)
        /// </summary>
        /// <param name="key">数据字典key(唯一且不重复)</param>
        /// <param name="value">与key对应的数值</param>
        public static void SetSysObj(Object key, Object value)
        {
            if (!_objItems.ContainsKey(key))
            {
                _objItems.Add(key, value);
            }
            else
            {
                _objItems[key] = value;
            }
        } 

        /// <summary>
        /// 根据Key获取系统变量值
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public Object GetObjByKey(Object key)
        {
            lock (_objItems)
            {
                if (!_objItems.ContainsKey(key))
                {
                    return null;
                }
                return _objItems[key];
            }
        }
    }
}
