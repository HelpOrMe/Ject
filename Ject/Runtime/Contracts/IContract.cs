namespace Ject.Contracts
{
    public interface IContract
    {
        void AddSubContract(ISignableContract subContract);

        IDependencyWriter NewDependencyWriter();

        IContract Copy();
    }
}