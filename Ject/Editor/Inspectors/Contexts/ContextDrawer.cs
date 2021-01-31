using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Ject.Injection;
using Ject.Preferences;
using Ject.Toolkit;
using Ject.Usage;
using Ject.Usage.Scene;
using JectEditor.Toolkit;
using JectEditor.Toolkit.Extensions;
using UnityEditor;
using UnityEngine;

namespace JectEditor.Inspectors.Contexts
{
    public abstract class ContextDrawer
    {
        protected readonly Context Context;
        protected readonly ContractWritersRawData writersRawData;
        
        protected readonly Dictionary<string, string> methodCommentsBuffer = new Dictionary<string, string>();
        protected static readonly Dictionary<string, string> SpecialTypeNames = new Dictionary<string, string>
        {
            ["Void"] = "void",
            ["Int32"] = "int",
            ["Single"] = "float",
            ["Double"] = "double",
            ["String"] = "string",
            ["Boolean"] = "bool"
        };
        
        protected GUIStyle ModStyle => _modStyle ??= EditorStyles.label.ColoredClone(
            PreferencesManager.Preferences.modColor);
        private GUIStyle _modStyle;
        protected GUIStyle TypeStyle => _typeStyle ??= EditorStyles.label.ColoredClone(
            PreferencesManager.Preferences.typeColor);
        private GUIStyle _typeStyle;
        protected GUIStyle TextStyle => _textStyle ??= EditorStyles.label.ColoredClone(
            PreferencesManager.Preferences.textColor);
        private GUIStyle _textStyle;
        protected GUIStyle CommentStyle => _commentStyle ??= EditorStyles.boldLabel.ColoredClone(
            PreferencesManager.Preferences.commentColor);
        private GUIStyle _commentStyle;
        protected GUIStyle ErrorStyle => _errorStyle ??= EditorStyles.boldLabel.ColoredClone(
            PreferencesManager.Preferences.errorColor);
        private GUIStyle _errorStyle;
        protected GUIStyle LineEnumStyle => _lineEnumStyle ??= EditorStyles.label.ColoredClone(
            PreferencesManager.Preferences.lineEnumColor);
        private GUIStyle _lineEnumStyle;
        
        protected InjectionInfo InjectionInfo => Context.injectionInfo;
        
        protected Rect Rect;
        protected Rect LastRect;
        protected int Indent;
        private int _lineCount;
        private int _prevGUIIndent;
        
        protected ContextDrawer(Context context, ContractWritersRawData writersRawData)
        {
            Context = context;
            this.writersRawData = writersRawData;
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
            DrawLabel("public class", ModStyle);
            DrawLabel(className, TypeStyle);
            BeginLine();
            DrawLabel("{", TextStyle);
        }
        
        protected void DrawUsingContract(Identifier id)
        {
            BeginLine();
            
            DrawLabel("using", ModStyle);
            if (writersRawData.contractWriterNames.ContainsKey(id))
                DrawLabel(writersRawData.contractWriterNames[id], TextStyle);
            else 
                DrawLabel("ErrorContractName", ErrorStyle);
            DrawLabel(";", TextStyle, -1);
            
            if (RemoveButton())
            {
                Context.usedContractWriterIds = Context.usedContractWriterIds.Where(other => id != other).ToArray();
            }
        }
        
        protected void DrawComment(Dictionary<string, Identifier> ids, string name)
        {
            DrawLabel("//", CommentStyle, 5);
                
            const string message = "Id";
            Identifier id = ids.ContainsKey(name) ? ids[name] : new Identifier(message);
                
            string comment = DrawText(id.ToString(), CommentStyle);
             
            if (comment.Length == 0)
            {
                ids.Remove(name);
                return;
            }
            if (comment != message)
            {
                ids[name] = new Identifier(comment);
            }
        }

        protected void DrawMethodComment(string name)
        {
            DrawLabel("//", CommentStyle, 5);
            
            if (!InjectionInfo.methodDependencyIds.ContainsKey(name))
            {
                DrawMethodCommentWelcome(name);
                return;
            }
            
            if (!methodCommentsBuffer.ContainsKey(name))
            {
                FillMethodCommentBuffer(name);
            }

            string comment = DrawText(methodCommentsBuffer[name], CommentStyle);

            if (comment.Length == 0)
            {
                methodCommentsBuffer.Remove(name);
                InjectionInfo.methodDependencyIds.Remove(name);
                return;
            }

            methodCommentsBuffer[name] = comment;
            InjectionInfo.methodDependencyIds[name].Clear();
            
            foreach (string idPairComment in Regex.Split(methodCommentsBuffer[name], " *, *"))
            {
                string[] idPair = Regex.Split(idPairComment, " *: *");
                if (idPair.Length == 2)
                {
                    InjectionInfo.methodDependencyIds[name][idPair[0]] = new Identifier(idPair[1]);
                }
            }
        }

        protected void DrawMethodCommentWelcome(string name)
        {
            const string message = "Id";
            string comment = DrawText(message, CommentStyle);

            if (comment != message)
            {
                InjectionInfo.methodDependencyIds[name] = new SerializableDictionary<string, Identifier>();
                methodCommentsBuffer[name] = comment;
            }
        }

        protected void FillMethodCommentBuffer(string name)
        {
            var commentPairs = new List<string>();
            foreach (KeyValuePair<string, Identifier> pair in InjectionInfo.methodDependencyIds[name])
            {
                commentPairs.Add($"{pair.Key}: {pair.Value}");
            }

            methodCommentsBuffer[name] = string.Join(", ", commentPairs);
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
            
            foreach (Identifier id in writersRawData.contractWriterNames.Keys)
            {
                if (!Context.usedContractWriterIds.Contains(id))
                {
                    names.Add(writersRawData.contractWriterNames[id]);
                    ids.Add(id);
                }
            }

            if (names.Count == 1)
                return;
            
            int selected = DrawPopup(0, names.ToArray(), new GUIStyle(TextStyle) {fontStyle = FontStyle.Italic});
            if (selected > 0)
            {
                Array.Resize(ref Context.usedContractWriterIds, Context.usedContractWriterIds.Length + 1);
                int upperBound = Context.usedContractWriterIds.GetUpperBound(0);
                Context.usedContractWriterIds[upperBound] = ids[selected - 1];
            }
        }
        
        protected void DrawFooter()
        {
            BeginLine();
            DrawLabel("}", TextStyle);
        }
        
        protected void BeginLine()
        {
            Rect = GUILayoutUtility.GetRect(GUIContent.none, EditorStyles.label);
            EditorGUI.DrawRect(Rect.AddH(2), PreferencesManager.Preferences.bgColor);
            LastRect = new Rect(Rect.x + 4, Rect.y, 15, Rect.height);
            
            EditorGUI.LabelField(LastRect, (++_lineCount).ToString(), LineEnumStyle);
            LastRect.x += Indent * 10 + 5;
        }

        protected virtual void DrawType(Type type, float space = 4)
        {
            if (PreferencesManager.Preferences.simpleTypes)
            {
                DrawLabel("var", ModStyle, space);
                return;
            }
            DrawFullType(type, space);
        }

        private void DrawFullType(Type type, float space = 4)
        {
            string typeName = SpecialTypeNames.ContainsKey(type.Name) ? SpecialTypeNames[type.Name] : type.Name;
            GUIStyle style = char.IsLower(typeName[0]) ? ModStyle : TypeStyle;

            if (type.GenericTypeArguments.Length > 0)
            {
                typeName = typeName.Split('`')[0];
                DrawLabel(typeName, style, space);
                DrawLabel("<", TextStyle, -1);
                
                foreach (Type genericType in type.GenericTypeArguments)
                {
                    DrawType(genericType, -1);
                    
                    if (type.GenericTypeArguments[type.GenericTypeArguments.GetUpperBound(0)] != genericType)
                    {
                        DrawLabel(", ", TextStyle, -1);
                    }
                }
                DrawLabel(">", TextStyle, -1);
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
    }
}