using System;
using System.Collections.Generic;
using Toolkit;

namespace Ject.Contracts
{
    public class DependencyTree
    {
        private readonly Dictionary<Type, Dictionary<Identifier, IDependency>> _dependencies;
        private readonly Dictionary<Identifier, IDependency> _dependenciesById;

        public DependencyTree()
        {
            _dependencies = new Dictionary<Type, Dictionary<Identifier, IDependency>>();
            _dependenciesById = new Dictionary<Identifier, IDependency>();
        }

        public DependencyTree(DependencyTree other)
        {
            _dependencies = other._dependencies;
            _dependenciesById = other._dependenciesById;
        }
        
        public void Extend(IDependency dependency)
        {
            if (dependency.Description.ImplementTypes.Count == 0)
            {
                throw new ProcessDependencyException(dependency, $"Dependency with id {dependency.Description.Id} " +
                                                                 $"has no implement types");
            }

            if (dependency.Description.IncludeIdOnlyResolve)
            {
                _dependenciesById[dependency.Description.Id] = dependency;
            }
            
            foreach (Type type in dependency.Description.ImplementTypes)
            {
                if (!_dependencies.ContainsKey(type))
                {
                    _dependencies[type] = new Dictionary<Identifier, IDependency>();
                }
                _dependencies[type][dependency.Description.Id] = dependency;
            }
        }
        
        public bool ContainsAggressive(Identifier id) => FindDependencyAggressive(id) != null;
        
        public bool Contains(Identifier id) => FindDependency(id) != null;
        
        public bool Contains(Type type, Identifier id = default) => FindDependency(type, id) != null;
        
        public IDependency FindDependencyAggressive(Identifier id)
        {
            IDependency dependency = FindDependency(id);
            if (dependency != null)
                return dependency;
            
            foreach (Type type in _dependencies.Keys)
            {
                if (_dependencies[type].ContainsKey(id))
                {
                    return _dependencies[type][id];
                }
            }

            return null;
        }

        public IDependency FindDependency(Identifier id) 
            => _dependenciesById.ContainsKey(id) ? _dependenciesById[id] : null;

        public IDependency FindDependency(Type baseType, Identifier id = default)
        {
            for (Type type = baseType; type != null; type = type.BaseType)
            {
                if (_dependencies.ContainsKey(type) && _dependencies[type].ContainsKey(id))
                {
                    return _dependencies[type][id];
                }
            }
            return null;
        }
    }
}