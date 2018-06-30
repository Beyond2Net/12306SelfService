using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrainCommon.Model
{
    public class CC
    {
        public static List<UsersModel> Users { set; get; }
        public static UsersModel GetUser(string userName)
        {
            UsersModel user = null;
            foreach (var item in Users)
            {
                if (item.UserName == userName)
                {
                    user = item;
                    break;
                }
            }
            return user;
        }
    }
}
