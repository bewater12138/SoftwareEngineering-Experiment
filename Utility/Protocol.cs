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
        CheckId
    }

    [Serializable]
    public class NetworkMsgHead
    {
        public MsgType type;
        public int charLength;
        public int byteLength => charLength * sizeof(char);
        public const int SerializedCharLength = 16;
        public const int SerializedByteLength = SerializedCharLength * sizeof(char);

        public bool RecvForm(TcpClient socket)
        {
            if(socket.Available>=SerializedByteLength)
            {
                byte[] bytes = new byte[SerializedByteLength];
                socket.GetStream().Read(bytes, 0, SerializedByteLength);
                string str = Encoding.Default.GetString(bytes, 0, bytes.Length);
                type = (MsgType)int.Parse(str.Substring(0, 8));
                charLength = int.Parse(str.Substring(8, 8));
                return true;
            }

            return false;
        }
    }

    public class NetworkMsgBody
    {
        public string jsonData = string.Empty;
        public bool RecvFrom(TcpClient socket,int need_byte_count) 
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
            var res = $"{(int)Head.type,8}{Head.charLength,8}";
            res += Body.jsonData;
            return res ;
        }
        public bool RecvBodyFrom(TcpClient socket)
        {
            return Body.RecvFrom(socket, Head.byteLength);
        }
    }

    public static class Protocol
    {
        static public NetworkMsg MakeMsg(MsgType type, object data)
        {
            NetworkMsg msg = new NetworkMsg();
            msg.Head.type = type;
            msg.Body.jsonData = JsonSerializer.Serialize(data);
            msg.Head.charLength = msg.Body.jsonData.Length;
            return msg;
        }
    }

    namespace NetworkObject
    {
        public class IDandPassword
        {
            public string id = string.Empty;
            public string password = string.Empty;
            public IDandPassword(string id, string password)
            {
                this.id = id;
                this.password = password;
            }

        }
    }
}
