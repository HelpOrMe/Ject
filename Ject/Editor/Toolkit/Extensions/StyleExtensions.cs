using UnityEngine;

namespace JectEditor.Toolkit.Extensions
{
    public static class StyleExtensions
    {
        public static GUIStyle Colored(this GUIStyle style, Color color)
        {
            style.normal.textColor = color;
            style.active.textColor = color;
            style.focused.textColor = color;
            style.hover.textColor = color;
            return style;
        }
        
        public static GUIStyle ColoredClone(this GUIStyle style, Color color)
        {
            style = style.Clone();
            style.normal.textColor = color;
            style.active.textColor = color;
            style.focused.textColor = color;
            style.hover.textColor = color;
            return style;
        }
        
        public static GUIStyle FontStyled(this GUIStyle style, FontStyle fontStyle)
        {
            style.fontStyle = fontStyle;
            return style;
        }
        
        public static GUIStyle FontStyledClone(this GUIStyle style, FontStyle fontStyle)
        {
            style = style.Clone();
            style.fontStyle = fontStyle;
            return style;
        }
        
        public static GUIStyle Clone(this GUIStyle style) => new GUIStyle(style);
    }
}