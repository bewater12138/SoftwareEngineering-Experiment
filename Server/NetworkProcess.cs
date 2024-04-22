using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Utility;
using Utility.NetworkObject;

namespace Server
{
    internal class NetworkProcess
    {
        static public void DispatchMsg(TcpClient client,NetworkMsg msg)
        {

            Log.LogInfo($"接收到客户消息 client={(client.Client.RemoteEndPoint == null ? "null" : client.Client.RemoteEndPoint.ToString())} msg.type={msg.Head.type}");
        }
    }
}
