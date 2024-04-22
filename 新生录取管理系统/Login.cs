using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
#pragma warning disable 8602

namespace 新生录取管理系统
{
    internal static class Login
    {
        public enum ErrorCode
        {
            None,Timeover,Wrong,NetworkError
        }

        //远程登录（阻塞方法）
        static public Account? LoginRemote(string id, string password,out ErrorCode err)
        {
            if(!Network.ConnectServer())
            {
                err = ErrorCode.NetworkError;
                return null;
            }
            Network.SendToServerAsync(id, password);

            var beg = System.Environment.TickCount;
            int time_limit = 5000;
            var is_time_over = () =>
            {
                return beg + time_limit < System.Environment.TickCount;
            };

            while(!Network.Instance.RecvdMsg)
            {
                Thread.SpinWait(5);
                if(is_time_over())
                {
                    err = ErrorCode.Timeover;
                    return null;
                }
            }

            err = ErrorCode.None;
            return null;
        }
        static public Account? LoginRemote(string id, string password)
        {
            ErrorCode err = ErrorCode.None;
            return LoginRemote(id, password, out err);
        }
    }
}
