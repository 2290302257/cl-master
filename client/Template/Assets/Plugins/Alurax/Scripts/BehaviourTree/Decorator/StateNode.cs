using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class StateNode : ConditionNode
    {
        protected override bool finishWithChild { get; set; } = false;
        
        protected override void OnStart()
        {
            base.OnStart();
            
            m_Blackboard.OnVariableChanged -= OnBlackboardChanged;
            m_Blackboard.OnVariableChanged += OnBlackboardChanged;
            
            m_Blackboard.OnVariableAdded -= OnBlackboardChanged;
            m_Blackboard.OnVariableAdded += OnBlackboardChanged;
            
            m_Blackboard.OnVariableRemoved -= OnBlackboardChanged;
            m_Blackboard.OnVariableRemoved += OnBlackboardChanged;
        }

        protected override void OnStop()
        {
            base.OnStop();
            
            m_Blackboard.OnVariableChanged -= OnBlackboardChanged;
            m_Blackboard.OnVariableAdded -= OnBlackboardChanged;
            m_Blackboard.OnVariableRemoved -= OnBlackboardChanged;
        }

        private void OnBlackboardChanged(BlackboardVariableBase variable)
        {
            if (CurState == State.Running || CurState == State.Success)
            {
                if (!CheckConditions())
                {
                    Parent.Restart();
                }
            }
        }
        
    }
    
    
}
