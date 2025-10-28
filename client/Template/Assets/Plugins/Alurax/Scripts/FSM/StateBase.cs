using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public abstract class StateBase
    {
        public abstract void Enter();
        public abstract void Exit(StateBase nextState);
        public abstract void Execute();
    }
}
