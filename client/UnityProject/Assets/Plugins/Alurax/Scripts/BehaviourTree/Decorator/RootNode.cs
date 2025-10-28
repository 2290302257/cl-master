using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class RootNode : DecoratorNode
    {
        public List<NodeBase> GetAllNodes()
        {
            var allNodes = new List<NodeBase>();
            var queue = new Queue<NodeBase>();
            queue.Enqueue(this);
            
            while (queue.Count > 0)
            {
                var node = queue.Dequeue();
                allNodes.Add(node);
                if (node is ContainerNode containerNode)
                {
                    containerNode.ForeachChild(
                        (child) => queue.Enqueue(child));
                }
            }

            return allNodes;
        }
    }
}
