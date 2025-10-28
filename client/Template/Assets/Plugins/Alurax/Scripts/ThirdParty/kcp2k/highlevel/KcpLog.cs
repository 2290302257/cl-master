// A simple logger class that uses Console.WriteLine by default.
// Can also do Logger.LogMethod = Debug.Log for Unity etc.
// (this way we don't have to depend on UnityEngine)
using System;

namespace kcp2k
{
    public static class KcpLog
    {
        public static Action<string> Info;
        public static Action<string> Warning;
        public static Action<string> Error   = Log.E;
    }
}
