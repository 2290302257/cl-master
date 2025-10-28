using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public abstract class EntityBase : MonoBehaviour
    {
        protected EntityDataBoard DataBoard;
        private List<EntityComponent> _components;
        
        protected virtual void Awake()
        {
            Reset();
        }

        protected virtual void Reset()
        {
            _components = new List<EntityComponent>();
            DataBoard = new EntityDataBoard(this);
            enabled = false;
        }
        
        public void Enable()
        {
            if (enabled)
                return;
            
            enabled = true;
            
            foreach (var i in _components)
            {
                EntityComponent.InternalOnEnable(i);
            }
        }

        public void Disable()
        {
            if (!enabled)
                return;

            enabled = false;
            
            foreach (var i in _components)
            {
                EntityComponent.InternalOnDisable(i);
            }
            DataBoard.Reset();
        }

        public void Destroy()
        {
            if (this)
            {
                Disable();

                foreach (var i in _components)
                {
                    EntityComponent.InternalOnDestroy(i);
                }

                DataBoard.Destroy();
                Reset();
            }
        }

        public List<EntityComponent> GetEntityComponents()
        {
            return _components;
        }
        
        public T AddEntityComponent<T>(params object[] parameters) where T : EntityComponent
        {
            EntityComponent target = AddEntityComponent(typeof(T), parameters);
            return target as T;
        }

        public T GetEntityComponent<T>() where T: EntityComponent
        {
            return (T)GetEntityComponent(typeof(T));
        }
        
        public void RemoveEntityComponent<T>() where T : EntityComponent
        {
            RemoveEntityComponent(typeof(T));
        }

        public EntityComponent AddEntityComponent(Type type, params object[] parameters)
        {
            if (type == null)
                return null;
            
            EntityComponent target = GetEntityComponent(type);
            if (target == null)
            {
                target = (EntityComponent)Activator.CreateInstance(type, parameters);
                _components.Add(target);
                EntityComponent.InternalAwake(target, DataBoard);
                EntityComponent.InternalStart(target);
                
                if (enabled)
                {
                    EntityComponent.InternalOnEnable(target);
                }
            }

            return target;
        }
        
        public EntityComponent GetEntityComponent(Type type)
        {
            if (_components == null || _components.Count == 0)
            {
                return null;
            }
            
            foreach (var component in _components)
            {
                var compType = component.GetType();
                if (compType == type || compType.IsSubclassOf(type))
                {
                    return component;
                }
            }
            
            return null;
        }
        
        public void RemoveEntityComponent(Type type)
        {
            if (type == null)
                return;
            
            EntityComponent target = GetEntityComponent(type);
            if (target != null)
            {
                EntityComponent.InternalOnDisable(target);
                EntityComponent.InternalOnDestroy(target);
                
                _components.Remove(target);
            }
        }
        
        public T Get<T>() where T : IEntityData
        {
            return DataBoard.Get<T>();
        }
        
        
        public T TryGet<T>() where T : IEntityData
        {
            if (DataBoard == null) return default;
            return DataBoard.TryGet<T>();
        }
        
        private void FixedUpdate()
        {
            float deltaTime = Time.fixedDeltaTime;
            foreach (var component in _components)
            {
                EntityComponent.InternalFixedUpdate(component, deltaTime);
            }
        }

        protected virtual void Update()
        {
            float deltaTime = Time.deltaTime;
            foreach (var component in _components)
            {
                EntityComponent.InternalUpdate(component, deltaTime);
            }
        }

        private void LateUpdate()
        {
            float deltaTime = Time.deltaTime;
            foreach (var component in _components)
            {
                EntityComponent.InternalLateUpdate(component, deltaTime);
            }
        }
    }
}