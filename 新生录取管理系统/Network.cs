using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
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
    class Msg(MsgType type,object? data)
    {
        public MsgType Type { get; set; } = type;
        public object? Data { get; set; } = data;
    }

    partial class Network
    {
        static public bool ConnectServer()
        {
            try
            {
                Instance.socketMutex.WaitOne();
                if (Instance.socket != null)
                {
                    Instance.socket.GetStream().Write(Protocol.MakeMsg(MsgType.SocketDisconnect).SerializeToBytes());
                    Instance.socket.Close();
                }
                Instance.socket = new TcpClient("127.0.0.1", 10027);
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

        static public void SendToServerAsync(MsgType type, object data)
        {
            lock (Instance.SendQueue)
            {
                Instance.SendQueue.Enqueue(new Msg(type, data));
            }
        }

        static public void SendToServerAsync(MsgType type)
        {
            lock (Instance.SendQueue)
            {
                Instance.SendQueue.Enqueue(new Msg(type, null));
            }
        }

        static public void SendToServerAsync(string id, string password)
        {
            SendToServerAsync(MsgType.CheckId, new IDandPassword(id, password));
        }
    }

    partial class Network:SingleInstance<Network>,IClose
    {
        TcpClient? socket;
        Mutex socketMutex = new Mutex();
        public TcpClient? ClientSocket => socket;
        public bool RecvdMsg {  get; protected set; }
        public bool SendFailFlag { get; protected set; }
        public Exception? LastError { get; protected set; }

        //发送队列
        private Queue<Msg> SendQueue = new Queue<Msg>();

        public Network() 
        {
            Task.Run(RecvThread);
            Task.Run(SendThread);
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
                    if (!Thread.Yield())
                    {
                        Thread.Sleep(1);
                    }
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
                    MsgDispatcher.Dispatch(msgBuffer.Msg);
                    msgBuffer.Reset();
                }
                Instance.socketMutex.ReleaseMutex();
            }
        }

        static private void WaitForSend()
        {
            while (true)
            {
                bool should_await = false;
                lock (Instance.SendQueue)
                {
                    if(Instance.SendQueue.Count == 0)should_await = true;
                }

                if(should_await)
                {
                    if (!Thread.Yield())
                    {
                        Thread.Sleep(1);
                    }
                    continue;
                }
                else
                {
                    break;
                }
            }
        }
        static private void SendAll()
        {
            while(true)
            {
                Msg? msg = null;
                lock (Instance.SendQueue)
                    {
                        if (Instance.SendQueue.Count > 0)
                        {
                            msg = Instance.SendQueue.Dequeue();
                        }
                    }

                if (msg != null)
                {
                    try
                    {
                        if(msg.Data != null)
                        {
                            Instance.ClientSocket.Send(msg.Type, msg.Data);
                        }
                        else
                        {
                            Instance.ClientSocket.Send(msg.Type);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"发送失败 msg.type={msg.Type}");
                        Console.WriteLine(e);
                    }
                }
            }
        }

        static private void SendThread()
        {
            while(true)
            {
                WaitForSend();
                SendAll();
            }
        }
        void IClose.Close() 
        {
            try
            {
                lock (Instance.SendQueue)
                {
                    Instance.SendQueue.Clear();
                    var nmsg = Protocol.MakeMsg(MsgType.UserQuit).SerializeToBytes();
                    socket?.Client.Send(nmsg);
                }
            }
            catch
            {

            }
            socket?.Client.Disconnect(false);
            socket?.Close();
        }
    }
}
