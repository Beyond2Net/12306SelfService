using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TrainCommon
{
    public class PathUtil
    {
        /// <summary>
        /// 获取本机IPv4地址
        /// </summary>
        /// <returns></returns>
        public static string GetIPv4()
        {
            string _IP = String.Empty;
            foreach (IPAddress _IPAddress in Dns.GetHostEntry(Dns.GetHostName()).AddressList)
            {
                if (_IPAddress.AddressFamily.ToString() == "InterNetwork")
                {
                    _IP = _IPAddress.ToString();
                }
            }
            return _IP;
        }

        /// <summary>
        /// 获取本机IPv4地址 Win32_NetworkAdapterConfiguration
        /// </summary>
        /// <returns></returns>
        public static string GetIPV4ByManagementClass()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection nics = mc.GetInstances();
            foreach (ManagementObject nic in nics)
            {
                if (Convert.ToBoolean(nic["ipEnabled"]) == true)
                {
                    return (nic["IPAddress"] as String[])[0];
                }
            }
            return String.Empty;
        }
    }
}
