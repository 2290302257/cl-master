using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Alurax
{
    public class ActPlayable
    {
        public uint Id;
        public string Name;
        public GameObject Actor;
        public ActObject ActObj;

        public float BeginTime;
        public float BeginOffset;
        public Action<bool> OnFinishCallback;

        public bool IsValid => ActObj != null && Actor != null;
    }

    public class ActDirector
    {
        private static Dictionary<uint, ActPlayable> s_ActPlayables = new Dictionary<uint, ActPlayable>();
        private static Dictionary<uint, ActPlayable> s_addPlayables = new Dictionary<uint, ActPlayable>();
        private static Dictionary<uint, ActPlayable> s_remPlayables = new Dictionary<uint, ActPlayable>();
        private static bool s_isUpdating;
        private static uint s_idCounter;


        static ActDirector()
        {
            TaskManager.Update(Update);
        }
        
        private static TaskManager.Timer __OnTimer;
        protected static TaskManager.Timer m_OnTimer
        {
            get
            {
                if (__OnTimer == null) __OnTimer = new TaskManager.Timer();
                return __OnTimer;
            }
        }
        
        public static void DestroyTimeout(UnityEngine.Object obj, float timeout)
        {
            if (obj == null)
                return;
            
            m_OnTimer.Timeout(() =>
            {
                if (obj)
                {
                    Object.Destroy(obj);
                }
            }, timeout);
        }
        
        public static ActPlayable GetPlayable(uint actId)
        {
            if (s_remPlayables.ContainsKey(actId))
                return null;

            if (s_addPlayables.TryGetValue(actId, out ActPlayable addplayable))
                return addplayable;

            s_ActPlayables.TryGetValue(actId, out ActPlayable playable);
            return playable;
        }

        private static Dictionary<uint, ActPlayable> GetCurrentPlayables()
        {
            return s_isUpdating ? s_addPlayables : s_ActPlayables;
        }

        public static uint Play(string assetName, GameObject actor)
        {
            return Play(assetName, 0f, actor, null, null);
        }
        
        public static uint Play(string assetName, float startTime, GameObject actor)
        {
            return Play(assetName, startTime, actor, null, null);
        }
        
        public static uint Play(string assetName, GameObject actor, Action<bool> onFinish)
        {
            return Play(assetName,0f, actor, null, onFinish);
        }
        
        public static uint Play(string assetName, GameObject actor, Action<ActPlayable> onPlay, Action<bool> onFinish)
        {
            return Play(assetName, 0f, actor, onPlay, onFinish);
        }
        
        public static uint Play(string assetName, float startTime, GameObject actor, Action<bool> onFinish)
        {
            return Play(assetName, startTime, actor, null, onFinish);
        }
        
        public static uint Play(string assetName,float startTime, GameObject actor, Action<ActPlayable> onPlay, Action<bool> onFinish)
        {
            if (string.IsNullOrEmpty(assetName))
                return 0;
            
            var playable = new ActPlayable()
            {
                Id = ++s_idCounter,
                Name = assetName,
                Actor = actor,
                OnFinishCallback = onFinish,
                BeginOffset = Mathf.Max(0f,startTime),
            };
            GetCurrentPlayables().Add(playable.Id, playable);

            Assets.InstantiateAsync<ActObject>($"{assetName}.asset", (o =>
            {
                if (GetPlayable(playable.Id) != null)
                {
                    playable.ActObj = o;
                    playable.BeginTime = Time.time;

                    foreach (var act in o.ActList)
                    {
                        ActBase.InternalCreated(act, o, actor);
                        if (act != null && act.StartTime == 0)
                            ActBase.InternalStart(act, playable.BeginOffset);
                    }
                    onPlay?.Invoke(playable);
                }
                else
                {
                    Object.Destroy(o);
                }
            }));

            return playable.Id;
        }

        public static uint Play(ActObject actObj, GameObject actor, Action<bool> onFinish)
        {
            return Play(actObj, 0f, actor, onFinish);
        }
        
        public static uint Play(ActObject actObj, float startTime, GameObject actor, Action<bool> onFinish)
        {
            if (actObj == null)
                return 0;

            var playable = new ActPlayable()
            {
                Id = ++s_idCounter,
                Name = actObj.name,
                Actor = actor,
                ActObj = actObj,
                BeginTime = Time.time,
                BeginOffset = startTime,
                OnFinishCallback = onFinish,
            };
            GetCurrentPlayables().Add(playable.Id, playable);

            foreach (var act in actObj.ActList)
            {
                ActBase.InternalCreated(act, actObj, actor);
                if (act != null && act.StartTime == 0)
                    ActBase.InternalStart(act, playable.BeginOffset);
            }

            return playable.Id;
        }

        public static void Stop(uint actId, bool isDone = false)
        {
            var playable = GetPlayable(actId);
            if (playable == null)
                return;

            var actObj = playable.ActObj;
            if (actObj)
            {
                foreach (var act in actObj.ActList)
                {
                    if (act != null)
                    {
                        if (!act.Finished)
                        {
                            ActBase.InternalFinish(act);
                        }

                        ActBase.InternalDestroyed(act);
                    }
                }
            }

            s_remPlayables.Add(actId, playable);
            playable.OnFinishCallback?.Invoke(isDone);
        }

        static void Update()
        {
            if (s_ActPlayables.Count == 0)
                return;
            
            // 处理更新逻辑
            s_isUpdating = true;
            {
                DoUpdate();
            }
            s_isUpdating = false;

            // 处理添加逻辑
            foreach (var (id, playable) in s_addPlayables)
            {
                s_ActPlayables.Add(id, playable);
            }

            s_addPlayables.Clear();

            // 处理删除逻辑
            foreach (var (id, playable) in s_remPlayables)
            {
                s_ActPlayables.Remove(id);
            }

            s_remPlayables.Clear();
        }

        private static void DoUpdate()
        {
            foreach (var (actId, handle) in s_ActPlayables)
            {
                if (!handle.IsValid)
                    continue; // 还没有加载完跳过
                if (s_remPlayables.ContainsKey(actId))
                    continue; // 已经删掉了跳过
                
                var begin = handle.BeginTime;
                var done = begin + handle.ActObj.Duration;

                float curTime = Time.time + handle.BeginOffset;

                var actList = handle.ActObj.ActList;
                foreach (var act in actList)
                {
                    if (act != null)
                    {
                        var startTime = begin + act.StartTime;
                        var endTime = startTime + Mathf.Max(0f, act.Duration);

                        if (!act.Started && curTime >= startTime)
                        {
                            
                            ActBase.InternalStart(act, handle.BeginOffset);
                        }

                        if (!act.Finished && curTime >= endTime)
                        {
                            ActBase.InternalFinish(act);
                        }

                        ActBase.InternalUpdate(act, curTime);
                    }
                }

                if (curTime >= done)
                {
                    if (!handle.ActObj.Loop)
                    {
                        Stop(actId, true);
                    }
                    else
                    {
                        foreach (var act in actList)
                        {
                            handle.BeginTime = Time.time;
                            if (act != null)
                            {
                                ActBase.InternalReset(act);
                                if (act.StartTime == 0)
                                {
                                    ActBase.InternalStart(act, handle.BeginOffset);
                                    ActBase.InternalUpdate(act, curTime);
                                }
                            }
                        }
                    }
                }
            }
        }
        
        
    }
} // namespace Alurax