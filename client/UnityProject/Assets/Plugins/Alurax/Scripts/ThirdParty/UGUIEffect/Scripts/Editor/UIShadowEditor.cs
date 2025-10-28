using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Coffee.UIEffects.Editors
{
    /// <summary>
    /// UIShadow editor.
    /// </summary>
    [CustomEditor(typeof(UIShadow))]
    [CanEditMultipleObjects]
    public class UIShadowEditor : Editor
    {
        UGUIEffect _uguiEffect;
        SerializedProperty _spStyle;
        SerializedProperty _spEffectDistance;
        SerializedProperty _spEffectColor;
        SerializedProperty _spUseGraphicAlpha;
        SerializedProperty _spBlurFactor;

        void OnEnable()
        {
            _uguiEffect = (target as UIShadow).GetComponent<UGUIEffect>();
            _spStyle = serializedObject.FindProperty("m_Style");
            _spEffectDistance = serializedObject.FindProperty("m_EffectDistance");
            _spEffectColor = serializedObject.FindProperty("m_EffectColor");
            _spUseGraphicAlpha = serializedObject.FindProperty("m_UseGraphicAlpha");
            _spBlurFactor = serializedObject.FindProperty("m_BlurFactor");
        }

        /// <summary>
        /// Implement this function to make a custom inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            //================
            // Shadow setting.
            //================
            EditorGUILayout.PropertyField(_spStyle);

            // When shadow is enable, show parameters.
            if (_spStyle.intValue != (int) ShadowStyle.None)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(_spEffectDistance);
                EditorGUILayout.PropertyField(_spEffectColor);
                EditorGUILayout.PropertyField(_spUseGraphicAlpha);

                if (_uguiEffect && _uguiEffect.blurMode != BlurMode.None)
                {
                    EditorGUILayout.PropertyField(_spBlurFactor);
                }

                EditorGUI.indentLevel--;
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}
