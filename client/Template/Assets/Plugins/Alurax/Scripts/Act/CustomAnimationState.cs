using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Alurax
{
    public class CustomAnimationState
    {
        #region Initialize
        private int _index = -1;
        public int Index
        {
            get => _index;
            set
            {
                Debug.Assert(_index == -1, "Should never reassign Index");
                _index = value;
            }
        }
        
        public string Name { get; private set; }
        public AnimationClip Clip { get; private set; }
        public AnimationClipPlayable ClipPlayable { get; private set; }
        #endregion
        
        #region Play
        public bool IsPlaying => ClipPlayable.GetPlayState() == PlayState.Playing;
        public bool IsDone => ClipPlayable.IsDone();
        public float Speed
        {
            get => (float)ClipPlayable.GetSpeed(); 
            set => ClipPlayable.SetSpeed(value); 
        }
        #endregion
        
        #region Fading
        public bool Fading { get; private set; }
        public float FadeSpeed { get; private set; }
        public float TargetWeight { get; private set; }
        public float Weight { get; private set; }
        public bool DirtyWeight { get; private set; }        
        #endregion
        
        public bool IsQueued { get; set; }
        protected AnimationLayer OwnerLayer;
        
        public void ResetDirtyFlags()
        {
            DirtyWeight = false;
        }

        public CustomAnimationState(AnimationLayer ownerLayer)
        {
            OwnerLayer = ownerLayer;
        }
        
        public void Initialize(string name, AnimationClip clip, AnimationClipPlayable clipPlayable)
        {
            Name = name;
            Clip = clip;
            ClipPlayable = clipPlayable;
        }
        
        public void Pause()
        {
            ClipPlayable.SetPlayState(PlayState.Paused);
        }

        public void Play()
        {
            ClipPlayable.SetPlayState(PlayState.Playing);
            ForceWeight(1.0f);
        }

        public void Stop()
        {
            Pause();
            ForceWeight(0.0f);
            SetTime(0.0f);
            
            ClipPlayable.SetDone(true);

            if (IsQueued)
            {
                OwnerLayer.RemoveState(Index);
            }
        }

        public void SetDone()
        {
            ClipPlayable.SetDone(true);
        }

        public float GetDuration()
        {
            return (float)ClipPlayable.GetDuration();
        }
        
        public float GetTime()
        {
            return (float)ClipPlayable.GetTime();
        }

        public void SetTime(float newTime)
        {
            ClipPlayable.SetTime(newTime);
            ClipPlayable.SetDone(newTime >= ClipPlayable.GetDuration());
        }

        public void ForceWeight(float weight)
        {
            TargetWeight = weight;
            Fading = false;
            FadeSpeed = 0f;
            
            SetWeight(weight);
        }

        public void SetWeight(float weight)
        {
            Weight = weight;
            DirtyWeight = true;
        }

        public void FadeTo(float weight, float speed)
        {
            Fading = Mathf.Abs(speed) > 0f;
            FadeSpeed = speed;
            TargetWeight = weight;
        }

        public void DestroyPlayable()
        {
            if (ClipPlayable.IsValid())
            {
                ClipPlayable.GetGraph().DestroySubgraph(ClipPlayable);
            }
        }
        
        
    }



    
}