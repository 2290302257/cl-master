using UnityEngine;
using UnityEditor;

namespace Alurax
{
	[CustomEditor(typeof(TweenScaleSizeDelta))]
	public class TweenScaleSizeDeltaEditor : UITweenerEditor
	{
		public override void OnInspectorGUI()
		{
			GUILayout.Space(6f);
			SetLabelWidth(120f);

			TweenScaleSizeDelta tw = target as TweenScaleSizeDelta;
			GUI.changed = false;

			Vector2 from = EditorGUILayout.Vector2Field("From", tw.from);
			Vector2 to = EditorGUILayout.Vector2Field("To", tw.to);

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