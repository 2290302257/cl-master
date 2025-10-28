using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;

namespace Alurax
{
    [Serializable]
    public abstract class BlackboardVariableBase
    {
        public event Action<BlackboardVariableBase> OnValueChanged;
        
        [SerializeField]
        protected string m_name;
        [SerializeField]
        protected string m_type;
        [SerializeReference]
        protected Blackboard m_blackboard;
        public string Name => m_name;

        internal void SetBlackboard(Blackboard blackboard)
        {
            m_blackboard = blackboard;
        }
        
        public string Type
        {
            get
            {
                if(string.IsNullOrEmpty(m_type))
                    m_type = this.GetType().Name;
                return m_type;
            }
        }

        public static FieldInfo InternalValueFieldInfo(Type variableType)
        {
            if (variableType.IsSubclassOf(typeof(BlackboardVariableBase)))
            {
                var valueField = variableType.BaseType?.GetField("m_value", BindingFlags.NonPublic | BindingFlags.Instance);
                return valueField;
            }
            
            return null;
        }
        
        public void InternalSetName(string name)
        {
            m_name = name;
        }
        
        protected void InternalNotifyChanged()
        {
            OnValueChanged?.Invoke(this);
        }

        public static BlackboardVariableBase Create(Type variableType)
        {
            if (!variableType.IsSubclassOf(typeof(BlackboardVariableBase)))
                return null;
            
            var ctorFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            
            var newVariable = Activator.CreateInstance(variableType, ctorFlags, (Binder)null, new object[] { null }, (CultureInfo)null) as BlackboardVariableBase; ;
            newVariable.SetValue(Activator.CreateInstance(newVariable.GetValueType()));
            
            return newVariable;
        }
        
        public abstract object GetSerializedValue();
        public abstract object GetValue();
        public abstract bool SetValue(object value);
        public abstract Type GetValueType();
        
        public abstract BlackboardVariableBase Clone();
        public abstract bool IsAssignableTo(Type valueType);
        
        public abstract int CompareToOther(BlackboardVariableBase other);
    }
    
    public abstract class BlackboardVariable<T> : BlackboardVariableBase
    {
        [SerializeField] 
        protected T m_value;

        public T Value
        {
            get
            {
                if (m_value != null) return m_value;
                return (T)GetValue();
            }
        }

        public BlackboardVariable(string name)
        {
            m_name = name;
            m_type = this.GetType().Name;
        }

        public BlackboardVariable(string name, T value)
        {
            m_name = name;
            m_type = this.GetType().Name;
            m_value = value;
        }
  
        public override object GetSerializedValue()
        {
            return GetValue();
        }

        public override object GetValue()
        {
            return m_value;
        }
        
        public override bool SetValue(object value)
        {
            if (value == null && m_value == null)
                return false;
            
            if (value != null && !typeof(T).IsAssignableFrom(value.GetType()))
            {
                Debug.LogError($"The new value type {value.GetType()} does not match the variable type {typeof(T)}.");
                return false;
            }
            
            if (m_value == null || !m_value.Equals(value))
            {
                m_value = (T)value;
                InternalNotifyChanged();
                return true;
            }

            return false;
        }

        public override Type GetValueType()
        {
            return Value?.GetType() ?? typeof(T);
        }

        public override BlackboardVariableBase Clone()
        {
            var cloneVariable = (BlackboardVariable<T>)Create(GetType());
            cloneVariable.SetValue(Value);
            
            return cloneVariable;
        }

        public override bool IsAssignableTo(Type valueType)
        {
            return valueType.IsAssignableFrom(typeof(T));
        }

        public override int CompareToOther(BlackboardVariableBase other)
        {
            if (other == null)
                return 1;

            if (other is BlackboardVariable<T> otherT)
            {
                if (Value.Equals(otherT.Value))
                {
                    return 0;
                }

                if (Value is Enum e)
                {
                    return e.CompareTo(otherT.Value as Enum);
                }
                else if (Value is bool b)
                {
                    return b.CompareTo(otherT.Value as bool?);
                }
                else if (Value is int i)
                {
                    return i.CompareTo(otherT.Value as int?);
                }
                else if (Value is uint i2)
                {
                    return i2.CompareTo(otherT.Value as uint?);
                }
                else if (Value is float f)
                {
                    return f.CompareTo(otherT.Value as float?);
                }
                else if (Value is string s)
                {
                    return s.CompareTo(otherT.Value as string);
                }
                else
                {
                    Debug.LogError($"The type {typeof(T)} is not supported.");
                }
            }
            
            Debug.LogError($"The type {typeof(T)} CompareTo type {other.GetType()}");
            return -1;
        }
    }
    
    public class BlackboardVariableBool : BlackboardVariable<bool>
    {
        public BlackboardVariableBool(string name) : base(name)
        {
        }
    }

    public class BlackboardVariableInt : BlackboardVariable<int>
    {
        public BlackboardVariableInt(string name) : base(name)
        {
        }
        
        public BlackboardVariableInt(string name, int value) : base(name, value)
        {
        }
    }
    
    public class BlackboardVariableUInt : BlackboardVariable<uint>
    {
        public BlackboardVariableUInt(string name) : base(name)
        {
        }
    }

    public class BlackboardVariableFloat : BlackboardVariable<float>
    {
        public BlackboardVariableFloat(string name) : base(name)
        {
        }
        
        public BlackboardVariableFloat(string name, float value) : base(name, value)
        {
        }
    }

    public class BlackboardVariableString : BlackboardVariable<string>
    {
        public BlackboardVariableString(string name) : base(name)
        {
        }
    }

    public class BlackboardVariableVector2 : BlackboardVariable<Vector2>
    {
        public BlackboardVariableVector2(string name) : base(name)
        {
        }
        
        public BlackboardVariableVector2(string name, Vector2 value) : base(name, value)
        {
        }
    }

    public class BlackboardVariableVector3 : BlackboardVariable<Vector3>
    {
        public BlackboardVariableVector3(string name) : base(name)
        {
        }
        
        public BlackboardVariableVector3(string name, Vector3 value) : base(name, value)
        {
        }
    }

    public class BlackboardVariableVector4 : BlackboardVariable<Vector4>
    {
        public BlackboardVariableVector4(string name) : base(name)
        {
        }
    }
    
    public class BlackboardVariableColor : BlackboardVariable<Color>
    {
        public BlackboardVariableColor(string name) : base(name)
        {
        }
    }
    
    public abstract class BlackboardVariableUnityComp<T> : BlackboardVariable<T> where T:UnityEngine.Component
    {
        public BlackboardVariableUnityComp(string name) : base(name)
        {
        }

        public override bool IsAssignableTo(Type valueType)
        {
            return valueType == typeof(string) || valueType.IsAssignableFrom(typeof(T));
        }
        
        public override bool SetValue(object value)
        {
            if (value is string path)
            {
                var transform = this.m_blackboard?.Owner as Transform;
                if (transform != null)
                {
                    var child = transform.Find(path);
                    if (child != null)
                    {
                        m_value = child.GetComponent<T>();
                        return true;
                    }
                }
            }
            else if(value is T t)
            {
                m_value = t;
                return true;
            }

            m_value = default(T);
            return false;
        }
        
        public override object GetSerializedValue()
        {
            if (m_value != null)
            {
                var transform = this.m_blackboard?.Owner as Transform;
                if (transform != null)
                { 
                    string path = transform.GetRelativePath(m_value.transform);
                    return path;
                }
            }
            
            return string.Empty;
        }
    }
    
    public class BlackboardVariableTransform : BlackboardVariableUnityComp<Transform>
    {
        public BlackboardVariableTransform(string name) : base(name)
        {
            
        }
    }
}
