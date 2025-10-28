using System;
using UnityEngine;
using Object = System.Object;

namespace Alurax
{
    [Serializable]
    public class TranformBlackboard:Blackboard
    {
        [SerializeField][SerializeReference]
        private Transform _transformOwner;

        public override Object Owner => _transformOwner;

        public TranformBlackboard():base()
        {
            
        }
        public TranformBlackboard(Object owner) : base(owner)
        {
        }
        
        public void SetTransform(Transform transform)
        {
            _transformOwner = transform;
        }
    }
    
    public class BlackboardComponent: MonoBehaviour
    { 
        
       [SerializeField]
       private TranformBlackboard _blackboard;
       public Blackboard Blackboard
       {
           get
           {
               if (_blackboard == null)
               {
                   _blackboard = new TranformBlackboard(transform);
               }
               return _blackboard;
           }
       }

       public void SetTransform()
       {
           (Blackboard as TranformBlackboard)?.SetTransform(transform);
       }
    }
} 