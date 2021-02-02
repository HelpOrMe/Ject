using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Ject.Toolkit;
using Ject.Usage;
using Ject.Usage.Scene;
using JectEditor.Inspectors.Contexts;
using JectEditor.Toolkit;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace JectEditor.Inspectors
{
    [CustomEditor(typeof(SceneContext))]
    public class SceneContextEditor : Editor
    {
        private SceneContext SceneContext => (SceneContext)target;

        private ContextDrawer _sceneContextDrawer;
        
        private bool _contactWritersFoldout = true;
        private bool _sceneContextFoldout = true;
        
        private readonly Dictionary<Object, bool> _objContextFoldouts = new Dictionary<Object, bool>();
        private readonly Dictionary<Object, ContextDrawer> _objContextDrawers = new Dictionary<Object, ContextDrawer>();

        private void OnEnable()
        {
            IndexContracts();
        }

        private void IndexContracts()
        {
            ContractWriters.RawData.contractWriterTypeNames.Clear();
            
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (!Regex.IsMatch(assembly.GetName().Name, @"(Unity|System|Ject)"))
                {
                    IndexContracts(assembly);
                }
            }
            
            foreach (Identifier id in ContractWriters.RawData.contractWriterNames.Keys.ToList())
            {
                if (!ContractWriters.RawData.contractWriterTypeNames.ContainsKey(id))
                {
                    ContractWriters.RawData.contractWriterNames.Remove(id);
                }
            }
        }

        private void IndexContracts(Assembly assembly)
        {
            Type contractWriterType = typeof(ContractWriter);
            
            foreach (Type type in assembly.GetTypes())
            {
                if (!contractWriterType.IsAssignableFrom(type)) 
                    continue;
                
                var id = new Identifier(type.Name);
                ContractWriters.RawData.contractWriterTypeNames[id] = type.AssemblyQualifiedName;
                
                if (!ContractWriters.RawData.contractWriterNames.ContainsKey(id))
                {
                    ContractWriters.RawData.contractWriterNames[id] = type.Name;
                }
            }
        }
        
        public override void OnInspectorGUI()
        {
            if (ContractWriters.RawData.contractWriterTypeNames.Count == 0)
            {
                EditorGUILayout.HelpBox("There are no contract writers." +
                                        "\nAssets/Create/Ject/C# Contract writer", MessageType.Error);
                return;
            }
            
            Draw();
            Repaint();
        }

        private void Draw()
        {
            DrawContractWriters();

            if (SceneContext.extraContexts.ContainsKey("Scene"))
            {
                EditorGUILayout.Separator();
                DrawSceneContext();
            }
            
            DrawContexts();
        }

        private void DrawSceneContext()
        {
            _sceneContextDrawer ??= new WritableContextDrawer(SceneContext.extraContexts["Scene"],
                ContractWriters.RawData, "SceneContext");
            
            EditorGUILayout.BeginHorizontal();
            
            _sceneContextFoldout = EditorGUILayout.Foldout(_sceneContextFoldout, "Scene context");
            if (ButtonDrawers.Minus())
            {
                SceneContext.extraContexts.Remove("Scene");
                _sceneContextDrawer = null;
                return;
            }
            
            EditorGUILayout.EndHorizontal();
            
            if (_sceneContextFoldout)
            {
                _sceneContextDrawer.Draw(); 
            }
        }
        
        private void DrawContractWriters()
        {
            _contactWritersFoldout = EditorGUILayout.Foldout(_contactWritersFoldout, "Contract writers", true);
            
            if (!_contactWritersFoldout) 
                return;
            
            foreach (Identifier identifier in ContractWriters.RawData.contractWriterTypeNames.Keys.ToArray())
            {
                DrawContractWritersEntry(identifier);
            }
        }

        private void DrawContractWritersEntry(Identifier id)
        {
            GUILayout.BeginHorizontal();
            DrawContractWriterKey(id);
            DrawContractWriterValue(id);
            GUILayout.EndHorizontal();
        }

        private void DrawContractWriterKey(Identifier id)
        {
            ContractWriters.RawData.contractWriterNames[id] = EditorGUILayout.TextField(
                ContractWriters.RawData.contractWriterNames[id]);
        }

        private void DrawContractWriterValue(Identifier id)
        {
            GUI.enabled = false;
            EditorGUILayout.LabelField(ContractWriters.RawData.contractWriterTypeNames[id], EditorStyles.textField);
            GUI.enabled = true;
        }

        private void DrawContexts()
        {
            if (SceneContext.objectContexts.Count > 0)
            {
                EditorGUILayout.Separator();
                DrawObjectContexts();
            }

            if (SceneContext.componentsUnderContext.Count > 0)
            {
                EditorGUILayout.Separator();
                DrawComponentsUnderContext();
            }
        }

        private void DrawObjectContexts()
        {
            EditorGUILayout.LabelField("Object contexts", EditorStyles.boldLabel);
            
            foreach (Object obj in SceneContext.objectContexts.Keys.ToArray())
            {
                if (obj == null)
                {
                    SceneContext.objectContexts.Remove(obj);
                    continue;
                }
                
                DrawObjectContextFoldout(obj);
                if (_objContextFoldouts[obj])
                {
                    EditorGUI.indentLevel++;
                    GetOrCreateContextDrawer(obj).Draw();
                    EditorGUI.indentLevel--;
                }
            }
        }

        private void DrawObjectContextFoldout(Object obj)
        {
            if (!_objContextFoldouts.ContainsKey(obj))
            {
                _objContextFoldouts[obj] = true;
            }
            
            EditorGUILayout.BeginHorizontal();
            _objContextFoldouts[obj] = EditorGUILayout.Foldout(_objContextFoldouts[obj], obj.ToString(), true);
            
            if (ButtonDrawers.Minus())
            {
                SceneContext.objectContexts.Remove(obj);
            }
            EditorGUILayout.EndHorizontal();
        }

        private void DrawComponentsUnderContext()
        {
            EditorGUILayout.LabelField("Components under context", EditorStyles.boldLabel);

            foreach (Component component in SceneContext.componentsUnderContext.ToArray())
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(component.ToString());
                if (ButtonDrawers.Minus())
                {
                    SceneContext.componentsUnderContext.Remove(component);
                    SceneContext.objectContexts.Remove(component);
                }
                EditorGUILayout.EndHorizontal();
            }
        }
        
        private ContextDrawer GetOrCreateContextDrawer(Object obj)
        {
            if (!_objContextDrawers.ContainsKey(obj))
            {
                Context context = SceneContext.objectContexts[obj];
                _objContextDrawers[obj] = (obj is Component)
                    ? (ContextDrawer) new TypedContextDrawer(context, ContractWriters.RawData, obj.GetType())
                    : new WritableContextDrawer(context, ContractWriters.RawData, obj.name);
            }
            
            return _objContextDrawers[obj];
        }
    }
}