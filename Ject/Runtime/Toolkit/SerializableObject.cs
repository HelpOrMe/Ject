using System;
using UnityEngine;

namespace Toolkit.Collections
{
    [Serializable]
    public class SerializableObject : ISerializationCallbackReceiver
    {
        [NonSerialized] public object Value;
        
        [SerializeField] private string valueTypeName;
        [SerializeField] private string valueJson;

        public SerializableObject(object value)
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
                Value = Activator.CreateInstance(Type.GetType(valueTypeName)!);
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
            // ReSharper disable once NonReadonlyMemberInGetHashCode
            return Value?.GetHashCode() ?? 0;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? base.ToString();
        }
    }
}