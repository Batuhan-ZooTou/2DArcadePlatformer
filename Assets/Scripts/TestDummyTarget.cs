using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDummyTarget : MonoBehaviour ,IDamageable,IKnockable
{
    private Vector2 workSpace;
    Rigidbody2D rb;
    public float healt=100;
    private bool invincible = false;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void damage(float amount)
    {
        if (!invincible)
        {
            //Instantiate(hitParticles, transform.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
            Debug.Log(amount + "delt");
            healt -= amount;
            invincible = true;
            StartCoroutine("ResetInvincible");
            CheckIfDead();
        }
    }
    public void Knockback(Vector2 angle, float strength, int direction)
    {
        SetVelocity(strength,angle,direction);
    }
    public void SetVelocity(float velocity, Vector2 angle, int direction)
    {
        angle.Normalize();
        workSpace.Set(angle.x * velocity * direction, angle.y * velocity);
        rb.velocity = workSpace;
    }
    public void CheckIfDead()
    {
        if (healt<=0)
        {
            healt = 0;
            Destroy(this.gameObject);
        }
    }
    IEnumerator ResetInvincible()
    {
        yield return new WaitForSeconds(0.25f);
        rb.velocity = new Vector2(0, 0);
        invincible = false;
    }
}
