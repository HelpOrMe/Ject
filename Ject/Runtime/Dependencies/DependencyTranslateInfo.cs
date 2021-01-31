using Ject.Contracts;

namespace Ject.Dependencies
{
    public readonly struct DependencyTranslateInfo
    {
        public readonly IDependencyWriter Writer;
        public readonly DependencyDescription Description;

        public DependencyTranslateInfo(IDependencyWriter writer, DependencyDescription description)
        {
            Writer = writer;
            Description = description;
        }
    }
}