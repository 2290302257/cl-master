using UnityEngine;

namespace Alurax
{
	/// <summary>
	/// Tween the object's rotation.
	/// </summary>

	[AddComponentMenu("UGUI/Tween/Tween RotationY")]
	public class TweenRotationY : UITweener
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
				? Quaternion.Slerp(Quaternion.Euler(eulerAngles.x, from, eulerAngles.z),
					Quaternion.Euler(eulerAngles.x, to, eulerAngles.z), factor)
				: Quaternion.Euler(new Vector3(eulerAngles.x, Mathf.Lerp(from, to, factor), eulerAngles.z));
		}

		/// <summary>
		/// Start the tweening operation.
		/// </summary>

		static public TweenRotationY Begin(GameObject go, float duration, Quaternion rot)
		{
			TweenRotationY comp = UITweener.Begin<TweenRotationY>(go, duration);
			comp.from = comp.value.eulerAngles.y;
			comp.to = rot.eulerAngles.y;

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
			value = Quaternion.Euler(eulerAngles.x, from, eulerAngles.z);
		}

		[ContextMenu("Assume value of 'To'")]
		void SetCurrentValueToEnd()
		{
			value = Quaternion.Euler(eulerAngles.x, to, eulerAngles.z);
		}
	}
}