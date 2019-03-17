using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.DataStructures
{
    class TwoWayMap<KeyType, ValueType>
    {
        Dictionary<KeyType, ValueType> keyToValue = new Dictionary<KeyType, ValueType>();
        Dictionary<ValueType, KeyType> valueToKey = new Dictionary<ValueType, KeyType>();

        public void Set(KeyType key, ValueType value)
        {
            if(keyToValue.ContainsKey(key))
            {
                Remove(key);
            }

            if(valueToKey.ContainsKey(value))
            {
                Remove(valueToKey[value]);
            }

            keyToValue[key] = value;
            valueToKey[value] = key;
        }

        public void Remove(KeyType key)
        {
            valueToKey.Remove(keyToValue[key]);
            keyToValue.Remove(key);
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
    }
}
