using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class SetVariableNode : ActionNode
    {
        [SerializeField]
        protected List<SetVariable> SetVariables = new List<SetVariable>();
        
        protected override void OnStart()
        {
            bool success = true;
            
            foreach (var setVariable in SetVariables)
            {
                success &= setVariable.Set(m_Blackboard);
            }
            
            Finish(success);
        }
    }

    [Serializable]
    public class SetVariable
    {
        public string VariableName;

        [SerializeReference] 
        public BlackboardVariableBase Variable;
        
        public void Bind(BlackboardVariableBase blackboardVariable)
        {
            if (blackboardVariable == null)
                return;
            
            VariableName = blackboardVariable.Name;
            Variable = blackboardVariable.Clone();
        }
        
        public bool Set(Blackboard blackboard)
        {
            var blackVariable = blackboard.Get(VariableName);
            if (blackVariable == null || Variable == null)
            {
                return false;
            }
            
            blackVariable.SetValue(Variable.GetValue());
            return true;
        }
    }

}
