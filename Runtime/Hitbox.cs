using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{
    [SerializeField] new BoxCollider2D collider;

    public BoxCollider2D Collider => collider;

    public event System.Action<IHitEvent> OnHit;

    Rect defaultHitboxPos;

    void Awake() 
    {
        defaultHitboxPos = new Rect(collider.offset, collider.size);    
    }

    public void HitHandler(IHitEvent hitEvent)
    {
        OnHit?.Invoke(hitEvent);
    }

    public void SetHitbox(Rect info)
    {
        collider.offset = info.position;
        collider.size = info.size;
    }

    public void ResetHitbox()
    {
        collider.offset = defaultHitboxPos.position;
        collider.size = defaultHitboxPos.size;
    }
}


public enum HitEventType { Damage, Healing }
public interface IHitEvent
{
    HitEventType EventType {get;}
}

public struct DamageHitEvent : IHitEvent
{
    public HitEventType EventType => HitEventType.Damage;

    public float Damage;
}

public struct HealingHitEvent : IHitEvent
{
    public HitEventType EventType => HitEventType.Healing;

    public float Amount;
}