using Ject.Contracts;

namespace Ject.Usage
{
    public abstract class ContractWriter
    {
        protected IContract Contract;

        internal void Write(IContract contract)
        {
            Contract = contract;
            Write();
            Contract = null;
        }

        protected abstract void Write();
    }
}