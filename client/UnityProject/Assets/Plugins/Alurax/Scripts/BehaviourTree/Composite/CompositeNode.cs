using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Alurax
{
    public abstract class CompositeNode : ContainerNode
    {
        [SerializeReference] 
        protected List<NodeBase> Children = new List<NodeBase>();
        
        [SerializeReference] 
        protected BlackboardVariableBool Random = new BlackboardVariableBool(null);

        private List<int> _randomOrders;
        
        protected override void OnStart()
        {
            RandomOrders();
        }

        protected override void OnStop()
        {
            for (int i = 0; i < Children.Count; i++)
            {
                GetChild(i).Stop();
            }
        }

        public override void AddChild(NodeBase child)
        {
            if (child == null || Children.Contains(child))
                return;

            if (child.Parent != null)
                child.Parent.RemoveChild(child);

            Children.Add(child);
            child.SetParent(this);
            
            RandomOrders();
            if (IsRunning)
            {
                Restart();                
            }
        }

        public override void RemoveChild(NodeBase child)
        {
            if (child != null)
            {
                if (Children.Remove(child))
                {
                    child.SetParent(null);
                    RandomOrders();
                }
                
                if (child.IsRunning)
                {
                    Restart();
                }                
            }
            else // 删除所有为空的子节点
            {
                bool dirty = false;
                for (int i = Children.Count - 1; i >= 0; i--)
                {
                    if (Children[i] == null)
                    {
                        Children.RemoveAt(i);
                        dirty = true;
                    }
                }

                if (dirty)
                {
                    RandomOrders();
                }
            }
        }

        public override void ForeachChild(Action<NodeBase> action)
        {
            for (int i = 0; i < Children.Count; i++)
            {
                action?.Invoke(GetChild(i));
            }
        }

        public NodeBase GetChild(int childIdx)
        {
            if (childIdx < 0 || childIdx >= Children.Count)
                return null;

            if (Random.Value && _randomOrders == null)
            {
                RandomOrders();
            }
            
            int idx = Random.Value ? _randomOrders[childIdx] : childIdx;
            return Children[idx];
        }

        public void SortOrders(Comparison<NodeBase> comparison)
        {
            Children.Sort(comparison);
            RandomOrders();
        }

        private void RandomOrders()
        {
            if (Random.Value)
            {
                _randomOrders = ShufflingOrder(Children.Count);
            }
        }
        
        private static List<int> ShufflingOrder(int length)
        {
            List<int> order = new List<int>();
            for (int i = 0; i < length; i++)
            {
                order.Add(i);
            }

            for (int i = 0; i < length; i++)
            {
                int temp = order[i];
                int randomIndex = UnityEngine.Random.Range(i, length);
                order[i] = order[randomIndex];
                order[randomIndex] = temp;
            }

            return order;
        }
    }
}