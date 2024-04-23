using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using System.Windows.Interop;
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

        static private void WaitForConnection()
        {
            do
            {
                bool should_wait = false;
                Instance.socketMutex.WaitOne();
                if (Instance.socket == null) should_wait = true;
                else if (!Instance.socket.Connected)
                {
                    Instance.socket.Close();
                    Instance.socket = null;
                    should_wait = true;
                }
                Instance.socketMutex.ReleaseMutex();

                if(should_wait)
                {
                    Thread.SpinWait(5);
                    continue;
                }
                break;
            } while (true);
        }
        static private void RecvThread()
        {
            MsgBuffer msgBuffer = new MsgBuffer();
            while(true)
            {
                //如果没有建立好的连接，就先等待
                WaitForConnection();

                Instance.socketMutex.WaitOne();
                if (msgBuffer.TryRecv(Instance.socket))
                {
                    MsgDispatcher.DispatchMsg(msgBuffer.Msg);
                    msgBuffer.Reset();
                }
                Instance.socketMutex.ReleaseMutex();
            }
        }
        void IClose.Close() 
        {
            socket?.GetStream().Write(Protocol.MakeMsg(MsgType.UserQuit).SerializeToBytes());
            socket?.Client.Disconnect(false);
            socket?.Close();
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
                Instance.Sending = true;
                await Instance.socket.GetStream().WriteAsync(bytes);
            }
            catch(Exception e)
            {
                Instance.LastError = e;
                Instance.SendFailFlag = true;
            }
            finally
            {
                Instance.Sending = false;
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
