using System;
using Ject.Contracts;
using Ject.Dependencies.Generic;
using Ject.Dependencies.Generic.Extensions;

namespace Ject.Dependencies.Extensions
{
    public static class ContractShortcutsExtensions
    {
        public static InstanceDependency<T> Describe<T>(this IContract contract, T instance)
            => contract.Describe<T>().AsInstance(instance);
        
        public static CastedDependency<TI> Describe<T, TI>(this IContract contract) where TI : T
            => contract.Describe().As<T>().ThatImplements<TI>();
        
        public static CastedDependency<T> Describe<T>(this IContract contract)
            => new CastedDependency<T>(contract.NewTranslateInfo());
      
        public static CastedDependency Describe(this IContract contract, Type type)
            => new CastedDependency(contract.NewTranslateInfo(), type);
        
        public static NonCastedDependency Describe(this IContract contract) 
            => new NonCastedDependency(contract.NewTranslateInfo());

        private static DependencyTranslateInfo NewTranslateInfo(this IContract contract)
            => new DependencyTranslateInfo(contract.NewDependencyWriter(), new DependencyDescription());
    }
}