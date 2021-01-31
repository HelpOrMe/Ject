using System;
using System.Collections.Generic;
using Ject.Contracts;

namespace Ject.Dependencies.Generic
{
    public class FactoryEnumerableDependency<T> : Dependency
    {
        private readonly Func<ISignedContract, IEnumerable<T>> _factoryFunc;
        private IEnumerator<T> _enumerator;
        
        public FactoryEnumerableDependency(DependencyTranslateInfo info, 
            Func<ISignedContract, IEnumerable<T>> factoryFunc) : base(info)
        {
            _factoryFunc = factoryFunc;
        }

        public override object Resolve()
        {
            _enumerator ??= _factoryFunc(Contract).GetEnumerator();
            _enumerator.MoveNext();
            return _enumerator.Current;
        }
    }
}