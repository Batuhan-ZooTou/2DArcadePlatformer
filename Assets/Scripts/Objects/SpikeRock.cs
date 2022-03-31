using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeRock : MonoBehaviour
{
    public float range=15f;
    public LayerMask player;
    private Rigidbody2D rb2d;
    private Vector2 startOffset1;
    private Vector2 startOffset2;
    private Animator anim;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        startOffset1 = new Vector2(GetComponent<Collider2D>().bounds.center.x - 0.7f, GetComponent<Collider2D>().bounds.center.y);
        startOffset2 = new Vector2(GetComponent<Collider2D>().bounds.center.x + 0.7f, GetComponent<Collider2D>().bounds.center.y);
    }

    void Update()
    {
        bool abovePlayer0 = Physics2D.Raycast(startOffset1, -Vector3.up, range, player);
        bool abovePlayer1 = Physics2D.Raycast(GetComponent<Collider2D>().bounds.center, -Vector3.up, range, player);
        bool abovePlayer2 = Physics2D.Raycast(startOffset2, -Vector3.up, range, player);
        if (abovePlayer0 || abovePlayer1 || abovePlayer2)
        {
            rb2d.gravityScale = 4f;
            anim.SetTrigger("Fall");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground"))
        {
            Destroy(this.gameObject);
        }
    }
    
}
