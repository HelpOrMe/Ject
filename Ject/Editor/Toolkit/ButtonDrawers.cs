using UnityEditor;
using UnityEngine;

namespace JectEditor.Toolkit
{
    public static class ButtonDrawers
    {
       public static bool Minus() 
           => GUILayout.Button(MinusIcon(), EditorStyles.label, GUILayout.Width(20), GUILayout.Height(18));

       public  static bool Plus() 
           => GUILayout.Button(PlusIcon(), EditorStyles.label, GUILayout.Width(20), GUILayout.Height(18));

       public static GUIContent MinusIcon() 
           => new GUIContent(EditorGUIUtility.IconContent("d_Toolbar Minus@2x").image, "Remove");
       
       public static GUIContent PlusIcon() 
           => new GUIContent(EditorGUIUtility.IconContent("d_CreateAddNew").image, "Add");
    }
}