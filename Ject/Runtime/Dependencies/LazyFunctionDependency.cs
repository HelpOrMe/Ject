using System;
using Ject.Contracts;

namespace Ject.Dependencies
{
    public class LazyFunctionDependency : Dependency
    {
        private readonly Func<ISignedContract, object> _lazyFunc;
        private object _instance;
        
        public LazyFunctionDependency(DependencyTranslateInfo info, Func<ISignedContract, object> lazyFunc) 
            : base(info)
        {
            _lazyFunc = lazyFunc;
        }

        public override object Resolve() => _instance ??= _lazyFunc(Contract);
    }
}