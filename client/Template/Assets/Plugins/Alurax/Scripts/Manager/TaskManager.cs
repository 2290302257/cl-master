using UnityEngine;
using System.Collections.Generic;
using System;

namespace Alurax
{
    public sealed class TaskManager
    {
        static private int s_UniqueId;
        static private List<Task> s_TaskList;
        static private Dictionary<int, Task> s_TaskHash;
        static private Dictionary<Action, Task> s_TaskDict;
        static private List<Task> s_TaskUpdateAdd;
        static private List<Task> s_TaskUpdateRemove;
        static private bool s_IsUpdating = false;

        private class Task
        {
            public int id;
            public int repeat;
            public float interval;
            public float nextExecTime;
            public int nextExecFrame;
            public bool ignoreTimeScale;
            public bool destroyed;
            public Action action;

            public Task(System.Action action, float interval, int repeat, float nextExecTime, bool ignoreTimeScale)
            {
                this.id = ++s_UniqueId;
                this.action = action;
                this.repeat = repeat;
                this.interval = interval;
                this.nextExecTime = nextExecTime;
                this.ignoreTimeScale = ignoreTimeScale;
                this.nextExecFrame = 0;
                this.destroyed = false;
            }

            public Task(System.Action action, int nextFrame)
            {
                this.id = ++s_UniqueId;
                this.action = action;
                this.nextExecFrame = nextFrame;
                this.nextExecTime = 0f;
                this.destroyed = false;
            }
        }

        static TaskManager()
        {
            s_TaskList = new List<TaskManager.Task>();
            s_TaskHash = new Dictionary<int, Task>();
            s_TaskDict = new Dictionary<System.Action, Task>();
            s_TaskUpdateAdd = new List<Task>();
            s_TaskUpdateRemove = new List<Task>();
#if UNITY_EDITOR
            if (!Application.isPlaying)
                return;
#endif
            Alurax.Inst.gameObject.AddComponent<InternalTask>();
        }
        
        static float GetTime(bool ignoreTimeScale)
        {
            if (ignoreTimeScale)
                return Time.unscaledTime;
            return (Time.time);
        }

        static public int Update(System.Action action)
        {
            return Add(action, 0, -1, true);
        }

        static public int Interval(System.Action action, float interval)
        {
            return Add(action, interval, 0, true);
        }

        static public int Interval(System.Action action, float interval, bool ignoreTimeScale)
        {
            return Add(action, interval, 0, ignoreTimeScale);
        }

        static public int Interval(System.Action action, float interval, int repeatCount)
        {
            return Add(action, interval, repeatCount, true);
        }

        static public int Interval(System.Action action, float interval, int repeatCount, bool ignoreTimeScale)
        {
            return Add(action, interval, repeatCount, ignoreTimeScale);
        }

        static public int Timeout(System.Action action)
        {
            return Add(action, 0, 1, true);
        }

        static public int Timeout(System.Action action, bool ignoreTimeScale)
        {
            return Add(action, 0, 1, ignoreTimeScale);
        }

        static public int Timeout(System.Action action, float time)
        {
            return Add(action, time, 1, true);
        }

        static public int Timeout(System.Action action, float time, bool ignoreTimeScale)
        {
            return Add(action, time, 1, ignoreTimeScale);
        }

        static public int NextFrame(System.Action action)
        {
            return Add(action, 1);
        }

        static public int NextFrame(System.Action action, int frameCount)
        {
            return Add(action, frameCount);
        }

        static public Action GetTaskAction(int taskId)
        {
            Task task;
            if (s_TaskHash.TryGetValue(taskId, out task))
            {
                return task.action;
            }

            return null;
        }

        static public bool Remove(int taskId)
        {
            Task task;
            if (s_TaskHash.TryGetValue(taskId, out task))
            {
                task.destroyed = true;
                s_TaskDict.Remove(task.action);
                s_TaskHash.Remove(taskId);
                if (s_IsUpdating)
                    s_TaskUpdateRemove.Remove(task);
                else
                    s_TaskList.Remove(task);
                return true;
            }

            return false;
        }

        static public bool Remove(System.Action action)
        {
            TaskManager.Task task;
            if (action != null && s_TaskDict.TryGetValue(action, out task))
            {
                task.destroyed = true;
                s_TaskDict.Remove(action);
                s_TaskHash.Remove(task.id);
                if (s_IsUpdating)
                    s_TaskUpdateRemove.Remove(task);
                else
                    s_TaskList.Remove(task);
                return true;
            }

            return false;
        }

        static private int Add(System.Action action, float interval, int repeatCount, bool ignoreTimeScale)
        {
            return Add(true, action, interval, repeatCount, ignoreTimeScale, 0);
        }

        static private int Add(System.Action action, int nextFrame)
        {
            return Add(false, action, 0f, 0, false, nextFrame);
        }

        static private int Add(bool delayTimer, System.Action action, float interval, int repeatCount, bool ignoreTimeScale, int nextFrame)
        {
            TaskManager.Task task;
            if (s_TaskDict.TryGetValue(action, out task))
            {
                return task.id;
            }

            if (delayTimer)
                task = new TaskManager.Task(action, interval, repeatCount, GetTime(ignoreTimeScale) + interval, ignoreTimeScale);
            else
                task = new TaskManager.Task(action, Time.frameCount + Math.Max(0, nextFrame));
            
            s_TaskHash.Add(task.id, task);
            s_TaskDict.Add(task.action, task);
            if (s_IsUpdating)
                s_TaskUpdateAdd.Add(task);
            else
                s_TaskList.Add(task);
            return task.id;
        }


        #region TaskManager MonoBehaviour

        class InternalTask : MonoBehaviour
        {
            void Update()
            {
                s_IsUpdating = true;

                int index = 0, count = s_TaskList.Count;
                for (; index < count; index++)
                {
                    TaskManager.Task task = s_TaskList[index];
                    if (task.destroyed)
                    {
                        s_TaskUpdateRemove.Add(task);
                        continue;
                    }

                    if (task.repeat == -1) //update
                    {
                        task.action();
                    }
                    else if (task.nextExecTime > 0f && task.nextExecFrame == 0) //next timer
                    {
                        float time = GetTime(task.ignoreTimeScale);
                        if (time >= task.nextExecTime)
                        {
                            if (task.repeat > 0 && --task.repeat == 0)
                            {
                                s_TaskUpdateRemove.Add(task);
                                s_TaskHash.Remove(task.id);
                                s_TaskDict.Remove(task.action);
                                task.destroyed = true;
                            }

                            task.action();
                            if (!task.destroyed)
                            {
                                task.nextExecTime = time + task.interval;
                            }
                        }
                    }
                    else if (task.nextExecTime == 0f && task.nextExecFrame > 0) //next frame
                    {
                        if (Time.frameCount == task.nextExecFrame)
                        {
                            s_TaskUpdateRemove.Add(task);
                            s_TaskHash.Remove(task.id);
                            s_TaskDict.Remove(task.action);
                            task.destroyed = true;
                            task.action();
                        }
                    }
                }

                s_IsUpdating = false;
                if (s_TaskUpdateAdd.Count > 0)
                {
                    foreach (var add in s_TaskUpdateAdd)
                    {
                        s_TaskList.Add(add);
                    }

                    s_TaskUpdateAdd.Clear();
                }

                if (s_TaskUpdateRemove.Count > 0)
                {
                    foreach (var remove in s_TaskUpdateRemove)
                    {
                        s_TaskList.Remove(remove);
                    }

                    s_TaskUpdateRemove.Clear();
                }
            }
        }

        #endregion

        #region TaskManager TimerClass

        public class Timer
        {
            private HashSet<System.Action> m_Task = new HashSet<System.Action>();

            public int Update(System.Action action)
            {
                m_Task.Add(action);
                return TaskManager.Update(action);
            }

            public int Interval(System.Action action, float interval)
            {
                m_Task.Add(action);
                return TaskManager.Interval(action, interval);
            }

            public int Interval(System.Action action, float interval, bool ignoreTimeScale)
            {
                m_Task.Add(action);
                return TaskManager.Interval(action, interval, ignoreTimeScale);
            }

            public int Interval(System.Action action, float interval, int repeatCount)
            {
                m_Task.Add(action);
                return TaskManager.Interval(action, interval, repeatCount);
            }

            public int Interval(System.Action action, float interval, int repeatCount, bool ignoreTimeScale)
            {
                m_Task.Add(action);
                return TaskManager.Interval(action, interval, repeatCount, ignoreTimeScale);
            }

            public int Timeout(System.Action action)
            {
                m_Task.Add(action);
                return TaskManager.Timeout(action);
            }

            public int Timeout(System.Action action, bool ignoreTimeScale)
            {
                m_Task.Add(action);
                return TaskManager.Timeout(action, ignoreTimeScale);
            }

            public int Timeout(System.Action action, float time)
            {
                m_Task.Add(action);
                return TaskManager.Timeout(action, time);
            }

            public int Timeout(System.Action action, float time, bool ignoreTimeScale)
            {
                m_Task.Add(action);
                return TaskManager.Timeout(action, time, ignoreTimeScale);
            }

            public int NextFrame(System.Action action)
            {
                m_Task.Add(action);
                return TaskManager.NextFrame(action);
            }

            public int NextFrame(System.Action action, int frameCount)
            {
                m_Task.Add(action);
                return TaskManager.NextFrame(action, frameCount);
            }

            public bool Remove(System.Action action)
            {
                m_Task.Remove(action);
                return TaskManager.Remove(action);
            }

            public bool Remove(int taskId)
            {
                var action = GetTaskAction(taskId);
                return Remove(action);
            }

            public void Clear()
            {
                foreach (var task in m_Task)
                {
                    TaskManager.Remove(task);
                }

                m_Task.Clear();
            }

            #endregion
        }
    }
}