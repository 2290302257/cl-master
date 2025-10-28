using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Alurax
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue>
    {
        [SerializeField]
        private List<TKey> keys = new List<TKey>();
        
        [SerializeReference]
        private List<TValue> values = new List<TValue>();

        public List<TKey> Keys => keys;
        public List<TValue> Values => values;

        public void MoveUp(TKey key)
        {
            int index = keys.IndexOf(key);
            if (index > 0)
            {
                keys.Insert(index - 1, keys[index]);
                keys.RemoveAt(index + 1);
                values.Insert(index - 1, values[index]);
                values.RemoveAt(index + 1);
            }
        }
        
        public TValue this[TKey key]
        {
            get => values[keys.IndexOf(key)];
            set
            {
                var index = keys.IndexOf(key);
                if (index == -1)
                {
                    keys.Add(key);
                    values.Add(value);
                }
                else
                {
                    values[index] = value;
                }
            }
        }

        public void Clear()
        {
            keys.Clear();
            values.Clear();
        }
        
        public bool ContainsKey(TKey key)
        {
            return keys.Contains(key);
        }
        
        public bool RenameKey(TKey oldKey, TKey newKey)
        {
            var index = keys.IndexOf(oldKey);
            if (index != -1)
            {
                keys[index] = newKey;
                return true;
            }
            return false;
        }
        
        public bool Remove(TKey key)
        {
            var index = keys.IndexOf(key);
            if (index != -1)
            {
                keys.RemoveAt(index);
                values.RemoveAt(index);
                return true;
            }

            return false;
        }
        
        public bool TryGetValue(TKey key, out TValue value)
        {
            var index = keys.IndexOf(key);
            if (index != -1)
            {
                value = values[index];
                return true;
            }

            value = default(TValue);
            return false;
        }
    }
}
