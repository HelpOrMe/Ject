using System;
using System.Linq;
using Ject.Usage.Scene;
using UnityEditor;

namespace JectEditor.Inspectors
{
    [CustomEditor(typeof(SceneWriter))]
    public class SceneWriterEditor : Editor
    {
        private SceneWriter SceneWriter => (SceneWriter)target;
        private static string[] _methods;
        
        public override void OnInspectorGUI()
        {
            Type type = typeof(SceneWriter);
            _methods ??= type.GetMethods().Select(m => m.Name).Where(n => n.StartsWith("Write")).ToArray();
            
            if (SceneWriter.sceneContext == null)
            {
                if (SceneWriter.TryGetComponent(out SceneWriter.sceneContext))
                    return;

                SceneWriter.entrypoint = SceneWriter.WriteEntrypoint.None;
                
                const string message = "Access only from scripts. \nAdd SceneContext component to use it.";
                EditorGUILayout.HelpBox(message, MessageType.Info);
                return;
            }

            SceneWriter.entrypoint = (SceneWriter.WriteEntrypoint)
                EditorGUILayout.EnumPopup("Entrypoint", SceneWriter.entrypoint);

            if (SceneWriter.entrypoint != SceneWriter.WriteEntrypoint.None)
            {
                int index = Array.IndexOf(_methods, SceneWriter.writeMethodName);
                index = EditorGUILayout.Popup("Write Method", index, _methods);
                
                if (index >= 0 && index < _methods.Length)
                {
                    SceneWriter.writeMethodName = _methods[index];
                }
            }
        }
    }
}