using UnityEditor;
using UnityEngine;

namespace JectEditor.Toolkit
{
    public static class ButtonDrawers
    {
        public static Vector2 ButtonSize = new Vector2(20, 18);

        public static bool Minus(Rect position) 
            => GUI.Button(position, MinusIcon(), EditorStyles.label);

        public static bool Plus(Rect position) 
            => GUI.Button(position, PlusIcon(), EditorStyles.label);
        
        public static bool Minus() 
           => GUILayout.Button(MinusIcon(), EditorStyles.label, 
               GUILayout.Width(ButtonSize.x), GUILayout.Height(ButtonSize.y));

       public static bool Plus() 
           => GUILayout.Button(PlusIcon(), EditorStyles.label, 
               GUILayout.Width(ButtonSize.x), GUILayout.Height(ButtonSize.y));

       public static GUIContent MinusIcon() 
           => new GUIContent(EditorGUIUtility.IconContent("d_Toolbar Minus@2x").image, "Remove");
       
       public static GUIContent PlusIcon() 
           => new GUIContent(EditorGUIUtility.IconContent("d_CreateAddNew").image, "Add");
    }
}