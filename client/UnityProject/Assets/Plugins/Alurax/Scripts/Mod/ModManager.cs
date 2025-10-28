using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection;

namespace Alurax
{
    public class ModManager
    {
        private static bool s_Dirty;
        private static List<ModBase> s_StartMods = new List<ModBase>();
        
        private static List<ModBase> s_ListMods = new List<ModBase>();
        private static Dictionary<Type, ModBase> s_DictMods = new Dictionary<Type, ModBase>(); 
        
        private static Dictionary<string, List<Type>> s_HandleTypes = new Dictionary<string, List<Type>>();
        
        static ModManager()
        {
            MakeModHandler();
            TaskManager.Update(Update);
        }

    #region Bind
        private static void MakeModHandler()
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if(t.IsSubclassOf(typeof(ModBase)))
                    {
                        Attribute[] attrs = Attribute.GetCustomAttributes(t, typeof(ModHandlerAttribute));
                        foreach (ModHandlerAttribute attr in attrs)
                        {
                            if (!s_HandleTypes.TryGetValue(attr.Name, out var list))
                            {
                                list = new List<Type>();
                                s_HandleTypes[attr.Name] = list;
                            }
                            list.Add(t);
                        }
                    }
                }
            }

            foreach (var type in s_HandleTypes)
            {
                string handleName = type.Key;
                type.Value.Sort((a, b) =>
                {
                    ModHandlerAttribute aAttr = null, bAttr = null;
                    foreach (ModHandlerAttribute attr in Attribute.GetCustomAttributes(a, typeof(ModHandlerAttribute)))
                    {
                        if (attr.Name == handleName)
                        {
                            aAttr = attr;
                            break;
                        }
                    }
                    
                    foreach (ModHandlerAttribute attr in Attribute.GetCustomAttributes(b, typeof(ModHandlerAttribute)))
                    {
                        if (attr.Name == handleName)
                        {
                            bAttr = attr;
                            break;
                        }
                    }

                    if (aAttr != null && bAttr != null)
                    {
                        return aAttr.Order.CompareTo(bAttr.Order);
                    }

                    return 0;
                });
            }
        }
    #endregion

        public static T Get<T>() where T : ModBase
        {
            if (s_DictMods.TryGetValue(typeof(T), out var o))
            {
                return (T)o;
            }
            return null;
        }
    
        public static void AddMods(string handleName)
        {
            if (s_HandleTypes.TryGetValue(handleName, out var types))
            {
                foreach (var t in types)
                {
                    var mod= (ModBase)Activator.CreateInstance(t);
                    Add(handleName, mod);
                }
            }
        }
        
        public static void Add(string handleName, ModBase mod)
        {
            mod.HandleName = handleName;
            
            s_Dirty = true;
            s_StartMods.Add(mod);
            s_ListMods.Add(mod);
            s_DictMods[mod.GetType()] = mod;
            
            ModBase.InternalOnCreated(mod);
        }
        
        public static void RemoveMods(string handleName)
        {
            for (int i = s_ListMods.Count - 1; i >= 0; i--)
            {
                var mod = s_ListMods[i];
                if (mod.HandleName == handleName)
                {
                    Remove(mod);
                }
            }
        }
        
        public static void Remove(ModBase mod)
        {
            ModBase.InternalOnDestroyed(mod);
            
            s_DictMods.Remove(mod.GetType());
            s_ListMods.Remove(mod);
            s_StartMods.Remove(mod);
        }

        public static void RemoveAll(params string[] excludes)
        {
            HashSet<string> hash = new HashSet<string>(excludes);
            for (int i = s_ListMods.Count - 1; i >= 0; i--)
            {
                var mod = s_ListMods[i];
                if (!hash.Contains(mod.HandleName))
                {
                    Remove(mod);
                }
            }
        }

        static void Update()
        {
            if (s_Dirty)
            {
                s_Dirty = false;
                foreach (var start in s_StartMods)
                {
                    if (start != null)
                    {
                        ModBase.InternalOnStarted(start);
                    }
                }
                s_StartMods.Clear();
            }
            
            for (int i = 0; i < s_ListMods.Count; i++)
            {
                var mod = s_ListMods[i];
                if (mod != null)
                {
                    ModBase.InternalOnUpdate(mod);
                }
                
            }
        } 
    }
}