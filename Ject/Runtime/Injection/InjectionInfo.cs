using System;
using Ject.Toolkit;

namespace Ject.Injection
{
    [Serializable]
    public class InjectionInfo
    {
        public string[] fieldNames = {};
        public string[] propertyNames = {};
        public string[] methodNames = {};

        public SerializableDictionary<string, Identifier> fieldDependencyIds = 
            new SerializableDictionary<string, Identifier>();
        
        public SerializableDictionary<string, Identifier> propertyDependencyIds = 
            new SerializableDictionary<string, Identifier>();
        
        public SerializableDictionary<string, SerializableDictionary<string, Identifier>> methodDependencyIds = 
            new SerializableDictionary<string, SerializableDictionary<string, Identifier>>();
    }
}