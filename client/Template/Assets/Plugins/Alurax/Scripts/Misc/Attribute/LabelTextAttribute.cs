#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Alurax
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class LabelTextAttribute : PropertyAttribute
    {
        public string label;
        public bool enable=true;
        public LabelTextAttribute(string label) {
            this.label = label;
        }
        public LabelTextAttribute(string label,bool enable) {
            this.label = label;
            this.enable = enable;
        }
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(LabelTextAttribute))]
    public class LabelTextDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var labelText = (LabelTextAttribute)attribute;
            if (!labelText.enable)
                GUI.enabled = false;
            EditorGUI.PropertyField(position, property, new GUIContent(labelText.label));
            if (!labelText.enable)
                GUI.enabled = true;
        }
    }
#endif
}
