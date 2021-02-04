using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Toolkit.Collections
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : Dictionary<TKey, TValue>, 
        INestedSerialization, ISerializationCallbackReceiver
    {
        [SerializeField] private TKey[] keys;
        [SerializeField] private TValue[] values;

        [SerializeField] private bool keysSerializedByJson;
        [SerializeField] private bool valuesSerializedByJson;

        [SerializeField] private string[] keyTypes;
        [SerializeField] private string[] valueTypes;
        
        [SerializeField] private string[] keyJsons;
        [SerializeField] private string[] valueJsons;
        
        public SerializableDictionary() { }
        public SerializableDictionary(IDictionary<TKey, TValue> dictionary) : base(dictionary) { }
        
        public void OnBeforeSerialize()
        {
            if (typeof(INestedSerialization).IsAssignableFrom(typeof(TKey)))
            {
                keysSerializedByJson = true;
            }
            
            if (typeof(INestedSerialization).IsAssignableFrom(typeof(TValue)))
            {
                valuesSerializedByJson = true;
            }

            if (keysSerializedByJson)
            {
                keyTypes = Keys.Select(key => key.GetType().AssemblyQualifiedName).ToArray();
                keyJsons = Keys.Select(key => JsonUtility.ToJson(key)).ToArray();
            }
            else keys = Keys.ToArray();

            if (valuesSerializedByJson)
            {
                valueTypes = Values.Select(value => value.GetType().AssemblyQualifiedName).ToArray();
                valueJsons = Values.Select(value => JsonUtility.ToJson(value)).ToArray();
            }
            else values = Values.ToArray();
        }

        public void OnAfterDeserialize()
        {
            if (keysSerializedByJson)
            {
                keys = new TKey[keyJsons.Length];
                for (int i = 0; i < keys.Length; i++)
                {
                    keys[i] = (TKey)JsonUtility.FromJson(keyJsons[i], Type.GetType(keyTypes[i]));
                }

                keyJsons = null;
                keyTypes = null;
            }
            
            if (valuesSerializedByJson)
            {
                values = new TValue[valueJsons.Length];
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = (TValue)JsonUtility.FromJson(valueJsons[i], Type.GetType(valueTypes[i]));
                }

                valueJsons = null;
                valueTypes = null;
            }
            
            if (keys == null || values == null || keys.Length != values.Length) return;

            Clear();
            for (int i = 0; i < keys.Length; i++)
            {
                this[keys[i]] = values[i];
            }

            keys = null;
            values = null;
        }
    }
}