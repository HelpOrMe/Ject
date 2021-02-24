using JectEditor.Preferences;
using JectEditor.Toolkit.Extensions;
using UnityEditor;
using UnityEngine;

namespace JectEditor.Inspectors.Contexts
{
    public static class TextStyles
    {
        public static GUIStyle ModStyle => _modStyle ??= EditorStyles.label.ColoredClone(
            PreferencesManager.Preferences.modColor);
        private static GUIStyle _modStyle;
        
        public static GUIStyle TypeStyle => _typeStyle ??= EditorStyles.label.ColoredClone(
            PreferencesManager.Preferences.typeColor);
        private static GUIStyle _typeStyle;
        
        public static GUIStyle TextStyle => _textStyle ??= EditorStyles.label.ColoredClone(
            PreferencesManager.Preferences.textColor);
        private static GUIStyle _textStyle;
        
        public static GUIStyle ValueStyle => _valueStyle ??= EditorStyles.boldLabel.ColoredClone(
            PreferencesManager.Preferences.valueColor);
        private static GUIStyle _valueStyle;
        
        public static GUIStyle ErrorStyle => _errorStyle ??= EditorStyles.boldLabel.ColoredClone(
            PreferencesManager.Preferences.errorColor);
        private static GUIStyle _errorStyle;
        
        public static GUIStyle LineEnumStyle => _lineEnumStyle ??= EditorStyles.label.ColoredClone(
            PreferencesManager.Preferences.lineEnumColor);
        private static GUIStyle _lineEnumStyle;
    }
}