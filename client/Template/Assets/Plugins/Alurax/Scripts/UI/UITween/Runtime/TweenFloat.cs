using System;
using UnityEngine;

namespace Alurax
{
	[AddComponentMenu("UGUI/Tween/Tween Float")]
	public class TweenFloat : UITweener
	{
		public float from = 1;
		public float to = 1;

		public float mValue;

		public Action<float> OnUpdateAction;
		
		public float value
		{
			get { return mValue; }
			set { mValue = value; }
		}

		protected override void OnUpdate(float factor, bool isFinished)
		{
			value = from * (1f - factor) + to * factor;
			OnUpdateAction?.Invoke(value);
		}

		static public TweenFloat Begin(GameObject go, float duration, float targetVolume)
		{
			TweenFloat comp = UITweener.Begin<TweenFloat>(go, duration);
			comp.from = comp.value;
			comp.to = targetVolume;
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