using System.Collections.Generic;
using System.Reflection;
using Ject.Contracts;
using Ject.Toolkit;

namespace Ject.Injection
{
    public class Injector : InjectorBase
    {
        public ISignedContract contract;

        public Injector(object patient) : this(patient, EmptySignedContract.instance) { }
        
        public Injector(object patient, ISignedContract contract) : base(patient)
        {
            this.contract = contract;
        }

        public void Inject(InjectionInfo info)
        {
            foreach (string name in info.fieldNames)
            {
                Identifier id = info.fieldDependencyIds.ContainsKey(name) 
                    ? info.fieldDependencyIds[name] 
                    : default; 
                InjectField(name, id);
            }
            
            foreach (string name in info.propertyNames)
            {
                Identifier id = info.propertyDependencyIds.ContainsKey(name) 
                    ? info.propertyDependencyIds[name] 
                    : default; 
                InjectProperty(name, id);
            }
            
            foreach (string name in info.methodNames)
            {
                Dictionary<string, Identifier> id = info.methodDependencyIds.ContainsKey(name) 
                    ? info.methodDependencyIds[name] 
                    : new Dictionary<string, Identifier>();
                InjectMethod(name, id);
            }
        }
        
        public void InjectField(string name, Identifier dependencyId = default)
        {
            FieldInfo field = GetField(name);
            field.SetValue(patient, contract.Resolve(field.FieldType, dependencyId));
        }
        
        public void InjectProperty(string name, Identifier dependencyId = default)
        {
            PropertyInfo property = GetProperty(name);
            property.SetValue(patient, contract.Resolve(property.PropertyType, dependencyId));
        }

        public object InjectMethod(string name) => InjectMethod(name, new Dictionary<string, Identifier>());
        
        public object InjectMethod(string name, Dictionary<string, Identifier> dependencyIds)
        {
            MethodInfo method = GetMethod(name);
            ParameterInfo[] parameters = method.GetParameters();
            var parameterValues = new object[parameters.Length];
            
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                Identifier id = dependencyIds.ContainsKey(parameter.Name) ? dependencyIds[parameter.Name] : default;
                parameterValues[i] = contract.Resolve(parameter.ParameterType, id);
            }

            return method.Invoke(patient, parameterValues);
        }
    }
}