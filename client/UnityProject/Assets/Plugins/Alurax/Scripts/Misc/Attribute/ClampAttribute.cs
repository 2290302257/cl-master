#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace Alurax
{
    [System.AttributeUsage(System.AttributeTargets.Field)]
    public class ClampAttribute : PropertyAttribute
    {
        public float Min;
        public float Max;
        public ClampAttribute(int min,int max) {
            this.Min = min;
            this.Max = max;
        }
        public ClampAttribute(float min,float max) {
            this.Min = min;
            this.Max = max;
        }
    }
#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(ClampAttribute))]
    public class ClampAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var clamp = (ClampAttribute)attribute;
            if(property.type=="int")
                property.intValue = Mathf.Clamp(property.intValue, (int)clamp.Min, (int)clamp.Max);
            else if(property.type=="float")
                property.floatValue = Mathf.Clamp(property.floatValue, clamp.Min, clamp.Max);
            EditorGUI.PropertyField(position, property, label);
        }
    }
#endif
}
