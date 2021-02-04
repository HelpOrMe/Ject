using System.Collections.Generic;
using System.Threading.Tasks;
using Toolkit;
using UnityEngine;

namespace Ject.Usage.Scene
{
    public partial class SceneWriter
    {
        public async Task WriteAsync(Dictionary<Component, List<Context>> componentContexts)
        {
            BeforeWrite?.Invoke();
            
            foreach (Identifier id in ContractWriters.ContractIds)
            {
                contracts[id] = ContractWriters.WriteContract(id);
                await Task.Yield();
            }
            
            foreach (Component component in componentContexts.Keys)
            {
                WriteComponent(component, componentContexts[component]);
                await Task.Yield();
            }
            
            AfterWrite?.Invoke();
            
            contracts.Clear();
            contextContracts.Clear();
        }
    }
}