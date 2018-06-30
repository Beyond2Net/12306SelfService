using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NativeWifi;

namespace ConsoleApp
{
    class WifiManager
    {
        //CMCC的WIFISSID
        public WIFISSID cmccWifiSSID;

        public WifiManager()
        {
            ScanSSID();
        }

        /// <summary>
        /// 将SSID转化成字符串
        /// </summary>
        static string GetStringForSSID(Wlan.Dot11Ssid ssid)
        {
            return Encoding.UTF8.GetString(ssid.SSID, 0, (int)ssid.SSIDLength);
        }

        /// <summary>  
        /// 枚举所有无线设备接收到的SSID  
        /// </summary>  
        public void ScanSSID()
        {
            WlanClient client = new WlanClient();
            foreach (WlanClient.WlanInterface wlanIface in client.Interfaces)
            {
                // Lists all networks with WEP security  
                Wlan.WlanAvailableNetwork[] networks = wlanIface.GetAvailableNetworkList(0);
                foreach (Wlan.WlanAvailableNetwork network in networks)
                {
                    WIFISSID targetSSID = new WIFISSID();
                    targetSSID.wlanInterface = wlanIface;
                    targetSSID.wlanSignalQuality = (int)network.wlanSignalQuality;
                    targetSSID.SSID = GetStringForSSID(network.dot11Ssid);
                    targetSSID.dot11DefaultAuthAlgorithm = network.dot11DefaultAuthAlgorithm.ToString();
                    targetSSID.dot11DefaultCipherAlgorithm = network.dot11DefaultCipherAlgorithm.ToString();
                    Console.WriteLine(targetSSID.SSID);
                    if (targetSSID.SSID.ToLower().Equals("cmcc"))
                    {
                        cmccWifiSSID = targetSSID;
                        return;
                    }
                }
            }
        } // EnumSSID 

        /// <summary>  
        /// 连接到CMCC 
        /// </summary>  
        /// <param name="ssid"></param>  
        public void ConnectToCMCC()
        {
            // Connects to a known network with WEP security  
            string profileName = cmccWifiSSID.SSID; // this is also the SSID
            Console.WriteLine("profileName" + profileName);
            cmccWifiSSID.wlanInterface.Connect(Wlan.WlanConnectionMode.Profile, Wlan.Dot11BssType.Any, profileName);
        }

        /// <summary>  
        /// 字符串转Hex  
        /// </summary>  
        /// <param name="str"></param>  
        /// <returns></returns>  
        public static string StringToHex(string str)
        {
            StringBuilder sb = new StringBuilder();
            byte[] byStr = System.Text.Encoding.Default.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)  
            for (int i = 0; i < byStr.Length; i++)
            {
                sb.Append(Convert.ToString(byStr[i], 16));
            }

            return (sb.ToString().ToUpper());
        }

    } // Class WifiManager


    class WIFISSID
    {
        public string SSID = "NONE";
        public string dot11DefaultAuthAlgorithm = "";
        public string dot11DefaultCipherAlgorithm = "";
        public bool networkConnectable = true;
        public string wlanNotConnectableReason = "";
        public int wlanSignalQuality = 0;
        public WlanClient.WlanInterface wlanInterface = null;
    }
}
