using UnityEngine;

namespace JectEditor.Toolkit.Extensions
{
    public static class RectExtensions
    {
        public static Rect WithX(this Rect rect, float x)
        {
            rect.x = x;
            return rect;
        }
        
        public static Rect WithY(this Rect rect, float y)
        {
            rect.y = y;
            return rect;
        }
        
        public static Rect WithW(this Rect rect, float width)
        {
            rect.width = width;
            return rect;
        }
        
        public static Rect WithXW(this Rect rect, float x, float width)
        {
            rect.x = x;
            rect.width = width;
            return rect;
        }
        
        public static Rect WithH(this Rect rect, float height)
        {
            rect.height = height;
            return rect;
        }
        
        public static Rect AddX(this Rect rect, float x)
        {
            rect.x += x;
            return rect;
        }
        
        public static Rect AddY(this Rect rect, float y)
        {
            rect.y += y;
            return rect;
        }
        
        public static Rect AddW(this Rect rect, float width)
        {
            rect.width += width;
            return rect;
        }
        
        public static Rect AddH(this Rect rect, float height)
        {
            rect.height += height;
            return rect;
        } 
    }
}