using UnityEngine;
using UnityEditor;

namespace Alurax
{
	[CustomEditor(typeof(TweenRotationX))]
	public class TweenRotationXEditor : UITweenerEditor
	{
		public override void OnInspectorGUI()
		{
			GUILayout.Space(6f);
			SetLabelWidth(120f);

			TweenRotationX tw = target as TweenRotationX;
			GUI.changed = false;

			float from = EditorGUILayout.FloatField("From", tw.from);
			float to = EditorGUILayout.FloatField("To", tw.to);
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