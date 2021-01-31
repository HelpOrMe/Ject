using System;
using Ject.Dependencies.Generic;

namespace Ject.Dependencies
{
    public class NonCastedDependency : Dependency
    {
        public NonCastedDependency(DependencyTranslateInfo info) : base(info) { }

        public CastedDependency<T> As<T>() => new CastedDependency<T>(TranslateInfo);
        
        public CastedDependency As(Type type) => new CastedDependency(TranslateInfo, type);
    }
}