using UnityEngine;

namespace Refsa.UVSlicer
{
    public static class RectHelpers
    {
        public static Rect Inset(Rect original, Vector2 size)
        {
            Vector2 sizeDiff = original.size - size;
            original.position += sizeDiff / 2f;
            original.size = size;

            return original;
        }

        public static Rect Inset(Rect original, float percent)
        {
            Vector2 sizeDiff = original.size - original.size * percent;
            original.position += sizeDiff / 4f;
            original.size -= sizeDiff;

            return original;
        }

        public static Rect PlaceBottomRight(Rect original, Vector2 size)
        {
            Rect newRect = new Rect(Vector2.zero, size);
            newRect.position = original.size - size;

            return newRect;
        }

        public static Rect ScaleSizeBy(this in Rect rect, float scale, Vector2 pivotPoint)
        {
            Rect result = rect;
            result.x -= pivotPoint.x;
            result.y -= pivotPoint.y;
            result.xMin *= scale;
            result.xMax *= scale;
            result.yMin *= scale;
            result.yMax *= scale;
            result.x += pivotPoint.x;
            result.y += pivotPoint.y;
            return result;
        }

        static readonly Vector2 TopLeftScale = new Vector2(-1, -1);
        static readonly Vector2 TopRightScale = new Vector2(1, -1);
        static readonly Vector2 BottomLeftScale = new Vector2(-1, 1);
        static readonly Vector2 BottomRightScale = new Vector2(1, 1);
        public static Rect KeepInside(this ref Rect inner, in Rect outer)
        {
            Vector2 extents = inner.size * 0.5f;
            Vector2 left = inner.center + new Vector2(-extents.x, 0);
            Vector2 right = inner.center + new Vector2(extents.x, 0);
            Vector2 up = inner.center + new Vector2(0, extents.y);
            Vector2 down = inner.center + new Vector2(0, -extents.y);

            if (!outer.Contains(left))
            {
                inner.position += new Vector2(-left.x, 0);
                UnityEngine.Debug.Log($"left");
            }
            else if (!outer.Contains(right))
            {
                inner.position += new Vector2(outer.size.x - right.x, 0);
                UnityEngine.Debug.Log($"right");
            }

            if (!outer.Contains(up))
            {
                inner.position += new Vector2(0, -up.y);
                UnityEngine.Debug.Log($"up");
            }
            else if (!outer.Contains(down))
            {
                inner.position += new Vector2(0, outer.size.y - down.y);
                UnityEngine.Debug.Log($"down");
            }

            return inner;
        }
    }
}