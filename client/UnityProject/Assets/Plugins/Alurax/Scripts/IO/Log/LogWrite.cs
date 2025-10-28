using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

namespace Alurax
{
    public class LogWrite
    {
        private const int LOG_MAX_LINE = 1000000;
        private const int EXCEPTION_MAX_LINE = 1000;
        
        static bool s_Enable;
        static int s_LogCount;
        static int s_ExceptionCount;
        static bool s_LogMax;
        static bool s_ExceptionMax;
        static string s_Filename = "";
        static StringBuilder s_LogCache = new StringBuilder();
        public static string LogPath => s_Filename;
        public static void Start(string buildNumber,string aliasName,bool startDelete, bool deleteYesterday,string overridePath = null)
        {
            if (!Application.isEditor)
            {
                s_Enable = true;
                s_LogCount = s_ExceptionCount = 0;
                s_LogMax = s_ExceptionMax = false;
                var name = $"{Application.productName}_{DateTime.Today:yyyy_MM_dd}";
                string path;

                if (!string.IsNullOrEmpty(overridePath))
                    path = overridePath;
                else
                {
                    if (Application.isMobilePlatform)
                        path = Application.persistentDataPath;
                    else
                        path = Application.dataPath + "/..";
                }
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                
                if (string.IsNullOrEmpty(aliasName))
                    s_Filename = $"{path}/{name}.log";
                else
                    s_Filename = $"{path}/{name}_{aliasName}.log";

                if (startDelete)
                {
                    if (File.Exists(s_Filename))
                        File.Delete(s_Filename);
                }
                
                if (deleteYesterday)
                {
                    foreach (var log in Directory.GetFiles(path, "*.log", SearchOption.TopDirectoryOnly))
                    {
                        var fileName = Path.GetFileNameWithoutExtension(log);
                        if (fileName.StartsWith(Application.productName) &&
                            !fileName.Contains($"{DateTime.Today:yyyy_MM_dd}"))
                            File.Delete(log);
                    }
                }

                Application.logMessageReceivedThreaded -= Add;
                Application.logMessageReceivedThreaded += Add;
                TaskManager.Remove(Update);
                TaskManager.Update(Update);
                Debug.Log($"//                            _ooOoo_\n//                           o8888888o\n//                           88\" . \"88\n//                           (| -_- |)\n//                            O\\ = /O\n//                        ____/`---'\\____\n//                      .   ' \\\\| |// `.\n//                       / \\\\||| : |||// \\\n//                     / _||||| -:- |||||- \\\n//                       | | \\\\\\ - /// | |\n//                     | \\_| ''\\---/'' | |\n//                      \\ .-\\__ `-` ___/-. /\n//                   ___`. .' /--.--\\ `. . __\n//                .\"\" '< `.___\\_<|>_/___.' >'\"\".\n//               | | : `- \\`.;`\\ _ /`;.`/ - ` : | |\n//                 \\ \\ `-. \\_ __\\ /__ _/ .-` / /\n//         ======`-.____`-.___\\_____/___.-`____.-'======\n//                            `=---='\n//\n//         .............................................\n//                  佛祖镇楼                  BUG辟易\n//              佛曰:{DateTime.Now:yyyy-MM-dd HH:mm:ss} buildNumber:{buildNumber}\n\n");
            }
        }
        
        static void Update()
        {
            if (s_LogCache.Length > 0)
            {
                try
                {
                    File.AppendAllText(s_Filename, s_LogCache.ToString());
                }
                catch (Exception e)
                {
                    //Log.E("=== LogWrite Log === exception: {0}", e.Message);
                }
                s_LogCache.Clear();
            }
        }
        
        static void Add(string logString, string stackTrace, LogType type)
        {
            if (s_Enable)
            {
                if(s_LogCount < LOG_MAX_LINE)
                {
                    var isLog = type == LogType.Log || type == LogType.Warning;
                    if (isLog)
                    {
                        s_LogCache.Append(logString).AppendLine();
                        s_LogCount++;
                    }else
                    {
                        if (s_ExceptionCount < EXCEPTION_MAX_LINE)
                        {
                            s_LogCache.Append(logString).AppendLine();
                            s_LogCache.Append(stackTrace).AppendLine();
                            s_ExceptionCount++;
                        }
                        else if (!s_ExceptionMax)
                        {
                            s_ExceptionMax = true;
                            s_LogCache.Append($"Exception is MAX  {EXCEPTION_MAX_LINE}").AppendLine();
                        }
                    }
                }
                else if (!s_LogMax)
                {
                   s_LogMax = true;
                   s_LogCache.Append($"Log is MAX  {LOG_MAX_LINE}").AppendLine();
                }
            }
        }
    }
}