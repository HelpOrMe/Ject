using System;
using UnityEngine;

namespace Toolkit.Collections
{
    [Serializable]
    public class SerializableObject<T> : ISerializationCallbackReceiver
    {
        [NonSerialized] public T Value;
        
        [SerializeField] private string valueTypeName;
        [SerializeField] private string valueJson;

        public SerializableObject() : this(Activator.CreateInstance<T>()) { }
        
        public SerializableObject(T value)
        {
            Value = value;
        }

        public void OnBeforeSerialize()
        {
            if (Value != null)
            {
                valueTypeName = Value.GetType().AssemblyQualifiedName;
                valueJson = JsonUtility.ToJson(Value);
            }
        }

        public void OnAfterDeserialize()
        {
            if (!string.IsNullOrEmpty(valueJson))
            {
                Value = (T)Activator.CreateInstance(Type.GetType(valueTypeName)!);
                JsonUtility.FromJsonOverwrite(valueJson, Value);
            }

            valueTypeName = null;
            valueJson = null;
        }
        
        public override bool Equals(object obj)
        {
            if (obj is SerializableObject other)
            {
                return Value.Equals(other.Value);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Value?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? base.ToString();
        }
    }
}