using System;
using System.IO;
using Google.Protobuf;
using UnityEngine;

namespace Alurax
{
    public interface INetPacketMetaSerializer
    {
        public byte[] Serialize(ushort msgId, int bodyLen);
        public void Deserialize(byte[] data, int offset, int length, out ushort msgId, out int bodyLen);
        public IMessage GetIMessage(ushort msgId, byte[] data, int offset, int length);
    }
    
    public struct NetPacket
    {
        internal static INetPacketMetaSerializer CustomMetaSerializer;
        public MemoryStream Send { get; private set; }
        public IMessage Receive { get; private set; }
        public ushort MsgId { get; private set; }
        
        internal void Serialize(MemoryStream stream, ushort msgId, byte[] bodyData)
        {
            stream.Position = 0;
            stream.SetLength(0);
            var headData = CustomMetaSerializer.Serialize(msgId, bodyData.Length);
            stream.Write(SerializeUShort((ushort)headData.Length), 0, sizeof(ushort));
            stream.Write(headData);
            stream.Write(bodyData);
            MsgId = msgId;
            Send= stream;
        }
        internal bool Deserialize(byte[] buffer,int len, int index,out int processLen)
        {
            // 读取数据
            processLen = 0;
            int bodyLen;
            ushort headLen;
            ushort msgId;
            
            try
            {
                headLen = DeserializeUShort(buffer,index);
                CustomMetaSerializer.Deserialize(buffer, index + sizeof(ushort), headLen, out msgId, out bodyLen);
            }
            catch (Exception e)
            {
                Log.E($"Deserialize error: {e}");
                return false;
            }
            // 校验完整长度
            int packetLen = sizeof(ushort) + headLen + bodyLen;
            if (len - index < packetLen)      
                return false;
            MsgId = msgId;
            var bodyOffset = index + sizeof(ushort) + headLen;
            var bodyLength =  bodyLen;
            Receive = CustomMetaSerializer.GetIMessage(MsgId,buffer, bodyOffset, bodyLength);
            processLen = packetLen;
            return true;
        }
        
        static ushort DeserializeUShort(byte[] buf, int index = 0)
        {
            ushort num = BitConverter.ToUInt16(buf, index);
            byte[] bytes = BitConverter.GetBytes(num);
            
            ushort res = (ushort)((bytes[0] << 8) | bytes[1]);
            return res;
        }
        static byte[] SerializeUShort(ushort num)
        {
            byte[] buf = new byte[2];
            buf[0] = (byte)((num >> 8) & 0xFF);
            buf[1] = (byte)(num & 0xFF);
            return buf;
        }
    }
}