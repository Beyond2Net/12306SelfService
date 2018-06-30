using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TrainSelfService;

namespace TrainCommon.Model
{
    public class UsersModel
    {
        /// <summary>
        /// 与用户通信的回调接口
        /// </summary>
        public ITrainServiceCallback callback;
        public string UserName { get; set; }
        public UsersModel(string username, ITrainServiceCallback callback)
        {
            this.UserName = username;
            this.callback = callback;
        }
    }
}
