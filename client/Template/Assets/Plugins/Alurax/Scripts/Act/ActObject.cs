using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Alurax
{
    [CreateAssetMenu(menuName ="Custom/Act/文件")]
    public class ActObject : ScriptableObject
    {
        [SerializeReference]
        public List<ActBase> ActList = new List<ActBase>();
        
        [LabelText("循环播放")]
        public bool Loop;
        
        [LabelText("总时间",false)]
        public float Duration;

        [LabelText("描述")]
        public string Desc;

        public Blackboard Blackboard { get; private set; }
        
        public void SetBlackboard(Blackboard blackboard)
        {
            Blackboard = blackboard;
        }
    }
}
