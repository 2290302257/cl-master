using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class NetConfig
    {
        public int RecvBufferSize = 1 * 1024 * 1024; // 最大的接收缓存大小 1MB
        public int KeepAliveIntervalMS = 3000; // 心跳包发送间隔
        public int ReConnectMaxCount = 5; //重连最大次数
        public INetPacketMetaSerializer MetaSerializer = null;
    }
}
