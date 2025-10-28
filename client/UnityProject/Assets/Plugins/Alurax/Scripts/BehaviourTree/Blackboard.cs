using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Serialization;
using Object = System.Object;

namespace Alurax
{
    [Serializable]
    public class Blackboard
    {
        [Serializable]
        protected class SerializableNameToVariableMap : SerializableDictionary<string, BlackboardVariableBase> { }
        
        [SerializeReference][HideInInspector]
        protected SerializableNameToVariableMap VariableMap;

        public List<BlackboardVariableBase> Variables => VariableMap.Values;
        
        public event Action<BlackboardVariableBase> OnVariableAdded;
        public event Action<BlackboardVariableBase> OnVariableRemoved;
        public event Action<BlackboardVariableBase> OnVariableChanged;
        public event Action OnVariableCleared; 
        
        protected Blackboard m_Parent; // @TODO: not work yet.
        
        private Object _owner;
        public virtual Object Owner => _owner;
        public Blackboard()
        {
            VariableMap = new SerializableNameToVariableMap();
        }

        public Blackboard(Object owner)
        {
            VariableMap = new SerializableNameToVariableMap();
            _owner = owner;
        }
        
        public void SetParentBlackboard(Blackboard parent)
        {
            this.m_Parent = parent;
        }

        public void Clear()
        {
            this.m_Parent = null;
            VariableMap.Clear();
            
            OnVariableCleared?.Invoke();
        }

        public void Destroy()
        {
            Clear();
            
            OnVariableAdded = OnVariableRemoved = OnVariableChanged = null;
            OnVariableCleared = null;
        }

        public void OnPostDeserialize()
        {
            foreach (var variable in Variables)
            {
                variable.OnValueChanged -= OnVariableValueChangedHandler;
                variable.OnValueChanged += OnVariableValueChangedHandler;
            }
        }
        
        public void MoveUp(string key)
        {
            VariableMap.MoveUp(key);
        }

        public BlackboardVariableBase SetValue(string name, object value)
        {
            var variableType = BlackboardVariableHandle.GetVariableType(value.GetType());
            return SetVariable(name, variableType, value);
        }
        
        public BlackboardVariableBase SetVariable(string name, Type variableType, object defaultValue)
        {
            if (variableType == null)
            {
                return null;
            }
            
            if (m_Parent != null && m_Parent.IsSet(name))
            {
                return m_Parent.SetVariable(name, variableType, defaultValue);
            }
            
            if (VariableMap.TryGetValue(name, out BlackboardVariableBase existVariable))
            {
                var defaultValueType = defaultValue.GetType();
                if (existVariable.IsAssignableTo(defaultValueType))
                {
                    existVariable.SetValue(defaultValue);
                    return existVariable;
                }
                else
                {
                    Debug.LogError($"The type for key {name} does not match the input value type. {existVariable.GetValueType()} != {defaultValueType}");
                    return null;
                }
            }
            else
            {
                BlackboardVariableBase newVariable = (BlackboardVariableBase)Activator.CreateInstance(variableType, name);
                newVariable.SetBlackboard(this); // 需要在SetValue之前，比如：BlackboardVariableUnityObject需使用黑板中的Owner
                newVariable.SetValue(defaultValue);
                newVariable.OnValueChanged += OnVariableValueChangedHandler;
                
                VariableMap[name] = newVariable;
                OnVariableAdded?.Invoke(newVariable);

                return newVariable;
            }
        }

        public void UnsetValue(string key)
        {
            if (m_Parent != null && m_Parent.IsSet(key))
            {
                m_Parent.UnsetValue(key);
                return;
            }
            
            if (VariableMap.ContainsKey(key))
            {
                VariableMap.Remove(key);
                OnVariableRemoved?.Invoke(VariableMap[key]);
            }
        }

        public bool IsSet(string key)
        {
            return VariableMap.ContainsKey(key) || (m_Parent != null && m_Parent.IsSet(key));
        }

        public T GetValue<T>(string key)
        {
            if (TryGetValue(key, out T value))
            {
                return value;
            }

            throw new Exception($"No value exists for {key} of type {typeof(T)}");
        }

        public bool TryGetValue<T>(string key, out T value)
        {
            value = default(T);

            var variable = Get(key);
            if (variable != null)
            {
                if (variable is BlackboardVariable<T> t)
                {
                    value = t.Value;
                }
                return true;
            }

            return false;
        }

        public T Get<T>(string key) where T : BlackboardVariableBase
        {
            var variable = Get(key);
            return variable != null ? variable as T : null;
        }
        
        public BlackboardVariableBase Get(string key)
        {
            if (string.IsNullOrEmpty(key))
                return null;
            
            if (VariableMap.TryGetValue(key, out var variable))
            {
                return variable;
            }

            return m_Parent != null ? m_Parent.Get(key) : null;
        }
        
        public bool Rename(string oldName, string newName)
        {
            bool isReName = VariableMap.RenameKey(oldName,newName);
            if (isReName)
            {
                var variable = VariableMap[newName] ;
                variable.InternalSetName(newName);
            }
            return isReName;
        }
        
        private void OnVariableValueChangedHandler(BlackboardVariableBase variable)
        {
            OnVariableChanged?.Invoke(variable);
        }
        
        public Blackboard Clone()
        {
            Blackboard clone = new Blackboard(Owner);
            foreach (var variable in Variables)
            {
                clone.SetValue(variable.Name, variable.GetValue());
            }
            return clone;
        }

        public static Blackboard Create(Object owner)
        {
            return new Blackboard(owner);
        }
    }

    
    
    public class BlackboardVariableHandle
    {
        private static Dictionary<Type, Type> s_VariableTypes = new Dictionary<Type, Type>(); // valueType -> variableType

        static BlackboardVariableHandle() 
        {
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var variableType in assembly.GetTypes())
                {
                    var valueField = BlackboardVariableBase.InternalValueFieldInfo(variableType);
                    if (valueField != null)
                    {
                        s_VariableTypes[valueField.FieldType] = variableType;
                    }

                } // end of variableType loop
            }
        }

        public static Type GetVariableType(Type valueType)
        {
            if (s_VariableTypes.TryGetValue(valueType, out var type))
            {
                return type;
            }                

            Debug.LogError($"The type {valueType} is not registered.");
            return null;
        }
        
        public static BlackboardVariableBase CreateVariable(Type valueType, string name)
        {
            var variableType = GetVariableType(valueType);
            if (variableType != null)
            {
                return (BlackboardVariableBase)Activator.CreateInstance(variableType, name);
            }
            
            return null;
        }
       
    }
    
    

}
