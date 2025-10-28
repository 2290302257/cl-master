#if UNITY_EDITOR
using UnityEditor;
#endif
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;
using System.Linq;
using Object = UnityEngine.Object;

namespace Alurax
{
    public class Assets
    {
        static Dictionary<string, (string,string[])> s_RuntimeDatabase = new Dictionary<string, (string,string[])>();
        static Dictionary<string, Asset> s_CacheAssetObject = new Dictionary<string, Asset>();
        static List<CacheObject> s_CacheInstObject = new List<CacheObject>();
        static Dictionary<string, AssetBundle> s_AssetBundleDB = new Dictionary<string, AssetBundle>();
        static Dictionary<string, string> s_ReplaceName = new Dictionary<string, string>();
        static Dictionary<string, AssetBundleCreateRequest> s_CacheLoadRequest = new Dictionary<string, AssetBundleCreateRequest>();
        static HashSet<string> s_CacheLoadingAssetFromAB = new HashSet<string>();
        
        private class Asset
        {
            public System.WeakReference WeakObject;
            public string ABName;
            public string[] DependABName;
            public int RefCount;
        }

        private class CacheObject
        {
            public Object AssetRef;
            public Asset Asset;
            public System.WeakReference WeakObject;
            public int RefCount;
        }

        public static bool UseAB
        {
            get
            {
#if UNITY_EDITOR
                return Menu.GetChecked(USE_EDTIOR_AB);
#else
                return true;
#endif
            }
        }

        public static bool UseEdtiorAsset
        {
            get
            {
#if UNITY_EDITOR
                return (UseAB == false);
#else
                return false;
#endif
            }
        }
        static Assets()
        {
            if (UseEdtiorAsset)
            {
#if UNITY_EDITOR
                LoadEditorDatabaseMap();
#endif
            }
            else if (UseAB)
            {
#if UNITY_EDITOR
                if(Application.isPlaying)
                    EditorUtility.DisplayDialog("","Edtior下正在使用AB模式","ok");
#endif
                LoadRuntimeDatabaseMap();
            }
        }
        
        public static bool HasAsset(string assetName)
        {
            if (UseEdtiorAsset)
            {
#if UNITY_EDITOR
                return s_EditorDatabase.ContainsKey(assetName);
#endif
            }
            else if (UseAB)
            {
                return s_RuntimeDatabase.ContainsKey(assetName);
            }
            return false;
        }

        public static string GetFilePath(string rawPath)
        {
            var filename= Path.GetFileNameWithoutExtension(rawPath);
            var ext =  Path.GetExtension(rawPath);
            if (s_ReplaceName.TryGetValue(filename, out var replace))
            {
                return $"{Download.DOWNLOAD_PATH}/{replace}{ext}";
            }

            return rawPath;
        }
        
        public static void LoadAssetAsync<T>(string assetName,System.Action<T> callback) where T : UnityEngine.Object
        {
            if(typeof(T) == typeof(GameObject))
            {
                Debug.LogError("can not use LoadAssetAsync load gameobject, must use InstantiateAsync replace it!");
                return;
            }
            InternalLoadAssetAsync(assetName, callback);
        }
        
        public static void LoadSceneAsync(string assetName,System.Action<string> callback)
        {
            InternalLoadSceneAssetAsync(assetName, callback);
        }

        public static void InstantiateAsync<T>(string assetName, System.Action<T> callback) where T : UnityEngine.Object
        {
            
            InternalLoadAssetAsync<Object>(assetName, (prefab) =>
            {
                var go = Object.Instantiate(prefab);
                if (UseAB)
                {
                    CacheObject asset = new CacheObject()
                    {
                        AssetRef = prefab,
                        Asset = s_CacheAssetObject[assetName],
                        WeakObject = new System.WeakReference(go),
                        RefCount = 1
                    };
                    s_CacheInstObject.Add(asset); //cache reference
                }
                callback?.Invoke((T)go);
            });
        }

        public static void ReleaseAsset(string assetName)
        {
            if (UseAB)
            {
                if (!string.IsNullOrEmpty(assetName) && s_CacheAssetObject.TryGetValue(assetName, out var asset))
                {
                    asset.RefCount = Mathf.Max(0, asset.RefCount - 1);  
                }
            }
        }
        
        public static void ReleaseScene(string assetName)
        {
            if (UseAB)
            {
                ReleaseAsset(assetName);
            }
        }
        
        public static void GC(Action callback)
        {
            Alurax.Inst.StartCoroutine(EnumeratorGC(callback));
        }

        private static IEnumerator EnumeratorGC(Action callback)
        {
            for (int i = 0; i < 3; i++)
            {
                System.GC.Collect();
                yield return null;
                AssetGC();
                yield return null;
                Resources.UnloadUnusedAssets();
                yield return null;
            }
            callback?.Invoke();
        }
        
        private static void AssetGC()
        {
            if (UseAB)
            {
                Resources.UnloadUnusedAssets();
                HashSet<string> alives = new HashSet<string>();

                for (int i = s_CacheInstObject.Count - 1; i >= 0; i--)
                {
                    var item = s_CacheInstObject[i];
                    if (!item.WeakObject.IsAlive || item.RefCount == 0)
                    {
                        // break ref
                        item.AssetRef = null;
                        item.Asset.RefCount = Mathf.Max(0, item.Asset.RefCount - 1);
                        s_CacheInstObject.RemoveAt(i);
                    }
                }

                for (int i = s_CacheAssetObject.Count - 1; i >= 0; i--)
                {
                    var item = s_CacheAssetObject.ElementAt(i);
                    var asset = item.Value;
                    if (asset.WeakObject!=null && asset.WeakObject.IsAlive && asset.RefCount > 0)
                    {
                        alives.Add(asset.ABName);
                        foreach (var depend in asset.DependABName)
                        {
                            alives.Add(depend);
                        }
                    }
                    else
                        s_CacheAssetObject.Remove(item.Key);
                }

                for (int i = s_AssetBundleDB.Count - 1; i >= 0; i--)
                {
                    var abName = s_AssetBundleDB.ElementAt(i).Key;
                    if (!s_CacheLoadingAssetFromAB.Contains(abName) &&
                        !s_CacheLoadRequest.TryGetValue(abName, out var __) &&
                        !alives.Contains(abName))
                    {
                        s_AssetBundleDB[abName].Unload(false);
                        s_AssetBundleDB.Remove(abName);
                    }
                }
                Resources.UnloadUnusedAssets();
                //Debug.Log(s_CacheAssetObject.Count +" " +s_AssetBundleDB.Count);
            }
        }
        
        
        private static void InternalLoadAssetAsync<T>(string assetName, System.Action<T> callback) where T : Object
        {

            if (UseEdtiorAsset)
            {
#if UNITY_EDITOR
                if (Application.isPlaying)
                {
                    Alurax.Inst.StartCoroutine(LoadEditorAsync<T>(assetName, callback));                
                }
                else
                {
                    LoadEditor<T>(assetName, callback);
                }
#endif
            }
            else if (UseAB)
            {
                Alurax.Inst.StartCoroutine(LoadRuntimeAsync<T>(assetName, callback));
            }

        }
        static private void InternalLoadSceneAssetAsync(string assetName, System.Action<string> callback)
        {
            if (UseEdtiorAsset)
            {
#if UNITY_EDITOR
                Alurax.Inst.StartCoroutine(LoadEditorSceneAssetAsync(assetName, callback));
#endif
            } else if (UseAB)
            {
                Alurax.Inst.StartCoroutine(LoadRuntimeSceneAssetAsync(assetName, callback));
            }
        }
        static void LoadRuntimeDatabaseMap()
        {
            var text = FileUtils.LoadStreamingText("bundlemap.txt");
            if (!string.IsNullOrEmpty(text))
            {
                BundleMap bundleMap = new BundleMap();
                bundleMap.Load(text);
                s_RuntimeDatabase = bundleMap.Map;
            }
        }

        internal static void UpdateBundleMap(string text)
        {
            BundleMap bundleMap = new BundleMap();
            bundleMap.Load(text);
            s_RuntimeDatabase = bundleMap.Map;
            foreach (var line in bundleMap.Lines)
                s_ReplaceName[line.Value.name] = line.Value.md5;
        }
        static private IEnumerator LoadRuntimeAsync<T>(string assetName, System.Action<T> callback) where T : Object
        {
            Asset asset;
            if (s_CacheAssetObject.TryGetValue(assetName, out asset))
            {
                if (!asset.WeakObject.IsAlive || !(T)asset.WeakObject.Target)
                    s_CacheAssetObject.Remove(assetName);
            }
            if (!s_CacheAssetObject.TryGetValue(assetName, out asset))
            {
                var data = s_RuntimeDatabase[assetName];
                var abName = data.Item1;
                var depend = data.Item2;
                yield return Alurax.Inst.StartCoroutine(InternalLoadAssetbundleAsync(abName, depend));
                s_CacheLoadingAssetFromAB.Add(abName);
                var assetRequest = s_AssetBundleDB[abName].LoadAssetAsync<T>(assetName);
                yield return assetRequest;
                s_CacheLoadingAssetFromAB.Remove(abName);
                if(assetRequest.asset==null)
                    Log.E($"abName: {abName} assetName:{assetName} type:{typeof(T)} is not found");
                if (!s_CacheAssetObject.TryGetValue(assetName, out asset))
                {
                    asset = new Asset()
                    {
                        ABName = abName,
                        DependABName = depend,
                        WeakObject = new System.WeakReference(assetRequest.asset as T)
                    };
                    s_CacheAssetObject[assetName] = asset;
                }
            }
            asset.RefCount++;
            callback?.Invoke((T)asset.WeakObject.Target);
        }

        static private IEnumerator LoadRuntimeSceneAssetAsync(string assetName, System.Action<string> callback) 
        {
            Asset asset;
            if (!s_CacheAssetObject.TryGetValue(assetName, out asset))
            {
                var data = s_RuntimeDatabase[assetName];
                var abName = data.Item1;
                var depend = data.Item2;
                yield return Alurax.Inst.StartCoroutine(InternalLoadAssetbundleAsync(abName, depend));
                if (!s_CacheAssetObject.TryGetValue(assetName, out asset))
                {
                    asset = new Asset()
                    {
                        ABName = abName,
                        DependABName = depend
                    };
                    s_CacheAssetObject[assetName] = asset;
                }
            }
            callback?.Invoke(Path.GetFileNameWithoutExtension(assetName));
        }
        
        static IEnumerator InternalLoadAssetbundleAsync(string abName, string[] dependName)
        {
            foreach (var depend in dependName)
            {
                yield return Alurax.Inst.StartCoroutine(InternalLoadAssetbundleAsync(depend));
            }
            yield return Alurax.Inst.StartCoroutine(InternalLoadAssetbundleAsync(abName));
        }

        static private IEnumerator InternalLoadAssetbundleAsync(string abName)
        {
            string realName = null;
            string header = "";
            if (s_ReplaceName.TryGetValue(abName, out realName))
            {
                header = Download.DOWNLOAD_PATH;
            }else
            {
                realName = abName;
                header = Application.streamingAssetsPath;
            }
            if (!s_AssetBundleDB.TryGetValue(abName, out var __))
            {
                string loadPath = $"{header}/{realName}.blk";;
                if(!s_CacheLoadRequest.TryGetValue(abName,out var request))
                {
                    request= AssetBundle.LoadFromFileAsync(loadPath,0,1);
                    s_CacheLoadRequest[abName] = request;
                    yield return request;
                    s_CacheLoadRequest.Remove(abName);
                    if (!s_AssetBundleDB.TryGetValue(abName, out var ___))
                        s_AssetBundleDB[abName] = request.assetBundle;
                }
                else
                {
                    while (!request.isDone)
                        yield return null;
                }
            }
        }

#region Editor
#if UNITY_EDITOR
        private static Dictionary<string, string> s_EditorDatabase = new Dictionary<string, string>();
        private static Dictionary<string, string> s_EditorScenePath = new Dictionary<string, string>();
        static HashSet<string> s_EditorCacheAssetObject = new HashSet<string>();
        private static void LoadEditorDatabaseMap()
        {
            try
            {
                var guids = AssetDatabase.FindAssets("t:ABPack");
                s_EditorDatabase.Clear();
                s_EditorScenePath.Clear();
                for (int i = 0; i < guids.Length; i++)
                {
                    var guid = guids[i];
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    var pack = AssetDatabase.LoadAssetAtPath<ABPack>(path);
                    EditorUtility.DisplayProgressBar("提示", path, (float)i / (float)guids.Length);
                   
                    foreach (var file in pack.GetFiles(false))
                    {
                        var fileName = Path.GetFileName(file);
                        if (s_EditorDatabase.TryGetValue(fileName, out var _))
                            Debug.LogError($"assets have same name {fileName}");
                        s_EditorDatabase[fileName] = file;
                        if (Path.GetExtension(file) == ".unity")
                        {
                            if (s_EditorScenePath.TryGetValue(fileName, out var _))
                                Debug.LogError($"scenes have same name {fileName}");
                            s_EditorScenePath[fileName] = file;
                        }
                    }
                }
            }
            finally
            {
                EditorUtility.ClearProgressBar();
            }
        }

        private static void LoadEditor<T>(string name, System.Action<T> callback) where T : Object
        {
            string assetPath = null;
            try
            {
                assetPath = s_EditorDatabase[name];
                
            }
            catch
            {
                string objName = (callback.Target is MonoBehaviour comp) ? comp.gameObject.GetScenePath() : "";
                Debug.LogError($"assets {name} is not found. {objName}");
            }

            callback?.Invoke(AssetDatabase.LoadAssetAtPath<T>(assetPath));
        }
        
        private static IEnumerator LoadEditorAsync<T>(string name, System.Action<T> callback) where T : Object
        {
            string assetPath = null;
            try
            {
                assetPath = s_EditorDatabase[name];
            }
            catch
            {
                string objName = (callback.Target is MonoBehaviour comp) ? comp.gameObject.GetScenePath() : "";
                Debug.LogError($"assets not found {name} {objName}");
                yield break;
            }

            if (s_EditorCacheAssetObject.Add(assetPath))
            {
                int r = UnityEngine.Random.Range(1, 5);
                for (int i = 0; i < r; i++)
                    yield return null;
            }
            callback?.Invoke(AssetDatabase.LoadAssetAtPath<T>(assetPath));
        }
        
        private static IEnumerator LoadEditorSceneAssetAsync(string assetName, System.Action<string> callback)
        {
            int r = UnityEngine.Random.Range(1, 5);
            for (int i = 0; i < r; i++)
                yield return null;
            try
            {
                callback?.Invoke(s_EditorScenePath[assetName]);
            }
            catch
            {
                Debug.LogError($"assets {assetName} is not found.");
            }
        }
        
        const string USE_EDTIOR_AB = "Custom/Use Editor AB";
        private static bool isEnabled;
        [MenuItem(USE_EDTIOR_AB)]
        private static void PerformAction()
        {
            isEnabled = !isEnabled;
            Menu.SetChecked(USE_EDTIOR_AB, isEnabled);
        }
#endif
#endregion
    }
}
