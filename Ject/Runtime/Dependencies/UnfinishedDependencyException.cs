using System;
using Ject.Contracts;

namespace Ject.Dependencies
{
    public class UnfinishedDependencyException : Exception
    {
        public UnfinishedDependencyException(IDependency dependency) : 
            base($"Unable resolve unfinished dependency {dependency}") { }
    }
}