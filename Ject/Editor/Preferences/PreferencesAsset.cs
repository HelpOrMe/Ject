using System;
using UnityEditor;
using UnityEngine;

namespace JectEditor.Preferences
{
    [Serializable]
    public class PreferencesAsset : ScriptableObject
    {
        [Space] 
        public bool simpleTypes;
        public bool methodParameters = true;
        public bool privateMembers = true;
        
        [Space]
        public Color bgColor = new Color32(30, 30, 30, 255);
        public Color modColor = new Color32(86, 156, 214, 255);
        public Color typeColor = new Color32(78, 201, 176, 255);
        public Color textColor = new Color32(220, 220, 220, 255);
        public Color valueColor = new Color32(75, 142, 46, 255);
        public Color errorColor = new Color32(198, 46, 46, 255);
        public Color lineEnumColor = new Color32(170, 170, 170, 255);

        
        [SettingsProvider]
        private static SettingsProvider CreateSettingsProvider() => PreferencesManager.Provider;
    }
}