using System;
using Toolkit;

namespace Ject.Contracts
{
    public class ResolveException : Exception
    {
        public ResolveException(string message) : base(message) { }
    }
}