using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public abstract class ModBase
    {
        virtual protected void OnCreated(){}
        virtual protected void OnStarted(){}
        virtual protected void OnUpdate(){}
        virtual protected void OnDestroyed(){}

        protected virtual bool EnableUpdate
        {
            get; set;
        }
        protected internal string HandleName;
        
        protected float LasetUpdateTime;
        protected virtual float UpdateInterval
        {
            get; set;
        } = 0.1f;

        private ActionManager __OnAction;
        protected ActionManager m_OnAction
        {
            get
            {
                if (__OnAction == null) __OnAction = new ActionManager();
                return __OnAction;
            }
        }
        private TaskManager.Timer __OnTimer;
        protected TaskManager.Timer m_OnTimer
        {
            get
            {
                if (__OnTimer == null) __OnTimer = new TaskManager.Timer();
                return __OnTimer;
            }
        }

        protected bool Destroyed { get; private set; }
        
        static internal void InternalOnCreated(ModBase mod)
        {
            mod.OnCreated();
        }
        
        static internal void InternalOnStarted(ModBase mod)
        {
            mod.OnStarted();
        }
        static internal void InternalOnUpdate(ModBase mod)
        {
            if (mod.EnableUpdate)
            {
                var time = Time.unscaledTime;
                if (time - mod.LasetUpdateTime >= mod.UpdateInterval)
                {
                    mod.OnUpdate();
                    mod.LasetUpdateTime = time;
                }
            }
        }
        static internal void InternalOnDestroyed(ModBase mod)
        {
            mod.__OnAction?.Clear();
            mod.__OnTimer?.Clear();
            mod.OnDestroyed();
            mod.Destroyed = true;
        }
    }
}