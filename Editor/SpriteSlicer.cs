using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class SpriteSlicer
{
    public static List<Texture2D> SliceSpritesFromClip(AnimationClip clip)
    {
        var sprites = GetSpritesFromClip(clip);

        return SliceSprites(sprites);
    }

    public static List<Texture2D> SliceSprites(List<Sprite> sprites)
    {
        var textures = new List<Texture2D>();

        if (sprites.Count == 0) return textures;

        var spriteTex = sprites[0].texture;

        if (!spriteTex.isReadable) return textures;

        foreach (var sprite in sprites)
        {
            var pixelRect = sprite.rect;
            var tex = new Texture2D(Mathf.CeilToInt(pixelRect.width), Mathf.CeilToInt(pixelRect.height), spriteTex.format, 0, true);
            tex.filterMode = FilterMode.Point;

            for ((int x, int i) = ((int)pixelRect.xMin, 0); x < pixelRect.xMax; x++, i++)
            {
                for ((int y, int j) = ((int)pixelRect.yMin, 0); y < pixelRect.yMax; y++, j++)
                {
                    tex.SetPixel(i, j, spriteTex.GetPixel(x, y));
                }
            }

            tex.Apply();
            textures.Add(tex);
        }

        return textures;
    }


    // https://answers.unity.com/questions/1245599/how-to-get-all-sprites-used-in-a-2d-animator.html
    public static List<Sprite> GetSpritesFromClip(AnimationClip clip)
    {
        var _sprites = new List<Sprite>();
        if (clip != null)
        {
            foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
            {
                ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                foreach (var frame in keyframes)
                {
                    _sprites.Add((Sprite)frame.value);
                }
            }
        }
        return _sprites;
    }

    public static int SpritesInClip(AnimationClip clip)
    {
        int frameCount = 0;
        if (clip != null)
        {
            foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(clip))
            {
                ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(clip, binding);
                foreach (var frame in keyframes)
                {
                    frameCount++;
                }
            }

        }
        return frameCount;
    }
}
