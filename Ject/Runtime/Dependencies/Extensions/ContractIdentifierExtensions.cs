using System;
using Ject.Contracts;
using Ject.Dependencies.Generic;
using Ject.Toolkit;

namespace Ject.Dependencies.Extensions
{
    public static class ContractIdentifierExtensions
    {
        public static InstanceDependency<T> Describe<T>(this IContract contract, T instance, string id)
            => contract.Describe(instance).WithIdentifier(id);
        
        public static InstanceDependency<T> Describe<T>(this IContract contract, T instance, Identifier id)
            => contract.Describe(instance).WithIdentifier(id);

        
        public static CastedDependency<T> Describe<T>(this IContract contract, string id)
            => contract.Describe<T>().WithIdentifier(id);

        public static CastedDependency<TI> Describe<T, TI>(this IContract contract, string id) where TI : T 
            => contract.Describe<T, TI>().WithIdentifier(id);
        
        public static CastedDependency<TI> Describe<T, TI>(this IContract contract, Identifier id) where TI : T 
            => contract.Describe<T, TI>().WithIdentifier(id);
        
        public static CastedDependency Describe(this IContract contract, Type type, string id)
            => contract.Describe().As(type).WithIdentifier(id);
        
        public static CastedDependency Describe(this IContract contract, Type type, Identifier id)
            => contract.Describe().As(type).WithIdentifier(id);
        
        public static NonCastedDependency Describe(this IContract contract, Identifier id)
            => contract.Describe().WithIdentifier(id);
    }
}