using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Saw : MonoBehaviour
{
    [SerializeField] private float freq = 1f;
    [SerializeField] private float heightAmpX = 1f;
    [SerializeField] private float heightAmpY = 1f;
    [SerializeField] private float spawnPosX;
    [SerializeField] private float spawnPosY;
    private void Awake()
    {
       spawnPosX = transform.position.x;
       spawnPosY = transform.position.y;
    }
    void Update()
    {
        float posX = Mathf.Sin(Time.time * freq) * heightAmpX;
        float posY = Mathf.Sin(Time.time * freq) * heightAmpY;
        transform.position = new Vector2(posX + spawnPosX, posY + spawnPosY);
    }
}
