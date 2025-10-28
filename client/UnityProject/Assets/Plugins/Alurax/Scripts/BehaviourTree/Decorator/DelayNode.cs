using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class DelayNode : DecoratorNode
    {
        [SerializeReference] 
        protected BlackboardVariableFloat WaitTime = new BlackboardVariableFloat(null);
        
        [SerializeReference] 
        protected BlackboardVariableFloat Variance = new BlackboardVariableFloat(null);

        protected override void OnStart()
        {
            float waitTime = WaitTime.Value + Random.Range(-Variance.Value, Variance.Value);
            m_OnTimer.Timeout(() =>
            {
                base.OnStart();
            }, waitTime);
        }
        
    }
}
