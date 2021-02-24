using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Ject.Usage;
using Ject.Usage.Scene;
using Toolkit;
using Toolkit.Collections;
using ToolkitEditor.Extensions;
using UnityEngine;

namespace JectEditor.Inspectors.Contexts
{
    public class WritableContextDrawer : ContextDrawer
    {
        private readonly string _contextName;
        private readonly Dictionary<string, string> _methodParametersBuffer = new Dictionary<string, string>();
        
        public WritableContextDrawer(Context context, ContractWritersRawData writersRawData, string contextName) 
            : base(context, writersRawData)
        {
            _contextName = contextName;
        }
        
        protected override void DrawBody()
        {
            DrawHeader(_contextName);
            Indent++;
            DrawMembers();
            Indent--;
            DrawFooter();
        }
        
        private void DrawMembers()
        {
            for (int i = 0; i < InjectionInfo.fieldNames.Length; i++)
            {
                DrawField(i);
            }
            for (int i = 0; i < InjectionInfo.propertyNames.Length; i++)
            {
                DrawProperty(i);
            }
            if ((InjectionInfo.fieldNames.Length > 0 || InjectionInfo.propertyNames.Length > 0) 
                && InjectionInfo.methodNames.Length > 0)
            {
                BeginLine();
            }
            for (int i = 0; i < InjectionInfo.methodNames.Length; i++)
            {
                DrawMethod(i);
            }
            
            DrawAddMember();
        }

        private void DrawField(int index)
        {
            BeginLine();
            DrawLabel("public", TextStyles.ModStyle);
            DrawLabel("object", TextStyles.ModStyle);

            string fieldName = InjectionInfo.fieldNames[index];
            InjectionInfo.fieldNames[index] = DrawText(fieldName, TextStyles.TextStyle);
            
            DrawLabel(";", TextStyles.TextStyle, -1);

            if (Rect.Contains(Event.current.mousePosition) 
                || InjectionInfo.fieldDependencyIds.ContainsKey(fieldName))
            {
                DrawMemberDependencyId(InjectionInfo.fieldDependencyIds, fieldName);
            }

            if (RemoveButton())
            {
                InjectionInfo.fieldNames[index] = null;
                InjectionInfo.fieldNames = InjectionInfo.fieldNames.Where(n => n != null).ToArray();
                InjectionInfo.fieldDependencyIds.Remove(fieldName);
            }
        }

        private void DrawProperty(int index)
        {
            BeginLine();
            DrawLabel("public", TextStyles.ModStyle);
            DrawLabel("object", TextStyles.ModStyle);
            
            string propertyName = InjectionInfo.propertyNames[index];
            InjectionInfo.propertyNames[index] = DrawText(propertyName, TextStyles.TextStyle);
            
            DrawLabel("{", TextStyles.TextStyle);
            DrawLabel("get", TextStyles.ModStyle);
            DrawLabel(";", TextStyles.TextStyle, -1);
            DrawLabel("set", TextStyles.ModStyle);
            DrawLabel(";", TextStyles.TextStyle, -1);
            DrawLabel("}", TextStyles.TextStyle);
            
            if (Rect.Contains(Event.current.mousePosition) 
                || InjectionInfo.propertyDependencyIds.ContainsKey(propertyName))
            {
                DrawMemberDependencyId(InjectionInfo.propertyDependencyIds, propertyName);
            }
            
            if (RemoveButton())
            {
                InjectionInfo.propertyNames[index] = null;
                InjectionInfo.propertyNames = InjectionInfo.propertyNames.Where(n => n != null).ToArray();
                InjectionInfo.propertyDependencyIds.Remove(propertyName);
            }
        }
        
        private void DrawMethod(int index)
        {
            BeginLine();
            DrawLabel("public", TextStyles.ModStyle);
            DrawLabel("void", TextStyles.ModStyle);
            
            string methodName = InjectionInfo.methodNames[index];
            InjectionInfo.methodNames[index] = DrawText(methodName, TextStyles.TextStyle);
            
            DrawLabel("(", TextStyles.TextStyle, -1);
            if (Rect.Contains(Event.current.mousePosition) 
                || InjectionInfo.methodDependencyIds.ContainsKey(methodName))
            {
                DrawMethodParameterDependencyIds(methodName);
            }
            DrawLabel(")", TextStyles.TextStyle, -1);
            DrawLabel("{ }", TextStyles.TextStyle);
            
            if (RemoveButton())
            {
                InjectionInfo.methodNames[index] = null;
                InjectionInfo.methodNames = InjectionInfo.methodNames.Where(n => n != null).ToArray();
                InjectionInfo.methodDependencyIds.Remove(methodName);
            }
        }

        private void DrawMethodParameterDependencyIds(string methodName)
        {
            if (_methodParametersBuffer.ContainsKey(methodName))
            {
                string newText = DrawText(_methodParametersBuffer[methodName], TextStyles.ValueStyle, -1);
                if (newText != _methodParametersBuffer[methodName])
                {
                    _methodParametersBuffer[methodName] = newText;
                }
                else return;
            }
            else
            {
                if (InjectionInfo.methodDependencyIds.ContainsKey(methodName))
                {
                    _methodParametersBuffer[methodName]= GetParameterDependencyIdsText(methodName);
                }
                else
                {
                    const string text = "param = None, ..."; 
                    string newText = DrawText(text, TextStyles.ValueStyle, -1);
                
                    if (newText != text)
                    {
                        _methodParametersBuffer[methodName] = newText;
                    }
                    else return;
                }
            }
            
            ParseParameterDependencyIdsText(methodName, _methodParametersBuffer[methodName]);
        }

        private string GetParameterDependencyIdsText(string methodName)
        {
            var builder = new StringBuilder();
            
            foreach (KeyValuePair<string, Identifier> keyValuePair in InjectionInfo.methodDependencyIds[methodName])
            {
                builder.Append(keyValuePair.Key).Append(" = ").Append(keyValuePair.Value.ToString()).Append(", ");
            }
            builder.Remove(builder.Length - 2, 2);
            
            return builder.ToString();
        }

        private void ParseParameterDependencyIdsText(string methodName, string text)
        {
            if (text.Replace(" ", "") == "")
            {
                _methodParametersBuffer.Remove(methodName);
                InjectionInfo.methodDependencyIds.Remove(methodName);
                return;
            }
            
            var dependencyIds = new SerializableDictionary<string, Identifier>();

            foreach (string pairText in Regex.Split(text, @"\s*,\s*"))
            {
                string[] pair = Regex.Split(pairText, @"\s*=\s*");
                if (pair.Length == 2)
                {
                    dependencyIds[pair[0]] = new Identifier(pair[1]);
                }
            }

            if (dependencyIds.Count > 0)
            {
                InjectionInfo.methodDependencyIds[methodName] = dependencyIds;
            }
        }
        
        private void DrawAddMember()
        {
            BeginLine();
            string[] names = {"Add member..", "Field", "Property", "Method"};
            
            int selected = DrawPopup(0, names, TextStyles.TextStyle.FontStyledClone(FontStyle.Italic));
            switch (selected)
            {
                case 1: AddToArray(ref InjectionInfo.fieldNames, "Field"); break;
                case 2: AddToArray(ref InjectionInfo.propertyNames, "Property"); break;
                case 3: AddToArray(ref InjectionInfo.methodNames, "Method");  break;
            }
        }
    }
}