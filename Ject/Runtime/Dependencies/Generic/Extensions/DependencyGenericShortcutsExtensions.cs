using System;
using Ject.Injection;

namespace Ject.Dependencies.Generic.Extensions
{
    public static class DependencyGenericShortcutsExtensions
    {
        public static InstanceDependency<T> AsInstance<T>(this CastedDependency<T> dependency) 
            => new InstanceDependency<T>(dependency.TranslateInfo, Activator.CreateInstance<T>());
        
        public static FactoryFunctionDependency<T> AsFactory<T>(this CastedDependency<T> dependency) 
            => new FactoryFunctionDependency<T>(dependency.TranslateInfo, Constructor.Construct<T>);

        public static LazyFunctionDependency<T> AsLazy<T>(this CastedDependency<T> dependency) 
            => new LazyFunctionDependency<T>(dependency.TranslateInfo, Constructor.Construct<T>);
    }
}