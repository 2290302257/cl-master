using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class StateFsm 
    {
        public StateBase DefaultState { get; protected set; }
        public StateBase CurrentState { get; protected set; }
        protected Dictionary<Type, StateBase> m_states = new Dictionary<Type, StateBase>();

        private bool _isExiting;
        
        public void Execute()
        {
            CurrentState?.Execute();
        }
        
        public StateBase AddState<T>(params object[] args) where T : StateBase
        {
            var stateType = typeof(T);
            if (m_states.ContainsKey(stateType))
            {
                Log.E($"state {stateType} is already added.");
                return null;
            }
            
            var newState = (StateBase)Activator.CreateInstance(typeof(T), args);
            m_states.Add(stateType, newState);

            return newState;
        }

        public StateBase GetState<T>() where T : StateBase
        {
            return GetState(typeof(T));
        }

        public StateBase GetState(Type type)
        {
            if (m_states.TryGetValue(type, out var state))
            {
                return state;
            }
            
            Log.E($"state {type} is not found");
            return null;         
        }

        public bool ChangeState(Type type)
        {
            var newState = GetState(type);
            if (newState == null || newState == CurrentState)
                return false;

            if (_isExiting)
            {
                Log.E($"state {CurrentState.GetType().Name} is exiting. but another coming: {type.Name}");
                return false;
            }
            
            // exit
            if (CurrentState != null)
            {
                _isExiting = true;
                
                CurrentState.Exit(newState);
                
                _isExiting = false;
            }
            
            // enter
            CurrentState = newState;
            CurrentState.Enter();

            return true;
        }
        
        public bool ChangeState<T>() where T : StateBase 
        {
            return ChangeState(typeof(T));
        }
        
        public void SetDefaultState<T>() where T : StateBase
        {
            DefaultState = GetState<T>();
        }
    }
}
