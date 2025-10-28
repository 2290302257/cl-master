using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    public class LogNode : ActionNode
    {
        public enum Level : int
        {
            Log = 0,
            Warning,
            Error,
            Assertion
        }

        [SerializeReference] 
        protected BlackboardVariableLogNodeLevel LogLevel = new BlackboardVariableLogNodeLevel(null);
        
        [SerializeReference]
        protected BlackboardVariableString LogMessage = new BlackboardVariableString(null);
        
        protected override void OnStart()
        {
            var level = (Level)LogLevel.Value;
            var msg = LogMessage.Value;
            switch (level)
            {
                case Level.Log:
                    Debug.Log(msg);
                    break;
                case Level.Warning:
                    Debug.LogWarning(msg);
                    break;
                case Level.Error:
                    Debug.LogError(msg);
                    break;
                case Level.Assertion:
                    Debug.LogAssertion(msg);
                    break;
            }
            
            Finish(true);
        }
    }
    
    public class BlackboardVariableLogNodeLevel : BlackboardVariable<LogNode.Level>
    {
        public BlackboardVariableLogNodeLevel(string name) : base(name)
        {
        }
    }
}
