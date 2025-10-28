using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class WaitStopNode : ActionNode
    {
        [SerializeReference]
        protected BlackboardVariableBool StopAsSuccess = new BlackboardVariableBool(null);

        protected override void OnStart()
        {
            
        }

        protected override void OnStop()
        {
            Finish(StopAsSuccess.Value);
        }
    }
}
