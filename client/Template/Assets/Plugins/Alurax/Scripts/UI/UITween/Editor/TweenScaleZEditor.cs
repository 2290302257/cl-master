using UnityEngine;
using UnityEditor;
namespace Alurax
{
	[CustomEditor(typeof(TweenScaleZ))]
	public class TweenScaleZEditor : UITweenerEditor
	{
		public override void OnInspectorGUI ()
		{
			GUILayout.Space(6f);
			SetLabelWidth(120f);

			TweenScaleZ tw = target as TweenScaleZ;
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