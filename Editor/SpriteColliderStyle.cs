using UnityEngine;
using UnityEditor;
using Refsa.UVSlicer;

public static class SpriteColliderStyle
{
    public static bool IsSetup { get; private set; } = false;

    public static Color BackgroundColor1 = new Color32(196, 196, 196, 255);
    public static Color BackgroundColor2 = new Color32(144, 144, 144, 255);
    public static Color BackgroundColor3 = new Color32(96, 96, 96, 255);
    public static Color BackgroundColor4 = new Color32(32, 32, 32, 255);

    public static Color FontColor1 = new Color32(63, 63, 63, 255);
    public static Color FontColor2 = new Color32(208, 208, 208, 255);
    public static Color FontColor3 = new Color32(237, 237, 237, 255);

    public static Color SelectedColor = new Color32(78, 255, 117, 255);
    public static Color ErrorColor = new Color32(255, 156, 156, 255);

    public static Color HitboxColor = new Color32(252, 255, 84, 255);
    public static Color HurtboxColor = new Color32(97, 236, 255, 255);

    public static Texture2D Background1; 
    public static Texture2D Background2;
    public static Texture2D Background3;
    public static Texture2D Background4;
    public static Texture2D Blank;

    public static Texture2D Selected;
    public static Texture2D Error;

    public static GUIStyle Background1Style;
    public static GUIStyle Background2Style;
    public static GUIStyle Background3Style;
    public static GUIStyle Background4Style;

    public static GUIStyle BlankBackgroundStyle;

    public static GUIStyle Font1Style;
    public static GUIStyle Font2Style;
    public static GUIStyle Font3Style;

    public static GUIStyle MenuBarStyle;
    public static GUIStyle MenuBarButtonStyle;
    public static GUIStyle MenuHeaderStyle;

    public static GUIStyle ClipFrameStyle;
    public static GUIStyle ClipFrameSelectedStyle;

    public static GUIStyle EventHeaderStyle;
    public static GUIStyle EventNodeStyle;
    public static GUIStyle HitboxEventNodeStyle;
    public static GUIStyle HurtboxEventNodeStyle;
    public static GUIStyle EventContainerStyle;
    public static GUIStyle EventNodeSelectedStyle;

    static string packagePath = @"\SpriteCollider\Editor";
    static string packageExternalPath = @"Packages/com.refsa.spritecollider/Editor/";
    static string smallRoundedPath = "Textures/Small_Rounded.png";
    static string smallRoundedBorderPath = "Textures/Small_Rounded_Border.png";
    static string mediumRoundedPath = "Textures/Medium_Rounded.png";
    static string menuBarButtonPath = "Textures/Menu_Bar_Button.png";
    static string menuBarHeaderPath = "Textures/Menu_Bar_Header.png";
    static string menuBarBackgroundPath = "Textures/Menu_Bar_Background.png";
    static string eventHeaderPath = "Textures/Event_Header.png";
    static string circlePath = "Textures/Circle.png";

    public static Texture2D SmallRoundedTexture;
    public static Texture2D SmallRoundedBorderTexture;
    public static Texture2D MediumRoundedTexture;
    public static Texture2D MenuBarButtonTexture;
    public static Texture2D MenuBarHeaderTexture;
    public static Texture2D MenuBarBackgroundTexture;
    public static Texture2D EventHeaderTexture;
    public static Texture2D CircleTexture;

    public static Texture2D EventIconHoverTexture;

    public static void Setup()
    {
        SetupTextures();
        SetupStyles();
    }

    static string GetAssetPath()
    {
        if (FolderExists(Application.dataPath, packagePath, out var path))
        {
            return path + @"\";
        }
        else
        {
            return packageExternalPath;
        }
    }

    static bool FolderExists(string path, string folderName, out string foundPath)
    {
        bool found = false;
        foundPath = "";
        foreach (var folder in System.IO.Directory.GetDirectories(path))
        {
            if (folder.EndsWith(folderName))
            {
                int index = folder.IndexOf(@"Assets\");
                foundPath = folder.Substring(index, folder.Length - index);

                return true;
            }
            else
            {
                if (FolderExists(folder, folderName, out foundPath))
                {
                    return true;
                }
            }
        }

        return found;
    }

    static void SetupTextures()
    {
        Background1 = TextureGenerator.GenerateTexture(BackgroundColor1);
        Background2 = TextureGenerator.GenerateTexture(BackgroundColor2);
        Background3 = TextureGenerator.GenerateTexture(BackgroundColor3);
        Background4 = TextureGenerator.GenerateTexture(BackgroundColor4);

        Selected = TextureGenerator.GenerateTexture(SelectedColor);
        Error = TextureGenerator.GenerateTexture(ErrorColor);

        Blank = TextureGenerator.GenerateTexture(Color.clear);

        string assetPath = GetAssetPath();
        UnityEngine.Debug.Log($"{assetPath}");

        SmallRoundedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + smallRoundedPath);
        SmallRoundedBorderTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + smallRoundedBorderPath);
        MediumRoundedTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + mediumRoundedPath);
        MenuBarButtonTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + menuBarButtonPath);
        MenuBarHeaderTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + menuBarHeaderPath);
        MenuBarBackgroundTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + menuBarBackgroundPath);
        EventHeaderTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + eventHeaderPath);
        CircleTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(assetPath + circlePath);

        TextureGenerator.SetColor(SmallRoundedTexture, Color.grey, out EventIconHoverTexture);
    }

    static void SetupStyles()
    {
        // General Styles
        {
            Background1Style = new GUIStyle();
            Background1Style.normal.background = Background1;
            Background1Style.normal.scaledBackgrounds = new[] { Background1 };

            Background2Style = new GUIStyle();
            Background2Style.normal.background = Background2;
            Background2Style.normal.scaledBackgrounds = new[] { Background2 };

            Background3Style = new GUIStyle();
            Background3Style.normal.background = Background3;
            Background3Style.normal.scaledBackgrounds = new[] { Background3 };

            Background4Style = new GUIStyle();
            Background4Style.normal.background = Background4;
            Background4Style.normal.scaledBackgrounds = new[] { Background4 };

            BlankBackgroundStyle = new GUIStyle();
            BlankBackgroundStyle.normal.background = Blank;
            BlankBackgroundStyle.normal.scaledBackgrounds = new[] { Blank };
        }

        // Menu Bar
        {
            TextureGenerator.SetColor(MenuBarBackgroundTexture, BackgroundColor3, out var menubarBackground);
            MenuBarStyle = new GUIStyle();
            MenuBarStyle.fixedHeight = 46;
            MenuBarStyle.border = new RectOffset(5, 5, 5, 5);
            MenuBarStyle.normal.background = menubarBackground;

            TextureGenerator.SetColor(MenuBarButtonTexture, BackgroundColor4, out var normalTex);
            TextureGenerator.SetColor(MenuBarButtonTexture, BackgroundColor2, out var hoverTex);
            TextureGenerator.SetColor(MenuBarButtonTexture, BackgroundColor1, out var activeTex);

            MenuBarButtonStyle = new GUIStyle();
            MenuBarButtonStyle.fixedWidth = 100;
            MenuBarButtonStyle.fixedHeight = 43;
            MenuBarButtonStyle.margin = new RectOffset(0, 10, 0, 0);

            MenuBarButtonStyle.normal.background = normalTex;
            MenuBarButtonStyle.hover.background = hoverTex;
            MenuBarButtonStyle.active.background = activeTex;

            MenuBarButtonStyle.normal.textColor = FontColor3;
            MenuBarButtonStyle.hover.textColor = FontColor3;
            MenuBarButtonStyle.active.textColor = FontColor3;
            MenuBarButtonStyle.alignment = TextAnchor.MiddleCenter;
            MenuBarButtonStyle.fontSize = 16;

            TextureGenerator.SetColor(MenuBarHeaderTexture, BackgroundColor4, out var headerNormalTex);
            MenuHeaderStyle = new GUIStyle();
            MenuHeaderStyle.fixedWidth = 300;
            MenuHeaderStyle.fixedHeight = 43;
            MenuHeaderStyle.margin = new RectOffset(5, 15, 0, 0);
            MenuHeaderStyle.normal.background = headerNormalTex;
            MenuHeaderStyle.normal.textColor = FontColor3;
            MenuHeaderStyle.alignment = TextAnchor.MiddleCenter;
            MenuHeaderStyle.fontSize = 20;
        }

        // Clip Frames
        {
            TextureGenerator.SetColor(MediumRoundedTexture, BackgroundColor1, out var normalTex);
            TextureGenerator.SetColor(MediumRoundedTexture, BackgroundColor2, out var hoverTex);
            TextureGenerator.SetColor(MediumRoundedTexture, BackgroundColor3, out var activeTex);
            TextureGenerator.SetColor(MediumRoundedTexture, SelectedColor, out var selectedTex);

            ClipFrameStyle = new GUIStyle();
            ClipFrameStyle.fixedHeight = 64;
            ClipFrameStyle.fixedWidth = 64;
            ClipFrameStyle.margin = new RectOffset(0, 20, 8, 0);
            ClipFrameStyle.normal.background = normalTex;
            ClipFrameStyle.hover.background = hoverTex;
            ClipFrameStyle.active.background = activeTex;

            ClipFrameSelectedStyle = new GUIStyle();
            ClipFrameSelectedStyle.fixedHeight = 64;
            ClipFrameSelectedStyle.fixedWidth = 64;
            ClipFrameSelectedStyle.margin = new RectOffset(0, 20, 8, 0);
            ClipFrameSelectedStyle.normal.background = selectedTex;
        }

        {
            EventContainerStyle = new GUIStyle();
            // EventContainerStyle.normal.background = Selected;
            EventContainerStyle.fixedWidth = 998;
            EventContainerStyle.fixedHeight = 50;
            EventContainerStyle.margin = new RectOffset(0, 0, 20, 20);

            TextureGenerator.SetColor(EventHeaderTexture, BackgroundColor2, out var headerTex);
            EventHeaderStyle = new GUIStyle();
            EventHeaderStyle.normal.background = headerTex;
            EventHeaderStyle.fixedWidth = 178;
            EventHeaderStyle.fixedHeight = 50;
            EventHeaderStyle.margin = new RectOffset(5, 59, 0, 0);
 
            EventNodeStyle = new GUIStyle(); 
            EventNodeStyle.fixedWidth = 32;
            EventNodeStyle.fixedHeight = 32;
            EventNodeStyle.margin = new RectOffset(0, 52, 9, 0);
            TextureGenerator.SetColor(SmallRoundedTexture, BackgroundColor1, out var normalTex);
            TextureGenerator.SetColor(SmallRoundedTexture, BackgroundColor2, out var hoverTex);
            TextureGenerator.SetColor(SmallRoundedTexture, BackgroundColor4, out var activeTex);
            EventNodeStyle.normal.background = normalTex;
            EventNodeStyle.hover.background = hoverTex;
            EventNodeStyle.active.background = activeTex;

            TextureGenerator.SetColor(SmallRoundedTexture, HitboxColor, out var hitboxNormalTex);
            HitboxEventNodeStyle = new GUIStyle(EventNodeStyle);
            HitboxEventNodeStyle.normal.background = hitboxNormalTex;

            TextureGenerator.SetColor(SmallRoundedTexture, HurtboxColor, out var hurtboxNormalTex);
            HurtboxEventNodeStyle = new GUIStyle(EventNodeStyle);
            HurtboxEventNodeStyle.normal.background = hurtboxNormalTex;

            TextureGenerator.SetColor(CircleTexture, SelectedColor, out var circleTex);
            EventNodeSelectedStyle = new GUIStyle();
            EventNodeSelectedStyle.normal.background = circleTex;
            EventNodeSelectedStyle.fixedHeight = 20;
            EventNodeSelectedStyle.fixedWidth = 20; 
            EventNodeSelectedStyle.margin = new RectOffset(6, 6, 6, 6);
        }
    }

    public static GUIStyle Copy(this GUIStyle self)
    {
        return new GUIStyle(self);
    }

    public static GUIStyle WithHeight(this GUIStyle self, float height)
    {
        self.fixedHeight = height;

        return self;
    }

    public static GUIStyle WithWidth(this GUIStyle self, float width)
    {
        self.fixedWidth = width;

        return self;
    }

    public static GUIStyle WithMargin(this GUIStyle self, int u, int d, int l, int r)
    {
        self.margin = new RectOffset(l, r, u, d);

        return self;
    }

    public static GUIStyle WithBorderRadius(this GUIStyle self, int u, int d, int l, int r)
    {
        self.border = new RectOffset(l, r, u, d);

        return self;
    }

    public static GUIStyle WithAlignment(this GUIStyle self, TextAnchor alignment)
    {
        self.alignment = alignment;

        return self;
    }

    public static GUIStyle WithoutHover(this GUIStyle self)
    {
        self.hover.background = null;   
        self.hover.scaledBackgrounds = null;
        return self;
    }

    public static GUIStyle WithoutActive(this GUIStyle self)
    {
        self.active.background = null;   
        self.active.scaledBackgrounds = null;
        return self;
    }

    public static GUIStyle WithHoverBackground(this GUIStyle self, Texture2D texture)
    {
        self.hover.background = texture;
        return self;
    }

    public static Rect CreateAreaRect(GUIStyle style)
    {
        using (new GUILayout.VerticalScope(style)) GUILayout.Label("");
        return GUILayoutUtility.GetLastRect();
    }

    public static void CreateSpacing(GUIStyle style)
    {
        using (new GUILayout.HorizontalScope(style)) GUILayout.Label("");
    }
}