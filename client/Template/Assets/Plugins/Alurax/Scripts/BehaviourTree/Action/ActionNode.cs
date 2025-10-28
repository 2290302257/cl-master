using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public abstract class ActionNode : NodeBase
    {
        protected sealed override void OnChildFinish(NodeBase child, bool success) { }
        protected override void OnStop()
        {
        }
    }
}
