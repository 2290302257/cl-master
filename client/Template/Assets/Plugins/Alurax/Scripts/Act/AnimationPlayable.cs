using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;
using System;

namespace Alurax
{
    public class AnimationPlayable : PlayableBehaviour
    {
        public List<AnimationLayer> Layers { get; private set; }
        
        private PlayableGraph graph => _playableSelf.GetGraph();
        private Playable _playableSelf;
        
        private AnimationLayerMixerPlayable _layerMixerPlayable;

        #region PlayableBehaviour
        public override void OnPlayableCreate(Playable playable)
        {
            _playableSelf = playable;
            _playableSelf.SetInputCount(1);
            _playableSelf.SetInputWeight(0, 1);
            
            _layerMixerPlayable = AnimationLayerMixerPlayable.Create(graph, 1);            
            graph.Connect(_layerMixerPlayable, 0, _playableSelf, 0);
        }
        
        public override void OnGraphStop(Playable playable)
        {
            if (!_playableSelf.IsValid())
                return;

            foreach (var layer in Layers)
            {
                for (int i = 0; i < layer.Count; i++)
                {
                    CustomAnimationState state = layer[i];
                    if (state == null)
                        continue;

                    state.SetTime(0);
                    state.ForceWeight(0);
                }
            }
        }

        public override void PrepareFrame(Playable owner, FrameData data)
        {
            UpdateQueuedStates();

            foreach (var layer in Layers)
            {
                if (layer.Fading)
                {
                    layer.SetWeight(Mathf.MoveTowards(layer.Weight, layer.TargetWeight, layer.FadeSpeed * data.deltaTime));
                    if (Mathf.Approximately(layer.Weight, layer.TargetWeight))
                    {
                        layer.ForceWeight(layer.TargetWeight);
                        if (layer.Weight == 0)
                        {
                            layer.Stop();
                        }
                    }
                }

                if (layer.DirtyWeight)
                {
                    _layerMixerPlayable.SetInputWeight(layer.Index, layer.Weight);
                    layer.ResetDirtyFlags();
                }
                
                UpdateLayerStates(layer, data.deltaTime);
            }
        }
        
        private void UpdateLayerStates(AnimationLayer layer, float deltaTime)
        {
            bool mustUpdateWeights = false;
            
            float totalWeight = 0f;
            for (int i = 0; i < layer.Count; i++)
            {
                CustomAnimationState state = layer[i];
                if (state == null)
                    continue;
                
                if (state.Fading) // 更新融合权重
                {
                    state.SetWeight(Mathf.MoveTowards(state.Weight, state.TargetWeight, state.FadeSpeed * deltaTime));
                    if (Mathf.Approximately(state.Weight, state.TargetWeight))
                    {
                        state.ForceWeight(state.TargetWeight);
                        if (state.Weight == 0f)
                        {
                            state.Stop();
                        }
                    }
                }
                
                totalWeight += state.Weight;
                if (state.DirtyWeight)
                {
                    mustUpdateWeights = true;
                    state.ResetDirtyFlags();
                }
            }

            // 刷新输入权重
            if (mustUpdateWeights)
            {
                bool hasAnyWeight = totalWeight > 0.0f;
                for (int i = 0; i < layer.Count; i++)
                {
                    CustomAnimationState state = layer[i];
                    if (state == null)
                        continue;

                    float weight = hasAnyWeight ? state.Weight / totalWeight : 0.0f;
                    layer.MixerPlayable.SetInputWeight(state.Index, weight);
                }
            }
        }

        private void UpdateQueuedStates()
        {
            foreach (var layer in Layers)
            {
                if (layer == null)
                    return;

                bool mustCalculateQueueTimes = true;
                float remainingTime = -1f;
       
                 var it = layer.StateQueue.First;
                 while(it != null)
                 {
                     if (mustCalculateQueueTimes)
                     {
                         remainingTime = layer.CalculateRemainingTime();
                         mustCalculateQueueTimes = false;
                     }
     
                     var queuedState = it.Value;
                     var itNext = it.Next;
                     
                     if (queuedState.FadeTime >= remainingTime)
                     {
                         layer.CrossFade(queuedState.State.Index, queuedState.FadeTime);
                         layer.StateQueue.Remove(it);
                         
                         mustCalculateQueueTimes = true;
                     }
                     
                     it = itNext;
                 }               
            }
        }
        #endregion
        
        public AnimationPlayable()
        {
            Layers = new List<AnimationLayer>();
        }
        
        public Playable GetPlayable()
        {
            return _playableSelf;
        }
        
        public void StopAll()
        {
            foreach (var layer in Layers)
            {
                if (layer == null)
                    continue;
                
                layer.Stop();
            }
            
            _playableSelf.SetDone(true);
        }
        
        public void RewindAll()
        {
            foreach (var layer in Layers)
            {
                if (layer == null)
                    continue;
                
                foreach (var state in layer.States)
                {
                    state?.SetTime(0);
                }
            }
        }
        
        private AnimationLayer InsertLayer()
        {
            AnimationLayer layer = new AnimationLayer();
            
            int availableIdx = Layers.FindIndex(l => l == null);
            if (availableIdx == -1)
            {
                availableIdx = Layers.Count;
                Layers.Add(layer);
            }
            else
            {
                Layers.Insert(availableIdx, layer);
            }

            layer.Index = availableIdx;
            return layer;
        }
        
        public AnimationLayer AddLayer(string name)
        {
            int foundIdx = Layers.FindIndex(l => l != null && l.Name == name);
            if (foundIdx != -1)
            {
                Debug.LogError(string.Format("Cannot add layer with name {0}, because a layer with that name already exists", name));
                return null;
            }

            AnimationLayer newLayer = InsertLayer();
            int index = newLayer.Index;

            if (index == _layerMixerPlayable.GetInputCount())
            {
                _layerMixerPlayable.SetInputCount(index + 1);
            }

            newLayer.Initialize(name, graph);
            graph.Connect(newLayer.MixerPlayable, 0, _layerMixerPlayable, newLayer.Index);
            
            _layerMixerPlayable.SetInputWeight(newLayer.Index, 1.0f);
            _layerMixerPlayable.SetLayerAdditive((uint)newLayer.Index, false);

            return newLayer;
        }

        public bool RemoveLayer(string name)
        {
            int index = Layers.FindIndex(l => l != null && l.Name == name);
            if (index == -1)
            {
                Debug.LogError(string.Format("Cannot remove layer with name {0}, because a layer with that name not exist", name));
                return false;
            }
            
            Layers[index].DestroyPlayable();
            Layers[index] = null;
            
            return true;
        }
        
        public AnimationLayer GetLayer(int layerIdx, bool isTrying = false)
        {
            if (layerIdx < 0 || layerIdx >= Layers.Count)
            {
                if (!isTrying)
                    Debug.LogError($"invalid layerIdx:{layerIdx} in {Layers.Count}");
                return null;
            }

            return Layers[layerIdx];
        }

        public bool SetLayerMask(AvatarMask mask, int layerIdx)
        {
            var layer = GetLayer(layerIdx);
            if (layer != null && mask != null)
            {
                _layerMixerPlayable.SetLayerMaskFromAvatarMask((uint)layer.Index, mask);
            }
            
            return false;
        }
        
    }    
}
