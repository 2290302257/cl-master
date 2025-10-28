using UnityEngine;

namespace Alurax
{
	/// <summary>
	/// Tween the object's rotation.
	/// </summary>

	[AddComponentMenu("UGUI/Tween/Tween RotationX")]
	public class TweenRotationX : UITweener
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
				? Quaternion.Slerp(Quaternion.Euler(from, eulerAngles.y, eulerAngles.z),
					Quaternion.Euler(to, eulerAngles.y, eulerAngles.z), factor)
				: Quaternion.Euler(new Vector3(Mathf.Lerp(from, to, factor), eulerAngles.y, eulerAngles.z));
		}

		/// <summary>
		/// Start the tweening operation.
		/// </summary>

		static public TweenRotationX Begin(GameObject go, float duration, Quaternion rot)
		{
			TweenRotationX comp = UITweener.Begin<TweenRotationX>(go, duration);
			comp.from = comp.value.eulerAngles.x;
			comp.to = rot.eulerAngles.x;

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
			from = value.eulerAngles.x;
		}

		[ContextMenu("Set 'To' to current value")]
		public override void SetEndToCurrentValue()
		{
			to = value.eulerAngles.x;
		}

		[ContextMenu("Assume value of 'From'")]
		void SetCurrentValueToStart()
		{
			value = Quaternion.Euler(from, eulerAngles.y, eulerAngles.z);
		}

		[ContextMenu("Assume value of 'To'")]
		void SetCurrentValueToEnd()
		{
			value = Quaternion.Euler(to, eulerAngles.y, eulerAngles.z);
		}
	}
}