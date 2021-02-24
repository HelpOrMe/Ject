using System;
using Toolkit;
using Toolkit.Serializable;

namespace Ject.Usage
{
    [Serializable]
    public class ContractWritersRawData
    {
        public SerializableDictionary<Identifier, string> contractWriterTypeNames;
        public SerializableDictionary<Identifier, string> contractWriterNames;
    }
}