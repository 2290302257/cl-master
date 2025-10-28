using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using kcp2k;
using UnityEngine;

namespace Alurax
{
    public class NetLocalServer 
    {
        protected readonly Action<ArraySegment<byte>> OnClientData;
        protected readonly Action<int, ArraySegment<byte>> OnServerData;
        private int _serverClientId;
        
        public NetLocalServer(
            int serverClientId,
            Action<ArraySegment<byte>> OnClientData,
            Action<int, ArraySegment<byte>> OnServerData)
        {
            this._serverClientId = serverClientId;
            this.OnClientData = OnClientData;
            this.OnServerData = OnServerData;

        }

        public void SendToServer(ArraySegment<byte> payload)
        {
            this.OnServerData?.Invoke(this._serverClientId,payload);
        }
        
        public void SendToClient(int clientId, ArraySegment<byte> payload)
        {
            if (clientId == this._serverClientId)
            {
                this.OnClientData?.Invoke(payload);
            }
        }
    }
}
