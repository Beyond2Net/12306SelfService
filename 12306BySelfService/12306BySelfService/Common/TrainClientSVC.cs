using _12306BySelfService.SelfServiceReference;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace _12306BySelfService.Common
{
    public class TrainClientSVC: ITrainServiceCallback
    {
        protected static TrainClientSVC _tsvc = null;

        protected static TrainServiceClient clientSvc= null;
        public TrainServiceClient GetClientService()
        {
            if (clientSvc == null)
            {
                InstanceContext context = new InstanceContext(this);
                clientSvc = new TrainServiceClient(context);
            }
            return clientSvc;
        }

        public static TrainClientSVC GetTrainSVC()
        {
            if (_tsvc == null)
            {
                _tsvc = new TrainClientSVC();
            }
            return _tsvc;
        }

        public void OnInkStrokesUpdate(byte[] bytesStroke)
        {
            ((ITrainServiceCallback)_tsvc).OnInkStrokesUpdate(bytesStroke);
        }

        public void ShowLogin(string userName)
        {
            ((ITrainServiceCallback)_tsvc).ShowLogin(userName);
        }

        public void ShowLogout(string userName)
        {
            ((ITrainServiceCallback)_tsvc).ShowLogout(userName);
        }

        public void ShowTalk(string userName, string Message)
        {
            ((ITrainServiceCallback)_tsvc).ShowTalk(userName, Message);
        }

        public void ShowOnLineNum(int userCount)
        {
            ((ITrainServiceCallback)_tsvc).ShowOnLineNum(userCount);
        }
    }
}
