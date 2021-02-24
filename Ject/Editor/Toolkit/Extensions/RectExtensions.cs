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

        public static Rect[] Column(this Rect rect, int length)
        {
            var rectArray = new Rect[length];
            rectArray[0] = rect;
            
            for (int i = 1; i < length; i++)
            {
                rectArray[i] = rectArray[i - 1].ColumnNext();
            }

            return rectArray;
        }
        
        public static Rect ColumnNext(this Rect rect)
        {
            const float space = 2;
            rect.y += rect.height + space;
            return rect;
        }
        
        public static Rect[] SplitRow(this Rect rect, params float[] sizes)
        {
            const float space = 4;
            
            var rectArray = new Rect[sizes.Length];
            rectArray[0] = rect.WithW(sizes[0] - space);
            
            for (int i = 1; i < sizes.Length; i++)
            {
                float sizeDecr = i == sizes.Length - 1 ? 0 : space;
                rectArray[i] = rect.WithXW(rectArray[i - 1].xMax + space, sizes[i] - sizeDecr);
            }

            return rectArray;
        }
    }
}