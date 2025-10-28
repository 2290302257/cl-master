using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class SelectorNode : CompositeNode
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
                Finish(true);
            }
            else
            {
                StartNextChild();
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
                Finish(false);
            }
        }
    }
}
