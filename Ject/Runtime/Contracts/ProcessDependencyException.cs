using System;

namespace Ject.Contracts
{
    public class ProcessDependencyException : Exception
    {
        public readonly IDependency Dependency;

        public ProcessDependencyException(IDependency dependency, string message) : base(message)
        {
            Dependency = dependency;
        }
    }
}