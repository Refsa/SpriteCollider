using UnityEditor;
using UnityEngine;

namespace Refsa.UVSlicer
{
    public static class EditorZoom
    {
        public static void BeginZoom(Rect rect, float zoom, float topPadding)
        {
            GUI.EndClip();

            GUIUtility.ScaleAroundPivot(Vector2.one / zoom, rect.size * 0.5f);

            GUI.BeginClip(new Rect(-((rect.width * zoom) - rect.width) * 0.5f, -(((rect.height * zoom) - rect.height) * 0.5f) + (topPadding * zoom),
                rect.width * zoom,
                rect.height * zoom));
        }

        public static void EndZoom(Rect rect, float zoom, float topPadding)
        {
            GUI.EndClip();

            GUI.BeginClip(new Rect(0f, topPadding, Screen.width, Screen.height));
        }
    }
}