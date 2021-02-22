using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ject.Contracts;
using Ject.Injection;
using Ject.Toolkit;
using UnityEditor;
using UnityEngine;

namespace Ject.Usage.Scene
{
    public partial class SceneWriter : MonoBehaviour
    {
        public event Action BeforeWrite;
        public event Action AfterWrite;
        public event Action<Component> BeforeComponentWrite;
        public event Action<Component> AfterComponentWrite;
        
        public WriteEntrypoint entrypoint = WriteEntrypoint.None;
        
        public bool usePartialMethod;
        public string writeMethodName = "Write";

        public MonoContextAccess contextAccess;

        protected readonly Dictionary<Identifier, ISignedContract> contracts = 
            new Dictionary<Identifier, ISignedContract>();
        
        protected readonly Dictionary<Context, ISignedContract> contextContracts = 
            new Dictionary<Context, ISignedContract>();
        
        public enum WriteEntrypoint
        {
            None,
            Awake,
            Start
        }
        
#if UNITY_EDITOR
        
        [MenuItem("Assets/Create/Ject/C# Contract Writer", false, 82)]
        private static void NewContractWriter()
        {
            ProjectWindowUtil.CreateScriptAssetFromTemplateFile(
                "Packages/ject/Resources/ScriptTemplates/C# Contract writer Template.cs.txt",
                "ContractWriter.cs");
        }
        
#endif        

        private void Awake()
        {
            if (entrypoint == WriteEntrypoint.Awake)
            {
                InvokeWriteMethod();
            }
        }

        private void Start()
        {
            if (entrypoint == WriteEntrypoint.Start)
            {
                InvokeWriteMethod();
            }
        }

        private void InvokeWriteMethod()
        {
            if (usePartialMethod)
            {
                InvokePartialMethod();
            }
            else
            {
                Write(contextAccess.GetComponentContexts());
            }
        }

        private void InvokePartialMethod()
        {
            MethodInfo method = GetType().GetMethod(writeMethodName);
            if (method == null)
                throw new MissingMemberException("Invalid partial write method name " + writeMethodName);

            method.Invoke(this, new object[] { contextAccess.GetComponentContexts() });
        }
        
        public void Write(Dictionary<Component, List<Context>> componentContexts)
        {
            BeforeWrite?.Invoke();
            
            foreach (Identifier id in ContractWriters.ContractIds)
            {
                contracts[id] = ContractWriters.WriteContract(id);
            }
        
            foreach (Component component in componentContexts.Keys)
            {
                BeforeComponentWrite?.Invoke(component);
                WriteComponent(component, componentContexts[component]);
                AfterComponentWrite?.Invoke(component);
            }
            
            AfterWrite?.Invoke();
        }

        protected void WriteComponent(Component component, List<Context> contexts)
        {
            var injector = new Injector(component);
            foreach (Context context in contexts)
            {
                if (context.usedContractWriterIds.Length == 0)
                {
                    Debug.LogWarning("One of the contexts does not use contracts!");
                    continue;
                }
                    
                injector.contract = GetContextContract(context);
                injector.Inject(context.injectionInfo);
            }
        }
        
        protected ISignedContract GetContextContract(Context context)
        {
            if (contextContracts.ContainsKey(context))
                return contextContracts[context];
            
            ISignedContract groupedContract = contracts[context.usedContractWriterIds.First()];
            for (int i = 1; i < context.usedContractWriterIds.Length; i++)
            {
                groupedContract.AddContract(contracts[context.usedContractWriterIds[i]]);
            }

            return contextContracts[context] = groupedContract;
        }
    }
}