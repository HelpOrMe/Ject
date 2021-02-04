﻿using System;
using Toolkit;
using Toolkit.Collections;

namespace Ject.Usage
{
    [Serializable]
    public class ContractWritersRawData
    {
        public SerializableDictionary<Identifier, string> contractWriterTypeNames;
        public SerializableDictionary<Identifier, string> contractWriterNames;
    }
}