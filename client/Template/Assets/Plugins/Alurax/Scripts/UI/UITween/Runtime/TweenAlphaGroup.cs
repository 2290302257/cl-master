using UnityEngine;
using UnityEngine.UI;

namespace Alurax
{
	/// <summary>
	/// Tween the object's alpha. Works with both UI widgets as well as renderers.
	/// </summary>

	[AddComponentMenu("UGUI/Tween/Tween Alpha Group")]
	public class TweenAlphaGroup : UITweener
	{
		[Range(0f, 1f)] public float from = 1f;
		[Range(0f, 1f)] public float to = 1f;

		bool mCached = false;
		CanvasGroup mSr;

		[System.Obsolete("Use 'value' instead")]
		public float alpha
		{
			get { return this.value; }
			set { this.value = value; }
		}

		void Cache()
		{
			mCached = true;
			mSr = GetComponent<CanvasGroup>();
		}

		/// <summary>
		/// Tween's current value.
		/// </summary>

		public float value
		{
			get
			{
				if (!mCached) Cache();
				return mSr != null ? mSr.alpha : 1f;
			}
			set
			{
				if (!mCached) Cache();

				else if (mSr != null)
				{
					float c = mSr.alpha;
					c = value;
					mSr.alpha = c;
				}
			}
		}

		/// <summary>
		/// Tween the value.
		/// </summary>

		protected override void OnUpdate(float factor, bool isFinished)
		{
			value = Mathf.Lerp(from, to, factor);
		}

		/// <summary>
		/// Start the tweening operation.
		/// </summary>

		static public TweenAlpha Begin(GameObject go, float duration, float alpha, float delay = 0f)
		{
			TweenAlpha comp = UITweener.Begin<TweenAlpha>(go, duration, delay);
			comp.from = comp.value;
			comp.to = alpha;

			if (duration <= 0f)
			{
				comp.Sample(1f, true);
				comp.enabled = false;
			}

			return comp;
		}

		public override void SetStartToCurrentValue()
		{
			from = value;
		}

		public override void SetEndToCurrentValue()
		{
			to = value;
		}
	}
}