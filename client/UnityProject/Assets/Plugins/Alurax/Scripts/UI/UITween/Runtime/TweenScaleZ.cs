using UnityEngine;

namespace Alurax
{
	[AddComponentMenu("UGUI/Tween/Tween ScaleZ")]
	public class TweenScaleZ : UITweener
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
			get { return cachedTransform.localScale.z; }
			set
			{
				cachedTransform.localScale =
					new Vector3(cachedTransform.localScale.x, cachedTransform.localScale.y, value);
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

		static public TweenScaleZ Begin(GameObject go, float duration, float scale)
		{
			TweenScaleZ comp = UITweener.Begin<TweenScaleZ>(go, duration);
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