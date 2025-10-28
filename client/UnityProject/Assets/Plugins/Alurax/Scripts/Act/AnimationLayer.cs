using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace Alurax
{
    public class AnimationLayer
    { 
        public struct QueuedState
        {
            public static uint sIdCounter = 0;
            public QueuedState(CustomAnimationState s, float t)
            {
                State = s;
                FadeTime = t;
            }

            public CustomAnimationState State;
            public float FadeTime;
        }
        
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
        protected PlayableGraph graph { get; private set; }
        #endregion
        
        #region Fading
        public bool Fading { get; private set; }
        public float FadeSpeed { get; private set; }
        public float TargetWeight { get; private set; }
        public float Weight { get; private set; }
        public bool DirtyWeight { get; private set; }        
        #endregion
        
        public CustomAnimationState this[int i] => States[i];
        public List<CustomAnimationState> States { get; private set; }
        public LinkedList<QueuedState> StateQueue { get; private set; }
        public AnimationMixerPlayable MixerPlayable { get; private set; }
        public int Count => States.Count;
        
        public AnimationLayer()
        {
            States = new List<CustomAnimationState>();
            StateQueue = new LinkedList<QueuedState>();
        }
        
        public void Initialize(string name, PlayableGraph g)
        {
            Name = name;
            
            graph = g;
            MixerPlayable = AnimationMixerPlayable.Create(graph, 1, true);            
        }
        
        public bool AnyStatePlaying()
        {
            return States.FindIndex(s => s != null && s.IsPlaying) != -1;
        }
        
        public bool Play(int index)
        {
            bool ok = false;
            
            for (int i = 0; i < States.Count; i++)
            {
                CustomAnimationState state = States[i];
                if (state == null)
                    continue;
                
                if (state.Index == index)
                {
                    state.Play();
                    ok = true;
                }
                else
                {
                    state.Stop();
                }
            }

            return ok;
        }
        
        public bool PlayQueued(int index, float fadeTime)
        {
            CustomAnimationState original = States[index];
            if (original == null || original.Clip == null)
                return false;
            
            string cloneName = $"{original.Name} Queued {QueuedState.sIdCounter++}";
            CustomAnimationState clone = AddState(original.Clip, cloneName);
            clone.IsQueued = true;
            
            StateQueue.AddLast(new QueuedState(clone, fadeTime));
            return true;
        }

        public bool CrossFade(int index, float time)
        {
            bool ok = false;
                
            for (int i = 0; i < States.Count; i++)
            {
                CustomAnimationState state = States[i];
                if (state == null)
                    continue;

                if (state.Index == index)
                {
                    state.Play();
                    state.ForceWeight(0);
                }

                if (state.IsPlaying == false)
                    continue;

                float targetWeight = state.Index == index ? 1.0f : 0.0f;
                
                float travel = Mathf.Abs(state.Weight - targetWeight);
                float speed = time != 0f ? travel / time : Mathf.Infinity;
                
                bool onTheWay = state.Fading && 
                                Mathf.Approximately(state.TargetWeight, targetWeight) && 
                                speed < state.FadeSpeed;

                if (!onTheWay)
                {
                    state.FadeTo(targetWeight, speed);
                    ok = true;
                }
            }

            return ok;
        }

        public void Stop()
        {
            foreach (var state in States)
            {
                if (state == null)
                    continue;
                
                state.Stop();
            }
        }
        
        public CustomAnimationState AddState(AnimationClip clip, string name)
        {
            // 提前检查
            if (clip == null || string.IsNullOrEmpty(name))
            {
                Debug.LogError($"Cannot add state with name {name}, because clip is {clip}");
                return null;
            }

            CustomAnimationState state = FindState(name);
            if (state != null)
            {
                Debug.LogError($"Cannot add state with name {name}, because a state with that name already exists");
                return state;
            }
            
            // 创建Playable
            var clipPlayable = AnimationClipPlayable.Create(graph, clip);
            clipPlayable.SetApplyFootIK(false);
            clipPlayable.SetApplyPlayableIK(false);
            if (!clip.isLooping)
            {
                clipPlayable.SetDuration(clip.length);
            }
            
            // 初始化和连接
            CustomAnimationState newState = InsertState();
            newState.Initialize(name, clip, clipPlayable);
            newState.Pause();

            int index = newState.Index;
            if (index == MixerPlayable.GetInputCount())
            {
                MixerPlayable.SetInputCount(index + 1);
            }
            graph.Connect(clipPlayable, 0, MixerPlayable, index);
            
            return newState;
        }
        
        private CustomAnimationState InsertState()
        {
            CustomAnimationState state = new CustomAnimationState(this);
            
            int availableIdx = States.FindIndex(s => s == null);
            if (availableIdx == -1)
            {
                availableIdx = States.Count;
                States.Add(state);
            }
            else
            {
                States.Insert(availableIdx, state);
            }

            state.Index = availableIdx;
            return state;
        }

        public void RemoveState(int index)
        {
            States[index]?.DestroyPlayable();
            States[index] = null;
        }

        public CustomAnimationState FindState(string name)
        {
            int index = States.FindIndex(s => s != null && s.Name == name);
            if (index == -1)
                return null;

            return States[index];
        }
        
        public void ResetDirtyFlags()
        {
            DirtyWeight = false;
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
            if (MixerPlayable.IsValid())
            {
                MixerPlayable.GetGraph().DestroySubgraph(MixerPlayable);
            }
        }
        
        public float CalculateRemainingTime()
        {
            float longestTime = -1f;

            foreach (var state in States)        
            {
                if (state == null || !state.IsPlaying || !state.ClipPlayable.IsValid())
                    continue;

                float remainingTime;
                
                float speed = state.Speed;
                if (speed > 0)
                {
                    remainingTime = Mathf.Max(state.GetDuration() - state.GetTime(), 0) / speed;
                }
                else if(speed < 0 )
                {
                    remainingTime = state.GetTime() / Mathf.Abs(speed);
                }
                else
                {
                    remainingTime = Mathf.Infinity;
                }

                if (remainingTime > longestTime)
                {
                    longestTime = remainingTime;
                }
            }

            return longestTime;
        }
    }
}
