using Ject.Usage.Scene;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace JectEditor.Hooks
{
    [InitializeOnLoad]
    public static class AddItemToSceneMenu
    {
        static AddItemToSceneMenu()
        {
            SceneHierarchyHooks.addItemsToSceneHeaderContextMenu += AddItemsToMenu;
        }

        private static void AddItemsToMenu(GenericMenu menu, Scene scene)
        {
            SceneContext sceneContext = GetSceneContext(scene);

            var content = new GUIContent("Add context"); 
            
            menu.AddSeparator("");
            if (sceneContext == null || sceneContext.extraContexts.ContainsKey("Scene"))
            {
                menu.AddDisabledItem(content);
                return;
            }

            menu.AddItem(content, false, () => sceneContext.extraContexts["Scene"] = new Context());
        }

        private static SceneContext GetSceneContext(Scene scene)
        {
            foreach (GameObject rootGameObject in scene.GetRootGameObjects())
            {
                if (rootGameObject.TryGetComponent(out SceneContext sceneContext))
                {
                    return sceneContext;
                }
            }

            return null;
        }
    }
}