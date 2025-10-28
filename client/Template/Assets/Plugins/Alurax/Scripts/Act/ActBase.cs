using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    [System.Serializable]
    public class ActBase
    {
        public bool Started { get; protected set; }
        public bool Finished { get; protected set; }
        
        [HideInInspector]
        public string Name;
        
        [LabelText("开始时间")] [Clamp(0,float.MaxValue)]
        public float StartTime;
        
        [LabelText("持续时间")][Clamp(0,float.MaxValue)]
        public float Duration;

        protected GameObject actor { get; private set; }
        protected ActObject actObject { get; private set; }
        protected Blackboard blackboard => actObject?.Blackboard;
        
        protected GameObject GetObjFromPath(string path)
        {
            GameObject obj = null;
            
            if (actor != null)
            {
                obj = string.IsNullOrEmpty(path) ? actor : actor.transform.Find(path)?.gameObject;
                if (obj == null)
                {
                    obj = actor.RecursivelyFind(path)?.gameObject;
                }
            }

            return obj;
        }
        
        protected virtual void OnCreated() { }
        protected virtual void OnStart(float offsetTime) { }
        protected virtual void OnUpdate(float curTime) { }
        protected virtual void OnFinish() { }
        protected virtual void OnDestroyed() { }
        
        /*******************************************************************\
        | Internal Methods (Don't call these methods)                       |
        \*******************************************************************/
        internal static void InternalCreated(ActBase act, ActObject actObj, GameObject actor)
        {
            if (act != null)
            {
                act.actor = actor;
                act.actObject = actObj;
                
                InternalReset(act);
                act.OnCreated();
            }
        }
        
        internal static void InternalStart(ActBase act,float offsetTime)
        {
            if (act != null)
            {
                if (!act.Started)
                {
                    act.Started = true;
                    act.OnStart(offsetTime);
                }
            }
        }
        
        internal static void InternalUpdate(ActBase act, float curTime)
        {
            act?.OnUpdate(curTime);
        }
        
        internal static void InternalFinish(ActBase act)
        {
            if (act != null)
            {
                if (!act.Finished)
                {
                    act.Finished = true;
                    act.OnFinish();
                }
            }
        }
        
        internal static void InternalDestroyed(ActBase act)
        {
            if (act != null)
            {
                act.OnDestroyed();
                
                act.actor = null;
                act.actObject = null;
            }
        }
        
        internal static void InternalReset(ActBase act)
        {
            if (act != null)
            {
                act.Started = act.Finished = false;
            }
        }
    }
}
