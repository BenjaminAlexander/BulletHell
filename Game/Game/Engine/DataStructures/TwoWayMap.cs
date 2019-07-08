using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.DataStructures
{
    class TwoWayMap<KeyType, ValueType>
    {
        SortedList<KeyType, ValueType> keyToValue;
        Dictionary<ValueType, KeyType> valueToKey = new Dictionary<ValueType, KeyType>();

        public TwoWayMap()
        {
            keyToValue = new SortedList<KeyType, ValueType>();
        }

        public TwoWayMap(IComparer<KeyType> comparer)
        {
            keyToValue = new SortedList<KeyType, ValueType>(comparer);
        }

        public int Count
        {
            get
            {
                return keyToValue.Count;
            }
        }

        public void Set(KeyType key, ValueType value)
        {
            RemoveKey(key);
            RemoveValue(value);

            keyToValue[key] = value;
            valueToKey[value] = key;
        }

        public bool RemoveKey(KeyType key)
        {
            if (ContainsKey(key))
            {
                valueToKey.Remove(keyToValue[key]);
                keyToValue.Remove(key);
                return true;
            }
            return false;
        }

        public bool RemoveValue(ValueType value)
        {
            if (ContainsValue(value))
            {
                keyToValue.Remove(valueToKey[value]);
                valueToKey.Remove(value);
                return true;
            }
            return false;
        }

        public bool ContainsKey(KeyType key)
        {
            return keyToValue.ContainsKey(key);
        }

        public bool ContainsValue(ValueType value)
        {
            return valueToKey.ContainsKey(value);
        }

        public ValueType GetValue(KeyType key)
        {
            return keyToValue[key];
        }

        public KeyType GetKey(ValueType value)
        {
            return valueToKey[value];
        }

        public ValueType this[KeyType key]
        {
            get
            {
                return GetValue(key);
            }

            set
            {
                Set(key, value);
            }
        }

        public KeyType this[ValueType val]
        {
            get
            {
                return GetKey(val);
            }

            set
            {
                Set(value, val);
            }
        }

        public ICollection<KeyType> Keys
        {
            get
            {
                return keyToValue.Keys;
            }
        }

        public ICollection<ValueType> Values
        {
            get
            {
                return keyToValue.Values;
            }
        }

        public void Clear()
        {
            keyToValue.Clear();
            valueToKey.Clear();
        }

        public IEnumerator<KeyValuePair<KeyType, ValueType>> GetEnumerator()
        {
            return keyToValue.GetEnumerator();
        }

        public KeyType GreatestKey
        {
            get
            {
                if (keyToValue.Count > 0)
                {
                    return keyToValue.Keys[keyToValue.Count - 1];
                }
                return default(KeyType);
            }
        }
    }
}
