using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace Alurax
{
    public class UIManager
    {
        private class Entity
        {
            public enum Phase
            {
                OPENING,
                OPENED,
                CLOSING,
                CLOSED
            }
            public string name;
            public UIView view;
            public bool created;
            public bool focused;
            public float closeTime = 0f;
            public Phase phase = Phase.CLOSED;
        }

        private class RegTask
        {
            public Action<string,Action> onBack;
            public string openName;
        }
        
        public class EntityTask
        {
            public string name;
            public enum Type
            {
                NORMAL,
                BACK_OPEN,
                BOTH_OPEN_AND_CLOSE,
            }
            public Type type = Type.NORMAL;
        }
        private static Dictionary<string, Type> s_Types = new Dictionary<string, Type>();
        private static List<Entity> s_Entities = new List<Entity>();
        private static List<EntityTask> s_EntitieTasks = new List<EntityTask>();
        private static Dictionary<string, RegTask> s_RegTasks = new Dictionary<string, RegTask>();
        private static Action<string,bool> OnVisibleChange;
        static UIManager()
        {
            MakeUIHandler();
            TaskManager.Update(Update);
        }

        static void Update()
        {
            for (int i = 0; i < s_Entities.Count; i++)
            {
                var entity = s_Entities[i];
                if (entity != null)
                {
                    bool needDestroy = (entity.phase == Entity.Phase.CLOSED) &&
                        (entity.closeTime != 0f && Time.unscaledTime - entity.closeTime >= Alurax.Inst.UIDestroyTime);
                    if (needDestroy)
                    {
                        DestroyEntity(entity);
                        i--;
                    }
                    else if (entity.phase != Entity.Phase.CLOSED)
                    {
                        UIView.InternalUpdate(entity.view);
                    }
                }
            }
        }

        private static Entity CreateEntity(Type type)
        {
            UIView view;
            Entity entity;
            if (!TryGet(type, out entity))
            {
                view = (UIView)Activator.CreateInstance(type);
                entity = new Entity() { name = type.Name,view = view, closeTime=0f, phase = Entity.Phase.CLOSED };
                s_Entities.Add(entity);
            }
           
            return entity;
        }
        
        private static void DestroyEntity(Entity entity)
        {
            var view = entity.view;
            s_Entities.Remove(entity);
            if (view.gameObject != null) {
                GameObject.Destroy(entity.view.gameObject);
                UIView.InternalDestroyed(view);
            }
        }

        public static void Open(string name)
        {
            InternalOpen(name, EntityTask.Type.NORMAL);
        }

        public static void OpenOnlyBack(string name)
        {
            InternalOpen(name, EntityTask.Type.BACK_OPEN);
        }

        public static void OpenAndPush(string name)
        {
            InternalOpen(name, EntityTask.Type.BOTH_OPEN_AND_CLOSE);
        }

        static void InternalOpen(string name, EntityTask.Type taskType)
        {
            Type type = s_Types[name];
            Entity entity = CreateEntity(type);
            if (entity.phase == Entity.Phase.CLOSED || entity.phase == Entity.Phase.CLOSING)
            {
                entity.phase = Entity.Phase.OPENING;
                entity.closeTime = 0f;
                UIView view = entity.view;
                LoadPrefab($"{type.Name}.prefab", view.gameObject,(prefab) =>
                {
                    if (entity.phase == Entity.Phase.OPENING)
                    {
                        entity.phase = Entity.Phase.OPENED;
                        s_EntitieTasks.Add(new EntityTask() { name = name, type = taskType });
                        if (taskType == EntityTask.Type.BOTH_OPEN_AND_CLOSE)
                        {
                            for (int i = s_EntitieTasks.Count-2; i >=0; i--)
                            {
                                var entityTask = s_EntitieTasks[i];
                                bool triggerClose = (entityTask.type == EntityTask.Type.BACK_OPEN || entityTask.type == EntityTask.Type.BOTH_OPEN_AND_CLOSE);
                                if (triggerClose)
                                {
                                    Entity closeEntity;
                                    if (TryGet(s_Types[entityTask.name], out closeEntity))
                                    {
                                        InternalOnlyClose(closeEntity, false);
                                    }
                                    break;
                                }
                            }
                        }
                        InternalOnlyOpen(entity, prefab);
                    }else
                    {
                        GameObject.Destroy(prefab);
                    }
                });
            }
        }

        static void InternalOnlyOpen(Entity entity,GameObject prefab)
        {
            var view = entity.view;
            if (prefab != null)
            {
                if (!entity.created)
                {
                    entity.created = true;
                    UIView.InternalCreated(view, prefab);
                }
                view.transform.SetParent(Alurax.Inst.UIRoot.transform, false);
                if (view.transform.TryGetComponent<Canvas>(out var canvas))
                { 
                    canvas.renderMode = RenderMode.ScreenSpaceCamera; 
                    canvas.worldCamera = Alurax.Inst.UICamera;
                    UIView.InternalOverrideCanvas(view,canvas);
                }
            }
            SetViewVisible(view, true);
            UIView.InternalOpened(view);
            OnVisibleChange?.Invoke(entity.name,true);
            SetViewAnim(view);
            if (!entity.focused)
            {
                entity.focused = true;
                UIView.InternalFocusChanged(view, true);
            }
        }

        public static void Close(string name)
        {
            InternalClose(name, false);
        }

        private static void InternalClose(string name, bool destroy)
        {
            Entity entity;
            if (TryGet(s_Types[name], out entity))
            {
                if (entity.phase == Entity.Phase.OPENED || entity.phase == Entity.Phase.OPENING)
                {
                    entity.phase = Entity.Phase.CLOSING;
                    CloseTriggerOpen(name, (openEntity,openPrefab) =>
                    {
                        bool hasOpen = (openEntity != null && openPrefab != null);
                        InternalOnlyClose(entity, destroy);
                        if (hasOpen)
                        {
                            RegTask regTask = null;
                            bool hasReg = (s_RegTasks.TryGetValue(openEntity.name, out regTask));
                            if (hasReg && regTask.onBack != null)
                            {
                                UILock.Lock();
                                regTask.openName = openEntity.name;
                                regTask.onBack(name,() =>
                                {
                                    UILock.UnLock();
                                    openEntity.phase = Entity.Phase.OPENED;
                                    hasOpen = (openEntity != null && openPrefab != null);
                                    if(hasOpen)
                                        InternalOnlyOpen(openEntity, openPrefab);
                                    else
                                    {
                                        Type type = s_Types[regTask.openName];
                                        Entity entity = CreateEntity(type);
                                        LoadPrefab($"{type.Name}.prefab", entity.view.gameObject, (go) =>
                                        {
                                            InternalOnlyOpen(entity, go);
                                        }); 
                                    }
                                });
                            }
                            else
                            {
                                openEntity.phase = Entity.Phase.OPENED;
                                InternalOnlyOpen(openEntity, openPrefab);
                            }
                        }
                    });
                }
            }
        }

        private static void InternalOnlyClose(Entity entity, bool destroy)
        {
            var view = entity.view;

            if (view.gameObject != null)
            {
                if (entity.focused)
                {
                    entity.focused = false;
                    UIView.InternalFocusChanged(view, false);
                }
                UIView.InternalClosed(view);
                OnVisibleChange?.Invoke(entity.name,false);
            }

            entity.phase = Entity.Phase.CLOSED;
            if (destroy)
            {
                DestroyEntity(entity);
            }
            else if (view.gameObject != null)
            {
                entity.closeTime = Time.unscaledTime;
                SetViewVisible(view, false);
            }
        }

        static void CloseTriggerOpen(string closeName,Action<Entity,GameObject> trigger)
        {
            bool closeTriggerOpen = false;
            for (int i = s_EntitieTasks.Count-1; i >= 0; i--)
            {
                var entityTask = s_EntitieTasks[i];
                if (closeTriggerOpen)
                {
                    bool triggerOpen = (entityTask.type == EntityTask.Type.BACK_OPEN || entityTask.type == EntityTask.Type.BOTH_OPEN_AND_CLOSE);
                    if (triggerOpen)
                    {
                        Type type = s_Types[entityTask.name];
                        Entity entity = CreateEntity(type);
                        LoadPrefab($"{type.Name}.prefab", entity.view.gameObject, (go) =>
                        {
                            trigger(entity, go);
                        });
                        return;
                    }
                }
                if(entityTask.name == closeName)
                {
                    if ((entityTask.type == EntityTask.Type.BOTH_OPEN_AND_CLOSE))
                    {
                        closeTriggerOpen = true;
                    }
                    s_EntitieTasks.RemoveAt(i);
                }
            }
            trigger?.Invoke(null,null);
        }

        [System.Obsolete("Use Close instead")]
        public static void CloseAndDestroy(string name)
        {
            InternalClose(name, true);
        }

        public static void CloseAndDestroyAll(params string[] excludes)
        {
            HashSet<string> hash = new HashSet<string>(excludes);
            for (int i = s_EntitieTasks.Count - 1; i >= 0; i--)
            {
                var entityTask = s_EntitieTasks[i];
                if (entityTask !=null && !hash.Contains(entityTask.name))
                {
                    Entity entity;
                    if (TryGet(s_Types[entityTask.name], out entity))
                    {
                        InternalOnlyClose(entity, true);
                    }
                    if(i < s_EntitieTasks.Count)
                        s_EntitieTasks.RemoveAt(i);
                }
            }

            for (int i = s_Entities.Count - 1; i >= 0; i--)
            {
                var entity = s_Entities[i];
                if (entity != null)
                {
                    bool needDestroy = entity.phase == Entity.Phase.CLOSED || 
                                       (!hash.Contains(entity.name) && entity.phase == Entity.Phase.OPENING);
                    if (needDestroy)
                    {
                        InternalOnlyClose(entity, true);
                    }
                }
            }
        }

        public static bool Exists(string name)
        {
            if (TryGet(s_Types[name], out var entity))
            {
                return entity.phase == Entity.Phase.OPENING || entity.phase == Entity.Phase.OPENED;
            }
            return false;
        }
        
        public static List<EntityTask> GetTaskStack()
        {
            return s_EntitieTasks;
        }
        public static List<EntityTask> GetTaskStackCopy()
        {
            var copy = new List<EntityTask>(s_EntitieTasks.Count);
            foreach (EntityTask entityTask in s_EntitieTasks)
            {
                var newTask = new UIManager.EntityTask
                {
                    name = entityTask.name,
                    type = entityTask.type
                };
                copy.Add(newTask);
            }
            return copy;
        }
        public static void RegCloseBack(string name,Action<string,Action> onBack)
        {
            if (!s_RegTasks.TryGetValue(name, out var task))
            {
                task = new RegTask();
                s_RegTasks[name] = task;
            }
            task.onBack = onBack;
        }
        public static void UnRegCloseBack(string name)
        {
            s_RegTasks.Remove(name);
        }

        public static void RegVisibleChange(Action<string,bool> onChange)
        {
            OnVisibleChange-= onChange;
            OnVisibleChange+= onChange;
        }
        
        public static void UnRegVisibleChange(Action<string,bool> onChange)
        {
            OnVisibleChange -=onChange;
        }
        
        public static void GetTaskDebug(System.Text.StringBuilder sb)
        {
            sb.Clear();
            for (int i = 0; i < s_EntitieTasks.Count; i++)
            {
                if (i != 0)
                {
                    sb.Append(",");
                }
                var task = s_EntitieTasks[i];
                sb.Append(task.type == EntityTask.Type.NORMAL ? $"<color=white>{task.name}</color>" : $"<color=red>{task.name}</color>");
            }
        }

        private static bool TryGet(Type type, out Entity entity)
        {
            for (int i = s_Entities.Count - 1; i >= 0; i--) {
                entity = s_Entities[i];
                if (entity.view.GetType() == type) {
                       return true;
                }
            }
            entity = null;
            return false;
        }
        private static void SetViewVisible(UIView view, bool visible)
        {
            view.gameObject.SetActive(visible);
        }
        
        private static void SetViewAnim(UIView view)
        {
            var anims = view.transform.GetComponentsInChildren<UIViewAnim>();
            int count = anims.Length;
            foreach (var anim in anims)
            {
               anim.Play(() =>
               {
                   if (--count == 0 && view != null && view.gameObject && view.gameObject.activeSelf)
                   {
                       UIView.InternalAnimOpened(view);
                   }
               });
            }
        }
        
        private static void MakeUIHandler()
        {
            foreach (Assembly a in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type t in a.GetTypes())
                {
                    if(t.IsSubclassOf(typeof(UIView)))
                    {
                        System.Attribute[] attrs = System.Attribute.GetCustomAttributes(t);
                        foreach (UIHandlerAttribute author in attrs)
                        {
#if UNITY_EDITOR
                            var interfaces = t.GetInterfaces();
                            if (interfaces.Length > 0)
                            {
                                Log.E($"Do not use interface in UIView: {t.Name}");
                                continue;
                            }
#endif
                            s_Types[author.GetName()] = t;
                        }
                    }
                }
            }
        }

        static void LoadPrefab(string assetName,GameObject check, Action<GameObject> finish)
        {
            if (check == null)
            {
                UILock.Lock();
                Assets.InstantiateAsync<GameObject>(assetName,(go) => {
                    UILock.UnLock();
                    finish?.Invoke(go);
                });
            }
            else
            {
                finish?.Invoke(check);
            }
        }
    }
}
