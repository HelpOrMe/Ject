namespace Ject.Contracts
{
    public interface IDependency
    {
        DependencyDescription Description { get; }

        void SignWith(ISignedContract contract);
        
        bool CanBeResolved();
        
        object Resolve();
    }
}