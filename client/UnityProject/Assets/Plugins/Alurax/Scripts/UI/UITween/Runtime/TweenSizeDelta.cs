using UnityEngine;

namespace Alurax
{
	/// <summary>
	/// Tween the widget's size.
	/// </summary>

	[AddComponentMenu("UGUI/Tween/Tween SizeDelta")]
	public class TweenSizeDelta : UITweener
	{
		public Vector2 from;
		public Vector2 to;

		RectTransform mWidget;

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
			get { return cachedWidget.sizeDelta; }
			set { cachedWidget.sizeDelta = value; }
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

		static public TweenSizeDelta Begin(RectTransform widget, float duration, Vector2 sizeDelta)
		{
			TweenSizeDelta comp = UITweener.Begin<TweenSizeDelta>(widget.gameObject, duration);
			comp.from = widget.sizeDelta;
			comp.to = sizeDelta;

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
	}
}