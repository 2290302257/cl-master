using UnityEngine;
using UnityEditor;

namespace Alurax
{
	[CustomEditor(typeof(TweenScaleY))]
	public class TweenScaleYEditor : UITweenerEditor
	{
		public override void OnInspectorGUI()
		{
			GUILayout.Space(6f);
			SetLabelWidth(120f);

			TweenScaleY tw = target as TweenScaleY;
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
