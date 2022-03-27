using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Platforms : MonoBehaviour
{
    PlatformEffector2D platformEffector2D;
    new Collider2D collider;
    public float reOpen = 0.5f;

    private void Start()
    {
        platformEffector2D=GetComponent<PlatformEffector2D>();
        collider = GetComponent<Collider2D>();
    }
    public IEnumerator DoubleSidedPlatform()
    {
        yield return new WaitForSeconds(reOpen);
        collider.isTrigger = false;
        //platformEffector2D.surfaceArc = 180;

    }
}
