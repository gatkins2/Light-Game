using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlackHole : MonoBehaviour
{
    public GameObject BlackHoleExit;    // Other black hole to travel to

    Transform sprite;                   // Black hole sprite to rotate

    // Start is called before the first frame update
    void Start()
    {
        sprite = transform.GetChild(0);
    }

    // Update is called once per frame
    void Update()
    {
        // Rotate sprite
        sprite.RotateAround(transform.position, Vector3.forward, -18f * Time.deltaTime);
    }
}
