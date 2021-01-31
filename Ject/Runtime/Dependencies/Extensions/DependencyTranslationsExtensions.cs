using System;
using System.Collections;
using Ject.Contracts;

namespace Ject.Dependencies.Extensions
{
    public static class DependencyTranslationsExtensions
    {
        public static FactoryEnumerableDependency AsFactory(this CastedDependency dependency, 
            Func<ISignedContract, IEnumerable> factoryFunc) 
            => new FactoryEnumerableDependency(dependency.TranslateInfo, factoryFunc);
        
        public static FactoryEnumeratorDependency AsFactory(this CastedDependency dependency, 
            Func<ISignedContract, IEnumerator> factoryFunc) 
            => new FactoryEnumeratorDependency(dependency.TranslateInfo, factoryFunc);
        
        public static FactoryFunctionDependency AsFactory(this CastedDependency dependency, 
            Func<ISignedContract, object> factoryFunc) 
            => new FactoryFunctionDependency(dependency.TranslateInfo, factoryFunc);

        public static LazyFunctionDependency AsLazy(this CastedDependency dependency,
            Func<ISignedContract, object> lazyFunc) 
            => new LazyFunctionDependency(dependency.TranslateInfo, lazyFunc);
        
        public static InstanceDependency AsInstance(this CastedDependency dependency, object instance) 
            => new InstanceDependency(dependency.TranslateInfo, instance);
    }
}