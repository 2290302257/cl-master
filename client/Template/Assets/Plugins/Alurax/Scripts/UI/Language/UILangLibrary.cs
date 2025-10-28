using System;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    [Serializable]
    public class LangLibraryData
    {
        public UILangLibrary library;
        public string key;
    }
    
    public class UILangLibrary : MonoBehaviour
    {
        [Serializable]
        public class LangData
        {
            public string name;
            public string preview;
        }
        public string RootName;
        public List<LangData> langs = new List<LangData>();
        private Dictionary<string, string> _dictLang = new Dictionary<string, string>();
        private void Awake()
        {
            foreach (var lang in langs)
                _dictLang[lang.name] = lang.preview;
        }

        public string GetText(string name, params object[] args)
        {
            string defaultValue;
            if(!_dictLang.TryGetValue(name,out defaultValue))
                defaultValue= name;
            return Lang.GetText($"{GetFullKey(name)}",defaultValue,args);
        }
        
#if UNITY_EDITOR
        public string GetTextEditor(string name, params object[] args)
        {
            foreach (var lang in langs)
            {
                if (lang.name == name)
                    return string.Format(lang.preview,args);
            }
            return name;
        }
#endif
        
        public string GetFullKey(string name)
        {
            return $"{RootName}_{name}";
        }
    }
}
