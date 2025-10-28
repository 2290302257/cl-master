using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public abstract class DecoratorNode : ContainerNode
    {
        [SerializeReference]
        protected NodeBase Child;

        protected virtual bool finishWithChild { get; set; } = true;
        
        protected override void OnStart()
        {
            if (Child != null)
            {
                Child.Start();
            }
            else if (finishWithChild)
            {
                Finish(true);
            }
        }
        
        protected override void OnStop()
        {
            if (Child != null)
            {
                Child.Stop();
            }
        }
        
        protected override void OnChildFinish(NodeBase child, bool success)
        {
            if (finishWithChild)
            {
                Finish(success);
            }
        }
        
        public override void AddChild(NodeBase child)
        {
            if (child == null || Child == child)
                return;
            
            if (Child != null)
                Child.SetParent(null);
            
            if (child.Parent != null)
                child.Parent.RemoveChild(child);
            
            Child = child;
            Child.SetParent(this);

            if (IsRunning)
            {
                Restart();
            }
        }

        public override void RemoveChild(NodeBase child)
        {
            if (child == null)
                return;
            
            if (Child == child)
            {
                Child.SetParent(null);
                Child = null;
            }
        }

        public override void ForeachChild(Action<NodeBase> action)
        {
            if (Child != null)
            {
                action?.Invoke(Child);
            }
        }
    }
}
