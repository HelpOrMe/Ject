using Ject.Contracts;

namespace Ject.Dependencies
{
    public class Dependency : IDependency
    {
        public DependencyTranslateInfo TranslateInfo => new DependencyTranslateInfo(Writer, Description);
        public DependencyDescription Description { get; }
        
        protected readonly IDependencyWriter Writer;
        protected ISignedContract Contract;

        protected Dependency(DependencyTranslateInfo info)
        {
            Writer = info.Writer;
            Description = info.Description;
            
            Writer.Rewrite(this);
        }

        public void SignWith(ISignedContract contract)
        {
            Contract = contract;
        }

        public virtual bool CanBeResolved() => true;

        public virtual object Resolve() => throw new UnfinishedDependencyException(this);
    }
}