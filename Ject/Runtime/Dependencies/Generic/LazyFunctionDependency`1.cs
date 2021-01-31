using System;
using Ject.Contracts;

namespace Ject.Dependencies.Generic
{
    public class LazyFunctionDependency<T> : Dependency
    {
        private readonly Func<ISignedContract, T> _lazyFunc;
        private T _instance;
        
        public LazyFunctionDependency(DependencyTranslateInfo info, Func<ISignedContract, T> lazyFunc) 
            : base(info)
        {
            _lazyFunc = lazyFunc;
        }

        public override object Resolve() => _instance ??= _lazyFunc(Contract);
    }
}