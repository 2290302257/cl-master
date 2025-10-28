using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class BehaviourTree
    {
        public Blackboard Blackboard { get; private set; }
        public RootNode RootNode { get; private set; }

        public BehaviourTree()
        {
            Blackboard = new Blackboard(null);
            RootNode = CreateNode<RootNode>();
        }

        public BehaviourTree(RootNode rootNode)
        {
            RootNode = rootNode;
            Blackboard = rootNode?.GetBlackboard();
            Blackboard?.OnPostDeserialize();
        }
        
        public List<NodeBase> GetAllNodes()
        {
            return RootNode?.GetAllNodes();
        }

        public void Tick()
        {
            RootNode?.Tick(Time.deltaTime);
        }
        
        public void Start()
        {
            RootNode?.Start();
        }

        public void Stop()
        {
            RootNode?.Stop();
        }
        
        public void Restart()
        {
            RootNode?.Restart();
        }
        
        public void SetParentBlackboard(Blackboard parentBlackboard)
        {
            Blackboard?.SetParentBlackboard(parentBlackboard);
        }
        
        public T CreateNode<T>()  where T: NodeBase, new()
        {
            return CreateNode(typeof(T)) as T;
        }

        public NodeBase CreateNode(System.Type type)
        {
            var node = (NodeBase)System.Activator.CreateInstance(type);
            node.SetBlackboard(Blackboard);
            
            return node;
        }

        public void RemoveNode(NodeBase node)
        {
            if (node == RootNode)
                return;

            if (node.Parent != null)
            {
                node.Parent.RemoveChild(node);
                if (node.IsRunning)
                {
                    node.Parent.Restart();
                }
            }
            
            if (node is ContainerNode containerNode)
            {
                containerNode.ForeachChild(
                    (child) => child.SetParent(null));
            }
        }
    }
}
