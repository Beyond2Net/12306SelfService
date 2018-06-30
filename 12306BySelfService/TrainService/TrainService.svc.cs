using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using TrainCommon.Model;
using TrainCommon;

namespace TrainSelfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码、svc 和配置文件中的类名“Service1”。
    // 注意: 为了启动 WCF 测试客户端以测试此服务，请在解决方案资源管理器中选择 Service1.svc 或 Service1.svc.cs，然后开始调试。
    public class TrainService : ITrainService
    {
        ITrainServiceCallback callBack;
        public TrainService()
        {
            //callback = context.GetCallbackChannel<IService1Callback>();  //向客户端打开通道建立连接
            if (CC.Users == null)
            {
                CC.Users = new List<UsersModel>();
            }
        }

        /// <summary>
        /// //显示在线人数
        /// </summary>
        public void SendInfoToAllUsers()
        {
            int userCount = CC.Users.Count;
            foreach (var user in CC.Users)
            {
                user.callback.ShowOnLineNum(userCount);
            }
        }

        #region 实现服务端接口
        public bool Delete(string trainid)
        {
            throw new NotImplementedException();
        }

        public DataTable GetAccount()
        {
            throw new NotImplementedException();
        }

        public string GetImage(string account)
        {
            throw new NotImplementedException();
        }

        public string GetLogName(string logid)
        {
            throw new NotImplementedException();
        }

        public string[] GetMessage(string logid)
        {
            throw new NotImplementedException();
        }


        public bool IsLoginSuccess(string account, string passwod)
        {
            return true;
        }

        /// <summary>
        /// 用户名登录
        /// </summary>
        /// <param name="userName"></param>
        public void Login(string userName)
        {
            
        }

        public void Logout(string userName)
        {
            UsersModel logoutUser = CC.GetUser(userName);
            foreach (var user in CC.Users)
            {
                if (user.UserName != logoutUser.UserName) //不需要发给退出用户
                {
                    user.callback.ShowLogout(userName);
                }
            }
            CC.Users.Remove(logoutUser);
            logoutUser = null;   //将其设置为null后，WCF会自动关闭该客户端
            SendInfoToAllUsers();
        }

        public DataTable QueryAll()
        {
            throw new NotImplementedException();
        }

        public DataTable QueryTrain(string StartStation, string Destination, string G, string D, string T, string K)
        {
            throw new NotImplementedException();
        }

        public DataTable ReturnTickets(string logid)
        {
            throw new NotImplementedException();
        }

        public bool SaveTicket(string logid, string trainid, string startStation, string destination, string passengerClass, string startTime, string seatClass)
        {
            throw new NotImplementedException();
        }

        public void SendInkStrokes(MemoryStream memory)
        {
            foreach (var user in CC.Users)
            {
                callBack = user.callback;
                callBack.OnInkStrokesUpdate(memory.GetBuffer());
            }
        }

        public void Talk(string userName, string Message)
        {
            throw new NotImplementedException();
        }

        public DataTable TicketInfo(string trainid)
        {
            throw new NotImplementedException();
        }

        public DataTable TrainReserve(string trainID)
        {
            throw new NotImplementedException();
        }

        #endregion

        /// <summary>
        /// HttpGet请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Object HttpGet(HttpItem item)
        {
            return HttpRequest.HttpGet(item);
        }

        /// <summary>
        /// HttpPost请求
        /// </summary>
        /// <param name="url"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public Object HttpPost(HttpItem item)
        {
            return HttpRequest.HttpPost(item);
        }


        /*********************************************************************************************/
        //这是两个测试方法
        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
        /*********************************************************************************************/
    }
}
