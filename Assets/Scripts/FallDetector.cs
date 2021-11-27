using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallDetector : MonoBehaviour
{
    private Transform cam;
    void Start()
    {
        cam = GetComponentInParent<Transform>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = new Vector3(cam.position.x, -5.5f, 0);
    }
}
