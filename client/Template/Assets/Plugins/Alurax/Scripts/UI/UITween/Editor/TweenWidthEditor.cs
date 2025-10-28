using UnityEngine;
using UnityEditor;
namespace Alurax
{
	[CustomEditor(typeof(TweenWidth))]
	public class TweenWidthEditor : UITweenerEditor
	{
		public override void OnInspectorGUI ()
		{
			GUILayout.Space(6f);
			SetLabelWidth(120f);

			TweenWidth tw = target as TweenWidth;
			GUI.changed = false;

			float from = EditorGUILayout.FloatField("From", tw.from);
			float to = EditorGUILayout.FloatField("To", tw.to);

			if (from < 0) from = 0;
			if (to < 0) to = 0;

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
