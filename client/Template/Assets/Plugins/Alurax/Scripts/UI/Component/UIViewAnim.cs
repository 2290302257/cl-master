using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Alurax
{
    public class UIViewAnim : MonoBehaviour,IExportProcess
    {
        public enum Type
        {
            Opening,
            Closing,
        }
        public Type type = Type.Opening;

        private int m_TimeIndex = 0;
        public virtual void Play(Action finish)
        {
            UITweener[] tweeners = gameObject.GetComponentsInChildren<UITweener>();
            Animation aniamtion = gameObject.GetComponent<Animation>();
            int count = tweeners.Length +(aniamtion!=null ?1:0);
            foreach (var tween in tweeners)
            {
                tween.ResetToBeginning();
                tween.PlayForward();
                tween.SetOnFinished(() =>
                {
                    if (--count == 0)
                        finish?.Invoke();   
                });
            }

            if (aniamtion)
            {
                var clip = aniamtion.clip;
                var length = clip != null ? clip.length : 0;
                aniamtion.Play();
                m_TimeIndex = TaskManager.Timeout(() =>
                {
                    if (--count == 0)
                        finish?.Invoke();   
                }, length);
            }
            
        }
        
        private void CleanTimerAction()
        {
            if (m_TimeIndex > 0)
            {
                TaskManager.Remove(m_TimeIndex);
                m_TimeIndex = 0;
            } 
        }
        
        void OnDestroy()
        {
            CleanTimerAction();
        }

        public void Process()
        {
#if UNITY_EDITOR
            Animation aniamtion = gameObject.GetComponent<Animation>();
            if (aniamtion)
                aniamtion.playAutomatically = false;
#endif
        }
    }
    


}