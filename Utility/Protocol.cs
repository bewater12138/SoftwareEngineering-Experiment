using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Utility
{
    public enum MsgType
    {
        CheckId,LoginFail,LoginSuccess,UserQuit,RegisterAccount,SocketDisconnect
    }

    [Serializable]
    public class NetworkMsgHead
    {
        public MsgType type;
        public int byteLength;
        public const int SerializedCharLength = 16;
        public const int SerializedByteLength = SerializedCharLength;

        public bool RecvForm(TcpClient socket)
        {
            if (socket.Available >= SerializedByteLength)
            {
                byte[] bytes = new byte[SerializedByteLength];
                socket.GetStream().Read(bytes, 0, SerializedByteLength);
                string str = Encoding.Default.GetString(bytes, 0, bytes.Length);
                type = (MsgType)int.Parse(str.Substring(0, 8));
                byteLength = int.Parse(str.Substring(8, 8));
                return true;
            }

            return false;
        }
    }

    public class NetworkMsgBody
    {
        public string jsonData = string.Empty;
        public bool RecvFrom(TcpClient socket, int need_byte_count)
        {
            if (socket.Available >= need_byte_count)
            {
                byte[] bytes = new byte[need_byte_count];
                socket.GetStream().Read(bytes, 0, need_byte_count);
                string str = Encoding.Default.GetString(bytes, 0, bytes.Length);
                jsonData = str;
                return true;
            }
            return false;
        }
        public T? Parse<T>() where T : class
        {
            T? ret = null;
            try
            {
                ret = JsonSerializer.Deserialize(jsonData, typeof(T)) as T;
            }
            catch
            {

            }
            return ret;
        }
    }

    [Serializable]
    public class NetworkMsg
    {
        public NetworkMsgHead Head { get; set; }
        public NetworkMsgBody Body { get; set; }
        public NetworkMsg()
        {
            Head = new NetworkMsgHead();
            Body = new NetworkMsgBody();
        }

        public string Serialize()
        {
            var res = $"{(int)Head.type,8}{Head.byteLength,8}";
            res += Body.jsonData;
            return res;
        }
        public byte[] SerializeToBytes()
        {
            string str = Serialize();
            return Encoding.Default.GetBytes(str);
        }

        public bool RecvHeadFrom(TcpClient socket)
        {
            return Head.RecvForm(socket);
        }
        public bool RecvBodyFrom(TcpClient socket)
        {
            return Body.RecvFrom(socket, Head.byteLength);
        }
    }

    public class MsgBuffer
    {
        public NetworkMsg Msg {get; protected set;}
        public bool HaveMsg { get; protected set;}
        private bool haveRecvHead = false;

        public MsgBuffer()
        {
            Msg = new NetworkMsg();
            HaveMsg = false;
        }

        public bool TryRecv(TcpClient socket)
        {
            if(HaveMsg) { return false; }

            if (!haveRecvHead)
            {
                if(Msg.Head.RecvForm(socket))
                {
                    haveRecvHead = true;
                }
            }

            if(haveRecvHead)
            {
                if(Msg.RecvBodyFrom(socket))
                {
                    HaveMsg = true;
                    return true;
                }
            }

            return false;
        }

        public void Reset()
        {
            haveRecvHead=false;
            HaveMsg = false;
        }
    }

    public static class Protocol
    {
        static public NetworkMsg MakeMsg(MsgType type, object data)
        {
            NetworkMsg msg = new NetworkMsg();
            msg.Head.type = type;
            msg.Body.jsonData = JsonSerializer.Serialize(data,data.GetType());
            msg.Head.byteLength = msg.Body.jsonData.Length;
            return msg;
        }

        static public NetworkMsg MakeMsg(MsgType type)
        {
            NetworkMsg msg = new NetworkMsg();
            msg.Head.type = type;
            msg.Body.jsonData = "";
            return msg;
        }


        static public void Send(this TcpClient socket, MsgType type, object network_object)
        {
            socket.GetStream().Write(MakeMsg(type, network_object).SerializeToBytes());
        }
        static public void Send(this TcpClient socket, MsgType type)
        {
            socket.GetStream().Write(MakeMsg(type).SerializeToBytes());
        }
    }

    namespace NetworkObject
    {
        public class IDandPassword
        {
            public string id { get; set; }
            public string password { get; set; }
            public IDandPassword(string id, string password)
            {
                this.id = id;
                this.password = password;
            }
        }

        public class AccountInfo
        {
            public Account? Account { get; set; }
            public AccountInfo(Account account)
            {
                Account = account;
            }
        }

        public class RegisterInfo
        {
            public string Name {  get; set; }
            public string ID {  get; set; }
            public string Password { get; set; }
            public RegisterInfo(string name, string iD, string password)
            {
                Name = name;
                ID = iD;
                Password = password;
            }
        }

        public class StudentInfo:Student
        {

        }
    }
}
