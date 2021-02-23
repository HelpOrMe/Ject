using System;
using System.Collections.Generic;
using Toolkit;

namespace Ject.Contracts
{
    public class DependencyDescription
    {
        public bool IncludeIdOnlyResolve = false;
        public Identifier Id = default;
        public readonly List<Type> ImplementTypes = new List<Type>();
    }
}