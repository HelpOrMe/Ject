using UnityEditor;
using UnityEngine;

namespace JectEditor.Preferences
{
    public static class PreferencesManager
    {
        public const string PreferencesAssetPath = "Packages/ject/Resources/Preferences.asset";
        
        public static PreferencesAsset Preferences => _preferences ??= LoadOrCreate();
        private static PreferencesAsset _preferences;
        public static PreferencesProvider Provider => _provider ??= NewProvider();
        private static PreferencesProvider _provider;

        public static PreferencesAsset LoadOrCreate()
        {
            var preferences = AssetDatabase.LoadAssetAtPath<PreferencesAsset>(PreferencesAssetPath);
            if (preferences == null)
            {
                preferences = ScriptableObject.CreateInstance<PreferencesAsset>();
                AssetDatabase.CreateAsset(preferences, PreferencesAssetPath);
            }
            
            return preferences;
        }

        public static PreferencesProvider NewProvider() 
            => _provider = new PreferencesProvider("Packages/Ject", SettingsScope.User, Preferences);
    }
}