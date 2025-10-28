using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

#if UNITY_EDITOR
namespace Alurax
{
    public partial class CustomAnimation : IAnimationClipSource
    {
        public void GetAnimationClips(List<AnimationClip> results) // IAnimationClipSource
        {
            foreach (var state in m_States)
            {
                if (state.clip != null)
                    results.Add(state.clip);
            }
        }
        

        private void RebuildEditorStates()
        {
            var list = new List<EditorState>();

            var playableStates = _playable.GetLayer(0).States;
            foreach (var state in playableStates)
            {
                var newState = new EditorState();
                newState.clip = state.Clip;
                newState.name = state.Name;
                newState.defaultState = false;
                
                list.Add(newState);
            }
            
            m_States = list.ToArray();
        }

        private EditorState CreateDefaultEditorState()
        {
            var defaultState = new EditorState();
            defaultState.name = KDefaultName;
            defaultState.clip = m_Clip;
            defaultState.defaultState = true;

            return defaultState;
        }
        
        private void Reset()
        {
            if (_graph.IsValid())
                _graph.Destroy();
        }

        private void OnValidate()
        {
            if (Application.isPlaying)
                return;

            // 更新动画组件
            _animator = GetComponent<Animator>();
            _animator.updateMode = m_AnimatePhysics ? AnimatorUpdateMode.AnimatePhysics : AnimatorUpdateMode.Normal;
            _animator.cullingMode = m_CullingMode;
            
            // 触发检查
            Clip = m_Clip;
            States = m_States;
            
            // 确保默认状态符合
            if (m_States == null || m_States.Length == 0)
            {
                m_States = new EditorState[1] { CreateDefaultEditorState() };   
            }
            
            if (m_States[0].defaultState == false || m_States[0].name != KDefaultName)
            {
                var oldArray = m_States;
                m_States = new EditorState[oldArray.Length + 1];
                m_States[0] = CreateDefaultEditorState();
                oldArray.CopyTo(m_States, 1);
            }
            
            if (m_States[0].clip != m_Clip)
                m_States[0].clip = m_Clip;
            
            // 确保所有的状态并确保名字唯一
            int stateCount = m_States.Length;
            string[] names = new string[stateCount];

            for (int i = 0; i < stateCount; i++)
            {
                EditorState state = m_States[i];
                if (state == null)
                {
                    state = new EditorState();
                    m_States[i] = state;
                }
                
                if (string.IsNullOrEmpty(state.name) && state.clip)
                {
                    state.name = state.clip.name;
                }
                
                state.defaultState = (i == 0);
                state.name = UnityEditor.ObjectNames.GetUniqueName(names, state.name);
                names[i] = state.name;

                if (state.clip && state.clip.legacy)
                {
                    Debug.LogErrorFormat(this.gameObject, "Animation clip {0} in state {1} is Legacy. Set clip.legacy to false, or reimport as Generic to use it with SimpleAnimationComponent", state.clip.name, state.name);
                    state.clip = null;
                }
            }
        }
    

        
        
    }    
}

#endif // UNITY_EDITOR

