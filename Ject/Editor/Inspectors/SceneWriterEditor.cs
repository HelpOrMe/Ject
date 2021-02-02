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

            if (SceneWriter.contextAccess == null)
            {
                if (SceneWriter.TryGetComponent(out SceneWriter.contextAccess))
                    return;

                SceneWriter.entrypoint = SceneWriter.WriteEntrypoint.None;

                const string message = "Access only from scripts. \nAdd SceneContext component to use it.";
                EditorGUILayout.HelpBox(message, MessageType.Info);
            }
            
            DrawInspector();
        }

        public void DrawInspector()
        {
            SceneWriter.entrypoint = (SceneWriter.WriteEntrypoint)
                EditorGUILayout.EnumPopup("Entrypoint", SceneWriter.entrypoint);

            if (SceneWriter.entrypoint == SceneWriter.WriteEntrypoint.None)
                return;
         
            string[] options = new string[_methods.Length];
            
            for (int i = 0; i < _methods.Length; i++)
            {
                options[i] = _methods[i];
                if (options[i] != nameof(SceneWriter.Write))
                {
                    options[i] += " (Partial)";
                }
            }
            
            int index = Array.IndexOf(_methods, SceneWriter.writeMethodName);
            index = EditorGUILayout.Popup("Write Method", index, options);
                
            if (index >= 0 && index < _methods.Length)
            {
                string methodName = _methods[index];
                SceneWriter.usePartialMethod = methodName != nameof(SceneWriter.Write);
                SceneWriter.writeMethodName = methodName;
            }
        }
    }
}