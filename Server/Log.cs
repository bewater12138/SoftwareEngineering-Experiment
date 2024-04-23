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
            Console.WriteLine($"[{DateTime.Now}] {Socket2String(client)} 已连入");
        }
        static public void ThreadStart(string tname)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[{DateTime.Now}] 线程启动 {tname}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        public enum DisconnectReason
        {
            LoginFail,
            NetworkError,
        }
        static public void UncheckedClientDisconnect(TcpClient uncheck, DisconnectReason reason)
        {
            Console.WriteLine($"[{DateTime.Now}] 未验证用户断开连接 {Socket2String(uncheck)} {reason}");
        }
        static public void ClientQuit(TcpClient uncheck)
        {
            Console.WriteLine($"[{DateTime.Now}] 用户退出 {Socket2String(uncheck)}");
        }
        static public void AcceptUser(Client client) 
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[{DateTime.Now}] 用户登入 id:{client.Account.Id} name:{client.Account.Name}");
            Console.ForegroundColor = ConsoleColor.White;
        }

        static public void LogInfo(string info)
        {
            Console.WriteLine($"[{DateTime.Now}] " + info);
        }

        static public void LogError(string error) 
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[{DateTime.Now}] " + error);
            Console.ForegroundColor= ConsoleColor.White;
        }

        static private string? Socket2String(TcpClient client)
        {
            return client.Client.RemoteEndPoint?.ToString();
        }
    }
}
