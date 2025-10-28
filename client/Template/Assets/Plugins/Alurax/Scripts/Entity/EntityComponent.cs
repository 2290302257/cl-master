
using UnityEngine;

namespace Alurax
{
    public abstract class EntityComponent
    {
        public EntityBase Entity => _dataBoard.Entity;
        
        private EntityDataBoard _dataBoard;
        private bool _activeSelf;
        protected bool activeSelf => _activeSelf;
        
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
        
        protected virtual void Awake() { }
        protected virtual void Start() { }
        protected virtual void OnDestroy() { }
        
        protected virtual void FixedUpdate(float deltaTime) { }
        protected virtual void Update(float deltaTime) { }
        protected virtual void LateUpdate(float deltaTime) {}
        
        protected virtual void OnEnable() { }
        protected virtual void OnDisable() { }

        protected T Get<T>() where T : IEntityData
        {
            return _dataBoard.Get<T>();
        }

        protected T GetEntity<T>() where T: EntityBase 
        {
            return _dataBoard.Entity as T;
        }
        
        protected T GetEntityComponent<T>() where T: EntityComponent
        {
            return (T)this.Entity.GetEntityComponent(typeof(T));
        }
        
        /*******************************************************************\
        | Internal Methods (Don't call these methods)                       |
        \*******************************************************************/
        static internal void InternalAwake(EntityComponent comp, EntityDataBoard dataBoard)
        {
            comp._dataBoard = dataBoard;
            comp._activeSelf = false;
            comp.Awake();
        }

        static internal void InternalStart(EntityComponent comp)
        {
            comp.Start();   
        }
        
        static internal void InternalOnEnable(EntityComponent comp)
        {
            if (comp.activeSelf)
                return;
            
            comp._activeSelf = true;                
            comp.OnEnable();
        }
        static internal void InternalFixedUpdate(EntityComponent comp,float deltaTime)
        {
            if (comp.activeSelf)
            {
                comp.FixedUpdate(deltaTime);   
            }
        }
        static internal void InternalUpdate(EntityComponent comp,float deltaTime)
        {
            if (comp.activeSelf)
            {
                comp.Update(deltaTime);   
            }
        }
        static internal void InternalLateUpdate(EntityComponent comp,float deltaTime)
        {
            if (comp.activeSelf)
            {
                comp.LateUpdate(deltaTime);   
            }
        }
        static internal void InternalOnDisable(EntityComponent comp)
        {
            comp.__OnAction?.Clear();
            comp.__OnTimer?.Clear();   
            
            if (!comp._activeSelf)
                return;

            comp._activeSelf = false;
            comp.OnDisable();
        }
        static internal void InternalOnDestroy(EntityComponent comp)
        {
            comp.OnDestroy();
        }
    }
    
}