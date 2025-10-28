using System;
using UnityEngine;
using UnityEngine.UI;

namespace Alurax
{
    [AddComponentMenu("UI/UIToggle", 30)]
    [RequireComponent(typeof(RectTransform))]
    public class UIToggle : Toggle
    {
        public GameObject m_On;
        public GameObject m_Off;

        protected override void Awake()
        {
            base.Awake();

            transition = Transition.None;
            this.onValueChanged.AddListener(OnValueChanged);
        }

    
        private void OnValueChanged(bool on)
        {
            if (transition == Transition.None)
            {
                m_On?.SetActive(on);
                m_Off?.SetActive(!on);
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            this.onValueChanged.RemoveListener(OnValueChanged);
        }
        
#if UNITY_EDITOR

        protected override void OnValidate()
        {
            base.OnValidate();
            
            OnValueChanged(isOn);
        }

#endif
    }
}
