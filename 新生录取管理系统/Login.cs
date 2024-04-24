using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.NetworkObject;
#pragma warning disable 8602

namespace 新生录取管理系统
{
    internal static class Login
    {
        public enum ErrorCode
        {
            None,Timeover,LoginFail,NetworkError,RegisterFail,Unknow
        }

        public enum Status
        {
            Wait,Fail,Success
        }
        static public Status LoginStatus { get; set; }
        static public Account? Account { get; set; }

        static public Status RegisterStatus { get; set; }
        static public string? RegisterFailInfo {  get; set; }

        //远程登录（阻塞方法）
        static public Account? LoginRemote(string id, string password,out ErrorCode err)
        {
            if(!Network.ConnectServer())
            {
                err = ErrorCode.NetworkError;
                return null;
            }
            Login.LoginStatus = Status.Wait;
            Network.SendToServerAsync(id, password);

            //等待服务端回应
            Console.WriteLine("等待服务端回应");
            var beg = System.Environment.TickCount;
            int time_limit = 5000;
            var is_time_over = () =>
            {
                return beg + time_limit < System.Environment.TickCount;
            };
            while(Login.LoginStatus == Status.Wait)
            {
                if(!Thread.Yield())
                {
                    Thread.Sleep(1);
                }

                if(is_time_over())
                {
                    err = ErrorCode.Timeover;
                    return null;
                }
            }

            //登录失败
            if(LoginStatus == Status.Fail)
            {
                err =ErrorCode.LoginFail;
                return null;
            }
            //登录成功
            else if(LoginStatus == Status.Success)
            {
                err = ErrorCode.None;
                return Account;
            }

            err = ErrorCode.None;
            return null;
        }
        static public Account? LoginRemote(string id, string password)
        {
            ErrorCode err = ErrorCode.None;
            return LoginRemote(id, password, out err);
        }


        //注册（阻塞方法）
        static public bool Register(string name,string id,string password,out ErrorCode err)
        {
            try
            {
                //发送注册消息到服务端
                RegisterStatus = Status.Wait;
                Network.SendToServerAsync(MsgType.RegisterAccount, new RegisterInfo(name, id, password));

                //等待服务端回应
                var beg = System.Environment.TickCount;
                int time_limit = 5000;
                var is_time_over = () =>
                {
                    return beg + time_limit < System.Environment.TickCount;
                };
                while (Login.RegisterStatus == Status.Wait)
                {
                    Thread.SpinWait(5);
                    if (is_time_over())
                    {
                        err = ErrorCode.Timeover;
                        return false;
                    }
                }

                //注册成功
                if(RegisterStatus == Status.Success)
                {
                    err = ErrorCode.None;
                    return true;
                }
                //注册失败
                else if(RegisterStatus == Status.Fail)
                {
                    err = ErrorCode.RegisterFail;
                    return false;
                }

                err = ErrorCode.Unknow;
                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                err = ErrorCode.Unknow;
                return false;
            }
        }
        static public bool Register(string name,string id,string password)
        {
            ErrorCode err = ErrorCode.None;
            return Register(name, id, password,out err);
        }
    }
}
