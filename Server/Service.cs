using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Server
{
    class Client
    {
        int entryTime;
        int lastMsgTime;
        const int TimeLimit = 5 * 60_000;  //5分钟无应答自动断开连接

        TcpClient socket;
        NetworkStream stream;
        MsgBuffer buffer;
        public TcpClient Socket => socket;
        public Account Account { get; protected set; }

        bool shouldDisconnect = false;
        public bool ShouldDisconnect { get {
                return shouldDisconnect || lastMsgTime + TimeLimit < System.Environment.TickCount;
            } }

        public Client(UncheckedClient uclient, Account account)
        {
            this.entryTime = System.Environment.TickCount;
            this.lastMsgTime = System.Environment.TickCount;
            this.socket = uclient.Socket;
            this.stream = uclient.Stream;
            this.buffer = new MsgBuffer();
            Account = account;
        }
        public void RecvAndDispatchMsg()
        {
            if (buffer.TryRecv(socket))
            {
                lastMsgTime = System.Environment.TickCount;
                MsgDispatcher.Dispatch(this, buffer.Msg);
                buffer.Reset();
            }
        }

        internal void SetShouldDisconnect(bool v)
        {
            shouldDisconnect = v;
        }
    }

    internal class Service:SingleInstance<Service>,IClose
    {
        bool shouldClose = false;

        List<Client> Clients => Security.Instance.Clients;

        public Service()
        {
            Task.Run(ServiceThread);
        }

        static private void WaitForClient()
        {
            do
            {
                bool should_wait = false;
                lock (Instance.Clients)
                {
                    if (Instance.Clients.Count == 0)
                    {
                        should_wait = true;
                    }
                }
                if (should_wait)
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

        static private void ServiceThread()
        {
            Log.ThreadStart(nameof(ServiceThread));
            while (!Instance.shouldClose)
            {
                //没有客户连接，就先等待
                WaitForClient();

                lock (Instance.Clients)
                {
                    foreach (var c in Instance.Clients)
                    {
                        c.RecvAndDispatchMsg();
                    }
                }

                //移除需要断开的连接
                lock (Instance.Clients)
                {
                    Instance.Clients.RemoveAll(c =>
                    {
                        if(c.ShouldDisconnect)
                        {
                            Log.ClientQuit(c.Socket);
                            c.Socket.Close();
                            return true;
                        }
                        return false;
                    });
                }
            }
        }

        void IClose.Close()
        {

        }
    }
}
