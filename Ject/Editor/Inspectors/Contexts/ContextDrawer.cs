using System;
using System.Collections.Generic;
using System.Linq;
using Ject.Injection;
using Ject.Toolkit;
using Ject.Usage;
using Ject.Usage.Scene;
using JectEditor.Preferences;
using ToolkitEditor;
using ToolkitEditor.Extensions;
using UnityEditor;
using UnityEngine;

namespace JectEditor.Inspectors.Contexts
{
    public abstract class ContextDrawer
    {
        protected readonly Context Context;
        protected readonly ContractWritersRawData WritersRawData;

        private readonly List<Rect> _lastNonLayoutRects = new List<Rect>();
        
        protected static readonly Dictionary<string, string> SpecialTypeNames = new Dictionary<string, string>
        {
            ["Void"] = "void",
            ["Int32"] = "int",
            ["Single"] = "float",
            ["Double"] = "double",
            ["String"] = "string",
            ["Boolean"] = "bool"
        };
        
        protected InjectionInfo InjectionInfo => Context.injectionInfo;
        
        protected Rect Rect;
        protected Rect LastRect;
        protected int Indent;
        private int _lineCount;
        private int _prevGUIIndent;
        
        protected ContextDrawer(Context context, ContractWritersRawData writersRawData)
        {
            Context = context;
            WritersRawData = writersRawData;
        }
        
        public void Draw()
        {
            _lineCount = 0;
            Indent = 0;

            _prevGUIIndent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;
            DrawBody();
            EditorGUI.indentLevel = _prevGUIIndent;
        }

        protected abstract void DrawBody();
        
        protected void DrawHeader(string className)
        {
            foreach (Identifier id in Context.usedContractWriterIds.ToList())
            {
                DrawUsingContract(id);
            }
            
            DrawAddContract();
            
            BeginLine();
            DrawLabel("public class", TextStyles.ModStyle);
            DrawLabel(className, TextStyles.TypeStyle);
            BeginLine();
            DrawLabel("{", TextStyles.TextStyle);
        }
        
        protected void DrawUsingContract(Identifier id)
        {
            BeginLine();
            
            DrawLabel("using", TextStyles.ModStyle);
            if (WritersRawData.contractWriterNames.ContainsKey(id))
                DrawLabel(WritersRawData.contractWriterNames[id], TextStyles.TextStyle);
            else 
                DrawLabel("ErrorContractName", TextStyles.ErrorStyle);
            DrawLabel(";", TextStyles.TextStyle, -1);
            
            if (RemoveButton())
            {
                Context.usedContractWriterIds = Context.usedContractWriterIds.Where(other => id != other).ToArray();
            }
        }
        
        protected bool AddButton() 
            => Rect.Contains(Event.current.mousePosition) && DrawButton(ButtonDrawers.PlusIcon(), EditorStyles.label);
        
        protected bool RemoveButton() 
            => Rect.Contains(Event.current.mousePosition) && DrawButton(ButtonDrawers.MinusIcon(), EditorStyles.label);
        
        protected void DrawAddContract()
        {
            BeginLine();
            var names = new List<string>{"Select contract.."};
            var ids = new List<Identifier>();
            
            foreach (Identifier id in WritersRawData.contractWriterNames.Keys)
            {
                if (!Context.usedContractWriterIds.Contains(id))
                {
                    names.Add(WritersRawData.contractWriterNames[id]);
                    ids.Add(id);
                }
            }

            if (names.Count == 1)
                return;
            
            int selected = DrawPopup(0, names.ToArray(), TextStyles.TextStyle.FontStyledClone(FontStyle.Italic));
            if (selected > 0)
            {
                AddToArray(ref Context.usedContractWriterIds, ids[selected - 1]);
            }
        }
        
        protected void DrawFooter()
        {
            BeginLine();
            DrawLabel("}", TextStyles.TextStyle);
        }

        protected void BeginLine()
        {
            if (Event.current.type != EventType.Layout)
            {
                Rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.label);
                LastRect = new Rect(Rect.x + 4, Rect.y, 17, Rect.height);
                _lastNonLayoutRects.Insert(_lineCount, Rect);
            }
            else
            {
                Rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.label);
                LastRect = new Rect(Rect.x + 4, Rect.y, 17, Rect.height);
                if (_lineCount < _lastNonLayoutRects.Count)
                {
                    Rect = _lastNonLayoutRects[_lineCount];
                }
            }
            
            EditorGUI.DrawRect(Rect.AddH(2), PreferencesManager.Preferences.bgColor);
            
            EditorGUI.LabelField(LastRect, (++_lineCount).ToString(), TextStyles.LineEnumStyle);
            LastRect.x += Indent * 10 + 5;
        }

        protected void DrawParameterDependencyId(string methodName, string parameterName)
        {
            if (InjectionInfo.methodDependencyIds.ContainsKey(methodName))
            {
                DrawMemberDependencyId(InjectionInfo.methodDependencyIds[methodName], parameterName);

                if (InjectionInfo.methodDependencyIds[methodName].Count == 0)
                {
                    InjectionInfo.methodDependencyIds.Remove(methodName);
                }
                return;
            }
            
            var parameterDependencyIds = new SerializableDictionary<string, Identifier>();
            DrawMemberDependencyId(parameterDependencyIds, parameterName);
            if (parameterDependencyIds.Count > 0)
            {
                InjectionInfo.methodDependencyIds[methodName] = parameterDependencyIds;
            }
        }
        
        protected void DrawMemberDependencyId(Dictionary<string, Identifier> dependencies, string memberName)
        {
            DrawLabel("=", TextStyles.TextStyle);
            if (dependencies.ContainsKey(memberName))
            {
                string text = DrawText(dependencies[memberName].ToString(), TextStyles.ValueStyle);
                if (text.Replace(" ", "") == "")
                {
                    dependencies.Remove(memberName);
                    return;
                }
                dependencies[memberName] = new Identifier(text);
                return;
            }
            {
                const string baseMessage = "None";
                string text = DrawText(baseMessage, TextStyles.ValueStyle);
                if (baseMessage != text)
                {
                    dependencies[memberName] = new Identifier(text);
                }
            }
        }
        
        protected virtual void DrawType(Type type, float space = 4)
        {
            if (PreferencesManager.Preferences.simpleTypes)
            {
                DrawLabel("var", TextStyles.ModStyle, space);
                return;
            }
            DrawFullType(type, space);
        }

        private void DrawFullType(Type type, float space = 4)
        {
            string typeName = SpecialTypeNames.ContainsKey(type.Name) ? SpecialTypeNames[type.Name] : type.Name;
            GUIStyle style = char.IsLower(typeName[0]) ? TextStyles.ModStyle : TextStyles.TypeStyle;

            if (type.GenericTypeArguments.Length > 0)
            {
                typeName = typeName.Split('`')[0];
                DrawLabel(typeName, style, space);
                DrawLabel("<", TextStyles.TextStyle, -1);
                
                foreach (Type genericType in type.GenericTypeArguments)
                {
                    DrawType(genericType, -1);
                    
                    if (type.GenericTypeArguments[type.GenericTypeArguments.GetUpperBound(0)] != genericType)
                    {
                        DrawLabel(", ", TextStyles.TextStyle, -1);
                    }
                }
                DrawLabel(">", TextStyles.TextStyle, -1);
            }
            else
            {
                DrawLabel(typeName, style, space);
            }
        }
        
        protected virtual void DrawLabel(string label, GUIStyle style, float space = 4) 
            => DrawContent(new GUIContent(label), style, space);
        
        protected virtual void DrawContent(GUIContent content, GUIStyle style, float space = 4)
        {
            float size = style.CalcSize(content).x;
            LastRect = LastRect.WithXW(LastRect.xMax + space, size);
            EditorGUI.LabelField(LastRect, content, style);
        }

        protected virtual string DrawText(string text, GUIStyle style, float space = 4)
        {
            float size = style.CalcSize(new GUIContent(text)).x;
            LastRect = LastRect.WithXW(LastRect.xMax + space, size);
            return EditorGUI.TextField(LastRect, text, style);
        }
        
        protected virtual bool DrawButton(GUIContent content, GUIStyle style, float space = 4)
        {
            float size = style.CalcSize(content).x;
            LastRect = LastRect.WithXW(LastRect.xMax + space, size);
            return GUI.Button(LastRect.WithH(18), content, style);
        }
        
        protected virtual int DrawPopup(int selected, string[] options, GUIStyle style, float space = 4)
        {
            float size = style.CalcSize(new GUIContent(options[selected])).x;
            LastRect = LastRect.WithXW(LastRect.xMax + space, size);
            return EditorGUI.Popup(LastRect, selected, options, style);
        }
        
        protected void AddToArray<T>(ref T[] array, T value)
        {
            Array.Resize(ref array, array.Length + 1);
            array[array.GetUpperBound(0)] = value;
        }

        protected T[] RemoveFromArray<T>(T[] array, T value) 
            => array.Where(arrayValue => !arrayValue.Equals(value)).ToArray();
    }
}