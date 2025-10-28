using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Alurax
{
	[CustomEditor(typeof(UITweener), true)]
	public class UITweenerEditor : Editor
	{
		public override void OnInspectorGUI()
		{
			GUILayout.Space(6f);
			SetLabelWidth(110f);
			base.OnInspectorGUI();
			DrawCommonProperties();
		}


		static public void SetLabelWidth(float width)
		{
			EditorGUIUtility.labelWidth = width;
		}

		static bool mEndHorizontal = false;

		static public void BeginContents()
		{
			BeginContents(minimalisticLook);
		}

		static public void BeginContents(bool minimalistic)
		{
			if (!minimalistic)
			{
				mEndHorizontal = true;
				GUILayout.BeginHorizontal();
				EditorGUILayout.BeginHorizontal("AS TextArea", GUILayout.MinHeight(10f));
			}
			else
			{
				mEndHorizontal = false;
				EditorGUILayout.BeginHorizontal(GUILayout.MinHeight(10f));
				GUILayout.Space(10f);
			}

			GUILayout.BeginVertical();
			GUILayout.Space(2f);
		}

		static public void EndContents()
		{
			GUILayout.Space(3f);
			GUILayout.EndVertical();
			EditorGUILayout.EndHorizontal();

			if (mEndHorizontal)
			{
				GUILayout.Space(3f);
				GUILayout.EndHorizontal();
			}

			GUILayout.Space(3f);
		}

		static public bool DrawHeader(string text, string key, bool forceOn, bool minimalistic)
		{
			bool state = EditorPrefs.GetBool(key, true);

			if (!minimalistic) GUILayout.Space(3f);
			if (!forceOn && !state) GUI.backgroundColor = new Color(0.8f, 0.8f, 0.8f);
			GUILayout.BeginHorizontal();
			GUI.changed = false;

			if (minimalistic)
			{
				if (state) text = "\u25BC" + (char)0x200a + text;
				else text = "\u25BA" + (char)0x200a + text;

				GUILayout.BeginHorizontal();
				GUI.contentColor = EditorGUIUtility.isProSkin
					? new Color(1f, 1f, 1f, 0.7f)
					: new Color(0f, 0f, 0f, 0.7f);
				if (!GUILayout.Toggle(true, text, "PreToolbar2", GUILayout.MinWidth(20f))) state = !state;
				GUI.contentColor = Color.white;
				GUILayout.EndHorizontal();
			}
			else
			{
				text = "<b><size=11>" + text + "</size></b>";
				if (state) text = "\u25BC " + text;
				else text = "\u25BA " + text;
				if (!GUILayout.Toggle(true, text, "dragtab", GUILayout.MinWidth(20f))) state = !state;
			}

			if (GUI.changed) EditorPrefs.SetBool(key, state);

			if (!minimalistic) GUILayout.Space(2f);
			GUILayout.EndHorizontal();
			GUI.backgroundColor = Color.white;
			if (!forceOn && !state) GUILayout.Space(3f);
			return state;
		}

		static public bool GetBool(string name, bool defaultValue)
		{
			return EditorPrefs.GetBool(name, defaultValue);
		}

		static public void SetBool(string name, bool val)
		{
			EditorPrefs.SetBool(name, val);
		}

		static public bool minimalisticLook
		{
			get { return GetBool("UGUI Minimalistic", false); }
			set { SetBool("UGUI Minimalistic", value); }
		}

		static public bool DrawHeader(string text)
		{
			return DrawHeader(text, text, false, minimalisticLook);
		}

		static public void SetDirty(UnityEngine.Object obj)
		{
#if UNITY_EDITOR
			if (obj)
			{
				UnityEditor.EditorUtility.SetDirty(obj);
			}
#endif
		}

		protected void DrawCommonProperties()
		{
			UITweener tw = target as UITweener;

			if (DrawHeader("Tweener"))
			{
				BeginContents();
				SetLabelWidth(110f);

				GUI.changed = false;

				UITweener.Style style = (UITweener.Style)EditorGUILayout.EnumPopup("Play Style", tw.style);
				AnimationCurve curve = EditorGUILayout.CurveField("Animation Curve", tw.animationCurve,
					GUILayout.Width(170f), GUILayout.Height(62f));
				//UITweener.Method method = (UITweener.Method)EditorGUILayout.EnumPopup("Play Method", tw.method);

				GUILayout.BeginHorizontal();
				float dur = EditorGUILayout.FloatField("Duration", tw.duration, GUILayout.Width(170f));
				GUILayout.Label("seconds");
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				float del = EditorGUILayout.FloatField("Start Delay", tw.delay, GUILayout.Width(170f));
				GUILayout.Label("seconds");
				GUILayout.EndHorizontal();

				int tg = EditorGUILayout.IntField("Tween Group", tw.tweenGroup, GUILayout.Width(170f));
				bool ts = EditorGUILayout.Toggle("Ignore TimeScale", tw.ignoreTimeScale);
				bool fx = EditorGUILayout.Toggle("Use Fixed Update", tw.useFixedUpdate);

				if (GUI.changed)
				{
					RegisterUndo("Tween Change", tw);
					tw.animationCurve = curve;
					//tw.method = method;
					tw.style = style;
					tw.ignoreTimeScale = ts;
					tw.tweenGroup = tg;
					tw.duration = dur;
					tw.delay = del;
					tw.useFixedUpdate = fx;
					SetDirty(tw);
				}

				EndContents();
			}
		}

		static public void RegisterUndo(string name, params Object[] objects)
		{
			if (objects != null && objects.Length > 0)
			{
				UnityEditor.Undo.RecordObjects(objects, name);

				foreach (Object obj in objects)
				{
					if (obj == null) continue;
					EditorUtility.SetDirty(obj);
				}
			}
		}
	}
}