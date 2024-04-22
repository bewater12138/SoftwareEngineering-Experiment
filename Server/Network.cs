using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utility;
#pragma warning disable 8602

namespace Server
{
    internal class Network : SingleInstance<Network>,IClose
    {
        TcpListener? listener;
        bool shouldClose = false;

        List<Client> clients = new List<Client>();
        List<UncheckedClient> uncheckedClients = new List<UncheckedClient>();

        public Network()
        {
            try 
            {
                listener = new TcpListener(System.Net.IPAddress.Any, 10027);
            }
            catch(System.Exception e) 
            {
                Log.LogInfo("TCP listener 创建失败");
                Console.WriteLine(e);
            }
            Log.LogInfo("TCP listener 创建成功");
        }

        static private void ListenThread()
        {
            Log.ThreadStart(nameof(ListenThread));
            while (!s_Instance.shouldClose)
            {
                var c = s_Instance.listener.AcceptTcpClient();
                lock(s_Instance.uncheckedClients)
                s_Instance.uncheckedClients.Add(new UncheckedClient(c));
                Log.AcceptSocket(c);
            }
        }
        static private void CheckThread()
        {
            Log.ThreadStart(nameof (CheckThread));
            while (!s_Instance.shouldClose)
            {
                int unchecked_count = 0;
                lock(s_Instance.uncheckedClients)
                {
                    unchecked_count = s_Instance.uncheckedClients.Count;
                }

                if(unchecked_count == 0)
                {
                    Thread.SpinWait(5);
                }

                lock (s_Instance.uncheckedClients)
                {
                    foreach (var c in s_Instance.uncheckedClients)
                    {
                        var res = c.TryCheck();

                        //登录成功，加入客户连接
                        if(res == UncheckedClient.State.Success)
                        {
                            lock (s_Instance.clients)
                            { 
                                s_Instance.clients.Add(new Client(c));
                            }
                        }
                    }

                    s_Instance.uncheckedClients.RemoveAll(c =>
                    {
                        if (!c.Unchecked)
                        {
                            //c.Stream.Dispose();
                            return true;
                        }
                        return false;
                    }
                    );
                }
            }
        }
        static private void ServiceThread()
        {
            Log.ThreadStart(nameof(ServiceThread));
            while(!s_Instance.shouldClose)
            {
                //没有客户连接，就先等待
                bool should_wait = false;
                lock(s_Instance.clients)
                {
                    if(s_Instance.clients.Count == 0)
                    {
                        should_wait = true;
                    }
                }
                if(should_wait)
                {
                    Thread.SpinWait(5);
                    continue;
                }

                lock(s_Instance.clients)
                {
                    foreach(var c in s_Instance.clients)
                    {
                        c.RecvMsg();
                    }
                }
            }
        }

        static public void Start()
        {
            try
            {
                s_Instance?.listener?.Start();
            }
            catch(System.Exception e) 
            {
                Console.WriteLine(e);
                return;
            }

            Task.Run(ListenThread);
            Task.Run(CheckThread);
            Task.Run(ServiceThread);
        }

        void IClose.Close() 
        {
        
        }
    }

    class Client
    {
        TcpClient socket;
        NetworkStream stream;
        bool recvdHead;
        NetworkMsg currentMsg;

        public Client(UncheckedClient uclient)
        {
            this.socket = uclient.Socket;
            this.stream = uclient.Stream;
            recvdHead = false;
            currentMsg = new NetworkMsg();
        }
        public void RecvMsg()
        {
            //接收消息头
            if(!recvdHead)
            {
                recvdHead = currentMsg.Head.RecvForm(socket);
            }

            //接收消息体
            if(recvdHead)
            {
                if(currentMsg.RecvBodyFrom(socket))
                {

                }
            }
        }
    }
 
    class UncheckedClient
    {
        TcpClient socket;
        NetworkStream stream;
        byte[] buffer;
        const int idLength = 10;
        const int passwordLength = 10;
        State state = State.Unchecked;

        public NetworkStream Stream => stream;
        public TcpClient Socket => socket;
        public bool Unchecked =>state == State.Unchecked;
        public bool Success =>state == State.Success;
        public bool Fail => state == State.Fail;

        public enum State
        {
            Unchecked,Success,Fail,
        }

        public UncheckedClient(TcpClient socket)
        {
            this.socket = socket;
            stream = socket.GetStream();
            buffer = new byte[1024];
        }

        public State TryCheck()
        {
            return State.Unchecked;
        }
    }
}
