using Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Reflection.Metadata.Ecma335;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.NetworkObject;
#pragma warning disable 8604

namespace Server
{
    class UncheckedClient
    {
        int begTickTime;
        const int timeLimit = 5000;
        TcpClient socket;
        NetworkStream stream;
        MsgBuffer buffer;
        const int idLength = 10;
        const int passwordLength = 10;
        State state = State.Unchecked;

        public NetworkStream Stream => stream;
        public TcpClient Socket => socket;
        public Account? Account {get;protected set;}
        public bool Unchecked => state == State.Unchecked;
        public bool Success => state == State.Success;
        public bool Fail => state == State.Fail;
        public bool Disconnected {get{
                return
                    state == State.Disconnect ||
                    begTickTime + timeLimit < System.Environment.TickCount;
                    //||socket.Client.Poll(0,SelectMode.SelectRead);
            } }

        public enum State
        {
            Unchecked, Success, Fail, Wait, Disconnect
        }

        public UncheckedClient(TcpClient socket)
        {
            this.begTickTime = System.Environment.TickCount;
            this.Account = null;
            this.socket = socket;
            this.stream = socket.GetStream();
            this.buffer = new MsgBuffer();
        }

        public State TryCheck()
        {
            if (buffer.TryRecv(socket))
            {
                bool wrong = false;
                IDandPassword? id_pwd = buffer.Msg.Body.Parse<IDandPassword>();
                if (id_pwd == null)
                {
                    wrong = true;
                }
                else
                {
                    Account = Security.FindAccount(id_pwd.id, id_pwd.password);
                    if (Account == null)
                    {
                        wrong = true;
                    }
                }

                if(wrong)
                {
                    bool ok = Network.SendMsg(socket, Protocol.MakeMsg(MsgType.LoginFail));
                    if (ok)
                    {
                        state = State.Fail;
                        return State.Fail;
                    }
                    else
                    {
                        state = State.Disconnect;
                        return State.Disconnect;
                    }
                }
                else
                {
                    Network.SendMsg(socket, Protocol.MakeMsg(MsgType.LoginSuccess, new AccountInfo(Account)));
                    state = State.Success;
                    return State.Success;
                }
            }

            return state;
        }
    }

    class Security : SingleInstance<Security>, IClose
    {
        bool shouldClose= false;
        List<Client> clients = new List<Client>();
        List<UncheckedClient> uncheckedClients = new List<UncheckedClient>();
        public List<Client> Clients => clients;
        public List<UncheckedClient> UncheckedClients => uncheckedClients;

        public Security()
        {

        }

        static public void Run()
        {
            Task.Run(CheckThread);
        }

        static private void CheckThread()
        {
            Log.ThreadStart(nameof(CheckThread));
            while (!Instance.shouldClose)
            {
                Check();
                ClearDisconnected();
            }
        }

        private static void Check()
        {
            //等待有未确认客户
            do
            {
                int unchecked_count = 0;
                lock (Instance.uncheckedClients)
                {
                    unchecked_count = Instance.uncheckedClients.Count;
                }

                if (unchecked_count == 0)
                {
                    Thread.SpinWait(5);
                    continue;
                }
                break;
            } while (true);

            lock (Instance.uncheckedClients)
            {
                foreach (var c in Instance.uncheckedClients)
                {
                    //Log.LogInfo(c.Socket.Available.ToString());
                    var res = c.TryCheck();

                    //登录成功，加入客户连接
                    if (res == UncheckedClient.State.Success)
                    {
                        lock (Instance.clients)
                        {
                            Client client = new Client(c, c.Account);
                            Log.AcceptUser(client);
                            Instance.clients.Add(client);
                        }
                    }
                }

                Instance.uncheckedClients.RemoveAll(c =>
                {
                    if (c.Fail)
                    {
                        Log.UncheckedClientDisconnect(c.Socket, Log.DisconnectReason.LoginFail);
                        c.Socket.Close();
                        return true;
                    }
                    else if(c.Success)
                    {
                        return true;
                    }
                    return false;
                }
                );
            }
        }
        private static void ClearDisconnected()
        {
            lock(Instance.uncheckedClients)
            {
                Instance.uncheckedClients.RemoveAll(uc =>
                {
                    if (uc.Disconnected)
                    {
                        uc.Socket.Close();
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
            }
        }

        private Account? m_FindAccount(string id, string password)
        {
            Thread.Sleep(1000);
            if (id == "111" && password == "222")
            {
                return new Account("111", "lyl", Privilege.Admin);
            }
            return null;
        }

        static public Account? FindAccount(string id, string password)
        {
            return Instance.m_FindAccount(id, password);
        }

        void IClose.Close()
        {

        }
    }
}
