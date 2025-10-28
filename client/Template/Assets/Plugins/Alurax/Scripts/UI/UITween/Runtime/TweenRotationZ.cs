using UnityEngine;

namespace Alurax
{
	/// <summary>
	/// Tween the object's rotation.
	/// </summary>

	[AddComponentMenu("UGUI/Tween/Tween RotationZ")]
	public class TweenRotationZ : UITweener
	{
		public float from;
		public float to;
		public bool quaternionLerp = false;

		RectTransform mTrans;

		public RectTransform cachedTransform
		{
			get
			{
				if (mTrans == null) mTrans = transform as RectTransform;
				return mTrans;
			}
		}

		public Vector3 eulerAngles => cachedTransform.eulerAngles;

		/// <summary>
		/// Tween's current value.
		/// </summary>

		public Quaternion value
		{
			get { return cachedTransform.localRotation; }
			set { cachedTransform.localRotation = value; }
		}

		/// <summary>
		/// Tween the value.
		/// </summary>

		protected override void OnUpdate(float factor, bool isFinished)
		{
			value = quaternionLerp
				? Quaternion.Slerp(Quaternion.Euler(eulerAngles.x, eulerAngles.y, from),
					Quaternion.Euler(eulerAngles.x, eulerAngles.y, to), factor)
				: Quaternion.Euler(new Vector3(eulerAngles.x, eulerAngles.y, Mathf.Lerp(from, to, factor)));
		}

		/// <summary>
		/// Start the tweening operation.
		/// </summary>

		static public TweenRotationZ Begin(GameObject go, float duration, Quaternion rot)
		{
			TweenRotationZ comp = UITweener.Begin<TweenRotationZ>(go, duration);
			comp.from = comp.value.eulerAngles.z;
			comp.to = rot.eulerAngles.z;

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
			from = value.eulerAngles.z;
		}

		[ContextMenu("Set 'To' to current value")]
		public override void SetEndToCurrentValue()
		{
			to = value.eulerAngles.z;
		}

		[ContextMenu("Assume value of 'From'")]
		void SetCurrentValueToStart()
		{
			value = Quaternion.Euler(eulerAngles.x, eulerAngles.y, from);
		}

		[ContextMenu("Assume value of 'To'")]
		void SetCurrentValueToEnd()
		{
			value = Quaternion.Euler(eulerAngles.x, eulerAngles.y, to);
		}
	}
}