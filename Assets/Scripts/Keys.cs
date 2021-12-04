using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Keys : MonoBehaviour
{
    private PlayerMovement playerMovement;
    public float rotationSpeed;
    private float y;

    void Start()
    {
        playerMovement = FindObjectOfType<PlayerMovement>();
        y = 0.0f;
    }
    private void FixedUpdate()
    {
        y += Time.deltaTime * rotationSpeed;
        transform.localRotation = Quaternion.Euler(0, y, 0);
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovement.keys+=1;
            Destroy(this.gameObject);
        }
    }
}
