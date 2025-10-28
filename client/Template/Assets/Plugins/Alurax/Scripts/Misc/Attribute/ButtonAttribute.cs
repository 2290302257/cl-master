using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
#if UNITY_EDITOR
using UnityEditor;
#endif 
using UnityEngine;

namespace Alurax
{
    [AttributeUsage(AttributeTargets.Method)]
    public class ButtonAttribute : PropertyAttribute
    {
        public string Name;
        public ButtonAttribute(string name)
        {
            Name = name;
        }
    }
    
#if UNITY_EDITOR
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class ButtonAttributeEditor : Editor
    {
        MonoBehaviour m_mono;
        MethodInfo[] m_MethodInfo;
        private void OnEnable()
        {
            m_mono = target as MonoBehaviour;
            m_MethodInfo = m_mono.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static).Where(method => Attribute.IsDefined(method, typeof(ButtonAttribute))).ToArray();
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            foreach (var method in m_MethodInfo)
            {
                var attr = method.GetCustomAttribute<ButtonAttribute>();
                if (GUILayout.Button(attr.Name))
                    method.Invoke(m_mono, new object[] { });
            }
        }
    }

#endif
}
