using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAfterImage : MonoBehaviour
{
    private Transform player;
    private SpriteRenderer SR;
    private SpriteRenderer PlayerSR;
    [SerializeField]
    private float alpha;
    private float activeTime = 0.1f;
    private float timeActivated;
    [SerializeField]
    private float alphaMultiplier = 0.85f;
    private float alphaSet = 0.8f;


    private Color color;
    private void OnEnable()
    {
        SR = GetComponent<SpriteRenderer>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        PlayerSR = player.GetComponent<SpriteRenderer>();

        alpha = alphaSet;
        SR.sprite = PlayerSR.sprite;
        transform.position = player.position;
        transform.rotation = player.rotation;
        transform.localScale = player.localScale;
        timeActivated = Time.time;
    }
    private void Update()
    {
        alpha *= alphaMultiplier;
        color = new Color(4f, 2f, 3f, alpha);
        SR.color = color;
        if (Time.time >= (timeActivated + activeTime))
        {
            AfterImagePool.Instance.AddToPool(gameObject);
        }
    }
}
