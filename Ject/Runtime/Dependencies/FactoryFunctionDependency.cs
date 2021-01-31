using System;
using Ject.Contracts;

namespace Ject.Dependencies
{
    public class FactoryFunctionDependency : Dependency
    {
        private readonly Func<ISignedContract, object> _factoryFunc;
        private bool _canBeResolvedNext = true;
        
        public FactoryFunctionDependency(DependencyTranslateInfo info, 
            Func<ISignedContract, object> factoryFunc) : base(info)
        {
            _factoryFunc = factoryFunc;
        }

        public override object Resolve()
        {
            object obj = _factoryFunc(Contract);
            _canBeResolvedNext = obj == null;
            return obj;
        }
    }
}