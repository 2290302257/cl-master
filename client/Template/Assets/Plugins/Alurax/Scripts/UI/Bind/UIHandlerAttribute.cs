using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = true)]
    public class UIHandlerAttribute : System.Attribute
    {
        string m_Name;
        public UIHandlerAttribute(string name)
        {
            m_Name = name;
        }
        public string GetName() => m_Name;
    }
}