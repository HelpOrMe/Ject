using System;
using Ject.Toolkit;

namespace Ject.Contracts
{
    public interface ISignedContract
    {
        void AddContract(ISignedContract contract);
        
        bool CanResolve(Type type, Identifier id = default);
        
        bool CanResolve(Identifier id);
        
        object Resolve(Type type, Identifier id = default);

        object Resolve(Identifier id);
        
        ISignedContract Copy();
    }
}