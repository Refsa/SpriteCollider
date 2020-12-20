using UnityEngine;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using Refsa.UVSlicer;
using System.Linq;
using UnityEditor.Callbacks;

public class SpriteColliderEditor : EditorWindow
{
    static SpriteColliderEditor instance;

    enum State
    {
        MoveBox,
        ResizeBox,
    }

    [MenuItem("Tools/SpriteCollider")]
    private static void ShowWindow()
    {
        var window = GetWindow<SpriteColliderEditor>();
        window.titleContent = new GUIContent("SpriteCollider");
        window.Show();

        window.wantsMouseMove = true;
        window.minSize = new Vector2(1024, 1024);
        window.maxSize = new Vector2(1024, 1024);

        window.isStyleSetup = false;

        instance = window;
    }

    [SerializeField] SpriteColliderData spriteColliderData;
    // [SerializeReference] AnimationClip currentAnimation;

    List<Sprite> clipSprites;
    List<Texture2D> slicedSprites;
    int selectedFrame = 0;
    int selectedColliderData = -1;
    int hoveredEventData = -1;

    Rect selectionRect;

    SpriteColliderData.Mode mode;
    State state;

    UVSlicer slicer;

    bool isStyleSetup = false;
    GUIStyle selectedStyle;
    Texture2D selectedTexture;

    bool selectedFrameChanged = false;
    int eventIndexToRemove = -1;

    int frameCount => slicedSprites != null ? slicedSprites.Count : 0;
    Vector2 eventScrollPos;
    Vector3[] lineVertTemp = new Vector3[2];
    List<Vector2> clipFrameLineOrigins = new List<Vector2>();

    #region Setup
    [OnOpenAsset(0)]
    public static bool Open(int instanceID, int line)
    {
        var asset = EditorUtility.InstanceIDToObject(instanceID) as SpriteColliderData;
        if (asset == null) return false;

        if (instance == null)
        {
            ShowWindow();
        }

        instance.Load(asset);
        instance.Repaint();
        return true;
    }

    void OnEnable()
    {
        instance = this;

        if (spriteColliderData is SpriteColliderData data && data.Clip is AnimationClip clip)
        {
            clipSprites = SpriteSlicer.GetSpritesFromClip(clip);
            slicedSprites = SpriteSlicer.SliceSprites(clipSprites);
        }

        if (slicer != null)
        {
            SetupSlicer();
        }

        isStyleSetup = false;
    }

    void SetupStyles()
    {
        selectedTexture = TextureGenerator.GenerateTexture(Color.green * 0.5f);
        selectedStyle = new GUIStyle(EditorStyles.helpBox);
        selectedStyle.normal.background = selectedTexture;

        SpriteColliderStyle.Setup();

        isStyleSetup = true;
    }

    void SetupSlicer()
    {
        slicer.uvRectChanged += () =>
        {
            if (selectedColliderData != -1)
            {
                UnityEngine.Debug.Log($"changed");
                var rect = slicer.UVRect;
                rect.size /= slicer.PixelSnap;
                rect.position /= slicer.PixelSnap;
                spriteColliderData.Colliders[selectedColliderData].Rects[selectedFrame].SpriteRect = rect;
            }
        };
    }
    #endregion

    #region Drawing 
    void OnGUI()
    {
        if (!isStyleSetup)
        {
            SetupStyles();
        }

        if (spriteColliderData == null)
        {
            DrawDefaultView();
            return;
        }

        HandleEvent(Event.current);

        using (new GUILayout.VerticalScope())
        {
            using (new GUILayout.VerticalScope(SpriteColliderStyle.Background2Style.Copy().WithHeight(511)))
            {
                DrawSlicer();
            }
            using (new GUILayout.VerticalScope(SpriteColliderStyle.Background1Style))
            {
                using (new GUILayout.HorizontalScope(SpriteColliderStyle.MenuBarStyle))
                {
                    if (spriteColliderData != null)
                        GUILayout.Label($"{spriteColliderData.Clip.name}", SpriteColliderStyle.MenuHeaderStyle);
                    else
                        GUILayout.Label($"SpriteCollider", SpriteColliderStyle.MenuHeaderStyle);

                    if (GUILayout.Button("Save", SpriteColliderStyle.MenuBarButtonStyle))
                    {
                        Save();
                    }
                    RepaintOnHover();
                    if (GUILayout.Button("Load", SpriteColliderStyle.MenuBarButtonStyle))
                    {
                        Load();
                    }
                    RepaintOnHover();
                    if (GUILayout.Button("Add", SpriteColliderStyle.MenuBarButtonStyle))
                    {
                        ShowAddMenu();
                    }
                    RepaintOnHover();
                    if (GUILayout.Button("Close", SpriteColliderStyle.MenuBarButtonStyle))
                    {
                        Save();
                        spriteColliderData = null;
                        selectedColliderData = -1;
                        selectedFrame = 0;
                    }
                    RepaintOnHover();
                }
            }
            using (new GUILayout.VerticalScope(SpriteColliderStyle.Background1Style.Copy().WithHeight(467)))
            {
                using (new GUILayout.VerticalScope(SpriteColliderStyle.Background2Style.Copy().WithHeight(467).WithMargin(0, 5, 5, 5)))
                {
                    DrawEventArea();
                }
            }
        }
    }

    void DrawDefaultView()
    {
        var rect = new Rect();
        rect.center = position.center - position.position - new Vector2(150f, 30f);
        rect.size = new Vector2(300f, 60f);

        GUI.Label(rect, "Drag AnimationClip or SpriteColliderData here");

        if (Event.current.type == EventType.DragPerform || Event.current.type == EventType.DragUpdated)
        {
            DragAndDrop.AcceptDrag();
            if (DragAndDrop.objectReferences[0] is SpriteColliderData spriteColliderData)
            {
                Load(spriteColliderData);
            }
            else if (DragAndDrop.objectReferences[0] is AnimationClip animationClip)
            {
                var scd = ScriptableObject.CreateInstance<SpriteColliderData>();
                scd.Clip = animationClip;

                Load(scd);
            }
        }
    }

    void DrawSlicer()
    {
        var textureAreaRect = SpriteColliderStyle.CreateAreaRect(SpriteColliderStyle.Background1Style.Copy().WithHeight(470).WithWidth(984).WithMargin(20, 21, 20, 20));
        var slicerRect = new Rect(textureAreaRect);

        if (slicer == null)
        {
            slicer = new UVSlicer(slicerRect);
            slicer.UVRectColor = Color.magenta;
            SetupSlicer();
        }

        if (selectedFrameChanged && Event.current.type != EventType.Layout)
        {
            // slicer.SetRects(slicerRect);
            // slicer.ResetUVRect();
            slicer.AreaRect = slicerRect;
            slicer.UVRectColor = Color.magenta;
            selectedFrameChanged = false;
            slicer.SetTexture(slicedSprites[selectedFrame]);
        }

        // EditorGUI.DrawTextureTransparent(slicerRect, SpriteColliderStyle.Selected, ScaleMode.StretchToFill);

        if (selectedFrame != -1 && selectedColliderData != -1)
        {
            var scd = spriteColliderData.Colliders[selectedColliderData];
            var eventData = scd.Rects[selectedFrame];
            slicer.UVRectActive = eventData.Active;
            switch (scd.Mode)
            {
                case SpriteColliderData.Mode.Hitbox: slicer.UVRectColor = SpriteColliderStyle.HitboxColor; break;
                case SpriteColliderData.Mode.Hurtbox: slicer.UVRectColor = SpriteColliderStyle.HurtboxColor; break;
            }
        }
        else
        {
            slicer.UVRectActive = false;
        }

        slicer.OnGUI();

        if (slicer.Repaint)
        {
            Repaint();
            slicer.Repaint = false;
        }

        SetPreviewProxyData();
    }

    void DrawEventArea()
    {
        // Clip Frames
        clipFrameLineOrigins.Clear();
        using (new GUILayout.HorizontalScope(SpriteColliderStyle.Background4Style.Copy().WithHeight(80).WithWidth(1014)))
        {
            SpriteColliderStyle.CreateSpacing(SpriteColliderStyle.Background4Style.Copy().WithWidth(226));
            for (int i = 0; i < frameCount; i++)
            {
                DrawClipFrame(i);
            }
        }

        // Events
        using (new GUILayout.VerticalScope(SpriteColliderStyle.Background3Style.Copy().WithHeight(382).WithWidth(1014)))
        {
            foreach (var clipFrameOrigin in clipFrameLineOrigins)
            {
                lineVertTemp[0] = clipFrameOrigin;
                lineVertTemp[1] = clipFrameOrigin - Vector2.down * 390f;
                // Handles.color = SpriteColliderStyle.BackgroundColor1;
                Handles.color = Color.white;
                Handles.DrawAAPolyLine(3f, lineVertTemp);
            }
            using (var scrollView = new GUILayout.ScrollViewScope(eventScrollPos, false, true))
            {
                if (spriteColliderData != null)
                    for (int i = 0; i < spriteColliderData.Colliders.Count; i++)
                    {
                        DrawEvent(i);
                    }

                eventScrollPos = scrollView.scrollPosition;
            }
        }
    }

    void DrawClipFrame(int frameIndex)
    {
        var frameStyle = SpriteColliderStyle.ClipFrameStyle;
        if (selectedFrame == frameIndex) frameStyle = SpriteColliderStyle.ClipFrameSelectedStyle;
        if (GUILayout.Button((Texture2D)null, frameStyle))
        {
            selectedFrame = frameIndex;
            slicer.SetTexture(slicedSprites[frameIndex]);
            selectedFrameChanged = true;

            if (selectedColliderData != -1)
            {
                var scd = spriteColliderData.Colliders[selectedColliderData];
                var eventData = scd.Rects[frameIndex];
                slicer.SetUVRect(eventData.SpriteRect);
            }
        }
        RepaintOnHover();
        var lastRect = GUILayoutUtility.GetLastRect();

        GUI.DrawTexture(lastRect, slicedSprites[frameIndex], ScaleMode.ScaleToFit);

        Vector2 lineOrigin = lastRect.center - Vector2.down * 32;
        clipFrameLineOrigins.Add(lineOrigin);
    }

    void DrawEvent(int eventIndex)
    {
        if (Event.current.type != EventType.Layout)
        {
            hoveredEventData = -1;
        }

        using (new GUILayout.HorizontalScope(SpriteColliderStyle.EventContainerStyle))
        {
            var scd = spriteColliderData.Colliders[eventIndex];
            var mode = scd.Mode;
            using (new GUILayout.HorizontalScope(SpriteColliderStyle.EventHeaderStyle))
            {
                GUILayout.Label($"{mode}", new GUIStyle().WithAlignment(TextAnchor.MiddleCenter).WithWidth(128).WithHeight(SpriteColliderStyle.EventHeaderStyle.fixedHeight));
                {
                    GUIStyle style = null;
                    switch (mode)
                    {
                        case SpriteColliderData.Mode.Hitbox:
                            style = SpriteColliderStyle.HitboxEventNodeStyle;
                            break;
                        case SpriteColliderData.Mode.Hurtbox:
                            style = SpriteColliderStyle.HurtboxEventNodeStyle;
                            break;
                    }
                    
                    if (GUILayout.Button((Texture2D)null, style.Copy().WithMargin(9, 9, 9, 9).WithHoverBackground(SpriteColliderStyle.EventIconHoverTexture)))
                    {
                        if (Event.current.button == 0)
                        {
                            selectedColliderData = eventIndex;
                            slicer.SetUVRect(spriteColliderData.Colliders[selectedColliderData].Rects[selectedFrame].SpriteRect);
                        }
                        else if (Event.current.button == 1)
                        {
                            ShowEventContextMenu(eventIndex);
                        }
                    }
                }
            }

            for (int i = 0; i < frameCount; i++)
            {
                var style = SpriteColliderStyle.EventNodeStyle;
                var colliderData = scd.Rects[i];
                if (colliderData.Active)
                {
                    switch (mode)
                    {
                        case SpriteColliderData.Mode.Hitbox:
                            style = SpriteColliderStyle.HitboxEventNodeStyle;
                            break;
                        case SpriteColliderData.Mode.Hurtbox:
                            style = SpriteColliderStyle.HurtboxEventNodeStyle;
                            break;
                    }
                }

                bool clicked = false;
                if (GUILayout.Button((Texture2D)null, style))
                {
                    if (Event.current.button == 0)
                    {
                        selectedFrame = i;
                        selectedColliderData = eventIndex;
                        slicer.SetTexture(slicedSprites[i]);
                        selectedFrameChanged = true;
                        slicer.SetUVRect(colliderData.SpriteRect);
                    }
                    else if (Event.current.button == 1)
                    {
                        scd.Rects[i].Active = !scd.Rects[i].Active;
                    }
                    clicked = true;
                }
                var eventButtonRect = GUILayoutUtility.GetLastRect();

                if (!clicked && Event.current.type != EventType.Layout)
                {
                    if (eventButtonRect.Contains(Event.current.mousePosition))
                    {
                        hoveredEventData = eventIndex;
                    }
                }

                var buttonRect = RectHelpers.Inset(eventButtonRect, Vector2.one * 34);
                // GUI.DrawTexture(buttonRect, SpriteColliderStyle.SmallRoundedBorderTexture);

                if (i == selectedFrame && selectedColliderData == eventIndex)
                {
                    buttonRect = RectHelpers.Inset(eventButtonRect, Vector2.one * 20);
                    GUI.DrawTexture(buttonRect, SpriteColliderStyle.EventNodeSelectedStyle.normal.background);
                }

                // Connector Line
                if (colliderData.Active && i < frameCount - 1)
                {
                    var next = scd.Rects[i + 1];
                    if (next.Active)
                    {
                        buttonRect = RectHelpers.Inset(eventButtonRect, Vector2.one * 20);

                        Vector2 origin = buttonRect.center;
                        origin.x += buttonRect.width * 0.5f + 6;
                        lineVertTemp[0] = origin;
                        lineVertTemp[1] = origin + Vector2.right * 52;
                        Handles.color = Color.black;
                        Handles.DrawAAPolyLine(3f, lineVertTemp);
                    }
                }
            }
        }

        var lastRect = GUILayoutUtility.GetLastRect();
        Vector2 lineOrigin = lastRect.center;
        lineOrigin.x = 183;
        lineVertTemp[0] = lineOrigin;
        lineVertTemp[1] = lineOrigin + Vector2.right * 59;
        Handles.color = Color.black;
        Handles.DrawAAPolyLine(3f, lineVertTemp);
    }

    void ShowAddMenu()
    {
        var gm = new GenericMenu();

        foreach (SpriteColliderData.Mode val in System.Enum.GetValues(typeof(SpriteColliderData.Mode)))
        {
            gm.AddItem(new GUIContent($"{val.ToString()}"), false, () => AddData(val));
        }

        gm.ShowAsContext();

        void AddData(SpriteColliderData.Mode mode)
        {
            spriteColliderData.AddColliderData(mode, slicedSprites.Count, slicer.DefaultUVRect);
        }
    }

    void ShowEventContextMenu(int eventIndex)
    {
        var eventData = spriteColliderData.Colliders[eventIndex];

        var gm = new GenericMenu();

        foreach (SpriteColliderData.Mode val in System.Enum.GetValues(typeof(SpriteColliderData.Mode)))
        {
            gm.AddItem(new GUIContent($"Mode/{val.ToString()}"), eventData.Mode == val, () => eventData.Mode = val);
        }

        gm.AddItem(new GUIContent("Remove"), false, () => eventIndexToRemove = eventIndex);

        gm.ShowAsContext();
    }
    #endregion

    void RepaintOnHover()
    {
        if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
        {
            Repaint();
        }
    }

    void HandleEvent(Event current)
    {
        switch (current.type)
        {
            case EventType.KeyDown:
                HandleKeyDownEvent(current);
                break;
            case EventType.KeyUp:
                break;
        }

        if (current.type != EventType.Layout)
        {
            if (eventIndexToRemove != -1)
            {
                spriteColliderData.Colliders.RemoveAt(eventIndexToRemove);
                eventIndexToRemove = -1;
            }
        }
    }

    void HandleKeyDownEvent(Event current)
    {
        switch (current.keyCode)
        {
            case KeyCode.D:
                if (selectedColliderData != -1)
                {
                    if (CopyToNextFrameData(selectedFrame, selectedColliderData))
                    {
                        selectedFrame++;
                        selectedFrameChanged = true;
                    }
                }
                break;
            case KeyCode.A:
                if (selectedColliderData != -1)
                {
                    if (CopyToPreviousFrameData(selectedFrame, selectedColliderData))
                    {
                        selectedFrame--;
                        selectedFrameChanged = true;
                    }
                }
                break;
            case KeyCode.X:
                if (selectedColliderData != -1)
                {
                    ClearFrameData(selectedFrame, selectedColliderData);
                }
                break;
        }
    }

    #region Actions
    bool CopyToNextFrameData(int frameIndex, int eventIndex)
    {
        if (frameIndex == slicedSprites.Count - 1) return false;

        var currentData = spriteColliderData.Colliders[eventIndex].Rects[frameIndex];
        var nextData = spriteColliderData.Colliders[eventIndex].Rects[frameIndex + 1];

        nextData.Active = currentData.Active;
        nextData.SpriteRect = currentData.SpriteRect;

        return true;
    }

    bool CopyToPreviousFrameData(int frameIndex, int eventIndex)
    {
        if (frameIndex == 0) return false;

        var currentData = spriteColliderData.Colliders[eventIndex].Rects[frameIndex];
        var nextData = spriteColliderData.Colliders[eventIndex].Rects[frameIndex - 1];

        nextData.Active = currentData.Active;
        nextData.SpriteRect = currentData.SpriteRect;

        return true;
    }

    bool ClearFrameData(int frameIndex, int eventIndex)
    {
        var currentData = spriteColliderData.Colliders[eventIndex].Rects[frameIndex];
        currentData.Active = false;
        currentData.SpriteRect = slicer.DefaultUVRect;

        return true;
    }
    #endregion

    #region Management
    void Save()
    {
        if (spriteColliderData == null) return;

        string exists = AssetDatabase.GetAssetPath(spriteColliderData);
        if (string.IsNullOrEmpty(exists))
        {
            string savePath = EditorUtility.SaveFilePanelInProject("Save SpriteCollider", spriteColliderData.Clip.name, "asset", "Save sprite collider data");
            if (!string.IsNullOrEmpty(savePath))
            {
                AssetDatabase.CreateAsset(spriteColliderData, savePath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }
        else
        {
            EditorUtility.SetDirty(spriteColliderData);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }

    void Load()
    {
        var assetPath = EditorUtility.OpenFilePanel("Open Sprite Collider Data", Application.dataPath, "asset");
        if (string.IsNullOrEmpty(assetPath)) return;

        assetPath = "Assets/" + assetPath.Replace(Application.dataPath, "");
        UnityEngine.Debug.Log($"{assetPath}");

        var asset = AssetDatabase.LoadAssetAtPath(assetPath, typeof(SpriteColliderData)) as SpriteColliderData;
        if (asset == null) return;

        Load(asset);
    }

    void Load(SpriteColliderData scd)
    {
        spriteColliderData = scd;

        clipSprites = SpriteSlicer.GetSpritesFromClip(spriteColliderData.Clip);
        slicedSprites = SpriteSlicer.SliceSprites(clipSprites);

        selectedFrame = 0;
        selectedColliderData = -1;
        selectedFrameChanged = true;
    }
    #endregion

    #region Preview
    SpriteColliderPreview previewProxy;
    void SetPreviewProxyData()
    {
        if (previewProxy == null || previewProxy.gameObject != Selection.activeGameObject)
        {
            var selected = Selection.activeGameObject;
            if (selected != null && selected.GetComponent<SpriteColliderPreview>() is SpriteColliderPreview preview)
            {
                previewProxy = preview;
            }
            else
            {
                previewProxy = null;
                return;
            }
        }

        previewProxy.data = spriteColliderData;
        previewProxy.frameIndex = selectedFrame;
        previewProxy.colliderDataIndex = selectedColliderData;
        previewProxy.SetSprite(clipSprites[selectedFrame]);
    }
    #endregion
}