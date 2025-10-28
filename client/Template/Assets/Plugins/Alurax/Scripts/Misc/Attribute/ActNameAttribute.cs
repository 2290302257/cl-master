using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    public class ActNameAttribute : PropertyAttribute
    {
        public string label;
        public ActNameAttribute(string label) {
            this.label = label;
        }
    }
}
