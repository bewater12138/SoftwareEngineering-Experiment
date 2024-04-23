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

        public Network()
        {
            try 
            {
                listener = new TcpListener(System.Net.IPAddress.Any, 10027);
            }
            catch(System.Exception e) 
            {
                Log.LogError("TCP listener 创建失败");
                Console.WriteLine(e);
            }
            Log.LogInfo("TCP listener 创建成功");
        }

        static public bool SendMsg(TcpClient client,NetworkMsg msg)
        {
            var bytes = msg.SerializeToBytes();
            try
            {
                client.GetStream().Write(bytes, 0, bytes.Length);
                return true;
            }
            catch
            {
                return false;
            }
        }

        static public async void SendMsgAsync(TcpClient client, NetworkMsg msg)
        {
            await Task.Run(() => { SendMsg(client, msg); });
        }

        static private void ListenThread()
        {
            Log.ThreadStart(nameof(ListenThread));
            while (!Instance.shouldClose)
            {
                var c = s_Instance.listener.AcceptTcpClient();
                lock(Security.Instance.UncheckedClients)
                    Security.Instance.UncheckedClients.Add(new UncheckedClient(c));
                Log.AcceptSocket(c);
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
        }

        void IClose.Close() 
        {
        
        }
    }
}
