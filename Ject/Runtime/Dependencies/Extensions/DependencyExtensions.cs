using Ject.Contracts;
using Toolkit;

namespace Ject.Dependencies.Extensions
{
    public static class DependencyExtensions
    {
        public static TD WithIdentifier<TD>(this TD dependency, string id) where TD : IDependency
            => dependency.WithIdentifier(new Identifier(id));
        
        public static TD WithIdentifier<TD>(this TD dependency, Identifier newIdentifier) where TD : IDependency
        {
            dependency.Description.Id = newIdentifier;
            return dependency;
        }

        public static TD WithIdOnlyResolve<TD>(this TD dependency) where TD : IDependency
        {
            dependency.Description.IncludeIdOnlyResolve = true;
            return dependency;
        }
    }
}