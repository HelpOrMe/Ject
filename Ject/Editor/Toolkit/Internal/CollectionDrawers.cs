using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace JectEditor.Toolkit.Internal
{
    internal static class CollectionDrawers
    {
        public static bool Selection(GUIContent label, string[] selectionArray, ref string[] currentArray, 
            string itemName, bool foldout)
        {
            if (!EditorGUILayout.Foldout(foldout, label, true)) return false;
            Selection(selectionArray, ref currentArray, itemName);
            
            return true;
        }

        public static void Selection(string[] selectionArray, ref string[] currentArray, string itemName)
        {
            int stackIndex = 0;
            
            for (int i = 0; i < currentArray.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                GUI.enabled = false;
                if (selectionArray.Contains(currentArray[i]))
                    EditorGUILayout.TextField(currentArray[i]);
                else ErrorText($"<Removed {itemName}>");
                GUI.enabled = true;

                currentArray[stackIndex] = currentArray[i];
                
                if (!ButtonDrawers.Minus())
                {
                    stackIndex++;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (stackIndex != currentArray.Length)
            {
                System.Array.Resize(ref currentArray, stackIndex);
            }
            SelectionPopup(selectionArray, ref currentArray, itemName);
        }
        
        public  static void SelectionPopup(string[] selectionArray, ref string[] currentArray, string itemName)
        {
            var nonDuplicatedList = new List<string>();
            foreach (string item in selectionArray)
            {
                if (!currentArray.Contains(item))
                {
                    nonDuplicatedList.Add(item);
                }
            }
            
            var popupItems = new List<string> {$"Select {itemName}.."};
            popupItems.AddRange(nonDuplicatedList);
            if (popupItems.Count == 1) return;
            
            int index = EditorGUILayout.Popup(0, popupItems.ToArray());
            if (index >= 0)
            {
                System.Array.Resize(ref currentArray, currentArray.Length + 1);
                currentArray[currentArray.GetUpperBound(0)] = popupItems[index];
            }
        }
        
        public static bool Selection(GUIContent label, List<string> selectionList, List<string> currentList, 
            string itemName, bool foldout)
        {
            if (!EditorGUILayout.Foldout(foldout, label, true)) return false;
            Selection(selectionList, currentList, itemName);
            
            return true;
        }

        public static void Selection(List<string> selectionList, List<string> currentList, string itemName)
        {
            int indexToRemove = -1;
            
            for (int i = 0; i < currentList.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                
                GUI.enabled = false;
                if (selectionList.Contains(currentList[i]))
                    EditorGUILayout.TextField(currentList[i]);
                else ErrorText($"<Removed {itemName}>");
                GUI.enabled = true;

                if (ButtonDrawers.Minus())
                {
                    indexToRemove = i;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (indexToRemove > -1)
            {
                currentList.RemoveAt(indexToRemove);
            }

            SelectionPopup(selectionList, currentList, itemName);
        }
        
        public static void SelectionPopup(ICollection<string> selectionList, ICollection<string> currentList,
            string itemName)
        {
            var popupItems = new List<string> {$"Select {itemName}.."};
            popupItems.AddRange(selectionList.Where(item => !currentList.Contains(item)));

            if (popupItems.Count == 1) return;
            
            int index = EditorGUILayout.Popup(0, popupItems.ToArray());
            if (index > 0)
            {
                currentList.Add(popupItems[index]);
            }
        }
        
        private static void ErrorText(string text)
        {
            Color textColor = Color.Lerp(Color.red, Color.white, 0.3f);
            var style = new GUIStyle(EditorStyles.textField)
            {
                normal = { textColor = textColor },
                hover = { textColor = textColor },
                focused = { textColor = textColor },
            };
            EditorGUILayout.TextField(text, style);
        }
        
        public static bool Dictionary<TKey, TValue>(GUIContent label, Dictionary<TKey, TValue> dictionary, 
            TKey defaultKey, TValue defaultValue, bool foldout, 
            Func<TKey, TKey> keyGuiMethod, Func<TKey, TValue, TValue> valueGuiMethod)
        {
            EditorGUILayout.BeginHorizontal();
            foldout = EditorGUILayout.Foldout(foldout, label, true);
            if (foldout && ButtonDrawers.Plus() && !dictionary.ContainsKey(defaultKey))
            {
                dictionary.Add(defaultKey, defaultValue);
            }
            EditorGUILayout.EndHorizontal();

            if (!foldout) return false;
            
            Dictionary(dictionary, defaultKey, defaultValue, keyGuiMethod, valueGuiMethod);
            return true;
        }

        public static void Dictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary,
            TKey defaultKey, TValue defaultValue, 
            Func<TKey, TKey> keyGuiMethod, Func<TKey, TValue, TValue> valueGuiMethod)
        {
            foreach (KeyValuePair<TKey, TValue> entry in new Dictionary<TKey, TValue>(dictionary))
            {
                EditorGUILayout.BeginHorizontal();
                TKey newKey = keyGuiMethod(entry.Key);
                TValue newValue = valueGuiMethod(entry.Key, entry.Value);

                dictionary.Remove(entry.Key);
                if (!ButtonDrawers.Minus())
                {
                    dictionary[newKey] = newValue;
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        
        public static bool List<T>(GUIContent label, List<T> list, T defaultValue, bool foldout, 
            Func<T, int, T> guiMethod)
        {
            EditorGUILayout.BeginHorizontal();
            foldout = EditorGUILayout.Foldout(foldout, label, true);
            if (foldout && ButtonDrawers.Plus())
            {
                list.Add(defaultValue);
            }
            EditorGUILayout.EndHorizontal();
            
            if (!foldout) return false;
            
            List(list, defaultValue, guiMethod);
            return true;
        }

        public static void List<T>(List<T> list, T defaultValue, Func<T, int, T> guiMethod)
        {
            int indexToRemove = -1;
            
            for (int i = 0; i < list.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                list[i] = guiMethod(list[i], i);
                if (ButtonDrawers.Minus())
                {
                    indexToRemove = i;
                }
                EditorGUILayout.EndHorizontal();
            }

            if (indexToRemove > -1)
            {
                list.RemoveAt(indexToRemove);
            }
        }
        
        public static bool Array<T>(GUIContent label, ref T[] array, T defaultValue, bool foldout, 
            Func<T, int, T> guiMethod)
        {
            EditorGUILayout.BeginHorizontal();
            foldout = EditorGUILayout.Foldout(foldout, label, true);
            if (foldout && ButtonDrawers.Plus())
            {
                System.Array.Resize(ref array, array.Length + 1);
                array[array.GetUpperBound(0)] = defaultValue;
            }
            EditorGUILayout.EndHorizontal();

            if (!foldout) return false;
            
            Array(ref array, defaultValue, guiMethod);
            return true;
        }

        public static void Array<T>(ref T[] array, T defaultValue, Func<T, int, T> guiMethod)
        {
            int stackIndex = 0;
            for (int i = 0; i < array.Length; i++)
            {
                EditorGUILayout.BeginHorizontal();
                array[stackIndex] = guiMethod(array[i], i);
                if (!ButtonDrawers.Minus())
                {
                    stackIndex++;
                }
                EditorGUILayout.EndHorizontal();
            }
            
            if (stackIndex != array.Length)
            {
                System.Array.Resize(ref array, stackIndex);
            }
        }
    }
}