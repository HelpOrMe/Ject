using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using Ject.Usage;
using Ject.Usage.Scene;
using JectEditor.Toolkit.Extensions;
using UnityEngine;

namespace JectEditor.Inspectors.Contexts
{
    public class TypedContextDrawer : ContextDrawer
    {
        private static readonly HashSet<string> IgnoreMembers = new HashSet<string>();
        private static readonly string IgnoreRegex = @"(__BackingField|add_|remove_|get_|set_)";
        
        private readonly Type _type;
        private bool _strikeThrough;

        private Rect TextRect => Rect.WithW(LastRect.xMax); 
        
        public TypedContextDrawer(Context context, ContractWritersRawData writersRawData, Type type)
            : base(context, writersRawData)
        {
            _type = type;
            
            if (IgnoreMembers.Count == 0)
            {
                AddIgnoreMembers();
            }
        }

        private void AddIgnoreMembers()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            Type monoType = typeof(MonoBehaviour);
            
            foreach (FieldInfo field in monoType.GetFields(flags))
            {
                IgnoreMembers.Add(field.Name);
            }
            foreach (PropertyInfo property in monoType.GetProperties(flags))
            {
                IgnoreMembers.Add(property.Name);
            }
            foreach (MethodInfo method in monoType.GetMethods(flags))
            {
                IgnoreMembers.Add(method.Name);
            }
        }
        
        protected override void DrawBody()
        {
            DrawHeader(_type.Name);
            Indent++;
            DrawMembers();
            Indent--;
            DrawFooter();
        }

        private void DrawMembers()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            bool propertyOrFieldDrawn = false;
            
            foreach (FieldInfo field in _type.GetFields(flags))
            {
                if (!IgnoreMembers.Contains(field.Name) 
                    && !Regex.IsMatch(field.Name, IgnoreRegex))
                {
                    DrawField(field);
                    propertyOrFieldDrawn = true;
                }
            }
            
            foreach (PropertyInfo property in _type.GetProperties(flags))
            {
                if (!IgnoreMembers.Contains(property.Name) 
                    && !Regex.IsMatch(property.Name, IgnoreRegex))
                {
                    DrawProperty(property);
                    propertyOrFieldDrawn = true;
                }
            }

            if (propertyOrFieldDrawn)
            {
                BeginLine();
            }
            
            foreach (MethodInfo method in _type.GetMethods(flags))
            {
                if (!IgnoreMembers.Contains(method.Name) 
                    && !Regex.IsMatch(method.Name, IgnoreRegex))
                {
                    DrawMethod(method);
                }
            }

            _strikeThrough = false;
        }

        private void DrawField(FieldInfo field)
        {
            string modifier = "public";
            if (field.IsFamily) modifier = "protected";
            if (field.IsPrivate) modifier = "private";

            _strikeThrough = !InjectionInfo.fieldNames.Contains(field.Name);
            
            BeginLine();
            DrawLabel(modifier, ModStyle);
            DrawType(field.FieldType);
            DrawLabel(field.Name, TextStyle);
            DrawLabel(";", TextStyle, -1);

            if (Event.current.type == EventType.MouseDown && TextRect.Contains(Event.current.mousePosition))
            {
                InjectionInfo.fieldNames = SwitchItem(InjectionInfo.fieldNames, field.Name);
            }
            
            if (Rect.Contains(Event.current.mousePosition) 
                || InjectionInfo.fieldDependencyIds.ContainsKey(field.Name))
            {
                DrawComment(InjectionInfo.fieldDependencyIds, field.Name);
            }
        }

        private void DrawProperty(PropertyInfo property)
        {
            if (!property.CanWrite)
                return;
            
            string setModifier = "";
            if (property.SetMethod.IsFamily) setModifier = "protected ";
            if (property.SetMethod.IsPrivate) setModifier = "private ";

            string getModifier = "";
            if (property.GetMethod.IsFamily) getModifier = "protected ";
            if (property.GetMethod.IsPrivate) getModifier = "private" ;

            _strikeThrough = !InjectionInfo.propertyNames.Contains(property.Name);
            
            BeginLine();
            DrawLabel("public", ModStyle);
            DrawType(property.PropertyType);
            DrawLabel(property.Name, TextStyle);
            DrawLabel("{", TextStyle);
            DrawLabel(getModifier + "get", ModStyle);
            DrawLabel(";", TextStyle, -1);
            DrawLabel(setModifier + "set", ModStyle);
            DrawLabel(";", TextStyle, -1);
            DrawLabel("}", TextStyle);

            if (Event.current.type == EventType.MouseDown && TextRect.Contains(Event.current.mousePosition))
            {
                InjectionInfo.propertyNames = SwitchItem(InjectionInfo.propertyNames, property.Name);
            }
            
            if (Rect.Contains(Event.current.mousePosition) 
                || InjectionInfo.propertyDependencyIds.ContainsKey(property.Name))
            {
                DrawComment(InjectionInfo.propertyDependencyIds, property.Name);
            }
        }

        private void DrawMethod(MethodInfo method)
        {
            string modifier = "public";
            if (method.IsFamily) modifier = "protected";
            if (method.IsPrivate) modifier = "private";

            _strikeThrough = !InjectionInfo.methodNames.Contains(method.Name);
            
            BeginLine();
            DrawLabel(modifier, ModStyle);
            DrawType(method.ReturnType);
            DrawLabel(method.Name, TextStyle);
            DrawLabel("(", TextStyle, -1);

            ParameterInfo[] parameters = method.GetParameters();
            foreach (ParameterInfo parameter in parameters)
            {
                DrawType(parameter.ParameterType, -1);
                DrawLabel(parameter.Name, TextStyle);
                if (parameters[parameters.GetUpperBound(0)] != parameter)
                {
                    DrawLabel(", ", TextStyle, -1);
                }
            }
            
            DrawLabel(") { }", TextStyle, -1);
            
            if (Event.current.type == EventType.MouseDown && TextRect.Contains(Event.current.mousePosition))
            {
                InjectionInfo.methodNames = SwitchItem(InjectionInfo.methodNames, method.Name);
            }
            
            if (Rect.Contains(Event.current.mousePosition) 
                || InjectionInfo.methodDependencyIds.ContainsKey(method.Name))
            {
                DrawMethodComment(method.Name);
            }
        }
        
        private string[] SwitchItem(string[] array, string item)
        {
            if (array.Contains(item))
            {
                return array.Where(arrayItem => arrayItem != item).ToArray();
            }
            
            Array.Resize(ref array, array.Length + 1);
            array[array.GetUpperBound(0)] = item;
            return array;
        }
    
        protected override string DrawText(string text, GUIStyle style, float space = 4)
        {
            if (_strikeThrough)
            {
                DrawLabel(text, style, space);
                return text;
            }
            return base.DrawText(text, style, space);
        }
        
        protected override void DrawLabel(string label, GUIStyle style, float space = 4) 
            => base.DrawLabel(_strikeThrough ? StrikeThrough(label) : label, style, space);

        private string StrikeThrough(string text)
        {
            var builder = new StringBuilder();
            foreach (char chr in text)
            {
                builder.Append(chr);
                builder.Append('\u0336');
            }
            return builder.ToString();
        }
    }
}