using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Utility;
using Utility.NetworkObject;
#pragma warning disable 8602
#pragma warning disable 8604

namespace 新生录取管理系统
{
    class Network:SingleInstance<Network>,IClose
    {
        TcpClient? socket;
        Mutex socketMutex = new Mutex();
        public TcpClient? ClientSocket => socket;
        public bool Sending {  get; protected set; }
        public bool RecvdMsg {  get; protected set; }
        public bool SendFailFlag { get; protected set; }
        public Exception? LastError { get; protected set; }

        public Network() 
        {
            Task.Run(RecvThread);
        }
        static private void RecvThread()
        {
            bool recvd_head = false;
            NetworkMsg msg = new NetworkMsg();
            while(true)
            {
                //如果没有建立好的连接，就先等待
                bool should_wait = false;
                s_Instance.socketMutex.WaitOne();
                if (s_Instance.socket == null) should_wait = true;
                else if (!s_Instance.socket.Connected)
                {
                    s_Instance.socket.Close();
                    s_Instance.socket = null;
                    should_wait = true;
                }
                s_Instance.socketMutex.ReleaseMutex();

                if (should_wait)
                {
                    Thread.SpinWait(5);
                    continue;
                }


                s_Instance.socketMutex.WaitOne();
                //接收消息头部
                if (!recvd_head)
                {
                    recvd_head = msg.Head.RecvForm(s_Instance.socket);
                }
                //接收消息体
                else
                {
                    if(msg.RecvBodyFrom(s_Instance.socket))
                    {
                        recvd_head = false;
                        NetworkProcess.DispatchMsg(msg);
                    }
                }
                s_Instance.socketMutex.ReleaseMutex();
            }
        }
        void IClose.Close() 
        {

        }

        static public bool ConnectServer()
        {
            try
            {
                s_Instance.socketMutex.WaitOne();
                if (s_Instance.socket != null) s_Instance.socket.Dispose();
                s_Instance.socket = new TcpClient("127.0.0.1", 10027);
            }
            catch (Exception e)
            {
                s_Instance.LastError = e;
                return false;
            }
            finally
            {
                s_Instance.socketMutex.ReleaseMutex();
            }
            return true;
        }
        static public async void SendToServerAsync(byte[] bytes)
        {
            try
            {
                s_Instance.Sending = true;
                await s_Instance.socket.GetStream().WriteAsync(bytes);
            }
            catch(Exception e)
            {
                s_Instance.LastError = e;
                s_Instance.SendFailFlag = true;
            }
            finally
            {
                s_Instance.Sending = false;
            }
        }
        static public void SendToServerAsync(string id, string password)
        {
            IDandPassword iap = new IDandPassword(id,password);
            var bytes = Encoding.Default.GetBytes(Protocol.MakeMsg(MsgType.CheckId, iap).Serialize());
            SendToServerAsync(bytes);
        }
    }
}
