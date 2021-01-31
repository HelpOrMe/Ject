using System.Collections.Generic;

namespace Ject.Contracts
{
    public class DependencyWriter : IDependencyWriter
    {
        private readonly List<IDependency> _dependencies;
        private int _prevIndex = -1;
        
        public DependencyWriter(List<IDependency> dependencies)
        {
            _dependencies = dependencies;
        }

        public void Rewrite(IDependency dependency)
        {
            if (_prevIndex == -1)
            {
                _prevIndex = _dependencies.Count;
                _dependencies.Add(dependency);
            }
            else _dependencies[_prevIndex] = dependency;
        }
    }
}