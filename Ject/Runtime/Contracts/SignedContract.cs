using System;
using System.Collections.Generic;
using System.Linq;
using Ject.Toolkit;

namespace Ject.Contracts
{
    public class SignedContract : ISignedContract
    {
        private readonly DependencyTree _dependencyTree;
        private readonly List<ISignedContract> _subContracts;
        
        public SignedContract(IEnumerable<IDependency> dependencies, IEnumerable<ISignableContract> subContracts)
        {
            _dependencyTree = new DependencyTree();
            _subContracts = new List<ISignedContract>();
            
            AddDependencies(dependencies);
            AddSubContracts(subContracts);
        }

        private SignedContract(SignedContract other)
        {
            _dependencyTree = new DependencyTree(other._dependencyTree);
            _subContracts = other._subContracts.ToList();
        }

        private void AddDependencies(IEnumerable<IDependency> dependencies)
        {
            foreach (IDependency dependency in dependencies)
            {
                _dependencyTree.Extend(dependency);
            }
        }

        private void AddSubContracts(IEnumerable<ISignableContract> contracts)
        {
            foreach (ISignableContract contract in contracts)
            {
                AddContract(contract.Sign());
            }
        }

        public void AddContract(ISignedContract contract)
        {
            _subContracts.Add(contract);
        }

        public bool CanResolve(Type type, Identifier id = default) 
            => _dependencyTree.Contains(type, id) 
               || _subContracts.Any(subContract => subContract.CanResolve(type, id));

        public bool CanResolve(Identifier id)
            => _dependencyTree.ContainsAggressive(id)
                || _subContracts.Any(subContract => subContract.CanResolve(id));

        public object Resolve(Type type, Identifier id = default)
        {
            IDependency dependency = _dependencyTree.FindDependency(type, id);

            if (dependency != null && dependency.CanBeResolved())
                return dependency.Resolve();

            foreach (ISignedContract subContract in _subContracts)
            {
                if (subContract.CanResolve(type, id))
                {
                    return subContract.Resolve(type, id);
                }
            }

            throw new ResolveException( $"Dependency with type {type.Name} ({id}) does not exist");
        }

        public object Resolve(Identifier id)
        {
            IDependency dependency = _dependencyTree.FindDependencyAggressive(id);

            if (dependency != null && dependency.CanBeResolved())
                return dependency.Resolve();

            foreach (ISignedContract subContract in _subContracts)
            {
                if (subContract.CanResolve(id))
                {
                    return subContract.Resolve(id);
                }
            }

            throw new ResolveException($"Dependency with id {id} does not exist");
        }

        public ISignedContract Copy() => new SignedContract(this);
    }
}