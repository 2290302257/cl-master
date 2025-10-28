using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public static class GameObjectExtension
{
    public static string GetScenePath(this GameObject obj)
    {
        Transform transform = obj.transform;
            
        StringBuilder sb = new StringBuilder();
        sb.Append($"/{transform.name}");
            
        while (transform.parent)
        {
            transform = transform.parent;
            sb.Insert(0, $"/{transform.name}");
        }
            
        sb.Remove(0, 1);
        return sb.ToString();
    }

    public static void SetParent(this GameObject obj, GameObject parent, bool worldPositionStays = false)
    {
        obj.transform.SetParent(parent.transform, worldPositionStays);    
    }
    
    public static T GetAddComponent<T>(this GameObject go) where T : Component
    {
        return GetAddComponent(go, typeof(T)) as T;
    }

    public static T GetAddComponent<T>(this Component comp) where T : Component
    {
        return GetAddComponent(comp.gameObject, typeof(T)) as T;
    }

    public static Component GetAddComponent(this GameObject go, System.Type type)
    {
        var result = go.GetComponent(type);
        if (result == null)
        {
            result = go.AddComponent(type);
        }

        return result;
    }
    
    
    public static T GetComponentInParentWithoutSelf<T>(this Component comp) where T : Component
    {
        var parent = comp.transform.parent;
        while (parent != null)
        {
            if (parent.TryGetComponent<T>(out var result))
            {
                return result;
            }
            parent=parent.parent;
        }
        return null;
    }
    
    public static T GetComponentInChildrenWithoutSelf<T>(this Component comp,bool includeInactive=false) where T : Component
    {
        foreach (var child in comp.GetComponentsInChildren<T>(includeInactive))
        {
            if(child.gameObject!=comp.gameObject)
                return child;
        }
        return null;
    }
    public static List<T> GetComponentsInChildrenWithoutSelf<T>(this Component comp,bool includeInactive=false) where T : Component
    {
        List<T> list = new List<T>();
        foreach (var child in comp.GetComponentsInChildren<T>(includeInactive))
        {
            if(child.gameObject!=comp.gameObject)
                list.Add(child);
        }
        return list;
    }
    
    public static List<Transform> GetAllChild(this Transform obj, bool onlyActive = false)
    {
        List<Transform> ret = new List<Transform>();
        foreach (Transform child in obj)
        {
            if (!onlyActive || child.gameObject.activeSelf)
            {
                ret.Add(child);
            }
        }

        return ret;
    }
    
    #region Recursively
    public static Transform RecursivelyFind(this GameObject self, string name)
    {
        return self.transform.RecursivelyFind(name);
    }
    
    public static void RecursivelySetLayer(this GameObject self, string layerName)
    {
        LayerMask layer = LayerMask.NameToLayer(layerName);
        self.RecursivelySetLayer(layer);
    }

    public static void RecursivelySetLayer(this GameObject self, LayerMask layer)
    {
        self.layer = layer;
        Transform transform = self.transform;
        int len = transform.childCount;
        for (int i = 0; i < len; i++)
        {
            Transform childTransform = transform.GetChild(i);
            if (layer != childTransform.gameObject.layer)
            {
                childTransform.gameObject.RecursivelySetLayer(layer);
            }
        }
    }
    #endregion


    #region UI
    public static Vector2 GetUIPosition(this GameObject self)
    {
        RectTransform rectTransform = self.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            return rectTransform.anchoredPosition;
        }
        else
        {
            return self.transform.position;
        }
    }

    public static void SetUIPosition(this GameObject self, Vector2 position)
    {
        RectTransform rectTransform = self.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchoredPosition = position;
        }
        else
        {
            self.transform.position = position;
        }
    }

    public static void SetUISize(this GameObject self, int width = -1, int height = -1)
    {
        RectTransform rectTransform = self.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            Vector2 sizeDelta = rectTransform.sizeDelta;
            if (width != -1)
            {
                sizeDelta.x = width;
            }

            if (height != -1)
            {
                sizeDelta.y = height;
            }

            rectTransform.sizeDelta = sizeDelta;
        }
    }

    public static Vector2 GetUISize(this GameObject go)
    {
        RectTransform rectTransform = go.GetComponent<RectTransform>();

        if (rectTransform != null)
        {
            return rectTransform.sizeDelta;
        }

        return go.transform.localScale;
    }
    #endregion
    

    public static Texture2D ToTexture2D(this RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(rTex.width, rTex.height, TextureFormat.RGBA32, false);
        
        RenderTexture.active = rTex;
        {
            tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0, false);
            tex.Apply();
        }
        RenderTexture.active = null;
        
        return tex;
    }
    
    public static void ClearChildren( this Transform parent)
    {
        for (int i = 0; i < parent.childCount; i++)
        {
            GameObject.DestroyImmediate(parent.GetChild(i).gameObject);
        }
    }
}    

