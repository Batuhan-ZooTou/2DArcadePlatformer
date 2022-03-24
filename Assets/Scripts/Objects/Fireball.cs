using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float speed;
    public float lifeTime;
    private Vector2 spawnPoint;
    public Rigidbody2D rb2d;
    public StateMachina stateMachina;
    public int direction=1;
    public float gravityTimer=1;
    private float gravitYcounter=1;
    private void FixedUpdate()
    {
        if (gameObject.activeInHierarchy == true)
        {
            if (rb2d.gravityScale==0)
            {
                rb2d.velocity = new Vector2(speed * direction, 0f);
            }
            else
            {
                rb2d.velocity = new Vector2(speed * direction, rb2d.velocity.y);
                if (direction>0)
                    transform.Rotate(0, 0, -3);
                else
                    transform.Rotate(0, 0, 3);
            }
        }
        else
            rb2d.velocity = new Vector2(0f, 0f);
    }
    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeInHierarchy == true)
        {
            gravitYcounter -= Time.deltaTime;
            if (gravitYcounter < 0)
                gravitYcounter = 0;
            if (gravitYcounter==0)
                rb2d.gravityScale = 4;
        }
        

    }
    private void OnEnable()
    {
        gravitYcounter = gravityTimer;
        rb2d.gravityScale = 0;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        Vector2 scale = transform.localScale;
        direction = stateMachina.direction;
        if (direction==1)
        {
            scale.x = 2.5f;
            transform.localScale = scale;
        }
        else
        {
            scale.x = -2.5f;
            transform.localScale = scale;
        }
        spawnPoint = transform.position;
        StartCoroutine("Destroy");
    }
    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(lifeTime);
        gameObject.SetActive(false);
        transform.position = spawnPoint;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            collision.gameObject.SetActive(false);
            FindObjectOfType<AudioManager>().PlaySound("Fireball");
            gameObject.SetActive(false);
        }
        if (collision.CompareTag("Ground"))
        {
            gameObject.SetActive(false);
        }
    }
}
