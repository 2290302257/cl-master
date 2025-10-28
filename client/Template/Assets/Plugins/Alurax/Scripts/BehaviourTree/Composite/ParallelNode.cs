using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Alurax
{
    public class ParallelNode : CompositeNode
    {
        public enum Policy
        {
            Any,
            All
        }

        [SerializeReference] 
        protected BlackboardVariableParallelNodePolicy PolicySuccess = new BlackboardVariableParallelNodePolicy(null, Policy.All);

        [SerializeReference] 
        protected BlackboardVariableParallelNodePolicy PolicyFailure = new BlackboardVariableParallelNodePolicy(null, Policy.All);

        private int _countSuccess;
        private int _countFailure;

        protected override void OnStart()
        {
            base.OnStart();
            
            _countSuccess = 0;
            _countFailure = 0;
            
            for (int i = 0; i < Children.Count; i++)
            {
                GetChild(i).Start();
            }
        }

        protected override void OnChildFinish(NodeBase child, bool success)
        {
            if (success)
            {
                _countSuccess++;
                if ((Policy)PolicySuccess.Value == Policy.Any)
                {
                    Finish(true);
                }
            }
            else
            {
                _countFailure++;
                if ((Policy)PolicyFailure.Value == Policy.Any)
                {
                    Finish(false);
                }
            }

            if (_countSuccess == Children.Count && (Policy)PolicySuccess.Value == Policy.All)
            {
                Finish(true);
            }
            else if (_countFailure == Children.Count && (Policy)PolicyFailure.Value == Policy.All)
            {
                Finish(false);
            }

            if (_countSuccess + _countFailure == Children.Count())
            {
                Finish(false);
            }
        }
    }
    
    public class BlackboardVariableParallelNodePolicy : BlackboardVariable<ParallelNode.Policy>
    {
        public BlackboardVariableParallelNodePolicy(string name) : base(name)
        {
        }
        
        public BlackboardVariableParallelNodePolicy(string name, ParallelNode.Policy value) : base(name)
        {
            SetValue(value);
        }
    }
}