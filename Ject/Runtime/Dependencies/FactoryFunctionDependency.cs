using System;
using Ject.Contracts;

namespace Ject.Dependencies
{
    public class FactoryFunctionDependency : Dependency
    {
        private readonly Func<ISignedContract, object> _factoryFunc;
        
        public FactoryFunctionDependency(DependencyTranslateInfo info, 
            Func<ISignedContract, object> factoryFunc) : base(info)
        {
            _factoryFunc = factoryFunc;
        }

        public override object Resolve() => _factoryFunc(Contract);
    }
}