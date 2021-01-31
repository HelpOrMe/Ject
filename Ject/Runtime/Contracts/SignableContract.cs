using System.Collections.Generic;

namespace Ject.Contracts
{
    public class SignableContract : ISignableContract
    {
        private readonly List<IDependency> _dependencies = new List<IDependency>();
        private readonly List<ISignableContract> _subContracts = new List<ISignableContract>();

        public SignableContract() { }

        public SignableContract(List<IDependency> dependencies, List<ISignableContract> subContracts)
        {
            _dependencies = dependencies;
            _subContracts = subContracts;
        }
        
        public void AddSubContract(ISignableContract subContract)
        {
            _subContracts.Add(subContract);
        }

        public IDependencyWriter NewDependencyWriter() => new DependencyWriter(_dependencies);

        public ISignedContract Sign() => new SignedContract(_dependencies, _subContracts);

        public IContract Copy() => new SignableContract(_dependencies, _subContracts);
    }
}