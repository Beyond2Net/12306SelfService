using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using TrainCommon;

namespace TrainSelfService
{
    // 注意: 使用“重构”菜单上的“重命名”命令，可以同时更改代码和配置文件中的接口名“IService1”。
    [ServiceContract(SessionMode = SessionMode.Required, CallbackContract = typeof(ITrainServiceCallback))]
    public interface ITrainService
    {
        [OperationContract(IsOneWay = true)]
        void SendInkStrokes(MemoryStream memory);

        [OperationContract(IsOneWay = true)]
        void Login(string userName);

        [OperationContract(IsOneWay = true)]
        void Logout(string userName);

        [OperationContract(IsOneWay = true)]
        void Talk(string userName, string Message);

        #region

        [OperationContract]
        string[] GetMessage(string logid);

        [OperationContract]
        DataTable GetAccount();

        [OperationContract]
        string GetLogName(string logid);

        [OperationContract]
        string GetImage(string account);

        [OperationContract]
        bool IsLoginSuccess(string account, string passwod);

        [OperationContract]
        DataTable QueryTrain(string StartStation, string Destination, string G, string D, string T, string K);

        [OperationContract]
        DataTable QueryAll();

        [OperationContract]
        DataTable TrainReserve(string trainID);

        [OperationContract]
        bool SaveTicket(string logid, string trainid, string startStation, string destination, string passengerClass, string startTime, string seatClass);

        [OperationContract]
        DataTable TicketInfo(string trainid);

        [OperationContract]
        DataTable ReturnTickets(string logid);

        [OperationContract]
        bool Delete(string trainid);

        #endregion

        [OperationContract]
        Object HttpGet(HttpItem item);

        [OperationContract]
        Object HttpPost(HttpItem item);


        /******************************************************************************************************/
        //这是两个测试方法
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        CompositeType GetDataUsingDataContract(CompositeType composite);

        // TODO: 在此添加您的服务操作
        /******************************************************************************************************/
    }

    //Coder at 2017/12/30 by XuKun
    /***************************************************************************************************************************************/
    // 使用下面示例中说明的数据约定将复合类型添加到服务操作。注意：这是数据约定测试
    [DataContract]
    public class CompositeType
    {
        bool boolValue = true;
        string stringValue = "Hello ";

        [DataMember]
        public bool BoolValue
        {
            get { return boolValue; }
            set { boolValue = value; }
        }

        [DataMember]
        public string StringValue
        {
            get { return stringValue; }
            set { stringValue = value; }
        }
    }
    /******************************************************************************************************/


    public interface ITrainServiceCallback
    {
        [OperationContract(IsOneWay = true)]
        void OnInkStrokesUpdate(byte[] bytesStroke);

        [OperationContract(IsOneWay = true)]
        void ShowLogin(string userName);

        [OperationContract(IsOneWay = true)]
        void ShowLogout(string userName);

        [OperationContract(IsOneWay = true)]
        void ShowTalk(string userName, string Message);

        [OperationContract(IsOneWay = true)]
        void ShowOnLineNum(int userCount);
    }

}
