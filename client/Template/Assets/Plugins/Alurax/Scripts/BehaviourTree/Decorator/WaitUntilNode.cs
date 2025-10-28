using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class WaitUntilNode : DecoratorNode
    {
        [SerializeReference]
        protected List<BlackboardVariableCondition> Conditions = new List<BlackboardVariableCondition>();

        private int _timerId = -1;
        
        protected override void OnStart()
        {
            if (CheckConditions())
            {
                base.OnStart();
            }
            else
            {
                _timerId = m_OnTimer.Update(WaitUntil);
            }
        }

        protected override void OnStop()
        {
            _timerId = -1;
            base.OnStop();
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
        
        private void WaitUntil()
        {
            if (CheckConditions())
            {
                m_OnTimer.Remove(_timerId);
                base.OnStart();
            }
        }
    }

}
