using System;
using System.Collections.Generic;
using Ject.Contracts;

namespace Ject.Dependencies.Generic.Extensions
{
    public static class DependencyGenericTranslationsExtensions
    {
        public static FactoryEnumerableDependency<T> AsFactory<T>(this CastedDependency<T> dependency, 
            Func<ISignedContract, IEnumerable<T>> factoryFunc) 
            => new FactoryEnumerableDependency<T>(dependency.TranslateInfo, factoryFunc);
        
        public static FactoryEnumeratorDependency<T> AsFactory<T>(this CastedDependency<T> dependency, 
            Func<ISignedContract, IEnumerator<T>> factoryFunc) 
            => new FactoryEnumeratorDependency<T>(dependency.TranslateInfo, factoryFunc);
        
        public static FactoryFunctionDependency<T> AsFactory<T>(this CastedDependency<T> dependency, 
            Func<ISignedContract, T> factoryFunc) 
            => new FactoryFunctionDependency<T>(dependency.TranslateInfo, factoryFunc);

        public static LazyFunctionDependency<T> AsLazy<T>(this CastedDependency<T> dependency,
            Func<ISignedContract, T> lazyFunc) 
            => new LazyFunctionDependency<T>(dependency.TranslateInfo, lazyFunc);
        
        public static LazyDependency<T> AsLazy<T>(this CastedDependency<T> dependency, Lazy<T> lazy) 
            => new LazyDependency<T>(dependency.TranslateInfo, lazy);
        
        public static InstanceDependency<T> AsInstance<T>(this CastedDependency<T> dependency, T instance) 
            => new InstanceDependency<T>(dependency.TranslateInfo, instance);
    }
}