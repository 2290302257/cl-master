using UnityEngine;
using UnityEditor;

namespace Alurax
{
	[CustomEditor(typeof(TweenRotation))]
	public class TweenRotationEditor : UITweenerEditor
	{
		public override void OnInspectorGUI()
		{
			GUILayout.Space(6f);
			SetLabelWidth(120f);

			TweenRotation tw = target as TweenRotation;
			GUI.changed = false;

			Vector3 from = EditorGUILayout.Vector3Field("From", tw.from);
			Vector3 to = EditorGUILayout.Vector3Field("To", tw.to);
			var quat = EditorGUILayout.Toggle("Quaternion", tw.quaternionLerp);

			if (GUI.changed)
			{
				RegisterUndo("Tween Change", tw);
				tw.from = from;
				tw.to = to;
				tw.quaternionLerp = quat;
				SetDirty(tw);
			}

			DrawCommonProperties();
		}
	}
}
