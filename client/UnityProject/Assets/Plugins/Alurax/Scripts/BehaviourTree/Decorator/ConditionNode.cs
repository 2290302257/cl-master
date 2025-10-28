using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class ConditionNode : DecoratorNode
    {
        [SerializeField]
        protected List<BlackboardVariableCondition> Conditions = new List<BlackboardVariableCondition>();
        
        protected override void OnStart()
        {
            if (CheckConditions())
            {
                base.OnStart();
            }
            else
            {
                Finish(false);
            }
        }
        
        protected bool CheckConditions()
        {
            foreach (var condition in Conditions)
            {
                if (!condition.Value.IsMeet(m_Blackboard))
                    return false;
            }
            
            return true;
        }
    }
    
    [Serializable]
    public class Condition
    {
        public enum Operator : int
        {
            Set = 0,
            NotSet,
            Equal,
            NotEqual,
            Greater,
            GreaterEqual,
            Less,
            LessEqual,
        }
        
        public Operator OpCode;

        public string VariableName;
        
        [SerializeReference]
        public BlackboardVariableBase Variable;

        public void SetVariableName(string variableName)
        {
            VariableName = variableName;
            Variable = null;
        }
        
        public void SetVariable(BlackboardVariableBase variable)
        {
            if (variable != null)
            {
                VariableName = variable.Name;
                Variable = variable.Clone();
            }
            else
            {
                VariableName = "";
                Variable = null;
            }
        }
        
        public bool IsMeet(Blackboard blackboard)
        {
            var blackVariable = blackboard.Get(VariableName);
            if (OpCode == Operator.Set)
            {
                return blackVariable != null;
            }
            else if (OpCode == Operator.NotSet)
            {
                return blackVariable == null;
            }
            else if (blackVariable == null)
            {
                return false; // 其余情况需要黑板有值
            }
            
            switch (OpCode)
            {
                case Operator.Equal:
                    return blackVariable.CompareToOther(Variable) == 0;
                case Operator.NotEqual:
                    return blackVariable.CompareToOther(Variable) != 0;
                case Operator.GreaterEqual:
                    return blackVariable.CompareToOther(Variable) >= 0;
                case Operator.Greater:
                    return blackVariable.CompareToOther(Variable) > 0;
                case Operator.LessEqual:
                    return blackVariable.CompareToOther(Variable) <= 0;
                case Operator.Less:
                    return blackVariable.CompareToOther(Variable) < 0;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
    
    [Serializable]
    public class BlackboardVariableCondition : BlackboardVariable<Condition>
    {
        public BlackboardVariableCondition(string name) : base(name)
        {
        }
    }
    
    // 通用的数值比较
    public enum ValueComparer
    {
        Equal = 0,
        NotEqual,
        Greater,
        GreaterEqual,
        Less,
        LessEqual,
    }
    
    [Serializable]
    public class BlackboardVariableValueComparer : BlackboardVariable<ValueComparer>
    {
        public BlackboardVariableValueComparer(string name) : base(name) { }
    }
}
