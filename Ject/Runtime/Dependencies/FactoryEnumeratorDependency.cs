using System;
using System.Collections;
using Ject.Contracts;

namespace Ject.Dependencies
{
    public class FactoryEnumeratorDependency : Dependency
    {
        private readonly Func<ISignedContract, IEnumerator> _factoryFunc;
        private IEnumerator _enumerator;
        private bool _canBeResolvedNext = true;
        
        public FactoryEnumeratorDependency(DependencyTranslateInfo info,
            Func<ISignedContract, IEnumerator> factoryFunc) : base(info)
        {
            _factoryFunc = factoryFunc;
        }

        public override bool CanBeResolved() => _canBeResolvedNext;
        
        public override object Resolve()
        {
            _enumerator ??= _factoryFunc(Contract);
            _canBeResolvedNext = _enumerator.MoveNext();
            return _enumerator.Current;
        }
    }
}