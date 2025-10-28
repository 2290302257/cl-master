using UnityEngine;

namespace Alurax
{
	/// <summary>
	/// Tween the object's position.
	/// </summary>

	[AddComponentMenu("UGUI/Tween/Tween PositionX")]
	public class TweenPositionX : UITweener
	{
		public float from;
		public float to;


		RectTransform mTrans;

		public RectTransform cachedTransform
		{
			get
			{
				if (mTrans == null) mTrans = transform as RectTransform;
				return mTrans;
			}
		}


		/// <summary>
		/// Tween's current value.
		/// </summary>
		public float value
		{
			get { return cachedTransform.anchoredPosition.x; }
			set { cachedTransform.anchoredPosition = new Vector2(value, cachedTransform.anchoredPosition.y); }
		}

		void Awake()
		{
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

		static public TweenPositionX Begin(GameObject go, float duration, float pos)
		{
			TweenPositionX comp = UITweener.Begin<TweenPositionX>(go, duration);
			comp.from = comp.value;
			comp.to = pos;

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