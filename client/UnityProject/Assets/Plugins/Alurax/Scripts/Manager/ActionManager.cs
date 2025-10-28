using System;
using System.Collections.Generic;

namespace Alurax
{
    public class ActionManager 
    {
        Dictionary<object, List<object>> m_Actions = new Dictionary<object, List<object>>();

        private void Add(object wkAction,object action)
        {
            if (!m_Actions.TryGetValue(wkAction, out var __))
                m_Actions[wkAction] = new List<object>();
       
            m_Actions[wkAction].Add(action);
        }
        
        public CustomAction RegAction(CustomAction CustomAction , Action action)
        {
            CustomAction += action;
            Add(CustomAction,action);
            return CustomAction;
        }
        public CustomAction<T1> RegAction<T1>(CustomAction<T1> CustomAction, Action<T1> action)
        {
            CustomAction += action;
            Add(CustomAction,action);
            return CustomAction;
        }
        
        public CustomAction<T1,T2> RegAction<T1,T2>(CustomAction<T1,T2> CustomAction, Action<T1,T2> action)
        {
            CustomAction += action;
            Add(CustomAction,action);
            return CustomAction;
        }
        
        public CustomAction<T1,T2,T3> RegAction<T1,T2,T3>(CustomAction<T1,T2,T3> CustomAction, Action<T1,T2,T3> action)
        {
            CustomAction += action;
            Add(CustomAction,action);
            return CustomAction;
        }
        public CustomAction<T1,T2,T3,T4> RegAction<T1,T2,T3,T4>(CustomAction<T1,T2,T3,T4> CustomAction, Action<T1,T2,T3,T4> action)
        {
            CustomAction += action;
            Add(CustomAction,action);
            return CustomAction;
        }
        public CustomAction<T1,T2,T3,T4,T5> RegAction<T1,T2,T3,T4,T5>(CustomAction<T1,T2,T3,T4,T5> CustomAction, System.Action<T1,T2,T3,T4,T5> action)
        {
            CustomAction += action;
            Add(CustomAction,action);
            return CustomAction;
        }
        public CustomAction<T1,T2,T3,T4,T5,T6> RegAction<T1,T2,T3,T4,T5,T6>(CustomAction<T1,T2,T3,T4,T5,T6> CustomAction, System.Action<T1,T2,T3,T4,T5,T6> action)
        {
            CustomAction += action;
            Add(CustomAction,action);
            return CustomAction;
        }
        
        public void UnRegAction(CustomAction CustomAction , Action action)
        {
            CustomAction -= action;
        }
        public void UnRegAction<T1>(CustomAction<T1> CustomAction, Action<T1> action)
        {
            CustomAction -= action;
        }
        
        public void UnRegAction<T1,T2>(CustomAction<T1,T2> CustomAction, Action<T1,T2> action)
        {
            CustomAction -= action;
        }
        
        public void UnRegAction<T1,T2,T3>(CustomAction<T1,T2,T3> CustomAction, Action<T1,T2,T3> action)
        {
            CustomAction -= action;
        }
        public void UnRegAction<T1,T2,T3,T4>(CustomAction<T1,T2,T3,T4> CustomAction, Action<T1,T2,T3,T4> action)
        {
            CustomAction -= action;
        }
        public void UnRegAction<T1,T2,T3,T4,T5>(CustomAction<T1,T2,T3,T4,T5> CustomAction, System.Action<T1,T2,T3,T4,T5> action)
        {
            CustomAction -= action;
        }
        public void UnRegAction<T1,T2,T3,T4,T5,T6>(CustomAction<T1,T2,T3,T4,T5,T6> CustomAction, System.Action<T1,T2,T3,T4,T5,T6> action)
        {
            CustomAction -= action;
        }
        
        public CustomFunc<T1> RegAction<T1>(CustomFunc<T1> CustomAction, Func<T1> action)
        {
            CustomAction += action;
            Add(CustomAction,action);
            return CustomAction;
        }
        
        public CustomFunc<T1,T2> RegAction<T1, T2>(CustomFunc<T1, T2> CustomAction, Func<T1, T2> action)
        {
            CustomAction += action;
            Add(CustomAction,action);
            return CustomAction;
        }
        public CustomFunc<T1,T2,T3> RegAction<T1,T2,T3>(CustomFunc<T1,T2,T3> CustomAction, Func<T1,T2,T3> action)
        {
            CustomAction += action;
            Add(CustomAction,action);
            return CustomAction;
        }
        public CustomFunc<T1,T2,T3,T4> RegAction<T1,T2,T3,T4>(CustomFunc<T1,T2,T3,T4> CustomAction, Func<T1,T2,T3,T4> action)
        {
            CustomAction += action;
            Add(CustomAction,action);
            return CustomAction;
        }
        public CustomFunc<T1,T2,T3,T4,T5> RegAction<T1,T2,T3,T4,T5>(CustomFunc<T1,T2,T3,T4,T5> CustomAction, Func<T1,T2,T3,T4,T5> action)
        {
            CustomAction += action;
            Add(CustomAction,action);
            return CustomAction;
        }
        public CustomFunc<T1,T2,T3,T4,T5,T6> RegAction<T1,T2,T3,T4,T5,T6>(CustomFunc<T1,T2,T3,T4,T5,T6> CustomAction, System.Func<T1,T2,T3,T4,T5,T6> action)
        {
            CustomAction += action;
            Add(CustomAction,action);
            return CustomAction;
        }
        public CustomFunc<T1,T2,T3,T4,T5,T6,T7> RegAction<T1,T2,T3,T4,T5,T6,T7>(CustomFunc<T1,T2,T3,T4,T5,T6,T7> CustomAction, System.Func<T1,T2,T3,T4,T5,T6,T7> action)
        {
            CustomAction += action;
            Add(CustomAction,action);
            return CustomAction;
        }
        
        public void UnRegAction<T1>(CustomFunc<T1> CustomAction, Func<T1> action)
        {
            CustomAction -= action;
        }
        
        public void UnRegAction<T1, T2>(CustomFunc<T1, T2> CustomAction, Func<T1, T2> action)
        {
            CustomAction -= action;
        }
        public void UnRegAction<T1,T2,T3>(CustomFunc<T1,T2,T3> CustomAction, Func<T1,T2,T3> action)
        {
            CustomAction -= action;
        }
        public void UnRegAction<T1,T2,T3,T4>(CustomFunc<T1,T2,T3,T4> CustomAction, Func<T1,T2,T3,T4> action)
        {
            CustomAction -= action;
        }
        public void UnRegAction<T1,T2,T3,T4,T5>(CustomFunc<T1,T2,T3,T4,T5> CustomAction, Func<T1,T2,T3,T4,T5> action)
        {
            CustomAction -= action;
        }
        public void UnRegAction<T1,T2,T3,T4,T5,T6>(CustomFunc<T1,T2,T3,T4,T5,T6> CustomAction, System.Func<T1,T2,T3,T4,T5,T6> action)
        {
            CustomAction -= action;
        }
        public void UnRegAction<T1,T2,T3,T4,T5,T6,T7>(CustomFunc<T1,T2,T3,T4,T5,T6,T7> CustomAction, System.Func<T1,T2,T3,T4,T5,T6,T7> action)
        {
            CustomAction -= action;
        }
        public void Clear()
        {
            foreach (var act in m_Actions)
            {
                var key = ((IAction)act.Key);
                var list = act.Value;
                if (list != null)
                {
                    foreach (var item in list)
                    {
                        if(item != null)
                        key.Dispose(item);  
                    }
                }
            }
            m_Actions.Clear();
        }
    }
    public interface IAction
    {
        void Dispose(object obj);
        bool IsNull();
    }
    
    public class CustomAction : IAction
    {
        internal Action action;
        public void Dispose(object obj)
        {
            if(obj is Action act)
                action -= act;
        }
        public bool IsNull()
        {
            return action == null;
        }
        public void Invoke()
        {
            action?.Invoke();
        }
        public static CustomAction operator +(CustomAction a,Action b)
        {
            a.action -= b;
            a.action += b;
            return a;
        }
        public static CustomAction operator -(CustomAction a,Action b)
        {
            a.action -= b;
            return a;
        }
    }
    public class CustomAction<T1> : IAction
    {
        Action<T1> action;
        public void Dispose(object obj)
        {
            if (obj is Action<T1> act)
                action -= act;
        }
        public bool IsNull()
        {
            return action == null;
        }
        public void Invoke(T1 t1)
        {
            action?.Invoke(t1);
        }
        public static CustomAction<T1> operator +(CustomAction<T1> a, Action<T1> b)
        {
            a.action -= b;
            a.action += b;
            return a;
        }
        public static CustomAction<T1> operator -(CustomAction<T1> a, Action<T1> b)
        {
            a.action -= b;
            return a;
        }
    }
    
    public class CustomAction<T1,T2> : IAction
    {
        Action<T1,T2> action;
        public void Dispose(object obj)
        {
            if (obj is Action<T1,T2> act)
                action -= act;
        }
        public bool IsNull()
        {
            return action == null;
        }
        public void Invoke(T1 t1 ,T2 t2)
        {
            action?.Invoke(t1,t2);
        }
        public static CustomAction<T1,T2> operator +(CustomAction<T1,T2> a, Action<T1,T2> b)
        {
            a.action -= b;
            a.action += b;
            return a;
        }
        public static CustomAction<T1,T2> operator -(CustomAction<T1,T2> a, Action<T1,T2> b)
        {
            a.action -= b;
            return a;
        }
    }
    
    public class CustomAction<T1,T2,T3> : IAction
    {
        Action<T1,T2,T3> action;
        public void Dispose(object obj)
        {
            if (obj is Action<T1,T2,T3> act)
                action -= act;
        }
        public bool IsNull()
        {
            return action == null;
        }
        public void Invoke(T1 t1,T2 t2,T3 t3)
        {
            action?.Invoke(t1,t2,t3);
        }
        public static CustomAction<T1,T2,T3> operator +(CustomAction<T1,T2,T3> a, Action<T1,T2,T3> b)
        {
            a.action -= b;
            a.action += b;
            return a;
        }
        public static CustomAction<T1,T2,T3> operator -(CustomAction<T1,T2,T3> a, Action<T1,T2,T3> b)
        {
            a.action -= b;
            return a;
        }
    }
    
    public class CustomAction<T1,T2,T3,T4> : IAction
    {
        Action<T1,T2,T3,T4> action;
        public void Dispose(object obj)
        {
            if (obj is Action<T1,T2,T3,T4> act)
                action -= act;
        }
        public bool IsNull()
        {
            return action == null;
        }
        public void Invoke(T1 t1,T2 t2,T3 t3,T4 t4)
        {
            action?.Invoke(t1,t2,t3,t4);
        }
        public static CustomAction<T1,T2,T3,T4> operator +(CustomAction<T1,T2,T3,T4> a, Action<T1,T2,T3,T4> b)
        {
            a.action -= b;
            a.action += b;
            return a;
        }
        public static CustomAction<T1,T2,T3,T4> operator -(CustomAction<T1,T2,T3,T4> a, Action<T1,T2,T3,T4> b)
        {
            a.action -= b;
            return a;
        }
    }
    
    public class CustomAction<T1,T2,T3,T4,T5> : IAction
    {
        System.Action<T1,T2,T3,T4,T5> action;
        public void Dispose(object obj)
        {
            if (obj is System.Action<T1,T2,T3,T4,T5> act)
                action -= act;
        }
        public bool IsNull()
        {
            return action == null;
        }
        public void Invoke(T1 t1,T2 t2,T3 t3,T4 t4,T5 t5)
        {
            action?.Invoke(t1,t2,t3,t4,t5);
        }
        public static CustomAction<T1,T2,T3,T4,T5> operator +(CustomAction<T1,T2,T3,T4,T5> a, System.Action<T1,T2,T3,T4,T5> b)
        {
            a.action -= b;
            a.action += b;
            return a;
        }
        public static CustomAction<T1,T2,T3,T4,T5> operator -(CustomAction<T1,T2,T3,T4,T5> a, System.Action<T1,T2,T3,T4,T5> b)
        {
            a.action -= b;
            return a;
        }
    }
    
    public class CustomAction<T1,T2,T3,T4,T5,T6> : IAction
    {
        System.Action<T1,T2,T3,T4,T5,T6> action;
        public void Dispose(object obj)
        {
            if (obj is System.Action<T1,T2,T3,T4,T5,T6> act)
                action -= act;
        }
        public bool IsNull()
        {
            return action == null;
        }
        public void Invoke(T1 t1,T2 t2,T3 t3,T4 t4,T5 t5,T6 t6)
        {
            action?.Invoke(t1,t2,t3,t4,t5,t6);
        }
        public static CustomAction<T1,T2,T3,T4,T5,T6> operator +(CustomAction<T1,T2,T3,T4,T5,T6> a, System.Action<T1,T2,T3,T4,T5,T6> b)
        {
            a.action -= b;
            a.action += b;
            return a;
        }
        public static CustomAction<T1,T2,T3,T4,T5,T6> operator -(CustomAction<T1,T2,T3,T4,T5,T6> a, System.Action<T1,T2,T3,T4,T5,T6> b)
        {
            a.action -= b;
            return a;
        }
    }
    
    public class CustomFunc<T1> : IAction
    {
        Func<T1> func;
        public void Dispose(object obj)
        {
            if (obj is Func<T1> act)
                func -= act;
        }
        public bool IsNull()
        {
            return func == null;
        }
        public T1 Invoke()
        {
            return func != null ? func.Invoke() : default(T1);
        }
        
        public static CustomFunc<T1> operator +(CustomFunc<T1> a, Func<T1> b)
        {
            a.func -= b;
            a.func += b;
            return a;
        }
        public static CustomFunc<T1> operator -(CustomFunc<T1> a, Func<T1> b)
        {
            a.func -= b;
            return a;
        }
    }
    public class CustomFunc<T1, T2> : IAction
    {
        Func<T1, T2> func;
        public void Dispose(object obj)
        {
            if (obj is Func<T1, T2> act)
                func -= act;
        }
        public bool IsNull()
        {
            return func == null;
        }
        public T2 Invoke(T1 t1)
        {
            return func != null ? func.Invoke(t1) : default(T2);
        }
        public static CustomFunc<T1, T2> operator +(CustomFunc<T1, T2> a, Func<T1, T2> b)
        {
            a.func -= b;
            a.func += b;
            return a;
        }
        public static CustomFunc<T1, T2> operator -(CustomFunc<T1, T2> a, Func<T1, T2> b)
        {
            a.func -= b;
            return a;
        }
     }
    
    public class CustomFunc<T1, T2, T3> : IAction
    {
        Func<T1, T2, T3> func;
        public void Dispose(object obj)
        {
            if (obj is Func<T1, T2, T3> act)
                func -= act;
        }
        public bool IsNull()
        {
            return func == null;
        }
        public T3 Invoke(T1 t1,T2 t2)
        {
            return func != null ? func.Invoke(t1,t2) : default(T3);
        }
        public static CustomFunc<T1, T2, T3> operator +(CustomFunc<T1, T2, T3> a, Func<T1, T2, T3> b)
        {
            a.func -= b;
            a.func += b;
            return a;
        }
        public static CustomFunc<T1, T2, T3> operator -(CustomFunc<T1, T2, T3> a, Func<T1, T2, T3> b)
        {
            a.func -= b;
            return a;
        }
    }
    
    public class CustomFunc<T1, T2, T3, T4> : IAction
    {
        Func<T1, T2, T3, T4> func;
        public void Dispose(object obj)
        {
            if (obj is Func<T1, T2, T3, T4> act)
                func -= act;
        }
        public bool IsNull()
        {
            return func == null;
        }
        public T4 Invoke(T1 t1, T2 t2,T3 t3)
        {
            return func != null ? func.Invoke(t1,t2,t3) : default(T4);
        }
        public static CustomFunc<T1, T2, T3, T4> operator +(CustomFunc<T1, T2, T3, T4> a, Func<T1, T2, T3, T4> b)
        {
            a.func -= b;
            a.func += b;
            return a;
        }
        public static CustomFunc<T1, T2, T3, T4> operator -(CustomFunc<T1, T2, T3, T4> a, Func<T1, T2, T3, T4> b)
        {
            a.func -= b;
            return a;
        }
    } 
    public class CustomFunc<T1, T2, T3, T4, T5> : IAction
    {
        Func<T1, T2, T3, T4, T5> func;
        public void Dispose(object obj)
        {
            if (obj is Func<T1, T2, T3, T4, T5> act)
                func -= act;
        }
        public bool IsNull()
        {
            return func == null;
        }
        public T5 Invoke(T1 t1, T2 t2,T3 t3,T4 t4)
        {
            return func != null ? func.Invoke(t1,t2,t3,t4) : default(T5);
        }
        public static CustomFunc<T1, T2, T3, T4, T5> operator +(CustomFunc<T1, T2, T3, T4, T5> a, Func<T1, T2, T3, T4, T5> b)
        {
            a.func -= b;
            a.func += b;
            return a;
        }
        public static CustomFunc<T1, T2, T3, T4, T5> operator -(CustomFunc<T1, T2, T3, T4, T5> a, Func<T1, T2, T3, T4, T5> b)
        {
            a.func -= b;
            return a;
        }
    }
    public class CustomFunc<T1, T2, T3, T4, T5, T6> : IAction
    {
        System.Func<T1, T2, T3, T4, T5, T6> func;
        public void Dispose(object obj)
        {
            if (obj is System.Func<T1, T2, T3, T4, T5, T6> act)
                func -= act;
        }
        public bool IsNull()
        {
            return func == null;
        }
        public T6 Invoke(T1 t1, T2 t2,T3 t3,T4 t4,T5 t5)
        {
            return func != null ? func.Invoke(t1,t2,t3,t4,t5) : default(T6);
        }
        public static CustomFunc<T1, T2, T3, T4, T5, T6> operator +(CustomFunc<T1, T2, T3, T4, T5, T6> a, System.Func<T1, T2, T3, T4, T5, T6> b)
        {
            a.func -= b;
            a.func += b;
            return a;
        }
        public static CustomFunc<T1, T2, T3, T4, T5, T6> operator -(CustomFunc<T1, T2, T3, T4, T5, T6> a, System.Func<T1, T2, T3, T4, T5, T6> b)
        {
            a.func -= b;
            return a;
        }
    }
    public class CustomFunc<T1, T2, T3, T4, T5, T6, T7> : IAction
    {
        System.Func<T1, T2, T3, T4, T5, T6, T7> func;
        public void Dispose(object obj)
        {
            if (obj is System.Func<T1, T2, T3, T4, T5, T6, T7> act)
                func -= act;
        }
        public bool IsNull()
        {
            return func == null;
        }
        public T7 Invoke(T1 t1, T2 t2,T3 t3,T4 t4,T5 t5,T6 t6)
        {
            return func != null ? func.Invoke(t1,t2,t3,t4,t5,t6) : default(T7);
        }
        public static CustomFunc<T1, T2, T3, T4, T5, T6, T7> operator +(CustomFunc<T1, T2, T3, T4, T5, T6, T7> a, System.Func<T1, T2, T3, T4, T5, T6, T7> b)
        {
            a.func -= b;
            a.func += b;
            return a;
        }
        public static CustomFunc<T1, T2, T3, T4, T5, T6, T7> operator -(CustomFunc<T1, T2, T3, T4, T5, T6, T7> a, System.Func<T1, T2, T3, T4, T5, T6, T7> b)
        {
            a.func -= b;
            return a;
        }
    }
}