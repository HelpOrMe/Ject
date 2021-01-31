using System;
using System.Collections.Generic;
using Ject.Toolkit;

namespace Ject.Contracts
{
    public class DependencyDescription
    {
        public bool IncludeIdOnlyResolve = false;
        public Identifier Id = Identifier.None;
        public readonly List<Type> ImplementTypes = new List<Type>();
    }
}