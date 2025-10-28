using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.GlobalIllumination;

namespace Alurax
{
    public class CooldownNode : DecoratorNode
    {
        [SerializeReference] 
        protected BlackboardVariableFloat CooldownTime = new BlackboardVariableFloat(null);
        
        [SerializeReference] 
        protected BlackboardVariableFloat Variance = new BlackboardVariableFloat(null);
        
        [SerializeReference] 
        protected BlackboardVariableBool CooldownAsFailure = new BlackboardVariableBool(null);
        
        private float _nextTime;
        
        protected override void OnStart()
        {
            float curTime = Time.time;
            if (curTime < _nextTime) // 未到冷却时间
            {
                if (CooldownAsFailure.Value)
                {
                    Finish(false);
                }
                else
                {
                    m_OnTimer.Timeout(CooldownStart, _nextTime - curTime);
                }
            }
            else // 已到冷却时间
            {
                CooldownStart();
            }
        }
        
        private void CooldownStart()
        {
            _nextTime = CooldownTime.Value + Random.Range(-Variance.Value, Variance.Value) + Time.time;
            base.OnStart();
        }
        
    }
}
