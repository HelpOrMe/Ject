namespace Ject.Contracts
{
    public interface IDependencyWriter
    {
        void Rewrite(IDependency dependency);
    }
}