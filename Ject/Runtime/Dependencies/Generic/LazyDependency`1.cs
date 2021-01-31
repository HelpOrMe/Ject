using System;

namespace Ject.Dependencies.Generic
{
    public class LazyDependency<T> : Dependency
    {
        private readonly Lazy<T> _lazy;

        public LazyDependency(DependencyTranslateInfo info, Lazy<T> lazy) : base(info)
        {
            _lazy = lazy;
        }

        public override object Resolve() => _lazy.Value;
    }
}