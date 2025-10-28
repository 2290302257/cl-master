using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

namespace Alurax
{
    public class BehaviourTreeComponent : MonoBehaviour
    {
        public BehaviourTreeScriptObject BehaviourTreeAsset;
        public BehaviourTree BehaviourTree { get; private set; }
        
        void Start()
        {
            if (BehaviourTreeAsset != null)
            {
                BehaviourTree = BehaviourTreeAsset.CreateBehaviourTree();
                BehaviourTree.Start();                 
            }
        }
    }
}
