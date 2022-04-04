using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AttackHitIndicator : MonoBehaviour
{
    public StateMachina player;
    public Vector2 knockbackAngle;
    public float strength;
    private List<IDamageable> detectedDamageable = new List<IDamageable>();
    private List<IKnockable> detectedKnockable = new List<IKnockable>();

    public void CheckMeleeAttack(Collider2D collision)
    {
        foreach (IDamageable item in detectedDamageable.ToList())
        {
            item.damage(player.atkDmg);
            RemoveFromDetected(collision);
        }
        
    }
    public void CheckMeleeKnockback(Collider2D collision)
    {
        foreach (IKnockable item in detectedKnockable.ToList())
        {
            item.Knockback(knockbackAngle, strength, player.direction);
            RemoveFromDetected(collision);
        }
    }
    public void AddToDetected(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            detectedDamageable.Add(damageable);
            CheckMeleeAttack(collision);
        }
        IKnockable knockable = collision.GetComponent<IKnockable>();
        if (knockable != null)
        {
            detectedKnockable.Add(knockable);
            CheckMeleeKnockback(collision);
        }
    }
    public void RemoveFromDetected(Collider2D collision)
    {
        IDamageable damageable = collision.GetComponent<IDamageable>();
        if (damageable != null)
        {
            detectedDamageable.Remove(damageable);
        }
        IKnockable knockable = collision.GetComponent<IKnockable>();
        if (knockable != null)
        {
            detectedKnockable.Remove(knockable);
            CheckMeleeAttack(collision);
        }

    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        AddToDetected(collision);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        RemoveFromDetected(collision);
    }
}
