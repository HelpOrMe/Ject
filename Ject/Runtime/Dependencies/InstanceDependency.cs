namespace Ject.Dependencies
{
    public class InstanceDependency : Dependency
    {
        private readonly object _instance;
        
        public InstanceDependency(DependencyTranslateInfo info, object instance) : base(info)
        {
            _instance = instance;
        }

        public override object Resolve() => _instance;
    }
}