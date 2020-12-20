using UnityEditor;
using UnityEngine;
using Unity.Mathematics;
using System.Collections.Generic;

#if UNITY_EDITOR
namespace Refsa.UVSlicer
{
    [System.Serializable]
    public class UVSlicer
    {
        public enum Action
        {
            None,
            DragUV,
            ExpandUV
        }

        Action currentAction;

        Texture2D texture;
        Texture2D areaBackground;

        Rect areaRect;

        Vector2 textureSize;
        Rect textureRect;

        int pixelSnap => Mathf.FloorToInt((float)areaRect.height / (float)textureSize.y);
        Rect uvRect;
        Rect uvRectVisual;
        bool uvRectHasMouse;
        Vector2 movedTotal = Vector2.zero;
        (bool, bool, bool) expandState;

        Vector2 mousePosition;
        bool hasMouse;
        public bool HasMouse => hasMouse;

        public Vector2 AreaOffset { get; set; }
        public Action CurrentAction => currentAction;
        public Rect UVRect => uvRect;
        public Rect DefaultUVRect => new Rect(Vector2.zero, Vector2.one * pixelSnap);
        public bool CursorVisible { get; private set; }
        public bool Repaint { get; set; }
        public Rect AreaRect { get => areaRect; set => areaRect = value; }
        public Color UVRectColor { get; set; } = Color.magenta;
        public bool UVRectActive { get; set; } = false;
        public int PixelSnap => pixelSnap;

        public event System.Action uvRectChanged;
        Vector3[] uvRectBoxLines = new Vector3[5];

        public UVSlicer(Rect containingRect)
        {
            SetRects(containingRect);
            areaBackground = TextureGenerator.GenerateTexture(Color.black * 0.5f);
        }

        #region Draw
        public void OnGUI()
        {
            Color guiColor = GUI.color;
            Matrix4x4 guiMatrix = GUI.matrix;

            hasMouse = areaRect.Contains(Event.current.mousePosition);
            uvRectHasMouse = uvRectVisual.Contains(Event.current.mousePosition);

            DrawUVSlicer();
            hasMouse = areaRect.Contains(Event.current.mousePosition);
            if (hasMouse)
            {
                OnMouseEvent(Event.current);
                OnKeyEvent(Event.current);
            }

            GUI.matrix = guiMatrix;
            GUI.color = guiColor;
            Repaint = true;

            // var stateLabelRect = new Rect(areaRect);
            // stateLabelRect.size = new Vector2(150f, 30f);
            // GUI.Label(stateLabelRect, $"State: {currentAction}");
        }

        void DrawUVSlicer()
        {            
            GUI.DrawTexture(areaRect, areaBackground, ScaleMode.StretchToFill, false, 1f, Color.black * 0.5f, 0f, 5f);

            if (texture == null) return;

            textureRect = new Rect(Vector2.zero, textureSize);
            textureRect.size *= pixelSnap;
            textureRect.center = areaRect.center;

            uvRectVisual = new Rect(uvRect);
            uvRectVisual.position += textureRect.position;

            bool uvRectGetsMouse = uvRectVisual.Contains(Event.current.mousePosition);
            if (uvRectHasMouse != uvRectGetsMouse)
            {
                Repaint = true;
            }
            uvRectHasMouse = uvRectGetsMouse;

            EditorGUI.DrawTextureTransparent(textureRect, texture, ScaleMode.StretchToFill);

            mousePosition = Event.current.mousePosition;
            CheckUVRectHover(mousePosition);
            if (hasMouse && currentAction == Action.None)
            {
                Repaint = true;
                Handles.DrawSolidDisc(mousePosition, Vector3.forward, 0.5f);
            }

            if (UVRectActive)
            {
                Color uvRectColor = UVRectColor;
                if (currentAction == Action.DragUV || currentAction == Action.ExpandUV)
                {
                    uvRectColor = Color.yellow;
                    Repaint = true;
                }
                else if (uvRectHasMouse) uvRectColor = Color.green;

                if (uvRectBoxLines == null || uvRectBoxLines.Length != 5) uvRectBoxLines = new Vector3[5];
                uvRectBoxLines[0] = uvRectVisual.position;
                uvRectBoxLines[1] = uvRectVisual.position + new Vector2(uvRectVisual.width, 0f);
                uvRectBoxLines[2] = uvRectVisual.position + new Vector2(uvRectVisual.width, uvRectVisual.height);
                uvRectBoxLines[3] = uvRectVisual.position + new Vector2(0f, uvRectVisual.height);
                uvRectBoxLines[4] = uvRectVisual.position;
                float lineWidth = 4f;
                for (int i = 0; i < 5; i++)
                {
                    Vector2 centerDir = ((Vector2)uvRectBoxLines[i] - uvRectVisual.center).normalized;
                    uvRectBoxLines[i] -= (Vector3)centerDir * lineWidth * 0.5f;
                }

                uvRectColor.a = 0.25f;
                EditorGUI.DrawRect(uvRectVisual, uvRectColor);

                uvRectColor.a = 1f;
                Handles.color = uvRectColor;
                Handles.DrawAAPolyLine(4f, uvRectBoxLines);
            }
        }
        #endregion

        #region Events
        public void OnMouseEvent(Event current)
        {
            CursorVisible = false;
            var expandRectActive = CheckExpandRectCursor();
            if (currentAction == Action.DragUV)
            {
                CursorVisible = false;
                EditorGUIUtility.AddCursorRect(new Rect(0f, 0f, Screen.width, Screen.height), MouseCursor.Pan);
            }
            else if (expandRectActive.Item1 && uvRectHasMouse || currentAction == Action.ExpandUV)
            {
                if (expandRectActive.Item2 || expandState.Item2)
                { 
                    CursorVisible = true;
                    EditorGUIUtility.AddCursorRect(new Rect(0f, 0f, Screen.width, Screen.height), MouseCursor.ResizeHorizontal);
                }
                else
                {
                    CursorVisible = true;
                    EditorGUIUtility.AddCursorRect(new Rect(0f, 0f, Screen.width, Screen.height), MouseCursor.ResizeVertical);
                }
            }

            if (current.type != EventType.MouseDown &&
                current.type != EventType.MouseDrag &&
                current.type != EventType.ScrollWheel &&
                current.type != EventType.MouseMove &&
                current.type != EventType.MouseUp) return;

            switch (current.type)
            {
                case EventType.MouseDown:
                    if (current.button == 0 && uvRectHasMouse)
                    {
                        if (expandRectActive.Item1)
                        {
                            currentAction = Action.ExpandUV;
                            expandState = expandRectActive;
                        }
                    }
                    else if (current.button == 1 && uvRectHasMouse)
                    {
                        currentAction = Action.DragUV;
                    }
                    current.Use();
                    break;
                case EventType.MouseUp:
                    if (currentAction == Action.None)
                    {

                    }

                    if (currentAction != Action.None)
                    {
                        uvRectChanged?.Invoke();
                    }
                    currentAction = Action.None;
                    expandState = (false, false, false);
                    current.Use();
                    break;
                case EventType.MouseDrag:
                    if (currentAction == Action.DragUV)
                    {
                        MoveSnapToGridDelta(current.delta, ref uvRect, pixelSnap);
                        Repaint = true;
                    }
                    else if (currentAction == Action.ExpandUV)
                    {
                        ExpandSnapToGridDelta(current.delta, expandState.Item2, expandState.Item3, ref uvRect, pixelSnap);
                        Repaint = true;
                    }
                    current.Use();
                    break;
                case EventType.ScrollWheel:
                    break;
            }
        }

        public void OnKeyEvent(Event current)
        {
            if (!hasMouse) return;
            if (current.type != EventType.KeyDown &&
                current.type != EventType.KeyUp) return;

            switch (current.type)
            {
                case EventType.KeyDown:
                    OnKeyDown(current.keyCode);
                    current.Use();
                    break;
                case EventType.KeyUp:
                    OnKeyUp(current.keyCode);
                    current.Use();
                    break;
            }

        }

        void OnKeyDown(KeyCode key)
        {
            
        }

        void OnKeyUp(KeyCode key)
        {
            switch (key)
            {
                default:
                    break;
            }
        }
        #endregion

        #region Setup
        public void SetTexture(Texture2D texture)
        {
            this.texture = texture;
            textureSize = new Vector2(texture.width, texture.height);
        }

        public void SetRects(Rect rect)
        {
            areaRect = rect;
            ResetUVRect();
        }
        #endregion

        #region Actions
        void CheckUVRectHover(Vector2 mousePosition)
        {
            bool uvRectHovered = uvRectVisual.Contains(mousePosition);
            if (uvRectHasMouse != uvRectHovered)
            {
                SceneView.RepaintAll();
            }
            uvRectHasMouse = uvRectHovered;
        }

        Vector2 MovedTotalSnap(Vector2 delta, int snap = 14)
        {
            movedTotal += delta;

            float xSign = Mathf.Sign(movedTotal.x);
            float ySign = Mathf.Sign(movedTotal.y);

            float xMoves = Mathf.Abs(movedTotal.x) / snap;
            movedTotal.x = math.frac(xMoves) * snap * xSign;

            float yMoves = Mathf.Abs(movedTotal.y) / snap;
            movedTotal.y = math.frac(yMoves) * snap * ySign;

            return
                new Vector2(
                    snap * math.floor(xMoves) * xSign,
                    snap * math.floor(yMoves) * ySign
                );
        }

        void MoveSnapToGridDelta(Vector2 delta, ref Rect rect, int snap = 14)
        {
            Vector2 totalMove = MovedTotalSnap(delta, snap);

            MoveSnapToGrid(totalMove, ref rect, snap);
        }

        void MoveSnapToGrid(Vector2 amount, ref Rect rect, int snap = 14)
        {
            var textureRect = new Rect(Vector2.zero, textureSize);
            textureRect.size *= pixelSnap;
            textureRect.center = areaRect.center;
            Vector2 textureRectPosition = textureRect.position;

            Vector2 newPos = rect.position + amount + textureRect.position;
            Vector2 bottomRight = newPos + rect.size;
            Vector2 bottomLeft = newPos + new Vector2(0f, rect.size.y);
            Vector2 topRight = newPos + new Vector2(rect.size.x, 0f);

            Vector2 containerTopLeft = textureRectPosition;
            Vector2 containerTopRight = textureRectPosition + new Vector2(textureRect.size.x, 0f);
            Vector2 containerBottomLeft = textureRectPosition + new Vector2(0f, textureRect.size.y);
            Vector2 containerBottomRight = textureRectPosition + textureRect.size;

            // bool canMove = true;
            bool canMoveX =
                newPos.x >= containerTopLeft.x - 0.0001f &&
                bottomLeft.x >= containerBottomLeft.x - 0.0001f &&
                topRight.x <= containerTopRight.x + 0.0001f &&
                bottomRight.x <= containerBottomRight.x + 0.0001f;

            bool canMoveY =
                newPos.y >= containerTopLeft.y - 0.0001f &&
                bottomLeft.y <= containerBottomLeft.y + 0.0001f &&
                topRight.y >= containerTopRight.y - 0.0001f &&
                bottomRight.y <= containerBottomRight.y + 0.0001f;

            newPos = rect.position;
            if (canMoveX)
                newPos.x += amount.x;
            if (canMoveY)
                newPos.y += amount.y;

            rect.position = newPos;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>(expandRect, expandWidth, expandMin)</returns>
        (bool, bool, bool) CheckExpandRectCursor()
        {
            Line2 left = new Line2(new Vector2(uvRectVisual.xMin, uvRectVisual.yMin), new Vector2(uvRectVisual.xMin, uvRectVisual.yMax));
            Line2 right = new Line2(new Vector2(uvRectVisual.xMax, uvRectVisual.yMin), new Vector2(uvRectVisual.xMax, uvRectVisual.yMax));
            Line2 up = new Line2(new Vector2(uvRectVisual.xMin, uvRectVisual.yMin), new Vector2(uvRectVisual.xMax, uvRectVisual.yMin));
            Line2 down = new Line2(new Vector2(uvRectVisual.xMin, uvRectVisual.yMax), new Vector2(uvRectVisual.xMax, uvRectVisual.yMax));

            float distLeft = left.DistanceToPoint(mousePosition);
            float distRight = right.DistanceToPoint(mousePosition);
            float distUp = up.DistanceToPoint(mousePosition);
            float distDown = down.DistanceToPoint(mousePosition);
            float minDistExpand = 25f;

            bool expandRect =
                distLeft < minDistExpand ||
                distRight < minDistExpand ||
                distUp < minDistExpand ||
                distDown < minDistExpand;

            if (expandRect)
            {
                float minWidth = distLeft < distRight ? distLeft : distRight;
                float minHeight = distUp < distDown ? distUp : distDown;

                bool expandWidth = minWidth < minHeight;
                bool expandMin = expandWidth ? distLeft < distRight : distUp < distDown;

                return (expandRect, expandWidth, expandMin);
            }

            return (false, false, false);
        }

        void ExpandSnapToGridDelta(Vector2 delta, bool scaleX, bool scaleMin, ref Rect rect, int snap = 32)
        {
            Vector2 totalMove = MovedTotalSnap(delta, snap);
            ExpandSnapToGrid(totalMove, scaleX, scaleMin, ref rect, snap);
        }

        void ExpandSnapToGrid(Vector2 move, bool scaleX, bool scaleMin, ref Rect rect, int snap = 32)
        {
            var textureRect = new Rect(Vector2.zero, textureSize);
            textureRect.size *= pixelSnap;
            textureRect.center = areaRect.center;

            if (scaleX)
            {
                if (scaleMin)
                {
                    if (uvRectVisual.xMin + move.x >= textureRect.xMin && uvRectVisual.xMin + move.x <= uvRectVisual.xMax)
                    {
                        rect.xMin += move.x;
                    }
                }
                else
                {
                    if (uvRectVisual.xMax + move.x > uvRectVisual.xMin && uvRectVisual.xMax + move.x <= textureRect.xMax)
                    {
                        rect.xMax += move.x;
                    }
                }
            }
            else
            {
                if (scaleMin)
                {
                    if (uvRectVisual.yMin + move.y >= textureRect.yMin && uvRectVisual.yMin + move.y <= uvRectVisual.yMax)
                    {
                        rect.yMin += move.y;
                    }
                }
                else
                {
                    if (uvRectVisual.yMax + move.y > uvRectVisual.yMin && uvRectVisual.yMax + move.y <= textureRect.yMax)
                    {
                        rect.yMax += move.y;
                    }
                }
            }
        }
        #endregion

        #region Utility
        public void SetUVRect(Rect rect)
        {
            this.uvRect = rect;
            this.uvRect.size *= pixelSnap;
            this.uvRect.position *= pixelSnap;
        }

        public void ResetUVRect()
        {
            uvRect = new Rect(Vector2.zero, Vector2.one * pixelSnap);
        }
        #endregion
    }
}
#endif