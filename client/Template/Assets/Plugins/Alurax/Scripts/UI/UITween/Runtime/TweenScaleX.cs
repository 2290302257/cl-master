using UnityEngine;

namespace Alurax
{
	[AddComponentMenu("UGUI/Tween/Tween ScaleX")]
	public class TweenScaleX : UITweener
	{
		public float from = 1f;
		public float to = 1f;
		public bool updateTable = false;

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
			get { return cachedTransform.localScale.x; }
			set
			{
				cachedTransform.localScale =
					new Vector3(value, cachedTransform.localScale.y, cachedTransform.localScale.z);
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

		static public TweenScaleX Begin(GameObject go, float duration, float scale)
		{
			TweenScaleX comp = UITweener.Begin<TweenScaleX>(go, duration);
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
