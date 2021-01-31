using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JectEditor.Toolkit
{
    public static class Drawers
    {
        public static readonly Dictionary<Type, Func<object, object>> TypeDrawers =
            new Dictionary<Type, Func<object, object>>
            {
                [typeof(string)] = o => EditorGUILayout.TextField((string)o),
                [typeof(int)] = o => EditorGUILayout.IntField((int)o),
                [typeof(float)] = o => EditorGUILayout.FloatField((float)o),
                [typeof(bool)] = o => EditorGUILayout.Toggle((bool)o),
                [typeof(Object)] = o => EditorGUILayout.ObjectField((Object)o, o.GetType(), true)
            };
        
        public static T Draw<T>(T obj)
        {
            for (Type t = typeof(T); t != null; t = t.BaseType)
            {
                if (TypeDrawers.ContainsKey(t))
                {
                    return (T)TypeDrawers[t](obj);
                }
            }
            return obj;
        }
    }
}