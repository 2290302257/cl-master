using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

namespace Alurax
{
    [Serializable]
    public abstract class NodeBase
    {
        #region Editor
        [HideInInspector]
        public Vector2 Position;

        [HideInInspector]
        public string Description;
        #endregion
        
        [HideInInspector]
        [SerializeReference]
        protected Blackboard m_Blackboard;
     
        [HideInInspector]
        [SerializeReference]
        protected ContainerNode m_ParentNode;
        
        public enum State
        {
            None = 0,
            Running,
            Success,
            Failure
        }
        public State CurState { get; private set; } = State.None;
        public bool IsRunning => CurState == State.Running;
        public ContainerNode Parent => m_ParentNode;
        
        #region Funcions
        protected abstract void OnStart();
        protected abstract void OnStop();
        protected abstract void OnChildFinish(NodeBase child, bool success);    
        #endregion
        
        private TaskManager.Timer _onTimer;
        protected TaskManager.Timer m_OnTimer
        {
            get
            {
                if (_onTimer == null) _onTimer = new TaskManager.Timer();
                return _onTimer;
            }
        }
        
        private bool _isRestart = false;
        
        ~NodeBase()
        {
            if (m_Blackboard != null)
            {
                m_Blackboard.OnVariableRemoved -= OnBlackboardVariableRemoved;
            }
        }

        protected virtual void OnUpdate(float deltaTime) { }

        public void Tick(float deltaTime)
        {
            if (IsRunning)
            {
                OnUpdate(deltaTime);
            }
        }
        
        public void Start()
        {
            if (IsRunning)
                return;

            ResetState();
            CurState = State.Running;
            
            OnStart();
        }
        
        public void Stop()
        {
            if (!IsRunning)
                return;
            
            m_OnTimer.Clear();
            CurState = State.None;
            
            OnStop();
        }

        public void Restart()
        {
            if (_isRestart)
                return;

            m_OnTimer.NextFrame(() => 
            {
                _isRestart = false;
                
                Stop();
                Start();
            });
        }

        public virtual void ResetState()
        {
            CurState = State.None;
        }
        
        public void SetParent(ContainerNode parent)
        {
            m_ParentNode = parent;
        }

        protected void Finish(bool success)
        {
            Stop();
            
            CurState = success ? State.Success : State.Failure;

            if (m_ParentNode != null && m_ParentNode.CurState == State.Running)
            {
                m_ParentNode?.OnChildFinish(this, success);                
            }
        }

        public void SetBlackboard(Blackboard blackboard)
        {
            m_Blackboard = blackboard;
            
            m_Blackboard.OnVariableRemoved -= OnBlackboardVariableRemoved;
            m_Blackboard.OnVariableRemoved += OnBlackboardVariableRemoved;
        }
        
        public Blackboard GetBlackboard()
        {
            return m_Blackboard;
        }
        
        private void OnBlackboardVariableRemoved(BlackboardVariableBase variable)
        {
            var fields = GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            fields = fields.Where(f =>
                f.FieldType.IsSubclassOf(typeof(BlackboardVariableBase))).ToArray();
        
            foreach (var field in fields)
            {
                var nodeVariable = field.GetValue(this) as BlackboardVariableBase;
                if (nodeVariable == variable)
                {
                    field.SetValue(this, variable.Clone());
                }
            }
        }
    }
}
