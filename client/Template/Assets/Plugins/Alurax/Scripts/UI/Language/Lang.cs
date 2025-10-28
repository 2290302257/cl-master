using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class Lang
    {

        // public static SystemLanguage SystemLanguage =>  Application.systemLanguage;
        // public static SystemLanguage CurrentLanguage { get; private set; }  = SystemLanguage.Chinese;
        //
        //
         private static Dictionary<string, string> s_Contents = new Dictionary<string, string>();
        //
        // public static void SetLang(SystemLanguage language)
        // {
        //     CurrentLanguage = language;
        //     s_Contents.Clear();
        // }
        
        
        public static string GetText(string key)
        {
            return GetText(key,key);
        }
        
        public static string GetText(string key, string defaultValue)
        {
            string value;
            if (!s_Contents.TryGetValue(key, out value))
                value = defaultValue;
            return value;
        }
        
        public static string GetText(string key,params object[] args)
        {
            return GetText(key, key, args);
        }
        
        public static string GetText(string key,string defaultValue,params object[] args)
        {
            string value;
            if (!s_Contents.TryGetValue(key, out value))
                value = defaultValue;
            return string.Format(value,args);
        }
    }
}
