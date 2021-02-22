using System;

namespace Ject.Contracts
{
    public class ResolveException : Exception
    {
        public ResolveException(string message) : base(message) { }
    }
}