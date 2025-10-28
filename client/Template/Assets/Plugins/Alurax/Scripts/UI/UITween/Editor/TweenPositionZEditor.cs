using UnityEngine;
using UnityEditor;
namespace Alurax
{
[CustomEditor(typeof(TweenPositionZ))]
	public class TweenPositionZEditor : UITweenerEditor
	{
		public override void OnInspectorGUI ()
		{
			GUILayout.Space(6f);
			SetLabelWidth(120f);
			TweenPositionZ tw = target as TweenPositionZ;
			GUI.changed = false;

			float from = EditorGUILayout.FloatField("From", tw.from);
			float to = EditorGUILayout.FloatField("To", tw.to);

			if (GUI.changed)
			{
				RegisterUndo("Tween Change", tw);
				tw.from = from;
				tw.to = to;
				SetDirty(tw);
			}

			DrawCommonProperties();
		}
	}
}
