using System;
using Ject.Contracts;

namespace Ject.Dependencies.Generic
{
    public class FactoryFunctionDependency<T> : Dependency
    {
        private readonly Func<ISignedContract, T> _getterFunc;
        
        public FactoryFunctionDependency(DependencyTranslateInfo info, Func<ISignedContract, T> getterFunc) 
            : base(info)
        {
            _getterFunc = getterFunc;
        }

        public override object Resolve() => _getterFunc(Contract);
    }
}