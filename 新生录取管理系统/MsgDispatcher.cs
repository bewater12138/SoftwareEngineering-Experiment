using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.NetworkObject;

namespace 新生录取管理系统
{
    internal static class MsgDispatcher
    {
        static public void DispatchMsg(NetworkMsg msg) 
        {
            Console.WriteLine($"接收服务端消息 type:{msg.Head.type}");
            switch (msg.Head.type)
            {
                case MsgType.LoginFail: Login.Status = Login.LoginStatus.Fail; break;
                case MsgType.LoginSuccess:
                    {
                        Login.Status = Login.LoginStatus.Success;
                        Login.Account = msg.Body.Parse<AccountInfo>()?.Account;
                        break;
                    }
            }
        }
    }
}
