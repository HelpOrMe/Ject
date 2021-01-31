using System;
using System.Linq;
using Ject.Usage;
using Ject.Usage.Scene;
using UnityEngine;

namespace JectEditor.Inspectors.Contexts
{
    public class WritableContextDrawer : ContextDrawer
    {
        private readonly string _contextName;

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

        protected void DrawField(int index)
        {
            BeginLine();
            DrawLabel("public", ModStyle);
            DrawLabel("object", ModStyle);

            string name = InjectionInfo.fieldNames[index];
            InjectionInfo.fieldNames[index] = DrawText(name, TextStyle);
            
            DrawLabel(";", TextStyle, -1);

            if (Rect.Contains(Event.current.mousePosition) 
                || InjectionInfo.fieldDependencyIds.ContainsKey(name))
            {
                DrawComment(InjectionInfo.fieldDependencyIds, name);
            }

            if (RemoveButton())
            {
                InjectionInfo.fieldNames[index] = null;
                InjectionInfo.fieldNames = InjectionInfo.fieldNames.Where(n => n != null).ToArray();
                InjectionInfo.fieldDependencyIds.Remove(name);
            }
        }

        protected void DrawProperty(int index)
        {
            BeginLine();
            DrawLabel("public", ModStyle);
            DrawLabel("object", ModStyle);
            
            string name = InjectionInfo.propertyNames[index];
            InjectionInfo.propertyNames[index] = DrawText(name, TextStyle);
            
            DrawLabel("{", TextStyle);
            DrawLabel("get", ModStyle);
            DrawLabel(";", TextStyle, -1);
            DrawLabel("set", ModStyle);
            DrawLabel(";", TextStyle, -1);
            DrawLabel("}", TextStyle);
            
            if (Rect.Contains(Event.current.mousePosition) 
                || InjectionInfo.propertyDependencyIds.ContainsKey(name))
            {
                DrawComment(InjectionInfo.propertyDependencyIds, name);
            }
            
            if (RemoveButton())
            {
                InjectionInfo.propertyNames[index] = null;
                InjectionInfo.propertyNames = InjectionInfo.propertyNames.Where(n => n != null).ToArray();
                InjectionInfo.propertyDependencyIds.Remove(name);
            }
        }
        
        protected void DrawMethod(int index)
        {
            BeginLine();
            DrawLabel("public", ModStyle);
            DrawLabel("void", ModStyle);
            
            string name = InjectionInfo.methodNames[index];
            InjectionInfo.methodNames[index] = DrawText(name, TextStyle);
            
            DrawLabel("(...) {   }", TextStyle, -1);
            
            if (Rect.Contains(Event.current.mousePosition) 
                || InjectionInfo.methodDependencyIds.ContainsKey(name))
            {
                DrawMethodComment(name);
            }
            
            if (RemoveButton())
            {
                InjectionInfo.methodNames[index] = null;
                InjectionInfo.methodNames = InjectionInfo.methodNames.Where(n => n != null).ToArray();
                InjectionInfo.methodDependencyIds.Remove(name);
            }
        }

        private void DrawAddMember()
        {
            BeginLine();
            string[] names = {"Add member..", "Field", "Property", "Method"};
            
            int selected = DrawPopup(0, names, new GUIStyle(TextStyle) {fontStyle = FontStyle.Italic});
            switch (selected)
            {
                case 1:
                    Array.Resize(ref InjectionInfo.fieldNames, InjectionInfo.fieldNames.Length + 1);
                    InjectionInfo.fieldNames[InjectionInfo.fieldNames.GetUpperBound(0)] = "FieldName";
                    break;
                case 2:
                    Array.Resize(ref InjectionInfo.propertyNames, InjectionInfo.propertyNames.Length + 1);
                    InjectionInfo.propertyNames[InjectionInfo.propertyNames.GetUpperBound(0)] = "PropertyName";
                    break;
                case 3:
                    Array.Resize(ref InjectionInfo.methodNames, InjectionInfo.methodNames.Length + 1);
                    InjectionInfo.methodNames[InjectionInfo.methodNames.GetUpperBound(0)] = "MethodName";
                    break;
            }
        }
    }
}