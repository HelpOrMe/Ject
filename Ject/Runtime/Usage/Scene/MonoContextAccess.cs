using System.Collections.Generic;
using UnityEngine;

namespace Ject.Usage.Scene
{
    public abstract class MonoContextAccess : MonoBehaviour
    {
        public abstract Dictionary<Component, List<Context>> GetComponentContexts();
    }
}