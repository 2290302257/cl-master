using UnityEngine;

namespace Alurax
{
	[AddComponentMenu("UGUI/Tween/Tween ScaleY")]
	public class TweenScaleY : UITweener
	{
		public float from = 1f;
		public float to = 1f;

		Transform mTrans;

		public Transform cachedTransform
		{
			get
			{
				if (mTrans == null) mTrans = transform;
				return mTrans;
			}
		}

		public float value
		{
			get { return cachedTransform.localScale.y; }
			set
			{
				cachedTransform.localScale =
					new Vector3(cachedTransform.localScale.x, value, cachedTransform.localScale.z);
			}
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

		static public TweenScaleY Begin(GameObject go, float duration, float scale)
		{
			TweenScaleY comp = UITweener.Begin<TweenScaleY>(go, duration);
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
	}
}