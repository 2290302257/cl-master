using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Playables;
using Alurax;

namespace Alurax
{
    [RequireComponent(typeof(Animator))]
    public partial class CustomAnimation: MonoBehaviour
    {
        #region Serializable
        [Serializable]
        public class EditorState
        {
            public AnimationClip clip;
            public string name;
            public bool defaultState;
        }

        [Serializable]
        public class EditorMask
        {
            public AvatarMask mask;
            public string name;
        }
        
        [SerializeField]
        protected bool m_PlayAutomatically = true;

        [SerializeField]
        protected bool m_AnimatePhysics = false;

        [SerializeField]
        protected AnimatorCullingMode m_CullingMode = AnimatorCullingMode.CullUpdateTransforms;

        [SerializeField]
        protected WrapMode m_WrapMode;

        [SerializeField]
        protected AnimationClip m_Clip;

        [SerializeField]
        protected EditorState[] m_States;
        
        [SerializeField]
        public EditorMask[] m_Masks;
        #endregion

        #region Public
        public Animator Animator => _animator;
        public CustomAnimationState this[string name] => GetState(name, 0);
        
        public AnimationClip Clip
        {
            get { return m_Clip; }
            set
            {
                LegacyClipCheck(value);
                m_Clip = value;
            }  
        }

        public EditorState[] States
        {
            get { return m_States; }
            set
            {
                if (value != null)
                {
                    foreach (var editorState in value)
                    {
                        LegacyClipCheck(editorState.clip);
                    }                
                }
                m_States = value;
            }
        }

        public EditorMask[] Masks
        {
            get { return m_Masks; }
            set
            {
                m_Masks = value;
            }
        }
        
        private static void LegacyClipCheck(AnimationClip clip)
        {
            if (clip && clip.legacy)
            {
                throw new ArgumentException(string.Format("Legacy clip {0} cannot be used in this component. Set .legacy property to false before using this clip", clip));
            }
        }
        #endregion

        #region Private
        private const string KDefaultName = "Default";
        private Animator _animator;
        
        private PlayableGraph _graph;
        private AnimationPlayable _playable;
        #endregion

        #region Mono Lifecycle
        protected virtual void Awake()
        {
            _animator = GetComponent<Animator>();
            _graph = PlayableGraph.Create(gameObject.name);
            _graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
            
            AnimationPlayable template = new AnimationPlayable();
            var playable = ScriptPlayable<AnimationPlayable>.Create(_graph, template, 1);
            
            _playable = playable.GetBehaviour();
            
            var layer = _playable.AddLayer("__");
            layer.AddState(m_Clip, KDefaultName);

            AnimationPlayableUtilities.Play(_animator, _playable.GetPlayable(), _graph);
        }
        
        protected virtual void OnEnable()
        {
            if (m_PlayAutomatically)
            {
                Stop();
                Play();
            }
        }

        protected virtual void OnDisable()
        {
            if (_graph.IsValid())
            {
                Stop();
                _graph.Stop();
            }
        }
        
        protected void OnDestroy()
        {
            if (_graph.IsValid())
            {
                _graph.Destroy();
            }
        }

        private void OnPlayableDone()
        {
            _graph.Stop();
        }
        #endregion
        
        protected void Kick()
        {
            if (!_animator.enabled)
            {
                _animator.enabled = true;
            }
            
            if (_graph.IsValid() && !_graph.IsPlaying())
            {
                _graph.Play();
            }
        }
        
        public bool Play()
        {
            if (m_Clip != null && m_PlayAutomatically)
            {
                Kick();
                Play(KDefaultName);

                return true;
            }
            
            return false;
        }
        
        public bool Play(string stateName, int layerIdx = 0)
        {
            var layer = _playable.GetLayer(layerIdx);
            var state = FindOrAddState(stateName, layer);
            
            if (state != null)
            {
                Kick();
                return layer.Play(state.Index);
            }
            else
            {
                Debug.LogError($"Cannot Play to state with name {stateName} layer {layerIdx} because there is no state with that name");
            }

            return false;
        }

        public bool PlayerQueued(string stateName, int layerIdx = 0)
        {
            var layer = _playable.GetLayer(layerIdx);
            var state = FindOrAddState(stateName, layer);
            
            if (state != null)
            {
                Kick();
                return layer.PlayQueued(state.Index, 0);
            }
            else
            {
                Debug.LogError($"Cannot PlayerQueued to state with name {stateName} layer {layerIdx} because there is no state with that name");
            }

            return false;
        }
        
        public bool CrossFade(string stateName, float time, int layerIdx = 0)
        {
            var layer = _playable.GetLayer(layerIdx);
            var state = FindOrAddState(stateName, layer);
            
            if (state != null)
            {
                Kick();
                
                if (time == 0)
                    return layer.Play(state.Index);
                else
                    return layer.CrossFade(state.Index, time);
            }
            else
            {
                Debug.LogError($"Cannot CrossFade to state with name {stateName} layer {layerIdx} because there is no state with that name");
            }
            return false;
        }

        public bool CrossFadeQueued(string stateName, float time, int layerIdx = 0)
        {
            var layer = _playable.GetLayer(layerIdx);
            var state = FindOrAddState(stateName, layer);
            
            if (state != null)
            {
                Kick();
                return layer.PlayQueued(state.Index, time);
            }
            else
            {
                Debug.LogError($"Cannot CrossFadeQueued to state with name {stateName} layer {layerIdx} because there is no state with that name");
            }

            return false;
        }
        
        public void Rewind()
        {
            _playable.RewindAll();
        }

        public void Rewind(string stateName, int layerIdx)
        {
            var state = GetState(stateName, layerIdx);
            if (state != null)
            {
                state.SetTime(0);
            }
        }
        
        public void Stop()
        {
            _playable?.StopAll();
        }

        public void Stop(string stateName, int layerIdx = 0)
        {
            var layer = _playable.GetLayer(layerIdx);
            var state = layer?.FindState(stateName);
            
            state?.Stop();
        }

        public List<CustomAnimationState> GetCurrentAnimationStateInfo(int layerIdx = 0)
        {
            List<CustomAnimationState> currentStates = new List<CustomAnimationState>();
            
            var layer = _playable.GetLayer(layerIdx);
            foreach (var state in layer.States)
            {
                if (state != null && state.IsPlaying)
                {
                    currentStates.Add(state);
                }
            }

            return currentStates;
        }

        public int GetLayerCount()
        {
            return _playable.Layers.Count;
        }

        public AnimationLayer TryGetLayer(int layerIdx)
        {
            return _playable.GetLayer(layerIdx, true);
        }

        public AnimationLayer GetLayer(string layerName)
        {
            foreach (var layer in _playable.Layers)
            {
                if (layer != null && layer.Name == layerName)
                {
                    return layer;
                }
            }

            return null;
        }
        
        public AnimationLayer AddLayer(string layerName)
        {
            return _playable.AddLayer(layerName);
        }

        public bool RemoveLayer(string layerName)
        {
            return _playable.RemoveLayer(layerName);
        }

        public bool SetLayerWeight(float weight, int layerIdx, float fadeTime = 0)
        {
            var layer = _playable.GetLayer(layerIdx);
            if (layer == null)
                return false;

            if (fadeTime == 0)
            {
                layer.ForceWeight(weight);
            }
            else
            {
                float travel = Mathf.Abs(layer.Weight - weight);
                float speed = travel / fadeTime;

                bool onTheWay = layer.Fading &&
                                Mathf.Approximately(layer.TargetWeight, weight) &&
                                speed < layer.FadeSpeed;

                if (onTheWay)
                    return false;
                
                layer.FadeTo(weight, speed);
            }
            
            return true;
        }

        public float GetLayerWeight(int layerIdx)
        {
            var layer = _playable.GetLayer(layerIdx);
            if (layer != null)
            {
                return layer.Weight;
            }

            return 0;
        }
        
        public bool SetLayerMask(string maskName, int layerIdx = 0)
        {
            AvatarMask mask = null;
            foreach (var editorMask in Masks)
            {
                if (editorMask.name == maskName)
                {
                    mask = editorMask.mask;
                    break;
                }
            }
            
            if (mask == null)
            {
                Debug.LogError($"Cannot find editor mask with name {maskName}.");
                return false;
            }

            return _playable.SetLayerMask(mask, layerIdx);
        }
        
        public CustomAnimationState GetState(string name, int layerIdx = 0)
        {
            var layer = _playable.GetLayer(layerIdx);
            return layer?.FindState(name);
        }
        
        public bool RemoveState(string name, int layerIdx = 0)
        {
            var layer = _playable.GetLayer(layerIdx);
            var state = layer?.FindState(name);

            if (state != null)
            {
                layer.RemoveState(state.Index);
                return true;
            }

            return false;
        }
        
        private CustomAnimationState FindOrAddState(string stateName, AnimationLayer layer)
        {
            if (layer == null)
                return null;
            
            var state = layer.FindState(stateName);
            if (state == null)
            {
                var editorState = FindEditorState(stateName);
                if (editorState != null)
                {
                    state = layer.AddState(editorState.clip, stateName);
                }
            }

            return state;
        }
        
        public EditorState FindEditorState(string stateName)
        {
            foreach (var editorState in States)
            {
                if (editorState.name == stateName)
                {
                    return editorState;
                }
            }

            Debug.LogError($"Cannot find editor state with name {stateName}.");
            return null;
        }
        

    }    
}

