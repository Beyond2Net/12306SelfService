using NativeWifi;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

public static class StringHelper
{
    public static string _KEY = "LGQCKEY1";  //密钥  
    public static string _IV = "LVHCKEY2";   //向量
    static string _vpath;

    public static string SHA1_Encrypt(this string password)
    {
        SHA1 sha = new SHA1CryptoServiceProvider();
        return Convert.ToBase64String(sha.ComputeHash(Encoding.Default.GetBytes(password)));
    }

    /// <summary>  
    /// DES 加密
    /// </summary>  
    /// <param name="data"></param>  
    /// <returns></returns>  
    public static string DES_Encode(this string data)
    {
        byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_KEY);
        byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(_IV);
        DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
        int i = cryptoProvider.KeySize;
        MemoryStream ms = new MemoryStream();
        CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateEncryptor(byKey, byIV), CryptoStreamMode.Write);

        StreamWriter sw = new StreamWriter(cst);
        sw.Write(data);
        sw.Flush();
        cst.FlushFinalBlock();
        sw.Flush();

        string strRet = Convert.ToBase64String(ms.GetBuffer(), 0, (int)ms.Length);
        return strRet;
    }

    /// <summary>  
    /// DES 解密
    /// </summary>  
    /// <param name="data"></param>  
    /// <returns></returns>  
    public static string DES_Decode(this string data)
    {
        byte[] byKey = System.Text.ASCIIEncoding.ASCII.GetBytes(_KEY);
        byte[] byIV = System.Text.ASCIIEncoding.ASCII.GetBytes(_IV);

        byte[] byEnc;
        if (String.IsNullOrEmpty(data)) return data;
        try
        {
            data.Replace("_%_", "/");
            data.Replace("-%-", "#");
            byEnc = Convert.FromBase64String(data);

            DESCryptoServiceProvider cryptoProvider = new DESCryptoServiceProvider();
            MemoryStream ms = new MemoryStream(byEnc);
            CryptoStream cst = new CryptoStream(ms, cryptoProvider.CreateDecryptor(byKey, byIV), CryptoStreamMode.Read);
            StreamReader sr = new StreamReader(cst);
            return sr.ReadToEnd();
        }
        catch
        {
            return data;
        }
    }

    public static String GetConfigValByKey(string appKey)
    {
        return ConfigurationManager.AppSettings[appKey].DES_Decode();
    }

    public static String GetConfigValByKey(string appKey, bool needDecode)
    {
        if (!needDecode)
        {
            return ConfigurationManager.AppSettings[appKey];
        }
        else
        {
            return ConfigurationManager.AppSettings[appKey].DES_Decode();
        }
        
    }

    public static void SetConfig(string appKey, string appVal)
    {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        config.AppSettings.Settings[appKey].Value = appVal.Trim().DES_Encode();
        config.Save();
        ConfigurationManager.RefreshSection("appSettings");
    }

    public static void SetConfig(string appKey, string appVal, bool needEncode)
    {
        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
        if (needEncode)
        {
            config.AppSettings.Settings[appKey].Value = appVal.Trim().DES_Encode();
        }
        else
        {
            config.AppSettings.Settings[appKey].Value = appVal.Trim();
        }
        config.Save();
        ConfigurationManager.RefreshSection("appSettings");
    }

    public static string VirtualPath
    {
        get
        {
            if (String.IsNullOrEmpty(_vpath))
            {
                _vpath = AppDomain.CurrentDomain.BaseDirectory;
            }
            return _vpath;
        }
    }

    /// <summary>
    /// 获得字符串中开始和结束字符串中间的值
    /// </summary>
    /// <param name="str">字符串</param>
    /// <param name="beginChar">开始</param>
    /// <param name="endChar">结束</param>
    /// <returns></returns> 
    public static string GetValueBetween(string str, string beginChar, string endChar)
    {
        Regex rg = new Regex("(?<=(" + beginChar + "))[.\\s\\S]*?(?=(" + endChar + "))", RegexOptions.Multiline | RegexOptions.Singleline);
        return rg.Match(str).Value;
    }

    /// <summary>
    /// 截取字符串
    /// </summary>
    /// <param name="originalStr"></param>
    /// <param name="subStr"></param>
    /// <param name="subChar"></param>
    /// <returns></returns>
    public static string GetSubstring(string originalStr, string subStr, char subChar)
    {
        int scale = originalStr.IndexOf(subStr);//默认获取匹配第一个字符串
        if (scale < 0)
            return "";

        MatchCollection mc = GetMatchSpecificStr(originalStr, subStr);
        Int32 mtCount = mc.Count;
        //匹配超过4个则取第一个
        if (mtCount >= 4)
        {

        }
        else
        {
            for (int i = 1; i < mtCount; i++)
            {
                subStr += subStr;
            }
        }

        int index = scale + subStr.Length;
        string newsubStr = String.Empty;
        int flag = 0;
        while (true)
        {
            if (originalStr[index + flag] == subChar)
            {
                newsubStr = originalStr.Substring(index, flag);
                break;
            }
            flag++;
        }
        newsubStr = newsubStr.Replace("=", "").Trim();
        newsubStr = newsubStr.Replace("'", "").Replace(":", "");
        return newsubStr;
    }
    public static MatchCollection GetMatchSpecificStr(string originalStr,string target)
    {
        string pattern = String.Format(@"{0}", target);
        RegexOptions options = RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.IgnoreCase;
        Regex r = new Regex(pattern, options);
        MatchCollection mCollect = r.Matches(originalStr);
        
        return mCollect;
    }

    public static string GetFormatVal(object val)
    {
        if (val != null)
        {
            return val.ToString();
        }
        return String.Empty;
    }

    /// <summary>
    /// 将传入的字符串中间部分字符替换成特殊字符
    /// </summary>
    /// <param name="value">需要替换的字符串</param>
    /// <param name="startLen">前保留长度</param>
    /// <param name="endLen">尾保留长度</param>
    /// <param name="replaceChar">特殊字符</param>
    /// <returns>被特殊字符替换的字符串</returns>
    public static string ReplaceWithSpecialChar(string value, int startLen = 4, int endLen = 4, char specialChar = '*')
    {
        try
        {
            int lenth = value.Length - startLen - endLen;

            string replaceStr = value.Substring(startLen, lenth);

            string specialStr = string.Empty;

            for (int i = 0; i < replaceStr.Length; i++)
            {
                specialStr += specialChar;
            }

            value = value.Replace(replaceStr, specialStr);
        }
        catch (Exception)
        {
            throw;
        }

        return value;
    }

    /// <summary>
    /// Dictionary通过value获取对应的key值
    /// </summary>
    /// <param name="dic"></param>
    public static Object GetDictKeyByValue(Dictionary<Object, Object> dic, Object value)
    {
        ////第一种 foreach KeyValuePair traversing
        //foreach (KeyValuePair<string, string> kvp in dic)
        //{
        //    if (kvp.Value.Equals("2"))
        //    {
        //        //...... kvp.Key;
        //    }
        //}

        ////第二种foreach dic.Keys
        //foreach (string key in dic.Keys)
        //{
        //    if (dic[key].Equals("2"))
        //    {
        //        //...... key
        //    }
        //}

        //Linq1
        //List<Object> keys = dic.Where(q => q.Value == value).Select(q => q.Key).ToList();  //get all keys

        //Linq2
        //List<Object> keyList = (from q in dic
        //                        where q.Value == value
        //                        select q.Key).ToList<Object>(); //get all keys

        //Linq3
        var firstKey = dic.FirstOrDefault(q => q.Value == value).Key;  //get first key

        return firstKey;
    }
    /// <summary>
    /// 根据信号值对信号进行分类
    /// </summary>
    /// <param name="strength"></param>
    /// <returns></returns>
    public static string ClassifySignalByStrength(int strength)
    {
        string classify;
        if (strength < 20)
        {
            classify = "弱";
        }
        else if (strength < 40)
        {
            classify = "较弱";
        }
        else if (strength < 60)
        {
            classify = "一般";
        }
        else if (strength < 80)
        {
            classify = "较强";
        }
        else
        {
            classify = "强";
        }

        return classify;
    }

}