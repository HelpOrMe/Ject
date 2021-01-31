using System;
using System.Reflection;
using Ject.Contracts;

namespace Ject.Injection
{
    public static class Constructor
    {
        public static T Construct<T>(ISignedContract contract)
            => (T)Construct(typeof(T), contract, 0);
        
        public static T Construct<T>(ISignedContract contract, int constructorIndex)
            => (T)Construct(typeof(T), contract, constructorIndex);

        public static object Construct(Type type, ISignedContract contract)
            => Construct(type, contract, 0);
        
        public static object Construct(Type type, ISignedContract contract, int constructorIndex)
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            ConstructorInfo[] constructors = type.GetConstructors(flags);

            if (constructorIndex >= constructors.Length)
                throw new MissingMemberException($"Invalid constructor index {type.Name}:{constructorIndex}");

            ConstructorInfo constructor = constructors[constructorIndex];
            ParameterInfo[] parameters = constructor.GetParameters();
            var parameterValues = new object[parameters.Length];
            
            for (int i = 0; i < parameters.Length; i++)
            {
                parameterValues[i] = contract.Resolve(parameters[i].ParameterType);
            }
            
            return constructor.Invoke(parameterValues);
        }
    }
}