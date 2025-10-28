using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Alurax
{
    public class WaitSecondsNode : ActionNode
    {
        [SerializeReference]
        protected BlackboardVariableFloat WaitTime = new BlackboardVariableFloat(null);
        
        [SerializeReference] 
        protected BlackboardVariableFloat Variance = new BlackboardVariableFloat(null);
        
        protected override void OnStart()
        {
            float time = Math.Max(0, WaitTime.Value + Random.Range(-Variance.Value, Variance.Value));
            m_OnTimer.Timeout(OnTimeout, time);
        }

        private void OnTimeout()
        {
            Finish(true);
        }
    }
}
