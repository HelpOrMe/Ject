using System;

namespace Ject.Dependencies
{
    public class CastedDependency : Dependency
    {
        public CastedDependency(DependencyTranslateInfo info, Type type) : base(info)
        {
            Description.ImplementTypes.Add(type);
        }

        public CastedDependency ThatImplements(params Type[] types)
        {
            Description.ImplementTypes.AddRange(types);
            return this;
        }
    }
}