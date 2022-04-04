using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    void damage(float amaount);
}
public interface IKnockable
{
    void Knockback(Vector2 angle,float strength,int direction);
}
