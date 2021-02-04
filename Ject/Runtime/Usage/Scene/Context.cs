using System;
using Ject.Injection;
using Toolkit;

namespace Ject.Usage.Scene
{
    [Serializable]
    public class Context
    {
        public Identifier[] usedContractWriterIds = {};
        public InjectionInfo injectionInfo;
    }
}