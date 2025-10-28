using System;
using System.Diagnostics;
using Cysharp.Text;

public static class Log
{
    static Action<string> Info = PrintLog;
    static Action<string> Warning= PrintWarning;
    static Action<string> Error = PrintError;
    
    enum LogEnum: byte
    {
        Verbose,
        Debug,
        Info,
        Warn,
        Error
    }
    
    [Conditional("UNITY_EDITOR")]
    public static void V(object o)
    {
        V(o.ToString());
    }
    [Conditional("UNITY_EDITOR")]
    public static void V(string format, params object[] args)
    {
        V(ZString.Format(format,args));
    }
    [Conditional("UNITY_EDITOR")]
    public static void V(string str)
    {
        InternalLog(LogEnum.Verbose,str);
    }
    
    [Conditional("UNITY_EDITOR")]
    [Conditional("UNITY_STANDALONE_WIN")]
    public static void D(object o)
    {
        D(o.ToString());
    }
    
    [Conditional("UNITY_EDITOR")]
    [Conditional("UNITY_STANDALONE_WIN")]
    public static void D(string str)
    {
        InternalLog(LogEnum.Debug,str);
    }
    
    public static void I(object o)
    {
        I(o.ToString());
    }
    
    public static void I(string str)
    {
        InternalLog(LogEnum.Info,str);
    }

    public static void W(object o)
    {
        W(o.ToString());
    }
    
    public static void W(string str)
    {
        InternalLog(LogEnum.Warn, str);
    }
    
    public static void E(object o)
    {
        E(o.ToString());
    }
    
    public static void E(string str)
    {
        InternalLog(LogEnum.Error, str);
    }
    
    public static bool EnableVerbose = true;
    public static bool EnableDebug   = true;
    public static bool EnableInfo    = true;
    public static bool EnableWarn    = true;
    public static bool EnableError   = true;
    
    static void InternalLog(LogEnum type, string text)
    {
        switch (type)
        {
            case LogEnum.Verbose:
                if (EnableVerbose)
                    Info?.Invoke(text);
                break;
            case LogEnum.Debug:
                if(EnableDebug)
                    Info?.Invoke(text);
                break;
            case LogEnum.Info:
                if(EnableInfo)
                    Info?.Invoke(text);
                break;
            case LogEnum.Warn:
                if(EnableWarn)
                    Warning?.Invoke(text);
                break;
            case LogEnum.Error:
                if(EnableError)
                    Error?.Invoke(text);
                break;
        }
    }

    static Utf16ValueStringBuilder Content(string str)
    {
        var sb = ZString.CreateStringBuilder();
        DateTime time = DateTime.Now;
        sb.AppendFormat("{0:00}/{1:00} {2:00}:{3:00}:{4:00}.{5:000}: {6}",
            time.Month,time.Day,time.Hour, time.Minute,time.Second,time.Millisecond,str);
        return sb;
    }
    
    static void PrintLog(string str)
    {
        using (Utf16ValueStringBuilder builder = Content(str))
        {
            UnityEngine.Debug.Log(builder);
        }
    }
    static void PrintWarning(string str)
    {
        using (Utf16ValueStringBuilder builder = Content(str))
        {
            UnityEngine.Debug.LogWarning(builder);
        }
    }

    static void PrintError(string str)
    {
        using (Utf16ValueStringBuilder builder = Content(str))
        {
            UnityEngine.Debug.LogError(builder);
        }
    }
}
