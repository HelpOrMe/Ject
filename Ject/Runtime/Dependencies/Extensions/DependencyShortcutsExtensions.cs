using System;
using System.Linq;
using Ject.Injection;

namespace Ject.Dependencies.Extensions
{
    public static class DependencyShortcutsExtensions
    {
        public static InstanceDependency AsInstance(this CastedDependency dependency) 
            => new InstanceDependency(dependency.TranslateInfo, 
                Activator.CreateInstance(dependency.Description.ImplementTypes.Last()));
        
        public static FactoryFunctionDependency AsFactory(this CastedDependency dependency) 
            => new FactoryFunctionDependency(dependency.TranslateInfo, contract 
                => Constructor.Construct(dependency.Description.ImplementTypes.Last(), contract));

        public static LazyFunctionDependency AsLazy(this CastedDependency dependency) 
            => new LazyFunctionDependency(dependency.TranslateInfo, contract 
                => Constructor.Construct(dependency.Description.ImplementTypes.Last(), contract));
    }
}