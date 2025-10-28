using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public abstract class ContainerNode : NodeBase
    {
        public abstract void AddChild(NodeBase child);
        public abstract void RemoveChild(NodeBase child);
        public abstract void ForeachChild(System.Action<NodeBase> action);

        public override void ResetState()
        {
            base.ResetState();
            
            ForeachChild(n =>
            {
                n.ResetState();
            });
        }

        protected override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            
            ForeachChild(n =>
            {
                n.Tick(deltaTime);
            });
        }
    }
}
