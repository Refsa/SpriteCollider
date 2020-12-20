using UnityEngine;

namespace Refsa.UVSlicer
{
    public static class TextureGenerator
    {
        public static Texture2D GenerateGridTexture(Color line, Color line2, Color bg)
        {
            Texture2D tex = new Texture2D(64, 64);
            Color[] cols = new Color[64 * 64];
            for (int y = 0; y < 64; y++)
            {
                for (int x = 0; x < 64; x++)
                {
                    Color col = bg;
                    if (y % 16 == 0 || x % 16 == 0) col = Color.Lerp(line2, bg, 0.85f);
                    if (y == 63 || x == 63) col = Color.Lerp(line, bg, 0.65f);
                    cols[(y * 64) + x] = col;
                }
            }
            tex.SetPixels(cols);
            tex.wrapMode = TextureWrapMode.Repeat;
            tex.filterMode = FilterMode.Bilinear;
            tex.name = "Grid";
            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateTexture(Color inner)
        {
            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, inner);

            tex.Apply();
            return tex;
        }

        public static Texture2D GenerateBorderTexture(Color mainColor, Color borderColor, Vector2Int size, int borderWidth = 1, FilterMode filterMode = FilterMode.Bilinear)
        {
            var newTexture = new Texture2D(size.x, size.y);

            int xMax = size.x - borderWidth - 1;
            int yMax = size.y - borderWidth - 1;

            for (int j = 0; j < size.x; j++)
            {
                for (int i = 0; i < size.y; i++)
                {
                    if (j <= borderWidth || j >= yMax || i <= borderWidth || i >= xMax)
                        newTexture.SetPixel(i, j, borderColor);
                    else
                        newTexture.SetPixel(i, j, mainColor);
                }
            }

            newTexture.filterMode = filterMode;
            newTexture.Apply();
            return newTexture;
        }

        public static Texture2D GenerateBorderTextureAspect(Color mainColor, Color borderColor, float aspectRatio)
        {
            int height = 50;
            int width = Mathf.FloorToInt(height * aspectRatio);

            var newTexture = new Texture2D(width, height);

            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    if (j <= 1 || j >= height - 2 || i <= 1 || i >= width - 2)
                        newTexture.SetPixel(i, j, borderColor);
                    else
                        newTexture.SetPixel(i, j, mainColor);
                }
            }

            newTexture.Apply();

            return newTexture;
        }

        public static Texture2D GenerateCheckerboardTexture(Color color1, Color color2)
        {
            var tex = new Texture2D(64, 64);

            for (int x = 0; x < 64; x++)
            {
                for (int y = 0; y < 64; y++)
                {
                    Color color = color1;

                    if ((x > 31 && y > 31) || (x < 31 && y < 31))
                        color = color2;

                    tex.SetPixel(x, y, color);
                }
            }

            tex.filterMode = FilterMode.Point;

            tex.Apply();
            return tex;
        }

        public static bool SetColor(Texture2D texture, Color color, out Texture2D tex)
        {
            if (!texture.isReadable)
            {
                tex = null;
                return false;
            }

            tex = new Texture2D(texture.width, texture.height);
            tex.filterMode = FilterMode.Bilinear;

            for (int x = 0; x < texture.width; x++)
            {
                for (int y = 0; y < texture.height; y++)
                {
                    var pixel = texture.GetPixel(x, y);
                    if (pixel.a != 0)
                    {
                        tex.SetPixel(x, y, color);
                    }
                    else
                    {
                        tex.SetPixel(x, y, Color.clear);
                    }
                }
            }
            tex.Apply();

            return true;
        }
    }
}