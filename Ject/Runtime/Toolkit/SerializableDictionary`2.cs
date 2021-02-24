using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Ject.Toolkit
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {

        [SerializeField] private SerializeContainer<TKey>[] keys;
        [SerializeField] private SerializeContainer<TValue>[] values;

        public SerializableDictionary() { }
        public SerializableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }

        public void OnBeforeSerialize()
        {
            keys = Keys.Select(key => new SerializeContainer<TKey>(key)).ToArray();
            values = Values.Select(value => new SerializeContainer<TValue>(value)).ToArray();
        }

        public void OnAfterDeserialize()
        {
            if (keys == null || values == null || keys.Length != values.Length) return;

            Clear();
            for (int i = 0; i < keys.Length; i++)
            {
                this[keys[i].value] = values[i].value;
            }

            keys = null;
            values = null;
        }

        [Serializable]
        private class SerializeContainer<T>
        {
            public T value;

            public SerializeContainer(T value)
            {
                this.value = value;
            }
        }
    }
}