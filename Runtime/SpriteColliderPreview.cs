using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
public class SpriteColliderPreview : MonoBehaviour
{
    public SpriteColliderData data;
    public int colliderDataIndex;
    public int frameIndex;

    public SpriteRenderer spriteRenderer;

    public void SetSprite(Sprite sprite)
    {
        spriteRenderer.sprite = sprite;
    }

    void OnDrawGizmosSelected()
    {
        if (Application.isPlaying || UnityEditor.Selection.activeGameObject != this.gameObject) return;
        if (data == null || colliderDataIndex == -1 || colliderDataIndex >= data.Colliders.Count || frameIndex == -1 || frameIndex >= data.Colliders[0].Rects.Count)
        {
            return;
        }

        var srBounds = spriteRenderer.bounds;

        var cd = data.Colliders[colliderDataIndex];
        var fi = cd.Rects[frameIndex];

        if (fi.Active)
        {
            float flipX = spriteRenderer.flipX ? -1 : 1;
            var texRect = spriteRenderer.sprite.rect;

            var subRect = new Rect(fi.SpriteRect);
            subRect.position /= texRect.size;
            subRect.size /= texRect.size;

            var wPos = (Vector2)srBounds.min + (Vector2.up * srBounds.extents.y * 2);
            var wSize = srBounds.size;

            if (flipX == -1)
            {
                wPos.x += srBounds.extents.x * 2;
                wSize.x *= -1;
            }

            var apos = wPos + wSize * subRect.position * new Vector2(1, -1);
            var asize = wSize * subRect.size;

            var abounds = new Bounds(apos + (asize / 2f) * new Vector2(1, -1), asize);
            Gizmos.color = Color.red * 0.5f;
            Gizmos.DrawCube(abounds.center, abounds.size);
        }

        var posOffset = (Vector2)srBounds.min + (Vector2.up * srBounds.extents.y * 2);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere((Vector3)posOffset, 0.05f);
        Gizmos.color = Color.green * 0.5f;
        Gizmos.DrawCube(spriteRenderer.bounds.center, spriteRenderer.bounds.size);
    }
}   
#endif