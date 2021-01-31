using System.Collections.Generic;
using Ject.Toolkit;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Ject.Usage.Scene
{
    public class SceneContext : MonoBehaviour
    {
        public Context sceneContext;
        
        [SerializeField] 
        public SerializableDictionary<Object, Context> objectContexts = new SerializableDictionary<Object, Context>();
        [SerializeField] 
        public List<Component> componentsUnderContext = new List<Component>();

        [MenuItem("GameObject/Add Context", true)]
        private static bool AddObjectContextValidate(MenuCommand command)
        {
            var sceneContext = FindObjectOfType<SceneContext>();
            
            return sceneContext != null 
                   && Selection.activeObject != null
                   && !sceneContext.objectContexts.ContainsKey(Selection.activeObject);
        }
        
        [MenuItem("GameObject/Add Context", false, -1)]
        [MenuItem("CONTEXT/Component/Add Context")]
        private static void AddObjectContext(MenuCommand command)
        {
            var sceneContext = FindObjectOfType<SceneContext>();
            if (sceneContext != null && !sceneContext.objectContexts.ContainsKey(command.context))
            {
                sceneContext.objectContexts.Add(command.context, new Context());
                if (command.context is Component component 
                    && !sceneContext.componentsUnderContext.Contains(component))
                {
                    sceneContext.componentsUnderContext.Add(component);
                }
            }
        }
        
        [MenuItem("CONTEXT/Component/Add to Context")]
        private static void AddObjectToContext(MenuCommand command)
        {
            var sceneContext = FindObjectOfType<SceneContext>();
            var component = (Component)command.context;
            
            if (sceneContext != null && !sceneContext.componentsUnderContext.Contains(component))
            {
                sceneContext.componentsUnderContext.Add(component);
            }
        }

        public Dictionary<Component, List<Context>> GetComponentContexts()
        {
            var componentsContexts = new Dictionary<Component, List<Context>>();
            foreach (Component component in componentsUnderContext)
            {
                componentsContexts[component] = GetObjectContexts(component);
            }

            return componentsContexts;
        }

        public List<Context> GetObjectContexts(Object obj)
        {
            var contexts = new List<Context>();

            if (objectContexts.ContainsKey(obj))
            {
                contexts.Add(objectContexts[obj]);
            }

            for (Transform iparent = GetContextParentOf(obj); iparent != null; iparent = iparent.parent)
            {
                if (objectContexts.ContainsKey(iparent.gameObject))
                {
                    contexts.Add(objectContexts[iparent.gameObject]);
                }
            }
            
            contexts.Add(sceneContext);
            
            return contexts;
        }

        private Transform GetContextParentOf(Object obj) =>
            obj switch
            {
                GameObject gameObj => gameObj.transform.transform,
                Component comp => comp.transform,
                _ => null
            };
    }
}