using log4net;
using log4net.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

[assembly: log4net.Config.XmlConfigurator(Watch = true)]
namespace TrainCommon
{
    public class LogHelper
    {
        //(高) OFF > FATAL > ERROR > WARN > INFO > DEBUG > ALL (低)

        private static log4net.ILog infoLog;
        private static log4net.ILog errorLog;
        private static log4net.ILog authLog;
        

        static LogHelper()
        {
            infoLog = log4net.LogManager.GetLogger("loginfo");
            authLog = log4net.LogManager.GetLogger("logauth");
            errorLog = log4net.LogManager.GetLogger("logerror");
        }

        /// <summary>
        /// 提示信息日志
        /// </summary>
        public static void Info(string msg)
        {
            infoLog.Info(msg);
        }

        /// <summary>
        /// 提示信息日志
        /// </summary>
        public static void Info(Exception ex)
        {
            infoLog.Info(ex);
        }

        /// <summary>
        /// 提示信息日志
        /// </summary>
        public static void Info(string msg, Exception ex)
        {
            infoLog.Info(msg, ex);
        }

        /// <summary>
        /// 登录日志
        /// </summary>
        public static void Auth(string msg)
        {
            authLog.Info(msg);
        }

        /// <summary>
        /// 登录日志
        /// </summary>
        public static void Auth(Exception ex)
        {
            authLog.Info(ex);
        }

        /// <summary>
        /// 登录日志
        /// </summary>
        public static void Auth(string msg, Exception ex)
        {
            authLog.Info(msg, ex);
        }

        /// <summary>
        /// 报错日志
        /// </summary>
        public static void Error(string msg)
        {
            errorLog.Error(msg);
        }

        /// <summary>
        /// 报错日志
        /// </summary>
        public static void Error(Exception ex)
        {
            errorLog.Error(ex);
        }

        /// <summary>
        /// 报错日志
        /// </summary>
        public static void Error(string msg, Exception ex)
        {
            errorLog.Error(msg, ex);
        }

        /// <summary>
        /// 写入日志到文本文件
        /// </summary>
        /// <param name="action">动作</param>
        /// <param name="strMessage">日志内容</param>
        /// <param name="time">时间</param>
        public static void Log(string action, string strMessage)
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + @"Temp\Logs\";
            //不存在特定路径文件夹就创建
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            string fileFullPath = path + DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            StringBuilder str = new StringBuilder();
            str.Append("Time: " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n");
            str.Append("Action: " + action + "\r\n");
            str.Append("Message: " + strMessage + "\r\n");
            str.Append("-----------------------------------------------------------\r\n\r\n");
            StreamWriter sw;
            if (!File.Exists(fileFullPath))
            {
                sw = File.CreateText(fileFullPath);
            }
            else
            {
                sw = File.AppendText(fileFullPath);
            }
            sw.WriteLine(str.ToString());
            sw.Close();
        }


    }
}

