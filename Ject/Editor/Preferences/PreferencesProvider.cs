using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JectEditor.Preferences
{
    public class PreferencesProvider : SettingsProvider
    {
        public event Action OnPreferencesUpdated;
        
        private readonly SerializedObject _serializedObject;
        
        public PreferencesProvider(string path, SettingsScope scope, Object targetObject) 
            : base(path, scope)
        {
            _serializedObject = new SerializedObject(targetObject);
        }

        public override void OnGUI(string searchContext)
        {
            SerializedProperty iterator = _serializedObject.GetIterator();
            iterator.NextVisible(true);  // Skip script field

            GUILayout.Space(10);
            while (iterator.NextVisible(true))
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(10);
                EditorGUILayout.PropertyField(iterator);
                EditorGUILayout.EndHorizontal();
            }

            if (_serializedObject.hasModifiedProperties)
            {
                _serializedObject.ApplyModifiedProperties();
                OnPreferencesUpdated?.Invoke();
            }
        }
    }
}