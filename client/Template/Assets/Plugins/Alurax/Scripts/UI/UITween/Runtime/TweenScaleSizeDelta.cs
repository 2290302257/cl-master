using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
	/// <summary>
	/// Tween the widget's size.
	/// </summary>

	[AddComponentMenu("UGUI/Tween/Tween SizeDelta")]
	public class TweenScaleSizeDelta : UITweener
	{
		public Vector2 from;
		public Vector2 to;

		RectTransform mWidget;
		Vector2 cachedSizeDelta;
		
		public RectTransform cachedWidget
		{
			get
			{
				if (mWidget == null) mWidget = transform as RectTransform;
				return mWidget;
			}
		}

		/// <summary>
		/// Tween's current value.
		/// </summary>
		
		public Vector2 value
		{
			get => cachedWidget.sizeDelta;
			set => cachedWidget.sizeDelta = value;
		}

		/// <summary>
		/// Tween the value.
		/// </summary>

		protected override void OnUpdate(float factor, bool isFinished)
		{
			value = from * (1f - factor) + to * factor;
		}

		/// <summary>
		/// Start the tweening operation.
		/// </summary>

		static public TweenScaleSizeDelta Begin(RectTransform widget, float duration, Vector2 scale)
		{
			TweenScaleSizeDelta comp = UITweener.Begin<TweenScaleSizeDelta>(widget.gameObject, duration);
			comp.from = comp.value;
			comp.to = scale;

			if (duration <= 0f)
			{
				comp.Sample(1f, true);
				comp.enabled = false;
			}

			return comp;
		}

		[ContextMenu("Set 'From' to current value")]
		public override void SetStartToCurrentValue()
		{
			from = value;
		}

		[ContextMenu("Set 'To' to current value")]
		public override void SetEndToCurrentValue()
		{
			to = value;
		}

		[ContextMenu("Assume value of 'From'")]
		void SetCurrentValueToStart()
		{
			value = from;
		}

		[ContextMenu("Assume value of 'To'")]
		void SetCurrentValueToEnd()
		{
			value = to;
		}

		private void OnEnable()
		{
			//UpdateCachedSizeDelta();
		}

		public void UpdateCachedSizeDelta()
		{
			cachedSizeDelta = cachedWidget.sizeDelta;
		}
	}
}
