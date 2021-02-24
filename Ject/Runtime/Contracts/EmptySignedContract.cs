using System;
using Ject.Toolkit;

namespace Ject.Contracts
{
    public class EmptySignedContract : ISignedContract
    {
        public static readonly EmptySignedContract Instance = new EmptySignedContract();
        
        public void AddContract(ISignedContract contract) { }
        public bool CanResolve(Type type, Identifier id = default) => false;
        public bool CanResolve(Identifier id) => false;
        public object Resolve(Type type, Identifier id = default) => default;
        public object Resolve(Identifier id) => default;

        public ISignedContract Copy() => this;
    }
}