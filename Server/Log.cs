using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    internal static class Log
    {
        static public void AcceptSocket(TcpClient client)
        {
            Console.WriteLine($"[{DateTime.Now}] {client.Client.RemoteEndPoint?.ToString()} 已连入");
        }
        static public void ThreadStart(string tname)
        {
            Console.WriteLine($"[{DateTime.Now}] 线程启动 {tname}");
        }
        static public void LogInfo(string info)
        {
            Console.WriteLine($"[{DateTime.Now}] " + info);
        }
    }
}
