using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Ject.Usage;
using Ject.Usage.Scene;
using JectEditor.Preferences;
using UnityEngine;

namespace JectEditor.Inspectors.Contexts
{
    public class TypedContextDrawer : ContextDrawer
    {
        private static readonly HashSet<string> IgnoreMembers = new HashSet<string>();
        private static readonly string IgnoreRegex = @"(__BackingField|add_|remove_|get_|set_)";
        
        private readonly Type _type;

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
            DrawHeader(_type.Name + "Context");
            Indent++;
            DrawMembers();
            Indent--;
            DrawFooter();
        }
        
        private void DrawMembers()
        {
            const BindingFlags flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

            foreach (string fieldName in InjectionInfo.fieldNames.ToList())
            {
                FieldInfo field = _type.GetField(fieldName, flags);
                if (field == null)
                {
                    InjectionInfo.fieldNames = InjectionInfo.fieldNames.Where(name => name != fieldName).ToArray();
                    continue;
                }
                DrawField(field);
            }
            
            foreach (string propertyName in InjectionInfo.propertyNames.ToList())
            {
                PropertyInfo property = _type.GetProperty(propertyName, flags);
                if (property == null)
                {
                    InjectionInfo.propertyNames = InjectionInfo.propertyNames
                        .Where(name => name != propertyName).ToArray();
                    continue;
                }
                DrawProperty(property);
            }

            if ((InjectionInfo.fieldNames.Length > 0 || InjectionInfo.propertyNames.Length > 0) 
                && InjectionInfo.methodNames.Length > 0)
            {
                BeginLine();
            }
            
            foreach (string methodName in InjectionInfo.methodNames.ToList())
            {
                MethodInfo method = _type.GetMethod(methodName, flags);
                if (method == null)
                {
                    InjectionInfo.methodNames = InjectionInfo.methodNames.Where(name => name != methodName).ToArray();
                    continue;
                }
                DrawMethod(method);
            }

            DrawAddMember();
        }

        private void DrawAddMember()
        {
            BindingFlags flags = BindingFlags.Instance | BindingFlags.Public;
            if (PreferencesManager.Preferences.privateMembers)
            {
                flags |= BindingFlags.NonPublic;
            }

            List<FieldInfo> fields = FilterMembers(_type.GetFields(flags));
            List<PropertyInfo> properties = FilterMembers(_type.GetProperties(flags));
            List<MethodInfo> methods = FilterMembers(_type.GetMethods(flags));
            
            var names = new List<string>{"Add member..."};
            names.AddRange(fields.Select(field => field.Name));
            names.AddRange(properties.Select(property => property.Name));
            names.AddRange(methods.Select(method => method.Name));
         
            BeginLine();
            int selected = DrawPopup(0, names.ToArray(), 
                new GUIStyle(TextStyles.TextStyle) {fontStyle = FontStyle.Italic});

            if (selected == 0)
                return;

            int ramp = fields.Count;
            if (selected < ramp)
            {
                AddToArray(ref InjectionInfo.fieldNames, names[selected]);
                return;
            }

            ramp += properties.Count;
            if (selected < ramp)
            {
                AddToArray(ref InjectionInfo.propertyNames, names[selected]);
                return;
            }

            ramp += methods.Count;
            if (selected < ramp)
            {
                AddToArray(ref InjectionInfo.methodNames, names[selected]);
            }
        }

        private List<T> FilterMembers<T>(T[] members) where T : MemberInfo 
            =>  members.Where(method => !IgnoreMembers.Contains(method.Name) 
                                        && !Regex.IsMatch(method.Name, IgnoreRegex)
                                        && !InjectionInfo.methodNames.Contains(method.Name))
                .ToList();
        
        private void DrawField(FieldInfo field)
        {
            string modifier = "public";
            if (field.IsFamily) modifier = "protected";
            if (field.IsPrivate) modifier = "private";

            BeginLine();
            DrawLabel(modifier, TextStyles.ModStyle);
            DrawType(field.FieldType);
            DrawLabel(field.Name, TextStyles.TextStyle);

            if (Rect.Contains(Event.current.mousePosition) 
                || InjectionInfo.fieldDependencyIds.ContainsKey(field.Name))
            {
                DrawMemberDependencyId(InjectionInfo.fieldDependencyIds, field.Name);
            }

            DrawLabel(";", TextStyles.TextStyle, -1);
            
            if (RemoveButton())
            {
                InjectionInfo.fieldNames = RemoveFromArray(InjectionInfo.fieldNames, field.Name);
                InjectionInfo.fieldDependencyIds.Remove(field.Name);
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

            BeginLine();
            DrawLabel("public", TextStyles.ModStyle);
            DrawType(property.PropertyType);
            DrawLabel(property.Name, TextStyles.TextStyle);
            DrawLabel("{", TextStyles.TextStyle);
            DrawLabel(getModifier + "get", TextStyles.ModStyle);
            DrawLabel(";", TextStyles.TextStyle, -1);
            DrawLabel(setModifier + "set", TextStyles.ModStyle);
            DrawLabel("; }", TextStyles.TextStyle, -1);

            if (Rect.Contains(Event.current.mousePosition) 
                || InjectionInfo.propertyDependencyIds.ContainsKey(property.Name))
            {
                DrawMemberDependencyId(InjectionInfo.propertyDependencyIds, property.Name);
            }

            if (RemoveButton())
            {
                InjectionInfo.propertyNames = RemoveFromArray(InjectionInfo.propertyNames, property.Name);
                InjectionInfo.propertyDependencyIds.Remove(property.Name);
            }
        }

        private void DrawMethod(MethodInfo method)
        {
            string modifier = "public";
            if (method.IsFamily) modifier = "protected";
            if (method.IsPrivate) modifier = "private";

            BeginLine();
            DrawLabel(modifier, TextStyles.ModStyle);
            DrawType(method.ReturnType);
            DrawLabel(method.Name, TextStyles.TextStyle);
            DrawLabel("(", TextStyles.TextStyle, -1);
            DrawMethodParameters(method);
            DrawLabel(") { }", TextStyles.TextStyle, -1);

            if (RemoveButton())
            {
                InjectionInfo.methodNames = RemoveFromArray(InjectionInfo.methodNames, method.Name);
                InjectionInfo.methodDependencyIds.Remove(method.Name);
            }
        }

        private void DrawMethodParameters(MethodInfo method)
        {
            float methodParamsBeginX = LastRect.xMax;

            bool dependencyIdsExists = InjectionInfo.methodDependencyIds.ContainsKey(method.Name);
            bool drawDependencyIds = Rect.Contains(Event.current.mousePosition) || dependencyIdsExists;
            
            ParameterInfo[] parameters = method.GetParameters();
            for (int i = 0; i < parameters.Length; i++)
            {
                ParameterInfo parameter = parameters[i];
                
                if (dependencyIdsExists && i > 0)
                {
                    BeginLine();
                    LastRect.xMax = methodParamsBeginX;
                }

                DrawType(parameter.ParameterType, -1);
                DrawLabel(parameter.Name, TextStyles.TextStyle);

                if (drawDependencyIds)
                {
                    DrawParameterDependencyId(method.Name, parameter.Name);
                }
                
                if (parameters[parameters.GetUpperBound(0)] != parameter)
                {
                    DrawLabel(", ", TextStyles.TextStyle, -1);
                }
            }
        }
    }
}