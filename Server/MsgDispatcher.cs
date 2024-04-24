using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Server
{
    static internal class MsgDispatcher
    {
        static public void Dispatch(Client client, NetworkMsg msg)
        {
            Log.LogInfo($"接收到客户消息 client={client.Socket.Client.RemoteEndPoint?.ToString()} msg.type={msg.Head.type}");

            switch (msg.Head.type)
            {
                case MsgType.UserQuit: client.SetShouldDisconnect(true); break;
            }
        }
    }
}
