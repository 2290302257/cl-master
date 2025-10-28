using UnityEngine;

namespace Alurax
{
	/// <summary>
	/// Tween the widget's size.
	/// </summary>

	[AddComponentMenu("UGUI/Tween/Tween Height")]
	public class TweenHeight : UITweener
	{
		public float from = 100;
		public float to = 100;

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

		public float value
		{
			get { return cachedWidget.sizeDelta.y; }
			set { cachedWidget.sizeDelta = new Vector2(cachedWidget.sizeDelta.x, value); }
		}

		/// <summary>
		/// Tween the value.
		/// </summary>

		protected override void OnUpdate(float factor, bool isFinished)
		{
			value = Mathf.RoundToInt(from * (1f - factor) + to * factor);

		}

		/// <summary>
		/// Start the tweening operation.
		/// </summary>

		static public TweenHeight Begin(RectTransform widget, float duration, int height)
		{
			TweenHeight comp = UITweener.Begin<TweenHeight>(widget.gameObject, duration);
			comp.from = widget.sizeDelta.y;
			comp.to = height;

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