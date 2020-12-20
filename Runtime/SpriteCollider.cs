using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteCollider : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] List<SpriteColliderData> colliderDatas;

    [SerializeField] Hitbox hitbox;

    Dictionary<AnimationClip, SpriteColliderData> colliderDataLookup;

    Vector2 posOffset = new Vector2(-1f / 48f, 0f);

    public bool DebugViewActive { get; set; }

    void OnEnable()
    {
        var srBounds = spriteRenderer.bounds;
        var texRect = spriteRenderer.sprite.rect;

        colliderDataLookup = new Dictionary<AnimationClip, SpriteColliderData>();
        foreach (var cd in colliderDatas)
        {
            var cdi = ScriptableObject.Instantiate(cd);
            colliderDataLookup.Add(cdi.Clip, cdi);
        }
    }

    public bool CheckEvent(SpriteColliderData.Mode mode, int layerMask, ref List<Collider2D> hits)
    {
        var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        var clip = clipInfo[0].clip;

        if (colliderDataLookup.TryGetValue(clip, out var scd))
        {
            var clipState = animator.GetCurrentAnimatorStateInfo(0);
            float normTime = clipState.normalizedTime - (int)clipState.normalizedTime;
            int frame = Mathf.FloorToInt(Mathf.Clamp01(normTime) * (float)scd.Colliders[0].Rects.Count);

            for (int i = 0; i < scd.Colliders.Count; i++)
            {
                var cd = scd.Colliders[i];
                if (cd.Mode != mode) continue;
                var rect = GetLocalRect(cd, frame);

                hits.AddRange(Physics2D.OverlapBoxAll((Vector2)transform.position + rect.position, rect.size, 0f, layerMask));
                return true;
            }
        }

        return false;
    }

    void Update()
    {
        var clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        var clip = clipInfo[0].clip;

        bool hasBodyCollider = false;

        if (colliderDataLookup.TryGetValue(clip, out var scd))
        {
            var clipState = animator.GetCurrentAnimatorStateInfo(0);
            float normTime = clipState.normalizedTime - (int)clipState.normalizedTime;
            int frame = Mathf.FloorToInt(Mathf.Clamp01(normTime) * (float)scd.Colliders[0].Rects.Count);

            for (int i = 0; i < scd.Colliders.Count; i++)
            {
                if (scd.Colliders[i].Mode == SpriteColliderData.Mode.Hitbox)
                {
                    var rect = GetLocalRect(scd.Colliders[i], frame);
                    hitbox.SetHitbox(rect);
                    hasBodyCollider = true;
                }
            }
        }

        if (!hasBodyCollider)
        {
            hitbox.ResetHitbox();
        }
    }

    Rect GetLocalRect(SpriteColliderData.ColliderData data, int frame)
    {
        var srBounds = spriteRenderer.bounds;
        var fi = data.Rects[frame];

        if (!fi.Active) return new Rect();

        var texRect = spriteRenderer.sprite.rect;

        var subRect = new Rect(fi.SpriteRect);
        subRect.position /= texRect.size;
        subRect.size /= texRect.size;

        var wPos = (Vector2)srBounds.min + (Vector2.up * srBounds.extents.y * 2);
        var wSize = srBounds.size;

        if (spriteRenderer.flipX)
        {
            wPos.x += srBounds.extents.x * 2;
            wSize.x *= -1;
        }

        var apos = wPos + wSize * subRect.position * new Vector2(1, -1);
        var asize = wSize * subRect.size;

        // DEBUG
        if (DebugViewActive)
        {
            Color color = Color.clear;
            switch (data.Mode)
            {
                case SpriteColliderData.Mode.Hitbox: color = Color.green * 0.75f; break;
                case SpriteColliderData.Mode.Hurtbox: color = Color.red * 0.75f; break;
            }

            var abounds = new Bounds((Vector3)apos + (new Vector3(asize.x, asize.y * -1, -5f) / 2f), asize);
            // Refsa.ReGizmo.ReGizmo.DrawBounds(abounds, color);
        }

        if (spriteRenderer.flipX)
        {
            asize.x *= -1;
            apos -= asize * new Vector2(0.5f, 0.5f);
        }
        else
        {
            apos -= asize * new Vector2(-0.5f, 0.5f);
        }
        return new Rect(apos - (Vector2)transform.position, asize);
    }
}
