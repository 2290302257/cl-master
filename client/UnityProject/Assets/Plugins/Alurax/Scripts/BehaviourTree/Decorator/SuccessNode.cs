using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class SuccessNode : DecoratorNode
    {
        protected override void OnChildFinish(NodeBase child, bool success)
        {
            Finish(true);
        }
    }
}
