namespace Ject.Contracts
{
    public interface ISignableContract : IContract
    {
        ISignedContract Sign();
    }
}