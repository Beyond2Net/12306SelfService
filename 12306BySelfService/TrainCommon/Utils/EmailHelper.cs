using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace TrainCommon
{
    public class EmailHelper
    {
        public String MailServer { set; get; }
        public Int32 MailPort { set; get; }
        public String UserName { set; get; }
        public String Password { set; get; }
        public String MailTo { set; get; }
        public String MailCC { set; get; }
        public Int32 MailPority { set; get; }
        public Boolean IsSSL { set; get; }

        /// <summary>
        /// 发送邮件
        /// </summary>
        /// <param name="to_mail_addresses">收件人</param>
        /// <param name="displayName">邮件显示名称</param>
        /// <param name="mail_priority">邮件优先级</param>
        /// <param name="attachment_file_path">附件路径</param>
        public void SendMail(string to_mail_addresses, string displayName, string attachment_file_path)
        {
            // 邮件信息配置
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(ConfigurationManager.AppSettings["UserName"].DES_Decode(), displayName, Encoding.UTF8);
            mail.To.Add(to_mail_addresses);
            string mail_cc_addresses = ConfigurationManager.AppSettings["mail_cc_addresses"].DES_Decode();
            if (!string.IsNullOrEmpty(mail_cc_addresses))
            {
                mail.CC.Add(mail_cc_addresses);
            }

            mail.Subject = ConfigurationManager.AppSettings["txtEmailTitle"];
            mail.SubjectEncoding = System.Text.Encoding.UTF8;

            mail.Body = ConfigurationManager.AppSettings["EmailContent"];
            mail.BodyEncoding = System.Text.Encoding.UTF8;
            int mail_priority = Convert.ToInt32(ConfigurationManager.AppSettings["mail_priority"]);
            switch (mail_priority)
            {
                case 1:
                    mail.Priority = MailPriority.High;
                    break;
                case 2:
                    mail.Priority = MailPriority.Normal;
                    break;
                case 3:
                    mail.Priority = MailPriority.Low;
                    break;
                default:
                    mail.Priority = MailPriority.Normal;
                    break;
            }
            mail.IsBodyHtml = true;

            System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(attachment_file_path);
            mail.Attachments.Add(attachment);

            string userName = ConfigurationManager.AppSettings["UserName"].DES_Decode();
            string password = ConfigurationManager.AppSettings["Password"].DES_Decode();
            SmtpClient smtpServer = new SmtpClient();
            smtpServer.Host = ConfigurationManager.AppSettings["MailServerName"].DES_Decode();
            smtpServer.Port = Convert.ToInt32(ConfigurationManager.AppSettings["MailServerPort"].DES_Decode());
            // 邮箱服务登录权限认证
            smtpServer.UseDefaultCredentials = false;
            smtpServer.Credentials = new NetworkCredential(userName, password);
            smtpServer.EnableSsl = true;
            object userState = mail;
            try
            {
                // 发送邮件
                smtpServer.SendAsync(mail, userState);
            }
            catch (SmtpException exception)
            {
                throw exception;
            }
            finally
            {
                mail.Dispose();
                smtpServer = null;
                if (!string.IsNullOrEmpty(attachment_file_path))
                {
                    File.Delete(attachment_file_path);
                }
            }
        }

        /// <summary>
        ///  测试邮件代码 若异步则返回值为Task<Int32>
        /// </summary>
        public Task<Int32> SelfServiceSendMail()
        {
            // 邮件信息配置
            MailMessage mail = new MailMessage();
            mail.From = new MailAddress(this.UserName, this.UserName, Encoding.UTF8);
            mail.To.Add(this.MailTo);

            mail.Subject = "测试邮件主题";
            mail.SubjectEncoding = Encoding.UTF8;

            mail.Body = String.Format("This is test email - 来自 {0} 的测试邮件！    \r\n发送时间： {1}", this.UserName, DateTime.Now.ToString());
            mail.BodyEncoding = Encoding.UTF8;
            mail.IsBodyHtml = true;
            //邮件优先级
            switch (this.MailPority)
            {
                case 0:
                    mail.Priority = MailPriority.High;
                    break;
                case 1:
                    mail.Priority = MailPriority.Normal;
                    break;
                case 2:
                    mail.Priority = MailPriority.Low;
                    break;
                default:
                    mail.Priority = MailPriority.Normal;
                    break;
            }

            string userName = this.UserName;
            string password = this.Password;
            SmtpClient smtpServer = new SmtpClient(this.MailServer, this.MailPort);//发件用户名和密码认证 

            // 邮箱服务登录权限认证
            smtpServer.UseDefaultCredentials = true;
            smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network; //通过网络发送到Smtp服务器 
            smtpServer.Credentials = new NetworkCredential(userName, password);
            //启用SSL加密
            smtpServer.EnableSsl = this.IsSSL;
            object userState = mail;

            // 发送邮件
            return Task.Run(() =>
            {
                try
                {
                    smtpServer.SendAsync(mail, userState);
                    ShowMessage(String.Format("这是测试邮件\r\nFrom: {0}\r\nTo: {1}\r\nStatus: 发送成功！", mail.From.Address, mail.To.ToString()));
                    return 1;
                }
                catch (Exception exception)
                {
                    ShowMessage(exception);
                    return 0;
                }
                finally
                {
                    mail.Dispose();
                    smtpServer = null;
                }
            });
        }

        /// <summary>
        ///  验证邮箱格式
        /// </summary>
        /// <param name="emailAddress"></param>
        /// <returns></returns>
        public bool VerifyEmailFormat(string emailAddress)
        {
            //正则表达式字符串
            //string emailStr = @"([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,5})+";
            string emailSuffixType = @"@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,5})+";
            //邮箱正则表达式对象
            Regex emailReg = new Regex(emailSuffixType);

            return emailReg.IsMatch(emailAddress);
        }

        /// <summary>
        ///  验证是否能连接邮箱服务器 如果异步则返回值Task<Boolean>
        /// </summary>
        /// <param name="smtpServer"></param>
        /// <param name="port"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public Task<Boolean> CheckSmtp(string smtpServer, int port, string username, string password)
        {
            TcpClient tcpClient = new TcpClient(smtpServer, port);
            NetworkStream stream = tcpClient.GetStream();

            return Task.Run(() =>
            {
                if (!WaiteFor(stream, "220"))
                    return false;

                SendCommand(stream, "HELO 211.152.50.xxx/r/n");
                if (!WaiteFor(stream, "250"))
                    return false;

                SendCommand(stream, "AUTH LOGIN/r/n");
                if (!WaiteFor(stream, "334"))
                    return false;

                SendCommand(stream, Base64Encode(username) + "/r/n");
                if (!WaiteFor(stream, "334"))
                    return false;

                SendCommand(stream, Base64Encode(password) + "/r/n");
                if (!WaiteFor(stream, "235"))
                    return false;
                return true;
            });
        }
        private bool WaiteFor(NetworkStream stream, string strCode)
        {
            int StreamSize;
            byte[] ReadBuffer = new byte[1024];
            StreamSize = stream.Read(ReadBuffer, 0, ReadBuffer.Length);
            string Returnvalue = Encoding.Default.GetString(ReadBuffer).Substring(0, StreamSize);

            Console.WriteLine(Returnvalue);

            return Returnvalue.Substring(0, 3).Equals(strCode);
        }
        private void SendCommand(NetworkStream stream, string strCmd)
        {
            byte[] WriteBuffer;

            WriteBuffer = Encoding.Default.GetBytes(strCmd);

            stream.Write(WriteBuffer, 0, WriteBuffer.Length);

        }
        private string Base64Encode(string str)
        {
            byte[] barray;
            barray = Encoding.Default.GetBytes(str);
            return Convert.ToBase64String(barray);
        }

        public static void ShowMessage(Exception ex)
        {
            MessageBox.Show("出错了：\r" + ex.Message, "错误信息", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        public static void ShowMessage(string ex)
        {
            MessageBox.Show(ex, "信息", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
