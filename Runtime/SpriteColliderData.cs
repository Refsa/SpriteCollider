using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpriteColliderData : ScriptableObject
{
    public enum Mode {
        Hitbox,
        Hurtbox,
    }

    // [System.Serializable]
    // public class ColliderData
    // {
    //     public string Name;
    //     public int Frame;
    //     public Mode Mode;
    //     public Rect SpriteRect;
    //     public Rect LocalRect;
    // }

    [System.Serializable]
    public class ColliderFrameData
    {
        public bool Active;
        public Rect SpriteRect;
        public Rect LocalRect;
        public Rect FlippedLocalRect;

        public ColliderFrameData(bool active, Rect spriteRect, Rect localRect)
        {
            Active = active;
            SpriteRect = spriteRect;
            LocalRect = localRect;
            FlippedLocalRect = localRect;
        }
    }

    [System.Serializable]
    public class ColliderData
    {
        public int Index;
        public string Name;
        public Mode Mode;
        public List<ColliderFrameData> Rects;
    }


    [SerializeField] public AnimationClip Clip;
    [SerializeField] public List<ColliderData> Colliders = new List<ColliderData>();

    public void AddColliderData(Mode mode, int frameCount, Rect spriteRect)
    {
        var colliderData = new ColliderData();
        colliderData.Mode = mode;
        colliderData.Rects = new List<ColliderFrameData>();
        for (int i = 0; i < frameCount; i++)
        {
            colliderData.Rects.Add(new ColliderFrameData(false, spriteRect, Rect.zero));
        }

        Colliders.Add(colliderData);
    }

    // public void AddCollider(ColliderData collider)
    // {
    //     Colliders.Add(collider);
    // }

    // public void RemoveCollider(ColliderData collider)
    // {
    //     Colliders.Remove(collider);
    // }

    // public void RemoveCollider(int index)
    // {
    //     Colliders.RemoveAt(index);
    // }

    // public bool TryGetCollider(int frame, out ColliderData collider)
    // {
    //     collider = Colliders.Find(e => e.Frame == frame);

    //     return collider != null;
    // }

    // public bool TryGetColliders(int frame, out List<ColliderData> colliders)
    // {
    //     colliders = this.Colliders.FindAll(e => e.Frame == frame);

    //     return colliders != null && colliders.Count > 0;
    // }

    // public bool DuplicateCollider(int index, int maxFrame)
    // {
    //     var colliderData = this.Colliders[index];

    //     if (colliderData.Frame + 1 >= maxFrame)
    //     {
    //         return false;
    //     }

    //     var newColliderData = new ColliderData {
    //         Name = colliderData.Name,
    //         Frame = colliderData.Frame + 1,
    //         Mode = colliderData.Mode,
    //         SpriteRect = colliderData.SpriteRect,
    //         LocalRect = colliderData.LocalRect,
    //     };

    //     AddCollider(newColliderData);

    //     return true;
    // }

    // public HashSet<int> FramesWithEvent(int index)
    // {
    //     var indices = new HashSet<int>();
    //     if (TryGetCollider(index, out var data))
    //     {
    //         foreach(int frame in this.Colliders.FindAll(e => e.Name == data.Name).Select(e => e.Frame))
    //         {
    //             if (!indices.Contains(frame))
    //             {
    //                 indices.Add(frame);
    //             }
    //         }
    //     }

    //     return indices;
    // }
}
