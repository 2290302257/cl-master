using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class SequenceNode : CompositeNode
    {
        private int _curChildIdx = -1;

        protected override void OnStart()
        {
            base.OnStart();
            
            _curChildIdx = -1;
            StartNextChild();
        }

        protected override void OnChildFinish(NodeBase child, bool success)
        {
            if (success)
            {
                StartNextChild();
            }
            else
            {
                Finish(false);
            }
        }
        
        private void StartNextChild()
        {
            _curChildIdx++;
            if (_curChildIdx < Children.Count)
            {
                GetChild(_curChildIdx).Start();
            }
            else
            {
                Finish(true);
            }
        }
    }
}
