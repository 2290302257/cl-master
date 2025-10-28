using System;
using UnityEngine.Profiling;

namespace Alurax
{
    public class ProfilerUtils 
    {
        public static void BeginRecord(string path,float length,Action finish = null)
        {
            Profiler.logFile = path;
            Profiler.enabled = true;
            Profiler.enableBinaryLog = true;
            
            TaskManager.Timeout(() =>
            {
                Profiler.enableBinaryLog = false;
                Profiler.enabled = false;
                finish?.Invoke();
            },length);
        }
    }
}
