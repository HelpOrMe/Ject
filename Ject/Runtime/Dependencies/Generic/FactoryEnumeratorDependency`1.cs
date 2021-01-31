using System;
using System.Collections.Generic;
using Ject.Contracts;

namespace Ject.Dependencies.Generic
{
    public class FactoryEnumeratorDependency<T> : Dependency
    {
        private readonly Func<ISignedContract, IEnumerator<T>> _factoryFunc;
        private IEnumerator<T> _enumerator;
        
        public FactoryEnumeratorDependency(DependencyTranslateInfo info,
            Func<ISignedContract, IEnumerator<T>> factoryFunc) : base(info)
        {
            _factoryFunc = factoryFunc;
        }

        public override object Resolve()
        {
            _enumerator ??= _factoryFunc(Contract);
            _enumerator.MoveNext();
            return _enumerator.Current;
        }
    }
}