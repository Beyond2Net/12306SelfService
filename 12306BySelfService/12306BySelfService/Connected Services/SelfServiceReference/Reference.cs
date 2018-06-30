﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace _12306BySelfService.SelfServiceReference {
    using System.Runtime.Serialization;
    using System;
    
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.Runtime.Serialization", "4.0.0.0")]
    [System.Runtime.Serialization.DataContractAttribute(Name="CompositeType", Namespace="http://schemas.datacontract.org/2004/07/TrainSelfService")]
    [System.SerializableAttribute()]
    public partial class CompositeType : object, System.Runtime.Serialization.IExtensibleDataObject, System.ComponentModel.INotifyPropertyChanged {
        
        [System.NonSerializedAttribute()]
        private System.Runtime.Serialization.ExtensionDataObject extensionDataField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private bool BoolValueField;
        
        [System.Runtime.Serialization.OptionalFieldAttribute()]
        private string StringValueField;
        
        [global::System.ComponentModel.BrowsableAttribute(false)]
        public System.Runtime.Serialization.ExtensionDataObject ExtensionData {
            get {
                return this.extensionDataField;
            }
            set {
                this.extensionDataField = value;
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public bool BoolValue {
            get {
                return this.BoolValueField;
            }
            set {
                if ((this.BoolValueField.Equals(value) != true)) {
                    this.BoolValueField = value;
                    this.RaisePropertyChanged("BoolValue");
                }
            }
        }
        
        [System.Runtime.Serialization.DataMemberAttribute()]
        public string StringValue {
            get {
                return this.StringValueField;
            }
            set {
                if ((object.ReferenceEquals(this.StringValueField, value) != true)) {
                    this.StringValueField = value;
                    this.RaisePropertyChanged("StringValue");
                }
            }
        }
        
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        
        protected void RaisePropertyChanged(string propertyName) {
            System.ComponentModel.PropertyChangedEventHandler propertyChanged = this.PropertyChanged;
            if ((propertyChanged != null)) {
                propertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(propertyName));
            }
        }
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="SelfServiceReference.ITrainService", CallbackContract=typeof(_12306BySelfService.SelfServiceReference.ITrainServiceCallback), SessionMode=System.ServiceModel.SessionMode.Required)]
    public interface ITrainService {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ITrainService/SendInkStrokes")]
        void SendInkStrokes(System.IO.MemoryStream memory);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ITrainService/SendInkStrokes")]
        System.Threading.Tasks.Task SendInkStrokesAsync(System.IO.MemoryStream memory);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ITrainService/Login")]
        void Login(string userName);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ITrainService/Login")]
        System.Threading.Tasks.Task LoginAsync(string userName);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ITrainService/Logout")]
        void Logout(string userName);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ITrainService/Logout")]
        System.Threading.Tasks.Task LogoutAsync(string userName);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ITrainService/Talk")]
        void Talk(string userName, string Message);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ITrainService/Talk")]
        System.Threading.Tasks.Task TalkAsync(string userName, string Message);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/GetMessage", ReplyAction="http://tempuri.org/ITrainService/GetMessageResponse")]
        string[] GetMessage(string logid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/GetMessage", ReplyAction="http://tempuri.org/ITrainService/GetMessageResponse")]
        System.Threading.Tasks.Task<string[]> GetMessageAsync(string logid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/GetAccount", ReplyAction="http://tempuri.org/ITrainService/GetAccountResponse")]
        System.Data.DataTable GetAccount();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/GetAccount", ReplyAction="http://tempuri.org/ITrainService/GetAccountResponse")]
        System.Threading.Tasks.Task<System.Data.DataTable> GetAccountAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/GetLogName", ReplyAction="http://tempuri.org/ITrainService/GetLogNameResponse")]
        string GetLogName(string logid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/GetLogName", ReplyAction="http://tempuri.org/ITrainService/GetLogNameResponse")]
        System.Threading.Tasks.Task<string> GetLogNameAsync(string logid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/GetImage", ReplyAction="http://tempuri.org/ITrainService/GetImageResponse")]
        string GetImage(string account);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/GetImage", ReplyAction="http://tempuri.org/ITrainService/GetImageResponse")]
        System.Threading.Tasks.Task<string> GetImageAsync(string account);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/IsLoginSuccess", ReplyAction="http://tempuri.org/ITrainService/IsLoginSuccessResponse")]
        bool IsLoginSuccess(string account, string passwod);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/IsLoginSuccess", ReplyAction="http://tempuri.org/ITrainService/IsLoginSuccessResponse")]
        System.Threading.Tasks.Task<bool> IsLoginSuccessAsync(string account, string passwod);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/QueryTrain", ReplyAction="http://tempuri.org/ITrainService/QueryTrainResponse")]
        System.Data.DataTable QueryTrain(string StartStation, string Destination, string G, string D, string T, string K);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/QueryTrain", ReplyAction="http://tempuri.org/ITrainService/QueryTrainResponse")]
        System.Threading.Tasks.Task<System.Data.DataTable> QueryTrainAsync(string StartStation, string Destination, string G, string D, string T, string K);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/QueryAll", ReplyAction="http://tempuri.org/ITrainService/QueryAllResponse")]
        System.Data.DataTable QueryAll();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/QueryAll", ReplyAction="http://tempuri.org/ITrainService/QueryAllResponse")]
        System.Threading.Tasks.Task<System.Data.DataTable> QueryAllAsync();
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/TrainReserve", ReplyAction="http://tempuri.org/ITrainService/TrainReserveResponse")]
        System.Data.DataTable TrainReserve(string trainID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/TrainReserve", ReplyAction="http://tempuri.org/ITrainService/TrainReserveResponse")]
        System.Threading.Tasks.Task<System.Data.DataTable> TrainReserveAsync(string trainID);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/SaveTicket", ReplyAction="http://tempuri.org/ITrainService/SaveTicketResponse")]
        bool SaveTicket(string logid, string trainid, string startStation, string destination, string passengerClass, string startTime, string seatClass);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/SaveTicket", ReplyAction="http://tempuri.org/ITrainService/SaveTicketResponse")]
        System.Threading.Tasks.Task<bool> SaveTicketAsync(string logid, string trainid, string startStation, string destination, string passengerClass, string startTime, string seatClass);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/TicketInfo", ReplyAction="http://tempuri.org/ITrainService/TicketInfoResponse")]
        System.Data.DataTable TicketInfo(string trainid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/TicketInfo", ReplyAction="http://tempuri.org/ITrainService/TicketInfoResponse")]
        System.Threading.Tasks.Task<System.Data.DataTable> TicketInfoAsync(string trainid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/ReturnTickets", ReplyAction="http://tempuri.org/ITrainService/ReturnTicketsResponse")]
        System.Data.DataTable ReturnTickets(string logid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/ReturnTickets", ReplyAction="http://tempuri.org/ITrainService/ReturnTicketsResponse")]
        System.Threading.Tasks.Task<System.Data.DataTable> ReturnTicketsAsync(string logid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/Delete", ReplyAction="http://tempuri.org/ITrainService/DeleteResponse")]
        bool Delete(string trainid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/Delete", ReplyAction="http://tempuri.org/ITrainService/DeleteResponse")]
        System.Threading.Tasks.Task<bool> DeleteAsync(string trainid);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/HttpGet", ReplyAction="http://tempuri.org/ITrainService/HttpGetResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.IO.MemoryStream))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.IO.Stream))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(string[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.Generic.Dictionary<string, string>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(_12306BySelfService.SelfServiceReference.CompositeType))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.MarshalByRefObject))]
        object HttpGet(string url, string param);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/HttpGet", ReplyAction="http://tempuri.org/ITrainService/HttpGetResponse")]
        System.Threading.Tasks.Task<object> HttpGetAsync(string url, string param);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/HttpPost", ReplyAction="http://tempuri.org/ITrainService/HttpPostResponse")]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.IO.MemoryStream))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.IO.Stream))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(string[]))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.Collections.Generic.Dictionary<string, string>))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(_12306BySelfService.SelfServiceReference.CompositeType))]
        [System.ServiceModel.ServiceKnownTypeAttribute(typeof(System.MarshalByRefObject))]
        object HttpPost(string url, System.Collections.Generic.Dictionary<string, string> parames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/HttpPost", ReplyAction="http://tempuri.org/ITrainService/HttpPostResponse")]
        System.Threading.Tasks.Task<object> HttpPostAsync(string url, System.Collections.Generic.Dictionary<string, string> parames);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/GetData", ReplyAction="http://tempuri.org/ITrainService/GetDataResponse")]
        string GetData(int value);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/GetData", ReplyAction="http://tempuri.org/ITrainService/GetDataResponse")]
        System.Threading.Tasks.Task<string> GetDataAsync(int value);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/GetDataUsingDataContract", ReplyAction="http://tempuri.org/ITrainService/GetDataUsingDataContractResponse")]
        _12306BySelfService.SelfServiceReference.CompositeType GetDataUsingDataContract(_12306BySelfService.SelfServiceReference.CompositeType composite);
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/ITrainService/GetDataUsingDataContract", ReplyAction="http://tempuri.org/ITrainService/GetDataUsingDataContractResponse")]
        System.Threading.Tasks.Task<_12306BySelfService.SelfServiceReference.CompositeType> GetDataUsingDataContractAsync(_12306BySelfService.SelfServiceReference.CompositeType composite);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ITrainServiceCallback {
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ITrainService/OnInkStrokesUpdate")]
        void OnInkStrokesUpdate(byte[] bytesStroke);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ITrainService/ShowLogin")]
        void ShowLogin(string userName);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ITrainService/ShowLogout")]
        void ShowLogout(string userName);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ITrainService/ShowTalk")]
        void ShowTalk(string userName, string Message);
        
        [System.ServiceModel.OperationContractAttribute(IsOneWay=true, Action="http://tempuri.org/ITrainService/ShowOnLineNum")]
        void ShowOnLineNum(int userCount);
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface ITrainServiceChannel : _12306BySelfService.SelfServiceReference.ITrainService, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class TrainServiceClient : System.ServiceModel.DuplexClientBase<_12306BySelfService.SelfServiceReference.ITrainService>, _12306BySelfService.SelfServiceReference.ITrainService {
        
        public TrainServiceClient(System.ServiceModel.InstanceContext callbackInstance) : 
                base(callbackInstance) {
        }
        
        public TrainServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName) : 
                base(callbackInstance, endpointConfigurationName) {
        }
        
        public TrainServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, string remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public TrainServiceClient(System.ServiceModel.InstanceContext callbackInstance, string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, endpointConfigurationName, remoteAddress) {
        }
        
        public TrainServiceClient(System.ServiceModel.InstanceContext callbackInstance, System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(callbackInstance, binding, remoteAddress) {
        }
        
        public void SendInkStrokes(System.IO.MemoryStream memory) {
            base.Channel.SendInkStrokes(memory);
        }
        
        public System.Threading.Tasks.Task SendInkStrokesAsync(System.IO.MemoryStream memory) {
            return base.Channel.SendInkStrokesAsync(memory);
        }
        
        public void Login(string userName) {
            base.Channel.Login(userName);
        }
        
        public System.Threading.Tasks.Task LoginAsync(string userName) {
            return base.Channel.LoginAsync(userName);
        }
        
        public void Logout(string userName) {
            base.Channel.Logout(userName);
        }
        
        public System.Threading.Tasks.Task LogoutAsync(string userName) {
            return base.Channel.LogoutAsync(userName);
        }
        
        public void Talk(string userName, string Message) {
            base.Channel.Talk(userName, Message);
        }
        
        public System.Threading.Tasks.Task TalkAsync(string userName, string Message) {
            return base.Channel.TalkAsync(userName, Message);
        }
        
        public string[] GetMessage(string logid) {
            return base.Channel.GetMessage(logid);
        }
        
        public System.Threading.Tasks.Task<string[]> GetMessageAsync(string logid) {
            return base.Channel.GetMessageAsync(logid);
        }
        
        public System.Data.DataTable GetAccount() {
            return base.Channel.GetAccount();
        }
        
        public System.Threading.Tasks.Task<System.Data.DataTable> GetAccountAsync() {
            return base.Channel.GetAccountAsync();
        }
        
        public string GetLogName(string logid) {
            return base.Channel.GetLogName(logid);
        }
        
        public System.Threading.Tasks.Task<string> GetLogNameAsync(string logid) {
            return base.Channel.GetLogNameAsync(logid);
        }
        
        public string GetImage(string account) {
            return base.Channel.GetImage(account);
        }
        
        public System.Threading.Tasks.Task<string> GetImageAsync(string account) {
            return base.Channel.GetImageAsync(account);
        }
        
        public bool IsLoginSuccess(string account, string passwod) {
            return base.Channel.IsLoginSuccess(account, passwod);
        }
        
        public System.Threading.Tasks.Task<bool> IsLoginSuccessAsync(string account, string passwod) {
            return base.Channel.IsLoginSuccessAsync(account, passwod);
        }
        
        public System.Data.DataTable QueryTrain(string StartStation, string Destination, string G, string D, string T, string K) {
            return base.Channel.QueryTrain(StartStation, Destination, G, D, T, K);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataTable> QueryTrainAsync(string StartStation, string Destination, string G, string D, string T, string K) {
            return base.Channel.QueryTrainAsync(StartStation, Destination, G, D, T, K);
        }
        
        public System.Data.DataTable QueryAll() {
            return base.Channel.QueryAll();
        }
        
        public System.Threading.Tasks.Task<System.Data.DataTable> QueryAllAsync() {
            return base.Channel.QueryAllAsync();
        }
        
        public System.Data.DataTable TrainReserve(string trainID) {
            return base.Channel.TrainReserve(trainID);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataTable> TrainReserveAsync(string trainID) {
            return base.Channel.TrainReserveAsync(trainID);
        }
        
        public bool SaveTicket(string logid, string trainid, string startStation, string destination, string passengerClass, string startTime, string seatClass) {
            return base.Channel.SaveTicket(logid, trainid, startStation, destination, passengerClass, startTime, seatClass);
        }
        
        public System.Threading.Tasks.Task<bool> SaveTicketAsync(string logid, string trainid, string startStation, string destination, string passengerClass, string startTime, string seatClass) {
            return base.Channel.SaveTicketAsync(logid, trainid, startStation, destination, passengerClass, startTime, seatClass);
        }
        
        public System.Data.DataTable TicketInfo(string trainid) {
            return base.Channel.TicketInfo(trainid);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataTable> TicketInfoAsync(string trainid) {
            return base.Channel.TicketInfoAsync(trainid);
        }
        
        public System.Data.DataTable ReturnTickets(string logid) {
            return base.Channel.ReturnTickets(logid);
        }
        
        public System.Threading.Tasks.Task<System.Data.DataTable> ReturnTicketsAsync(string logid) {
            return base.Channel.ReturnTicketsAsync(logid);
        }
        
        public bool Delete(string trainid) {
            return base.Channel.Delete(trainid);
        }
        
        public System.Threading.Tasks.Task<bool> DeleteAsync(string trainid) {
            return base.Channel.DeleteAsync(trainid);
        }
        
        public object HttpGet(string url, string param) {
            return base.Channel.HttpGet(url, param);
        }
        
        public System.Threading.Tasks.Task<object> HttpGetAsync(string url, string param) {
            return base.Channel.HttpGetAsync(url, param);
        }
        
        public object HttpPost(string url, System.Collections.Generic.Dictionary<string, string> parames) {
            return base.Channel.HttpPost(url, parames);
        }
        
        public System.Threading.Tasks.Task<object> HttpPostAsync(string url, System.Collections.Generic.Dictionary<string, string> parames) {
            return base.Channel.HttpPostAsync(url, parames);
        }
        
        public string GetData(int value) {
            return base.Channel.GetData(value);
        }
        
        public System.Threading.Tasks.Task<string> GetDataAsync(int value) {
            return base.Channel.GetDataAsync(value);
        }
        
        public _12306BySelfService.SelfServiceReference.CompositeType GetDataUsingDataContract(_12306BySelfService.SelfServiceReference.CompositeType composite) {
            return base.Channel.GetDataUsingDataContract(composite);
        }
        
        public System.Threading.Tasks.Task<_12306BySelfService.SelfServiceReference.CompositeType> GetDataUsingDataContractAsync(_12306BySelfService.SelfServiceReference.CompositeType composite) {
            return base.Channel.GetDataUsingDataContractAsync(composite);
        }
    }
}
