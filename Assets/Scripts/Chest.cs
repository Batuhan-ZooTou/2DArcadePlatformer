using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    private SpriteRenderer SR;
    public Sprite openState;
    public ParticleSystem partical;
    void Start()
    {
        SR = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            partical.Play();
            SR.sprite = openState;
            partical = null;
        }
    }
}
