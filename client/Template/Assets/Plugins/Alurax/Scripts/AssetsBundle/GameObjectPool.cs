using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Alurax
{
    public class GameObjectPool
    {
        struct Data
        {
            public Stack<GameObject> stack;
            public int maxCount;
        }
        private static Dictionary<GameObject, string> s_DictName =
            new Dictionary<GameObject, string>();
        private static Dictionary<string, Data> s_DictPool =
            new Dictionary<string, Data>();
        private static Dictionary<string, Action<GameObject>> s_TakeFromPool =
            new Dictionary<string, Action<GameObject>>();
        private static Dictionary<string, Action<GameObject>> s_ReturnToPool =
            new Dictionary<string, Action<GameObject>>();

        public static void TakePoolAsync(string assetName, System.Action<GameObject> callback)
        {
            TakePoolAsync(assetName, callback, true, 0);
        }

        public static void TakePoolAsync(string assetName, System.Action<GameObject> callback,bool usePool)
        {
            TakePoolAsync(assetName, callback, usePool, 0);
        }
        
        public static void TakePoolAsync(string assetName,System.Action<GameObject> callback,bool usePool,int maxCount)
        {
            if (usePool)
            {
                if (!s_DictPool.TryGetValue(assetName, out var data))
                {
                    s_DictPool[assetName] = new Data() {stack = new Stack<GameObject>() , maxCount = maxCount};
                    data = s_DictPool[assetName];
                }
                while (data.stack.Count > 0)
                {
                    var go = data.stack.Pop();
                    if (go)
                    {
                        go.gameObject.SetActive(true);
                        s_DictName[go] = assetName;
                        if(s_TakeFromPool.TryGetValue(assetName, out var takeFromPool))
                            takeFromPool?.Invoke(go);
                        callback?.Invoke(go);
                        return;  
                    }
                }
            }
            
            Assets.InstantiateAsync<GameObject>(assetName, (go) =>
            {
                if (usePool)
                {
                    s_DictName[go] = assetName;
                }
                callback?.Invoke(go);
            });
        }

        public static void ReturnPool(GameObject go)
        {
            if (go)
            {
                if (s_DictName.TryGetValue(go, out var assetName))
                {
                    if (s_DictPool.TryGetValue(assetName, out var data))
                    {
                        if (!data.stack.Contains(go))
                        {
                            if (data.maxCount == 0 || data.stack.Count < data.maxCount)
                            {
                                s_DictName.Remove(go);
                                go.gameObject.SetActive(false);
                                go.transform.SetParent(Alurax.Inst.transform);
                                s_DictPool[assetName].stack.Push(go);
                                if (s_ReturnToPool.TryGetValue(assetName, out var returnToPool))
                                    returnToPool?.Invoke(go);
                                return;
                            }
                        }
                    }
                }
                Destroy(go);
            }
        }

        public static void Destroy(GameObject go)
        {
            if (go)
            {
                s_DictName.Remove(go);
                Object.Destroy(go);
            }
        }

        public static void DestroyPool(string assetName, bool withInstanced = false)
        {
            if(s_DictPool.TryGetValue(assetName, out var data))
            {
                foreach (var go in data.stack)
                {
                    if (go) Object.Destroy(go);
                }
                s_DictPool.Remove(assetName);
            }
            
            if (withInstanced)
            {
                foreach (var item in s_DictName)
                {
                    var go = item.Key;
                    if (go && item.Value == assetName) 
                        Object.Destroy(go);
                }
            }
        }
        
        public static void DestroyPool(bool withInstanced = false)
        {
            foreach (var item in s_DictPool)
            {
                var data = item.Value;
                foreach (var go in data.stack) 
                    if (go) Object.Destroy(go);
            }   
            s_DictPool.Clear();
            
            if (withInstanced)
            {
                foreach (var item in s_DictName)
                {
                    var go = item.Key;
                    if (go) Object.Destroy(go);
                }
            }
            s_DictName.Clear();
        }

        public static int GetPoolCount(string assetName)
        {
            if(s_DictPool.TryGetValue(assetName, out var data))
            {
                return data.stack.Count;
            }
            return 0;
        }

        public static void RegReturnPool(string assetName, Action<GameObject> callbak)
        {
            if(!s_ReturnToPool.ContainsKey(assetName))
            {
                s_ReturnToPool[assetName] = callbak;
            }else
            {
                s_ReturnToPool[assetName] -= callbak;
                s_ReturnToPool[assetName] += callbak;
            }
        }

        public static void UnRegReturnPool(string assetName)
        {
            s_ReturnToPool.Remove(assetName);
        }
        
        public static void RegTakePool(string assetName, Action<GameObject> callbak)
        {
            if(!s_TakeFromPool.ContainsKey(assetName))
            {
                s_TakeFromPool[assetName] = callbak;
            }else
            {
                s_TakeFromPool[assetName] -= callbak;
                s_TakeFromPool[assetName] += callbak;
            }
        }

        public static void UnRegTakePool(string assetName)
        {
            s_TakeFromPool.Remove(assetName);
        }
    }
}
