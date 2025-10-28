using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Alurax
{
    [CreateAssetMenu(menuName ="Custom/打包/AB配置文件")]
    public class ABPack :ScriptableObject
    {
        public string PackName;
        public string Extension;
        public ABBaseRule Rule;

        public bool Builtin=true;
        public List<Object> DirAssets; 
        
#if UNITY_EDITOR
        public List<string> GetFiles(bool onlyBuiltIn)
        {
            List<string> list= new List<string>();
            if (onlyBuiltIn && !Builtin) return list;
            HashSet<string> checkName = new HashSet<string>();
            foreach (var dirAsset in DirAssets)
            {
                if (dirAsset != null)
                {
                    var dir = AssetDatabase.GetAssetPath(dirAsset);
                    if (AssetDatabase.IsValidFolder(dir))
                    {
                        HashSet<string> exts = new HashSet<string>(Extension.Split(","));
                        foreach (var file in Directory.GetFiles(dir, "*.*", SearchOption.AllDirectories))
                        {
                            if (!file.Contains(".svn"))
                            {
                                var ext = Path.GetExtension(file);
                                if (ext != ".meta")
                                {
                                    if (exts.Contains("*.*") || exts.Contains($"*{ext}"))
                                    {
                                        var path = file.Replace("\\", "/");
                                        list.Add(path);
                                        if (!checkName.Add(path))
                                        {
                                            Debug.LogError($"file in the same package {path}");
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return list;
        }


 
        public List<ABBaseRule.PackData> Pack(bool onlyBuiltIn)
        {
            List<ABBaseRule.Data> passData = new List<ABBaseRule.Data>();
            foreach (var file in GetFiles(onlyBuiltIn))
            {
                passData.Add(new ABBaseRule.Data() { name = file, alias = Path.GetFileName(file) });
            }
            if (passData.Count == 0)
                return new List<ABBaseRule.PackData>();
            else
                return Rule.Pack(PackName, this,passData);
        }
#endif
    }


}

