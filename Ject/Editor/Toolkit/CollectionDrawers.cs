using System.Collections.Generic;
using UnityEngine;

namespace JectEditor.Toolkit
{
    public static class CollectionDrawers
    { 
        public static bool Selection(string label, string[] selectionArray, ref string[] currentArray,
            string itemName, bool foldout) 
            => Selection(new GUIContent(label), selectionArray, ref currentArray, itemName, foldout);
        
        public static bool Selection(GUIContent label, string[] selectionArray, ref string[] currentArray,
            string itemName, bool foldout) 
            => Internal.CollectionDrawers.Selection(label, selectionArray, ref currentArray, itemName, foldout);

        public static void Selection(string[] selectionArray, ref string[] currentArray, string itemName)
            => Internal.CollectionDrawers.Selection(selectionArray, ref currentArray, itemName);
        
        public static bool Selection(string label, List<string> selectionList, List<string> currentList,
            string itemName, bool foldout) 
            => Selection(new GUIContent(label), selectionList, currentList, itemName, foldout);
        
        public static bool Selection(GUIContent label, List<string> selectionList, List<string> currentList,
            string itemName, bool foldout) 
            => Internal.CollectionDrawers.Selection(label, selectionList, currentList, itemName, foldout);
        
        public static void Selection(List<string> selectionList, List<string> currentList, string itemName) 
            => Internal.CollectionDrawers.Selection(selectionList, currentList, itemName);
        
        public static bool Dictionary<TKey, TValue>(string label, Dictionary<TKey, TValue> dictionary,
            TKey defaultKey, TValue defaultValue, bool foldout)
            => Dictionary(new GUIContent(label), dictionary, defaultKey, defaultValue, foldout);
        
        public static bool Dictionary<TKey, TValue>(GUIContent label, Dictionary<TKey, TValue> dictionary,
            TKey defaultKey, TValue defaultValue, bool foldout)
            => Internal.CollectionDrawers.Dictionary(label, dictionary, defaultKey, defaultValue, foldout,
                Drawers.Draw,
                (_, value) => Drawers.Draw(value)
            );
        
        public static void Dictionary<TKey, TValue>(Dictionary<TKey, TValue> dictionary,
            TKey defaultKey, TValue defaultValue)
            => Internal.CollectionDrawers.Dictionary(dictionary, defaultKey, defaultValue,
                Drawers.Draw,
                (_, value) => Drawers.Draw(value)
            );
        
        public static bool List<T>(string label, List<T> list, T defaultValue, bool foldout)
            => List(new GUIContent(label), list, defaultValue, foldout);

        public static bool List<T>(GUIContent label, List<T> list, T defaultValue, bool foldout) 
            => Internal.CollectionDrawers.List(label, list, defaultValue, foldout, 
                (item, i) => Drawers.Draw(item));
        
        public static void List<T>(List<T> list, T defaultValue) 
            => Internal.CollectionDrawers.List(list, defaultValue, (item, i) => Drawers.Draw(item));
        
        public static bool Array<T>(string label, ref T[] array, T defaultValue, bool foldout) 
            => Array(new GUIContent(label), ref array, defaultValue, foldout);
        
        public static bool Array<T>(GUIContent label, ref T[] array, T defaultValue, bool foldout)
            => Internal.CollectionDrawers.Array(label, ref array, defaultValue, foldout, 
                (item, i) => Drawers.Draw(item));
        
        public static void Array<T>(ref T[] array, T defaultValue)
            => Internal.CollectionDrawers.Array(ref array, defaultValue, (item, i) => Drawers.Draw(item));
    }
}