namespace Ject.Dependencies.Generic
{
    public class InstanceDependency<T> : Dependency
    {
        private readonly T _instance;
        
        public InstanceDependency(DependencyTranslateInfo info, T instance) : base(info)
        {
            _instance = instance;
        }

        public override object Resolve() => _instance;
    }
}