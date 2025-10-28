using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class RandomNode : DecoratorNode
    {
        [Range(0, 1)]
        [SerializeReference]
        public BlackboardVariableFloat Probability = new BlackboardVariableFloat(null);

        protected override void OnStart()
        {
            base.OnStart();
            
            if (Random.value <= Probability.Value)
            {
                Child.Start();
            }
            else
            {
                Finish(false);
            }
        }
    }
}
