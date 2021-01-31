using System;
using System.Collections;
using Ject.Contracts;

namespace Ject.Dependencies
{
    public class FactoryEnumerableDependency : Dependency
    {
        private readonly Func<ISignedContract, IEnumerable> _factoryFunc;
        private IEnumerator _enumerator;
        private bool _canBeResolvedNext = true;
        
        public FactoryEnumerableDependency(DependencyTranslateInfo info, 
            Func<ISignedContract, IEnumerable> factoryFunc) : base(info)
        {
            _factoryFunc = factoryFunc;
        }

        public override bool CanBeResolved() => _canBeResolvedNext;

        public override object Resolve()
        {
            _enumerator ??= _factoryFunc(Contract).GetEnumerator();
            _canBeResolvedNext = _enumerator.MoveNext();
            return _enumerator.Current;
        }
    }
}