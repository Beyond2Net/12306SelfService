using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TrainCommon
{
    public class HttpRequest
    {
        //客户端Cookie
        public static string InitialCookie { get; set; }
        public static string AuthFirstCookie { get; set; }
        public static string BeforLoginCookie { get; set; }
        //public static string AfterLoginCookie { get; set; }
        public static string JSessionCookie { get; set; }

        public static CookieContainer cookieContainer = new CookieContainer();

        /// <summary>
        /// 发起一个HTTP请求（以POST方式）
        /// </summary>
        /// <param name="Url">url地址</param>
        /// <param name="data">发送数据</param>
        /// <returns></returns>
        public static string HttpPost(HttpItem item)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Stream responseStream = null;
            StreamReader answerData = null;
            String srcString = "";
            try
            {
                //拼接参数
                string postData = string.Empty;
                if (item.PostData != null)
                {
                    item.PostData.All(o =>
                    {
                        if (String.IsNullOrEmpty(postData))
                        {
                            postData = string.Format("{0}={1}", o.Key, o.Value);
                        }
                        else
                        {
                            postData += string.Format("&{0}={1}", o.Key, o.Value);
                        }
                        return true;
                    });
                }
                var retString = string.Empty;
                byte[] postDatas = Encoding.UTF8.GetBytes(postData);

                // 设置提交的相关参数
                request = WebRequest.Create(item.URL) as HttpWebRequest;
                request.ServicePoint.ConnectionLimit = item.Connectionlimit;
                request.Timeout = item.Timeout; //设置超时时间
                request.Method = "POST";
                //webRequest.ClientCertificates.Add();//客户端证书
                request.KeepAlive = false;
                request.ProtocolVersion = item.ProtocolVersion;//Http协议版本
                request.AllowAutoRedirect = false;
                request.ContentType = item.ContentType;
                request.UserAgent = item.UserAgent;
                request.ContentLength = postDatas.Length;
                request.Accept = item.Accept;
                request.Headers.Add("x-requested-with", "XMLHttpRequest");
                //添加Cookie
                if (!String.IsNullOrEmpty(item.Cookie))
                {
                    request.Headers[HttpRequestHeader.Cookie] = item.Cookie;
                }
                  // 提交请求数据
                Stream outputStream = request.GetRequestStream();
                outputStream.Write(postDatas, 0, postDatas.Length);
                outputStream.Close();
                //请求返回**********************************************************************************
                response = request.GetResponse() as HttpWebResponse;
                //CookieCollection cookieCollect = response.Cookies;
                string set_cookie = response.GetResponseHeader("Set-Cookie");
                //AfterLoginCookie = set_cookie;
                responseStream = response.GetResponseStream();
                answerData = new StreamReader(responseStream, Encoding.UTF8);//Encoding.GetEncoding("UTF-8")
                srcString = answerData.ReadToEnd();
                retString = srcString; //返回值赋值
                if (item.IsInitialLoaded)
                {
                    AuthFirstCookie = response.GetResponseHeader("Set-Cookie");
                    AuthFirstCookie = Regex.Replace(AuthFirstCookie, "path(?:[^,]+),?", "", RegexOptions.IgnoreCase);
                }
                answerData.Close();
                responseStream.Close();
                response.Close();
                request.Abort();

                return retString;
            }
            catch (Exception ex)
            {
                LogHelper.Log("HttpPost", ex.Message);
                throw new Exception(ex.Message);
            }
            finally
            {
                if (response != null) { response.Close(); }
                if (responseStream != null) { responseStream.Close(); }
                if (answerData != null) { answerData.Close(); }
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        /// <summary>
        /// 发起一个HTTP请求（以GET方式）
        /// </summary>
        /// <param name="sourceUrl">Url原下载地址</param>
        /// <param name="localUrl">本地下载目录地址</param>
        /// <returns></returns>
        public static Object HttpGet(String sourceUrl, String localUrl)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream responseStream = null;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(sourceUrl + localUrl);
                request.ContentType = "application/x-www-form-urlencoded";
                request.Method = "GET";
                request.KeepAlive = true;
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8";
                request.UserAgent = "Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/63.0.3239.84 Safari/537.36";
                //"Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)"
                //"Mozilla/5.0 (Windows NT 6.1; WOW64; rv:5.0.1) Gecko/20100101 Firefox/5.0.1"
                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                //设置客户端Cookie
                BeforLoginCookie = response.GetResponseHeader("Set-Cookie");
                BeforLoginCookie = Regex.Replace(BeforLoginCookie, "path(?:[^,]+),?", "", RegexOptions.IgnoreCase);

                if (responseStream != null)
                {
                    Bitmap img = new Bitmap(responseStream);
                    string dirpath = localUrl + @"Temp\";
                    //创建文件夹
                    if (!Directory.Exists(dirpath))
                        Directory.CreateDirectory(dirpath);
                    dirpath = dirpath + @"captcha-image.jpg";
                    //删除过期图片(自动覆盖)
                    //if (File.Exists(dirpath))
                    //{
                    //    File.Delete(dirpath);
                    //}
                    img.Save(dirpath, ImageFormat.Jpeg);
                    img.Dispose();

                    responseStream.Close();
                    response.Close();
                    request.Abort();

                    return dirpath;
                }
                return String.Empty;
            }
            catch (Exception ex)
            {
                LogHelper.Log("HttpGet", ex.Message);
                return String.Empty;
            }
            finally
            {
                if (response != null) { response.Close(); }
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

        public static Object HttpGet(HttpItem item)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;
            try
            {
                //拼接参数
                string postData = string.Empty;
                if (item.PostData != null)
                {
                    item.PostData.All(o =>
                    {
                        if (String.IsNullOrEmpty(postData))
                        {
                            postData = string.Format("?{0}={1}", o.Key, o.Value);
                        }
                        else
                        {
                            postData += string.Format("&{0}={1}", o.Key, o.Value);
                        }
                        return true;
                    });
                }
                request = (HttpWebRequest)WebRequest.Create(item.URL + postData);
                request.ContentType = item.ContentType;
                request.Method = "GET";
                request.Accept = item.Accept;
                request.UserAgent = item.UserAgent;
                if (item.NeedInitReqCookieContainer)
                {
                    request.CookieContainer = new CookieContainer();
                }
                //添加Referer
                if (!String.IsNullOrEmpty(item.Referer))
                {
                    request.Referer = item.Referer;
                }
                //添加Cookie
                if (!String.IsNullOrEmpty(item.Cookie))
                {
                    request.Headers[HttpRequestHeader.Cookie] = item.Cookie;
                }
                response = (HttpWebResponse)request.GetResponse();
                Stream responseStream = response.GetResponseStream();
                if (!String.IsNullOrEmpty(BeforLoginCookie))
                {
                    BeforLoginCookie = String.Empty;
                }
                BeforLoginCookie = response.GetResponseHeader("Set-Cookie");
                BeforLoginCookie = Regex.Replace(BeforLoginCookie, "path(?:[^,]+),?", "", RegexOptions.IgnoreCase);
                //第一次初始化客户端Cookie
                if (item.IsInitialLoaded)
                {
                    InitialCookie = response.GetResponseHeader("Set-Cookie");
                    InitialCookie = Regex.Replace(InitialCookie, "path(?:[^,]+),?", "", RegexOptions.IgnoreCase);
                }
                //设置系统Cookie
                if (item.NeedSetJSessionID)
                {
                    JSessionCookie = response.GetResponseHeader("Set-Cookie");
                    JSessionCookie = Regex.Replace(JSessionCookie, "path(?:[^,]+),?", "", RegexOptions.IgnoreCase);
                    SystemCache.SetSysObj("JSessionCookieID", JSessionCookie);
                }
                string responseStr = String.Empty;
                if (responseStream != null)
                {
                    StreamReader streamReader = new StreamReader(responseStream, item.Encoding);
                    responseStr = streamReader.ReadToEnd();
                    streamReader.Close();
                }
                responseStream.Close();
                response.Close();
                request.Abort();

                return responseStr;
            }
            catch (Exception ex)
            {
                LogHelper.Log("HttpGet", ex.Message);
                return null;
            }
            finally
            {
                if (response != null) { response.Close(); }
                if (request != null)
                {
                    request.Abort();
                }
            }
        }

    }

    public class HttpItem
    {
        /// <summary>
        /// 请求URL必须填写
        /// </summary>
        public string URL { get; set; }
        string _Method = "GET";
        Dictionary<String, String> _postData;
        public Dictionary<String, String> PostData
        {
            get { return _postData; }
            set { _postData = value; }
        }
        /// <summary>
        /// 请求方式默认为GET方式,当为POST方式时必须设置Postdata的值
        /// </summary>
        public string Method
        {
            get { return _Method; }
            set { _Method = value; }
        }
        int _Timeout = 50000;
        /// <summary>
        /// 默认请求超时时间
        /// </summary>
        public int Timeout
        {
            get { return _Timeout; }
            set { _Timeout = value; }
        }
        int _ReadWriteTimeout = 30000;
        /// <summary>
        /// 默认写入Post数据超时间
        /// </summary>
        public int ReadWriteTimeout
        {
            get { return _ReadWriteTimeout; }
            set { _ReadWriteTimeout = value; }
        }
        /// <summary>
        /// 设置Host的标头信息
        /// </summary>
        public string Host
        {
            get { return "kyfw12306.cn"; }
            set{ }
        }
        Boolean _KeepAlive = true;
        /// <summary>
        ///  获取或设置一个值，该值指示是否与 Internet 资源建立持久性连接默认为true。
        /// </summary>
        public Boolean KeepAlive
        {
            get { return _KeepAlive; }
            set { _KeepAlive = value; }
        }
        string _Accept = "application/json,text/javascript,text/html,application/xhtml+xml, */*; q=0.01";
        /// <summary>
        /// 请求标头值 默认为text/html, application/xhtml+xml, */*
        /// </summary>
        public string Accept
        {
            get { return _Accept; }
            set { _Accept = value; }
        }
        string _ContentType = "application/x-www-form-urlencoded";
        /// <summary>
        /// 请求返回类型默认 text/html
        /// </summary>
        public string ContentType
        {
            get { return _ContentType; }
            set { _ContentType = value; }
        }
        string _UserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_12_3) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/59.0.3071.115 Safari/537.36";
        /// <summary>
        /// 客户端访问信息默认Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)
        /// </summary>
        public string UserAgent
        {
            get { return _UserAgent; }
            set { _UserAgent = value; }
        }
        /// <summary>
        /// 返回数据编码默认为NUll,可以自动识别,一般为utf-8,gbk,gb2312
        /// </summary>
        public Encoding Encoding { get { return Encoding.UTF8; } set { } }
        private PostDataType _PostDataType = PostDataType.String;
        /// <summary>
        /// Post的数据类型
        /// </summary>
        public PostDataType PostDataType
        {
            get { return _PostDataType; }
            set { _PostDataType = value; }
        }
        /// <summary>
        /// Post请求时要发送的字符串Post数据
        /// </summary>
        public string Postdata { get; set; }
        /// <summary>
        /// Post请求时要发送的Byte类型的Post数据
        /// </summary>
        public byte[] PostdataByte { get; set; }
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection { get; set; }
        /// <summary>
        /// 请求时的Cookie
        /// </summary>
        public string Cookie { get; set; }
        /// <summary>
        /// 来源地址，上次访问地址
        /// </summary>
        public string Referer { get; set; }
        /// <summary>
        /// 证书绝对路径
        /// </summary>
        public string CerPath { get; set; }
        /// <summary>
        /// 设置代理对象，不想使用IE默认配置就设置为Null，而且不要设置ProxyIp
        /// </summary>
        public WebProxy WebProxy { get; set; }
        private Boolean isToLower = false;
        /// <summary>
        /// 是否设置为全文小写，默认为不转化
        /// </summary>
        public Boolean IsToLower
        {
            get { return isToLower; }
            set { isToLower = value; }
        }
        private Boolean allowautoredirect = false;
        /// <summary>
        /// 支持跳转页面，查询结果将是跳转后的页面，默认是不跳转
        /// </summary>
        public Boolean Allowautoredirect
        {
            get { return allowautoredirect; }
            set { allowautoredirect = value; }
        }
        private int connectionlimit = 1024;
        /// <summary>
        /// 最大连接数
        /// </summary>
        public int Connectionlimit
        {
            get { return connectionlimit; }
            set { connectionlimit = value; }
        }
        /// <summary>
        /// 代理Proxy 服务器用户名
        /// </summary>
        public string ProxyUserName { get; set; }
        /// <summary>
        /// 代理 服务器密码
        /// </summary>
        public string ProxyPwd { get; set; }
        /// <summary>
        /// 代理 服务IP,如果要使用IE代理就设置为ieproxy
        /// </summary>
        public string ProxyIp { get; set; }
        private ResultType resulttype = ResultType.String;
        /// <summary>
        /// 设置返回类型String和Byte
        /// </summary>
        public ResultType ResultType
        {
            get { return resulttype; }
            set { resulttype = value; }
        }
        private WebHeaderCollection header = new WebHeaderCollection();
        /// <summary>
        /// header对象
        /// </summary>
        public WebHeaderCollection Header
        {
            get { return header; }
            set { header = value; }
        }
        /// <summary>
        //     获取或设置用于请求的 HTTP 版本。返回结果:用于请求的 HTTP 版本。默认为 System.Net.HttpVersion.Version11。
        /// </summary>
        public Version ProtocolVersion
        {
            get { return HttpVersion.Version11; }
            set { }
        }
        private Boolean _expect100continue = true;
        /// <summary>
        ///  获取或设置一个 System.Boolean 值，该值确定是否使用 100-Continue 行为。如果 POST 请求需要 100-Continue 响应，则为 true；否则为 false。默认值为 true。
        /// </summary>
        public Boolean Expect100Continue
        {
            get { return _expect100continue; }
            set { _expect100continue = value; }
        }
        /// <summary>
        /// 设置509证书集合
        /// </summary>
        public X509CertificateCollection ClentCertificates { get; set; }
        /// <summary>
        /// 设置或获取Post参数编码,默认的为Default编码
        /// </summary>
        public Encoding PostEncoding { get; set; }
        private ResultCookieType _ResultCookieType = ResultCookieType.String;
        /// <summary>
        /// Cookie返回类型,默认的是只返回字符串类型
        /// </summary>
        public ResultCookieType ResultCookieType
        {
            get { return _ResultCookieType; }
            set { _ResultCookieType = value; }
        }
        private ICredentials _ICredentials = CredentialCache.DefaultCredentials;
        /// <summary>
        /// 获取或设置请求的身份验证信息。
        /// </summary>
        public ICredentials ICredentials
        {
            get { return _ICredentials; }
            set { _ICredentials = value; }
        }

        public HttpResult HttpResult { get; set; }

        public bool IsInitialLoaded { set; get; }

        public bool NeedSetJSessionID { set; get; }
        public bool NeedInitReqCookieContainer { set; get; }
    }

    /// <summary>
    /// Http返回参数类
    /// </summary>
    public class HttpResult
    {
        /// <summary>
        /// Http请求返回的Cookie
        /// </summary>
        public string Cookie { get; set; }
        /// <summary>
        /// Cookie对象集合
        /// </summary>
        public CookieCollection CookieCollection { get; set; }
        /// <summary>
        /// 返回的String类型数据 只有ResultType.String时才返回数据，其它情况为空
        /// </summary>
        public string Html { get; set; }
        /// <summary>
        /// 返回的Byte数组 只有ResultType.Byte时才返回数据，其它情况为空
        /// </summary>
        public byte[] ResultByte { get; set; }
        /// <summary>
        /// header对象
        /// </summary>
        public WebHeaderCollection Header { get; set; }
        /// <summary>
        /// 返回状态说明
        /// </summary>
        public string StatusDescription { get; set; }
        /// <summary>
        /// 返回状态码,默认为OK
        /// </summary>
        public HttpStatusCode StatusCode { get; set; }
    }
    /// <summary>
    /// 返回类型
    /// </summary>
    public enum ResultType
    {
        /// <summary>
        /// 表示只返回字符串 只有Html有数据
        /// </summary>
        String,
        /// <summary>
        /// 表示返回字符串和字节流 ResultByte和Html都有数据返回
        /// </summary>
        Byte
    }
    /// <summary>
    /// Post的数据格式默认为string
    /// </summary>
    public enum PostDataType
    {
        /// <summary>
        /// 字符串类型，这时编码Encoding可不设置
        /// </summary>
        String,
        /// <summary>
        /// Byte类型，需要设置PostdataByte参数的值编码Encoding可设置为空
        /// </summary>
        Byte,
        /// <summary>
        /// 传文件，Postdata必须设置为文件的绝对路径，必须设置Encoding的值
        /// </summary>
        FilePath
    }
    /// <summary>
    /// Cookie返回类型
    /// </summary>
    public enum ResultCookieType
    {
        /// <summary>
        /// 只返回字符串类型的Cookie
        /// </summary>
        String,
        /// <summary>
        /// CookieCollection格式的Cookie集合同时也返回String类型的cookie
        /// </summary>
        CookieCollection
    }

}
