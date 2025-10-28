using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Alurax
{
    [CreateAssetMenu(menuName ="Custom/BehaviourTree/文件")]
    public class BehaviourTreeScriptObject : ScriptableObject
    {
        #region Editor
        public float InspectorViewWidth;
        public Rect GraphViewRect;
        public Rect BlackboardViewRect;
        #endregion
        
        [SerializeReference]
        public Blackboard Blackboard;
        
        [SerializeReference]
        public RootNode RootNode;
        
        [SerializeReference]
        public List<NodeBase> AllNodes = new List<NodeBase>();

        public void SetAll(List<NodeBase> allNodes)
        {
            if (allNodes == null)
                return;
            
            AllNodes = allNodes;
            RootNode = allNodes.Find((node) => { return node is RootNode; }) as RootNode;
            Blackboard = RootNode.GetBlackboard();
            
            RemoveOrphanNodes();
        }

        private void RemoveOrphanNodes()
        {
            List<NodeBase> allNodesFromRoot = RootNode.GetAllNodes();
            foreach (var node in allNodesFromRoot)
            {
                if (node != null && node.Parent != null)
                {
                    if (!AllNodes.Contains(node))
                    {
                        node.Parent.RemoveChild(node);                        
                    }
                }
                
                if (node is ContainerNode containerNode)
                {
                    containerNode.RemoveChild(null); // 删除所有空节点
                }
            }       
        }

        public BehaviourTree CreateBehaviourTree(List<NodeBase> outAllNodes = null)
        {
            var instance = Instantiate(this);
            if (outAllNodes != null)
            {
                outAllNodes.AddRange(instance.AllNodes);
            }
            
            return new BehaviourTree(instance.RootNode);
        }
    }
    
}
