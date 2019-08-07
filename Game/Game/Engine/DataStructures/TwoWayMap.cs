using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Engine.DataStructures
{
    class TwoWayMap<KeyType, ValueType> : IEnumerable<KeyValuePair<KeyType, ValueType>>
    {
        private SortedList<KeyType, ValueType> keyToValue;
        private Dictionary<ValueType, KeyType> valueToKey = new Dictionary<ValueType, KeyType>();

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
                ICollection keyToValueIC = (ICollection)keyToValue;
                ICollection valueToKeyIC = (ICollection)valueToKey;

                lock (keyToValueIC.SyncRoot)
                {
                    lock (valueToKeyIC.SyncRoot)
                    {
                        return keyToValue.Count;
                    }
                }
            }
        }

        public void Set(KeyType key, ValueType value)
        {
            ICollection keyToValueIC = (ICollection)keyToValue;
            ICollection valueToKeyIC = (ICollection)valueToKey;

            lock (keyToValueIC.SyncRoot)
            {
                lock (valueToKeyIC.SyncRoot)
                {
                    RemoveKey(key);
                    RemoveValue(value);

                    keyToValue[key] = value;
                    valueToKey[value] = key;
                }
            }
        }

        public bool RemoveKey(KeyType key)
        {
            ICollection keyToValueIC = (ICollection)keyToValue;
            ICollection valueToKeyIC = (ICollection)valueToKey;

            lock (keyToValueIC.SyncRoot)
            {
                lock (valueToKeyIC.SyncRoot)
                {
                    if (ContainsKey(key))
                    {
                        valueToKey.Remove(keyToValue[key]);
                        keyToValue.Remove(key);
                        return true;
                    }
                    return false;
                }
            }
        }

        public bool RemoveValue(ValueType value)
        {
            ICollection keyToValueIC = (ICollection)keyToValue;
            ICollection valueToKeyIC = (ICollection)valueToKey;

            lock (keyToValueIC.SyncRoot)
            {
                lock (valueToKeyIC.SyncRoot)
                {
                    if (ContainsValue(value))
                    {
                        keyToValue.Remove(valueToKey[value]);
                        valueToKey.Remove(value);
                        return true;
                    }
                    return false;
                }
            }
        }

        public bool ContainsKey(KeyType key)
        {
            ICollection keyToValueIC = (ICollection)keyToValue;
            ICollection valueToKeyIC = (ICollection)valueToKey;

            lock (keyToValueIC.SyncRoot)
            {
                lock (valueToKeyIC.SyncRoot)
                {
                    return keyToValue.ContainsKey(key);
                }
            }
        }

        public bool ContainsValue(ValueType value)
        {
            ICollection keyToValueIC = (ICollection)keyToValue;
            ICollection valueToKeyIC = (ICollection)valueToKey;

            lock (keyToValueIC.SyncRoot)
            {
                lock (valueToKeyIC.SyncRoot)
                {
                    return valueToKey.ContainsKey(value);
                }
            }
        }

        public ValueType GetValue(KeyType key)
        {
            ICollection keyToValueIC = (ICollection)keyToValue;
            ICollection valueToKeyIC = (ICollection)valueToKey;

            lock (keyToValueIC.SyncRoot)
            {
                lock (valueToKeyIC.SyncRoot)
                {
                    return keyToValue[key];
                }
            }
        }

        public ValueType GetValue(KeyType key, out bool containsKey)
        {
            ICollection keyToValueIC = (ICollection)keyToValue;
            ICollection valueToKeyIC = (ICollection)valueToKey;

            lock (keyToValueIC.SyncRoot)
            {
                lock (valueToKeyIC.SyncRoot)
                {
                    if (keyToValue.ContainsKey(key))
                    {
                        containsKey = true;
                        return keyToValue[key];
                    }
                    else
                    {
                        containsKey = false;
                        return default(ValueType);
                    }
                }
            }
        }

        public KeyType GetKey(ValueType value, out bool containsValue)
        {
            ICollection keyToValueIC = (ICollection)keyToValue;
            ICollection valueToKeyIC = (ICollection)valueToKey;

            lock (keyToValueIC.SyncRoot)
            {
                lock (valueToKeyIC.SyncRoot)
                {
                    if (valueToKey.ContainsKey(value))
                    {
                        containsValue = true;
                        return valueToKey[value];
                    }
                    else
                    {
                        containsValue = false;
                        return default(KeyType);
                    }
                }
            }
        }

        public KeyType GetKey(ValueType value)
        {
            ICollection keyToValueIC = (ICollection)keyToValue;
            ICollection valueToKeyIC = (ICollection)valueToKey;

            lock (keyToValueIC.SyncRoot)
            {
                lock (valueToKeyIC.SyncRoot)
                {
                    return valueToKey[value];
                }
            }
        }

        public KeyValuePair<KeyType, ValueType> ElementAt(int index)
        {
            ICollection keyToValueIC = (ICollection)keyToValue;
            ICollection valueToKeyIC = (ICollection)valueToKey;

            lock (keyToValueIC.SyncRoot)
            {
                lock (valueToKeyIC.SyncRoot)
                {
                    return keyToValue.ElementAt(index);
                }
            }
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
                ICollection keyToValueIC = (ICollection)keyToValue;
                ICollection valueToKeyIC = (ICollection)valueToKey;

                lock (keyToValueIC.SyncRoot)
                {
                    lock (valueToKeyIC.SyncRoot)
                    {
                        return keyToValue.Keys;
                    }
                }
            }
        }

        public ICollection<ValueType> Values
        {
            get
            {
                ICollection keyToValueIC = (ICollection)keyToValue;
                ICollection valueToKeyIC = (ICollection)valueToKey;

                lock (keyToValueIC.SyncRoot)
                {
                    lock (valueToKeyIC.SyncRoot)
                    {
                        return keyToValue.Values;
                    }
                }
            }
        }

        public void Clear()
        {
            ICollection keyToValueIC = (ICollection)keyToValue;
            ICollection valueToKeyIC = (ICollection)valueToKey;

            lock (keyToValueIC.SyncRoot)
            {
                lock (valueToKeyIC.SyncRoot)
                {
                    keyToValue.Clear();
                    valueToKey.Clear();
                }
            }
        }

        public IEnumerator<KeyValuePair<KeyType, ValueType>> GetEnumerator()
        {
            ICollection keyToValueIC = (ICollection)keyToValue;
            ICollection valueToKeyIC = (ICollection)valueToKey;

            lock (keyToValueIC.SyncRoot)
            {
                lock (valueToKeyIC.SyncRoot)
                {
                    return keyToValue.GetEnumerator();
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public KeyType GreatestKey
        {
            get
            {
                ICollection keyToValueIC = (ICollection)keyToValue;
                ICollection valueToKeyIC = (ICollection)valueToKey;

                lock (keyToValueIC.SyncRoot)
                {
                    lock (valueToKeyIC.SyncRoot)
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
    }
}
